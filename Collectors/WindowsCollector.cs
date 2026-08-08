using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using Microsoft.Win32;
using WinISA.Models;

namespace WinISA.Collectors;

public static class WindowsCollector
{
    public static WindowsVersionInfo GetVersion()
    {
        var os = Environment.OSVersion;
        var product = QueryWmi<string>("SELECT * FROM Win32_OperatingSystem", "Caption") ?? "Windows";
        var build = QueryWmi<string>("SELECT * FROM Win32_OperatingSystem", "BuildNumber") ?? os.Version.Build.ToString();
        DateTime? installDate = null;
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT InstallDate FROM Win32_OperatingSystem");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo != null)
            {
                var d = mo["InstallDate"];
                if (d != null)
                {
                    installDate = ManagementDateTimeConverter.ToDateTime(d.ToString());
                }
            }
        }
        catch { /* ignore */ }

        string edition = "Unknown";
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (key != null)
            {
                edition = key.GetValue("EditionID")?.ToString() ?? "Unknown";
                if (edition == "Unknown") edition = key.GetValue("ProductName")?.ToString() ?? "Unknown";
            }
        }
        catch { /* ignore */ }

        return new WindowsVersionInfo
        {
            OS = os.VersionString,
            Edition = edition == "Unknown" ? null : edition,
            Build = build,
            ProductName = product,
            InstallDate = installDate.HasValue ? installDate.Value.ToString("yyyy-MM-dd HH:mm") : null
        };
    }

    public static string GetProductKey()
    {
        try
        {
            const string regPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion";
            using var key = Registry.LocalMachine.OpenSubKey(regPath);
            if (key == null) return "Unavailable";
            var dpid = key.GetValue("DigitalProductId") as byte[];
            if (dpid == null) return "Unavailable";
            return DecodeProductKey(dpid);
        }
        catch { return "Unavailable"; }
    }

    private static string DecodeProductKey(byte[] digitalProductId)
    {
        const int keyOffset = 52;
        var raw = new byte[15];
        Array.Copy(digitalProductId, keyOffset, raw, 0, 15);
        const string charset = "BCDFGHJKMPQRTVWXY2346789";
        var key = new char[29];
        for (int i = 24; i >= 0; i--)
        {
            int cur = 0;
            for (int j = 14; j >= 0; j--)
            {
                cur = cur * 256 + raw[j];
                raw[j] = (byte)(cur / 24);
                cur %= 24;
            }
            key[i] = charset[cur];
        }
        var sb = new StringBuilder();
        for (int i = 0; i < 25; i++)
        {
            sb.Append(key[i]);
            if (i == 4 || i == 9 || i == 14 || i == 19)
                sb.Append('-');
        }
        return sb.ToString();
    }

    public static string GetMicrosoftAccountEmail()
    {
        try
        {
            const string path = @"Software\Microsoft\IdentityCRL\UserExtendedProperties";
            using var key = Registry.CurrentUser.OpenSubKey(path);
            if (key != null)
            {
                foreach (var subKeyName in key.GetSubKeyNames())
                {
                    using var subKey = key.OpenSubKey(subKeyName);
                    if (subKey != null)
                    {
                        var email = subKey.GetValue("Email")?.ToString();
                        if (!string.IsNullOrEmpty(email))
                            return email;
                    }
                }
            }
            const string path2 = @"Software\Microsoft\Windows\CurrentVersion\Explorer\Identity";
            using var key2 = Registry.CurrentUser.OpenSubKey(path2);
            if (key2 != null)
            {
                var email = key2.GetValue("EMailAddress")?.ToString();
                if (!string.IsNullOrEmpty(email))
                    return email;
            }
            return "No Microsoft account detected";
        }
        catch { return "Unavailable"; }
    }

    public static string GetBootTime()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT LastBootUpTime FROM Win32_OperatingSystem");
            using var results = searcher.Get();
            var mo = results.Cast<ManagementObject>().FirstOrDefault();
            if (mo == null) return "Unknown";
            var bootStr = mo["LastBootUpTime"]?.ToString();
            if (string.IsNullOrEmpty(bootStr)) return "Unknown";
            var bootTime = ManagementDateTimeConverter.ToDateTime(bootStr);
            return bootTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
        catch { return "Unknown"; }
    }

    private static T? QueryWmi<T>(string query, string property, T? defaultValue = default)
    {
        try
        {
            using var searcher = new ManagementObjectSearcher(query);
            using var results = searcher.Get();
            var first = results.Cast<ManagementObject>().FirstOrDefault();
            if (first == null) return defaultValue;
            var val = first[property];
            if (val == null) return defaultValue;
            return (T)Convert.ChangeType(val, typeof(T));
        }
        catch { return defaultValue; }
    }
}