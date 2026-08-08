using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WinISA.Models;

namespace WinISA.Collectors;

public static class ProcessCollector
{
    public static List<ProcessInfo> GetInfo()
    {
        var list = new List<ProcessInfo>();
        try
        {
            var processes = Process.GetProcesses();
            foreach (var p in processes.OrderByDescending(p => p.WorkingSet64).Take(50))
            {
                try
                {
                    list.Add(new ProcessInfo
                    {
                        Name = p.ProcessName,
                        PID = p.Id,
                        MemoryMB = $"{p.WorkingSet64 / (1024.0 * 1024.0):F1} MB",
                        TotalProcessorTime = p.TotalProcessorTime.TotalSeconds > 0
                            ? $"{p.TotalProcessorTime.TotalSeconds:F1}s"
                            : "0s"
                    });
                }
                catch { /* skip this process */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}