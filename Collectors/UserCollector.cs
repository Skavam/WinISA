using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class UserCollector
{
    public static List<LocalUserInfo> GetLocalUsers()
    {
        var list = new List<LocalUserInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_UserAccount WHERE LocalAccount=True");
            using var results = searcher.Get();
            foreach (ManagementObject u in results)
            {
                var name = u["Name"]?.ToString();
                if (string.IsNullOrEmpty(name)) continue;

                list.Add(new LocalUserInfo
                {
                    Name = name,
                    FullName = u["FullName"]?.ToString(),
                    Domain = u["Domain"]?.ToString(),
                    SID = u["SID"]?.ToString(),
                    Status = (bool)(u["Disabled"] ?? false) ? "Disabled" :
                             (bool)(u["Lockout"] ?? false) ? "Locked" : "Enabled"
                });
            }
        }
        catch { /* ignore */ }

        return list;
    }
}