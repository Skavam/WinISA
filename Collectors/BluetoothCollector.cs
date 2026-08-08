using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class BluetoothCollector
{
    public static List<BluetoothDeviceInfo> GetDevices()
    {
        var list = new List<BluetoothDeviceInfo>();

        // Try the native Win32_BluetoothDevice class first
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_BluetoothDevice");
            using var results = searcher.Get();
            foreach (ManagementObject d in results)
            {
                try
                {
                    var name = d["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new BluetoothDeviceInfo
                    {
                        Name = name,
                        Address = d["DeviceAddress"]?.ToString(),
                        Status = d["Status"]?.ToString(),
                        Connected = d["Connected"] != null && (bool)d["Connected"]
                    });
                }
                catch { /* skip this device */ }
            }

            // If we got devices, return them
            if (list.Count > 0) return list;
        }
        catch { /* fallback to PnPEntity */ }

        // Fallback: query PnPEntity with Bluetooth class GUID
        try
        {
            const string bluetoothClassGuid = "{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}";
            using var searcher = new ManagementObjectSearcher($"SELECT * FROM Win32_PnPEntity WHERE ClassGuid='{bluetoothClassGuid}'");
            using var results = searcher.Get();
            foreach (ManagementObject d in results)
            {
                try
                {
                    var name = d["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new BluetoothDeviceInfo
                    {
                        Name = name,
                        Address = d["DeviceID"]?.ToString(), // often contains MAC
                        Status = d["Status"]?.ToString(),
                        Connected = false // not available in this fallback
                    });
                }
                catch { /* skip */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}