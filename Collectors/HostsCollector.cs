using System.Collections.Generic;
using System.IO;
using WinISA.Models;
using System;


namespace WinISA.Collectors;

public static class HostsCollector
{
    public static List<string> GetEntries()
    {
        var entries = new List<string>();
        try
        {
            var systemRoot = Environment.GetEnvironmentVariable("SystemRoot") ?? "C:\\Windows";
            var hostsPath = Path.Combine(systemRoot, "System32", "drivers", "etc", "hosts");

            if (File.Exists(hostsPath))
            {
                var lines = File.ReadAllLines(hostsPath);
                foreach (var line in lines)
                {
                    var trimmed = line.Trim();
                    if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                        continue;
                    entries.Add(trimmed);
                    if (entries.Count >= 50)
                        break;
                }
            }
        }
        catch { /* ignore */ }

        return entries;
    }
}