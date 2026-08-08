using System.Collections.Generic;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class UwpCollector
{
    public static List<UwpAppInfo> GetApps()
    {
        var list = new List<UwpAppInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_InstalledUWPApp");
            using var results = searcher.Get();
            foreach (ManagementObject a in results.Cast<ManagementObject>().Take(200))
            {
                try
                {
                    var name = a["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new UwpAppInfo
                    {
                        Name = name,
                        Version = a["Version"]?.ToString(),
                        Publisher = a["Publisher"]?.ToString(),
                        Architecture = a["Architecture"]?.ToString(),
                        PackageFamilyName = a["PackageFamilyName"]?.ToString()
                    });
                }
                catch { /* skip this app */ }
            }
        }
        catch { /* may not exist on some systems */ }

        return list;
    }
}