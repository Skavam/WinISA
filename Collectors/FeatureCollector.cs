using System.Collections.Generic;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class FeatureCollector
{
    public static List<WindowsFeatureInfo> GetFeatures()
    {
        var list = new List<WindowsFeatureInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OptionalFeature");
            using var results = searcher.Get();
            foreach (ManagementObject f in results.Cast<ManagementObject>().Take(50))
            {
                var name = f["Name"]?.ToString();
                if (string.IsNullOrEmpty(name)) continue;

                list.Add(new WindowsFeatureInfo
                {
                    Name = name,
                    InstallState = f["InstallState"]?.ToString() == "1" ? "Enabled" : "Disabled"
                });
            }
        }
        catch { /* ignore */ }

        return list;
    }
}