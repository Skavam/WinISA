using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class MotherboardCollector
{
    public static MotherboardInfo? GetInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            return new MotherboardInfo
            {
                Manufacturer = mo["Manufacturer"]?.ToString(),
                Model = mo["Product"]?.ToString(),
                SerialNumber = mo["SerialNumber"]?.ToString(),
                Version = mo["Version"]?.ToString()
            };
        }
        catch
        {
            return null;
        }
    }
}