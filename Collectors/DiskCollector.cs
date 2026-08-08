using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class DiskCollector
{
    public static List<DiskInfo> GetInfo()
    {
        var result = new List<DiskInfo>();

        // 1. Get all physical disk drives and build model and serial maps by Index
        var modelMap = new Dictionary<string, string>();
        var serialMap = new Dictionary<string, string>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            using var results = searcher.Get();
            foreach (ManagementObject pd in results)
            {
                var idx = pd["Index"]?.ToString();
                if (string.IsNullOrEmpty(idx)) continue;

                var model = pd["Model"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(model))
                    modelMap[idx] = model;

                var serial = pd["SerialNumber"]?.ToString()?.Trim();
                if (!string.IsNullOrEmpty(serial))
                    serialMap[idx] = serial;
            }
        }
        catch { /* ignore */ }

        // 2. Get all ready logical drives
        var logicalDrives = DriveInfo.GetDrives().Where(d => d.IsReady);
        foreach (var drive in logicalDrives)
        {
            try
            {
                string model = "Unknown";
                string serial = null;
                var diskId = drive.Name.TrimEnd('\\');

                // 3. Find partitions associated with this logical disk
                var partitionQuery = $"ASSOCIATORS OF {{Win32_LogicalDisk.DeviceID='{diskId}'}} WHERE AssocClass = Win32_LogicalDiskToPartition";
                var partitions = QueryWmiList(partitionQuery);
                foreach (ManagementObject part in partitions)
                {
                    // 4. For each partition, find the physical disk drive
                    var diskQuery = $"ASSOCIATORS OF {{Win32_DiskPartition.DeviceID='{part["DeviceID"]}'}} WHERE AssocClass = Win32_DiskDriveToDiskPartition";
                    var disks = QueryWmiList(diskQuery);
                    foreach (ManagementObject disk in disks)
                    {
                        var idx = disk["Index"]?.ToString();
                        if (!string.IsNullOrEmpty(idx))
                        {
                            if (modelMap.TryGetValue(idx, out var m))
                                model = m;
                            if (serialMap.TryGetValue(idx, out var s))
                                serial = s;
                            break;
                        }
                    }
                    if (model != "Unknown") break;
                }

                result.Add(new DiskInfo
                {
                    DeviceID = drive.Name,
                    VolumeName = string.IsNullOrEmpty(drive.VolumeLabel) ? null : drive.VolumeLabel,
                    FileSystem = drive.DriveFormat,
                    Size = drive.TotalSize > 0 ? $"{drive.TotalSize / (1024.0 * 1024.0 * 1024.0):F2} GB" : null,
                    FreeSpace = drive.AvailableFreeSpace >= 0 ? $"{drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0):F2} GB" : null,
                    Model = model,
                    SerialNumber = serial
                });
            }
            catch { /* skip this drive */ }
        }

        return result;
    }

    // Helper method to run a WMI query and return a list of ManagementObject
    private static List<ManagementObject> QueryWmiList(string query)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(query);
            var results = searcher.Get();
            return results.Cast<ManagementObject>().ToList();
        }
        catch
        {
            return new List<ManagementObject>();
        }
    }
}