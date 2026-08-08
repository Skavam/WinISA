using System;
using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class PartitionCollector
{
    public static List<DiskPartitionInfo> GetPartitions()
    {
        var list = new List<DiskPartitionInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskPartition");
            using var results = searcher.Get();
            foreach (ManagementObject p in results)
            {
                try
                {
                    var name = p["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    var size = p["Size"] != null ? Convert.ToInt64(p["Size"]) : (long?)null;
                    var startOffset = p["StartingOffset"] != null ? Convert.ToInt64(p["StartingOffset"]) : (long?)null;

                    list.Add(new DiskPartitionInfo
                    {
                        Name = name,
                        DiskIndex = p["DiskIndex"]?.ToString(),
                        Index = p["Index"]?.ToString(),
                        Type = p["Type"]?.ToString(),
                        SizeGB = size.HasValue ? $"{size.Value / (1024.0 * 1024.0 * 1024.0):F2} GB" : null,
                        StartingOffset = startOffset.HasValue ? $"{startOffset.Value} bytes" : null,
                        Bootable = p["Bootable"] != null && Convert.ToBoolean(p["Bootable"])
                    });
                }
                catch { /* skip this partition */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}