using System;
using System.Collections.Generic;
using WinISA.Models;
using System.Linq;

namespace WinISA.Collectors;

public static class FirewallCollector
{
    public static List<FirewallRuleInfo> GetRules()
    {
        var list = new List<FirewallRuleInfo>();
        try
        {
            Type t = Type.GetTypeFromProgID("HNetCfg.FwPolicy2");
            if (t == null) return list;

            dynamic policy = Activator.CreateInstance(t);
            dynamic rules = policy.Rules;

            foreach (var rule in rules)
            {
                try
                {
                    // Extract properties safely
                    string name = rule.Name;
                    string description = rule.Description;
                    int direction = rule.Direction;
                    int action = rule.Action;
                    int protocol = rule.Protocol;
                    string localPorts = rule.LocalPorts;
                    string remotePorts = rule.RemotePorts;
                    string localAddresses = rule.LocalAddresses;
                    string remoteAddresses = rule.RemoteAddresses;
                    bool enabled = rule.Enabled;

                    list.Add(new FirewallRuleInfo
                    {
                        Name = name,
                        Description = description,
                        Direction = direction == 1 ? "Inbound" : (direction == 2 ? "Outbound" : "Unknown"),
                        Action = action == 0 ? "Block" : (action == 1 ? "Allow" : "Unknown"),
                        Protocol = protocol == 6 ? "TCP" : (protocol == 17 ? "UDP" : protocol.ToString()),
                        LocalPorts = localPorts,
                        RemotePorts = remotePorts,
                        LocalAddresses = localAddresses,
                        RemoteAddresses = remoteAddresses,
                        Enabled = enabled
                    });
                }
                catch { /* skip this rule */ }
            }
        }
        catch { /* admin might be required, ignore */ }

        return list.Take(200).ToList();
    }
}