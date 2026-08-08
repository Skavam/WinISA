using System.Collections.Generic;
using System.Management;
using WinISA.Models;
using System;

namespace WinISA.Collectors;

public static class MappedDriveCollector
{
    public static List<MappedDriveInfo> GetInfo()
    {
        var list = new List<MappedDriveInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk WHERE DriveType=4");
            using var results = searcher.Get();
            foreach (ManagementObject d in results)
            {
                var name = d["DeviceID"]?.ToString();
                if (string.IsNullOrEmpty(name)) continue;

                list.Add(new MappedDriveInfo
                {
                    DeviceID = name,
                    UNC = d["ProviderName"]?.ToString(),
                    FileSystem = d["FileSystem"]?.ToString(),
                    Size = d["Size"] != null ? $"{Convert.ToInt64(d["Size"]) / (1024.0 * 1024.0 * 1024.0):F2} GB" : null,
                    FreeSpace = d["FreeSpace"] != null ? $"{Convert.ToInt64(d["FreeSpace"]) / (1024.0 * 1024.0 * 1024.0):F2} GB" : null
                });
            }
        }
        catch { /* ignore */ }
        return list;
    }
}