using System;
using System.Collections.Generic;
using Microsoft.Win32;
using WinISA.Models;
using System.Linq;

namespace WinISA.Collectors;

public static class DotNetCollector
{
    public static List<DotNetFrameworkInfo> GetFrameworks()
    {
        var list = new List<DotNetFrameworkInfo>();

        // 1. .NET Framework 4.x (classic)
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v4\Full");
            if (key != null)
            {
                var version = key.GetValue("Version")?.ToString();
                var release = key.GetValue("Release")?.ToString();
                if (!string.IsNullOrEmpty(version))
                {
                    list.Add(new DotNetFrameworkInfo
                    {
                        Name = ".NET Framework 4.x",
                        Version = version,
                        Release = release,
                        Type = "Framework"
                    });
                }
            }
        }
        catch { /* ignore */ }

        // 2. .NET Framework 3.5 (if present)
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP\v3.5");
            if (key != null)
            {
                var version = key.GetValue("Version")?.ToString();
                if (!string.IsNullOrEmpty(version))
                {
                    list.Add(new DotNetFrameworkInfo
                    {
                        Name = ".NET Framework 3.5",
                        Version = version,
                        Release = null,
                        Type = "Framework"
                    });
                }
            }
        }
        catch { /* ignore */ }

        // 3. .NET Core / .NET 5+ runtimes and SDKs
        var corePaths = new[]
        {
            @"SOFTWARE\dotnet\Setup\InstalledVersions\x64",
            @"SOFTWARE\dotnet\Setup\InstalledVersions\x86"
        };

        foreach (var path in corePaths)
        {
            try
            {
                using var key = Registry.LocalMachine.OpenSubKey(path);
                if (key == null) continue;

                // Values are like "Microsoft.NETCore.App": "6.0.36"
                foreach (var name in key.GetValueNames())
                {
                    if (name.StartsWith("Microsoft.") || name.Contains(".App") || name.Contains(".Runtime"))
                    {
                        var ver = key.GetValue(name)?.ToString();
                        if (!string.IsNullOrEmpty(ver))
                        {
                            list.Add(new DotNetFrameworkInfo
                            {
                                Name = name,
                                Version = ver,
                                Release = null,
                                Type = "Core"
                            });
                        }
                    }
                }
            }
            catch { /* ignore */ }
        }

        // 4. Also try to get SDKs from the SDK folder (optional)
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\dotnet\Setup\InstalledVersions\x64\sdk");
            if (key != null)
            {
                foreach (var name in key.GetValueNames())
                {
                    var ver = key.GetValue(name)?.ToString();
                    if (!string.IsNullOrEmpty(ver))
                    {
                        list.Add(new DotNetFrameworkInfo
                        {
                            Name = $"SDK: {name}",
                            Version = ver,
                            Release = null,
                            Type = "SDK"
                        });
                    }
                }
            }
        }
        catch { /* ignore */ }

        // Remove duplicates and sort
        var distinct = list.GroupBy(x => x.Name + x.Version).Select(g => g.First()).ToList();
        return distinct.OrderBy(f => f.Name).Take(50).ToList();
    }
}