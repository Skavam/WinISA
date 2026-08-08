using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class CpuCollector
{
    public static CpuInfo? GetInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            return new CpuInfo
            {
                Name = mo["Name"]?.ToString()?.Trim(),
                Manufacturer = mo["Manufacturer"]?.ToString(),
                MaxClockSpeed = mo["MaxClockSpeed"]?.ToString(),
                Cores = mo["NumberOfCores"]?.ToString(),
                LogicalProcessors = mo["NumberOfLogicalProcessors"]?.ToString(),
                Architecture = mo["Architecture"]?.ToString() switch
                {
                    "0" => "x86",
                    "1" => "MIPS",
                    "2" => "Alpha",
                    "3" => "PowerPC",
                    "5" => "ARM",
                    "6" => "Itanium",
                    "9" => "x64",
                    _ => null
                },
                ProcessorId = mo["ProcessorId"]?.ToString()  // NEW
            };
        }
        catch
        {
            return null;
        }
    }
}