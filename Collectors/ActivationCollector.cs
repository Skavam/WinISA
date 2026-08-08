using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class ActivationCollector
{
    public static ActivationInfo? GetStatus()
    {
        try
        {
            const string query = "SELECT * FROM SoftwareLicensingProduct WHERE ApplicationID='55c92734-d682-4d71-983e-d6ec3f16059f' AND PartialProductKey IS NOT NULL";
            using var searcher = new ManagementObjectSearcher(query);
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return null;

            var licenseStatus = mo["LicenseStatus"]?.ToString();
            string statusText = licenseStatus switch
            {
                "0" => "Unlicensed",
                "1" => "Licensed",
                "2" => "OOBGrace",
                "3" => "OOTGrace",
                "4" => "NonGenuineGrace",
                "5" => "Notification",
                "6" => "ExtendedGrace",
                _ => "Unknown"
            };

            return new ActivationInfo
            {
                LicenseStatus = statusText,
                ProductID = mo["ProductID"]?.ToString(),
                PartialProductKey = mo["PartialProductKey"]?.ToString(),
                RemainingGracePeriod = mo["RemainingGracePeriod"]?.ToString()
            };
        }
        catch
        {
            return null;
        }
    }
}
