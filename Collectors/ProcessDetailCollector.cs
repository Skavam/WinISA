using System.Collections.Generic;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class ProcessDetailCollector
{
    public static List<ProcessDetailInfo> GetInfo()
    {
        var list = new List<ProcessDetailInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Process");
            using var results = searcher.Get();
            foreach (ManagementObject p in results.Cast<ManagementObject>().Take(200))
            {
                try
                {
                    var pid = p["ProcessId"] != null ? System.Convert.ToInt32(p["ProcessId"]) : 0;
                    var name = p["Name"]?.ToString();
                    var cmd = p["CommandLine"]?.ToString();
                    var exe = p["ExecutablePath"]?.ToString();

                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new ProcessDetailInfo
                    {
                        Name = name,
                        PID = pid,
                        CommandLine = cmd,
                        ExecutablePath = exe
                    });
                }
                catch { /* skip this process */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}