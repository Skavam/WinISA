using System.Collections.Generic;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class UsbCollector
{
    public static List<UsbDeviceInfo> GetDevices()
    {
        var list = new List<UsbDeviceInfo>();
        try
        {
            const string usbClassGuid = "{36fc9e60-c465-11cf-8056-444553540000}";
            using var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity WHERE ClassGuid='{usbClassGuid}'");
            using var results = searcher.Get();
            foreach (ManagementObject u in results.Cast<ManagementObject>().Take(100))
            {
                try
                {
                    var name = u["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new UsbDeviceInfo
                    {
                        Name = name,
                        DeviceID = u["DeviceID"]?.ToString(),
                        Manufacturer = u["Manufacturer"]?.ToString(),
                        Status = u["Status"]?.ToString()
                    });
                }
                catch { /* skip this device */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}