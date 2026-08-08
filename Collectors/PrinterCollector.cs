using System.Collections.Generic;
using System.Management;
using WinISA.Models;

namespace WinISA.Collectors;

public static class PrinterCollector
{
    public static List<PrinterInfo> GetInfo()
    {
        var list = new List<PrinterInfo>();
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_Printer");
            using var results = searcher.Get();
            foreach (ManagementObject p in results)
            {
                try
                {
                    var name = p["Name"]?.ToString();
                    if (string.IsNullOrEmpty(name)) continue;

                    list.Add(new PrinterInfo
                    {
                        Name = name,
                        DriverName = p["DriverName"]?.ToString(),
                        PortName = p["PortName"]?.ToString(),
                        Status = p["PrinterStatus"]?.ToString() ?? "Unknown"
                    });
                }
                catch { /* skip this printer */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}