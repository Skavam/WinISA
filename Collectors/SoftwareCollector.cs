using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;
using WinISA.Models;

namespace WinISA.Collectors;

public static class SoftwareCollector
{
    public static List<SoftwareInfo> GetInfo()
    {
        var list = new List<SoftwareInfo>();

        try
        {
            var paths = new[]
            {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            foreach (var basePath in paths)
            {
                using var key = Registry.LocalMachine.OpenSubKey(basePath);
                if (key == null) continue;

                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    try
                    {
                        using var subKey = key.OpenSubKey(subKeyName);
                        if (subKey == null) continue;

                        var displayName = subKey.GetValue("DisplayName")?.ToString();
                        if (string.IsNullOrEmpty(displayName)) continue;

                        list.Add(new SoftwareInfo
                        {
                            DisplayName = displayName,
                            DisplayVersion = subKey.GetValue("DisplayVersion")?.ToString(),
                            Publisher = subKey.GetValue("Publisher")?.ToString(),
                            InstallDate = subKey.GetValue("InstallDate")?.ToString()
                        });
                    }
                    catch { /* skip this entry */ }
                }
            }
        }
        catch { /* ignore */ }

        return list.OrderBy(s => s.DisplayName).Take(200).ToList();
    }
}