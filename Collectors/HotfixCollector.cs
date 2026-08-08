using System.Collections.Generic;
using System.Linq;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class HotfixCollector
{
    public static List<HotfixInfo> GetInfo()
    {
        var list = new List<HotfixInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_QuickFixEngineering");
            using var results = searcher.Get();
            foreach (ManagementObject h in results)
            {
                try
                {
                    var id = h["HotFixID"]?.ToString();
                    if (string.IsNullOrEmpty(id)) continue;

                    list.Add(new HotfixInfo
                    {
                        HotFixID = id,
                        Description = h["Description"]?.ToString(),
                        InstallDate = h["InstallDate"]?.ToString()
                    });
                }
                catch { /* skip this entry */ }
            }
        }
        catch { /* ignore */ }

        return list.OrderBy(h => h.HotFixID).ToList();
    }
}