using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using WinISA.Models;

namespace WinISA.Collectors;

public static class NetworkCollector
{
    public static List<NetworkAdapterInfo> GetInfo()
    {
        var list = new List<NetworkAdapterInfo>();
        try
        {
            var adapters = NetworkInterface.GetAllNetworkInterfaces()
                .Where(n => n.OperationalStatus == OperationalStatus.Up && n.NetworkInterfaceType != NetworkInterfaceType.Loopback);

            foreach (var nic in adapters)
            {
                try
                {
                    var ipProps = nic.GetIPProperties();
                    var ipv4 = ipProps.UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
                    var ipv6 = ipProps.UnicastAddresses.FirstOrDefault(a => a.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6);
                    var gw = ipProps.GatewayAddresses.FirstOrDefault()?.Address?.ToString();
                    var dns = ipProps.DnsAddresses.Any() ? string.Join(", ", ipProps.DnsAddresses.Select(a => a.ToString())) : null;

                    list.Add(new NetworkAdapterInfo
                    {
                        Description = nic.Description,
                        MAC = nic.GetPhysicalAddress()?.ToString() ?? "00-00-00-00-00-00",
                        IPv4 = ipv4?.Address?.ToString(),
                        IPv6 = ipv6?.Address?.ToString(),
                        SubnetMask = ipv4?.IPv4Mask?.ToString(),
                        Gateway = gw,
                        DNS = dns,
                        DHCPEnabled = ipProps.DhcpServerAddresses.Count > 0
                    });
                }
                catch { /* skip this adapter */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}