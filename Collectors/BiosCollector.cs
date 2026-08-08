using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class BiosCollector
{
    public static BiosInfo? GetInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BIOS");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            return new BiosInfo
            {
                Manufacturer = mo["Manufacturer"]?.ToString(),
                Name = mo["Name"]?.ToString(),
                Version = mo["SMBIOSBIOSVersion"]?.ToString(),
                SerialNumber = mo["SerialNumber"]?.ToString(),
                ReleaseDate = mo["ReleaseDate"]?.ToString()
            };
        }
        catch
        {
            return null;
        }
    }
}