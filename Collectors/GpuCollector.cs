using System.Linq;
using System.Management;
using WinISA.Models;
using System;

namespace WinISA.Collectors;

public static class GpuCollector
{
    public static GpuInfo? GetInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            long? ram = mo["AdapterRAM"] != null ? Convert.ToInt64(mo["AdapterRAM"]) : (long?)null;

            return new GpuInfo
            {
                Name = mo["Name"]?.ToString(),
                DriverVersion = mo["DriverVersion"]?.ToString(),
                DriverDate = mo["DriverDate"]?.ToString(),
                AdapterRAM = ram.HasValue ? $"{ram.Value / (1024.0 * 1024.0):F0} MB" : null
            };
        }
        catch
        {
            return null;
        }
    }
}