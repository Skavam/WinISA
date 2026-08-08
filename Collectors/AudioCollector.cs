using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class AudioCollector
{
    public static List<AudioDeviceInfo> GetInfo()
    {
        var list = new List<AudioDeviceInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_SoundDevice");
            using var results = searcher.Get();
            foreach (ManagementObject d in results)
            {
                try
                {
                    var name = d["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;
                    list.Add(new AudioDeviceInfo
                    {
                        Name = name,
                        Manufacturer = d["Manufacturer"]?.ToString(),
                        Status = d["Status"]?.ToString()
                    });
                }
                catch { /* skip this device */ }
            }
        }
        catch { /* ignore */ }
        return list;
    }
}