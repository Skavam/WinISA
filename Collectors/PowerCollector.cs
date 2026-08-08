using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class PowerCollector
{
    public static List<PowerSchemeInfo> GetSchemes()
    {
        var list = new List<PowerSchemeInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PowerPlan");
            using var results = searcher.Get();
            foreach (ManagementObject p in results)
            {
                try
                {
                    var name = p["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new PowerSchemeInfo
                    {
                        Name = name,
                        IsActive = p["IsActive"] != null && (bool)p["IsActive"],
                        Description = p["Description"]?.ToString()
                    });
                }
                catch { /* skip this scheme */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}