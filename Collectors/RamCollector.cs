using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class RamCollector
{
    public static RamInfo? GetInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            long total = mo["TotalVisibleMemorySize"] != null ? System.Convert.ToInt64(mo["TotalVisibleMemorySize"]) : 0;
            long free = mo["FreePhysicalMemory"] != null ? System.Convert.ToInt64(mo["FreePhysicalMemory"]) : 0;
            long totalVirtual = mo["TotalVirtualMemorySize"] != null ? System.Convert.ToInt64(mo["TotalVirtualMemorySize"]) : 0;
            long freeVirtual = mo["FreeVirtualMemory"] != null ? System.Convert.ToInt64(mo["FreeVirtualMemory"]) : 0;

            return new RamInfo
            {
                TotalPhysical = total > 0 ? $"{total / (1024.0 * 1024.0):F2} GB" : null,
                FreePhysical = free > 0 ? $"{free / (1024.0 * 1024.0):F2} GB" : null,
                TotalVirtual = totalVirtual > 0 ? $"{totalVirtual / (1024.0 * 1024.0):F2} GB" : null,
                FreeVirtual = freeVirtual > 0 ? $"{freeVirtual / (1024.0 * 1024.0):F2} GB" : null
            };
        }
        catch
        {
            return null;
        }
    }
}