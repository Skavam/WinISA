using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class BatteryCollector
{
    public static BatteryInfo? GetInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Battery");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            return new BatteryInfo
            {
                Name = mo["Name"]?.ToString(),
                Manufacturer = mo["Manufacturer"]?.ToString(),
                EstimatedChargeRemaining = mo["EstimatedChargeRemaining"]?.ToString(),
                Status = mo["Status"]?.ToString(),
                Chemistry = mo["Chemistry"]?.ToString(),
                DesignCapacity = mo["DesignCapacity"]?.ToString()
            };
        }
        catch
        {
            return null;
        }
    }
}