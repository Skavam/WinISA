using System;
using System.Collections.Generic;
using WinISA.Models;
using System.Linq;

namespace WinISA.Collectors;

public static class EnvironmentCollector
{
    public static List<EnvironmentVariableInfo> GetInfo()
    {
        var list = new List<EnvironmentVariableInfo>();

        try
        {
            // System variables
            foreach (var key in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.Machine).Keys)
            {
                var name = key.ToString();
                if (string.IsNullOrEmpty(name)) continue;
                var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Machine);
                list.Add(new EnvironmentVariableInfo
                {
                    Name = name,
                    Value = value,
                    Target = "System"
                });
            }

            // User variables
            foreach (var key in Environment.GetEnvironmentVariables(EnvironmentVariableTarget.User).Keys)
            {
                var name = key.ToString();
                if (string.IsNullOrEmpty(name)) continue;
                var value = Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.User);
                list.Add(new EnvironmentVariableInfo
                {
                    Name = name,
                    Value = value,
                    Target = "User"
                });
            }
        }
        catch { /* ignore */ }

        // Limit to 100 to avoid huge JSON
        return list.Take(100).ToList();
    }
}