using System.Collections.Generic;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class DriverCollector
{
    public static List<DriverInfo> GetInfo()
    {
        var list = new List<DriverInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_PnPSignedDriver");
            using var results = searcher.Get();
            var drivers = results.Cast<ManagementObject>().Take(200);
            foreach (var d in drivers)
            {
                try
                {
                    var name = d["DeviceName"]?.ToString();
                    var version = d["DriverVersion"]?.ToString();
                    var date = d["DriverDate"]?.ToString();
                    var inf = d["InfName"]?.ToString();
                    var hw = d["HardwareID"]?.ToString();

                    // Skip if everything is null or empty
                    if (string.IsNullOrEmpty(name) && string.IsNullOrEmpty(version) &&
                        string.IsNullOrEmpty(date) && string.IsNullOrEmpty(inf) && string.IsNullOrEmpty(hw))
                        continue;

                    list.Add(new DriverInfo
                    {
                        DeviceName = string.IsNullOrEmpty(name) ? null : name,
                        DriverVersion = string.IsNullOrEmpty(version) ? null : version,
                        DriverDate = string.IsNullOrEmpty(date) ? null : date,
                        InfName = string.IsNullOrEmpty(inf) ? null : inf,
                        HardwareID = string.IsNullOrEmpty(hw) ? null : hw
                    });
                }
                catch { /* skip this driver */ }
            }
        }
        catch { /* ignore */ }
        return list;
    }
}