using System;
using System.Text;
using WinISA.Helpers;
using WinISA.Models;
using System.Linq;

namespace WinISA.Collectors;

public static class WiFiCollector
{
    public static WiFiInfo? GetInfo()
    {
        try
        {
            using var client = new WlanClient();
            foreach (var iface in client.EnumerateInterfaces())
            {
                var conn = iface.CurrentConnection;
                if (conn.isState == WlanClient.WlanInterfaceState.Connected)
                {
                    var ssid = Encoding.UTF8.GetString(conn.dot11Ssid.SSID, 0, (int)conn.dot11Ssid.SSIDLength);
                    return new WiFiInfo
                    {
                        SSID = ssid,
                        BSSID = string.Join("-", conn.bssid.Select(b => b.ToString("X2"))),
                        SignalQuality = conn.wlanSignalQuality.ToString(),
                        RadioType = conn.dot11PhyType.ToString(),
                        Security = conn.dot11AuthAlgorithm.ToString()
                    };
                }
            }
            return new WiFiInfo { SSID = "No active WiFi connection" };
        }
        catch
        {
            return new WiFiInfo { SSID = "WiFi info unavailable" };
        }
    }
}