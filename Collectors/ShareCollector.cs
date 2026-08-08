using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class ShareCollector
{
    public static List<NetworkShareInfo> GetShares()
    {
        var list = new List<NetworkShareInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Share");
            using var results = searcher.Get();
            foreach (ManagementObject s in results)
            {
                var name = s["Name"]?.ToString();
                if (string.IsNullOrEmpty(name)) continue;

                list.Add(new NetworkShareInfo
                {
                    Name = name,
                    Path = s["Path"]?.ToString(),
                    Description = s["Description"]?.ToString(),
                    Status = s["Status"]?.ToString()
                });
            }
        }
        catch { /* ignore */ }

        return list;
    }
}