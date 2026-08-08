using System.Collections.Generic;
using System.Net.NetworkInformation;
using WinISA.Models;

namespace WinISA.Collectors;

public static class PortCollector
{
    public static List<PortInfo> GetListeningPorts()
    {
        var list = new List<PortInfo>();
        try
        {
            var ipProps = IPGlobalProperties.GetIPGlobalProperties();

            // TCP listeners
            var tcpEndpoints = ipProps.GetActiveTcpListeners();
            foreach (var ep in tcpEndpoints)
            {
                list.Add(new PortInfo
                {
                    Protocol = "TCP",
                    Address = ep.Address.ToString(),
                    Port = ep.Port,
                    State = "Listening",
                    ProcessId = null
                });
            }

            // UDP listeners
            var udpEndpoints = ipProps.GetActiveUdpListeners();
            foreach (var ep in udpEndpoints)
            {
                list.Add(new PortInfo
                {
                    Protocol = "UDP",
                    Address = ep.Address.ToString(),
                    Port = ep.Port,
                    State = "Listening",
                    ProcessId = null
                });
            }
        }
        catch { /* ignore */ }

        return list;
    }
}