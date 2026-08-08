using Microsoft.Win32;
using WinISA.Models;

namespace WinISA.Collectors;

public static class DirectXCollector
{
    public static string GetVersion()
    {
        try
        {
            // Try 64-bit registry path first
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\DirectX");
            if (key != null)
            {
                var ver = key.GetValue("Version")?.ToString();
                if (!string.IsNullOrEmpty(ver)) return ver;
            }

            // Fallback to 32-bit (WOW6432Node)
            using var key2 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\WOW6432Node\Microsoft\DirectX");
            if (key2 != null)
            {
                var ver = key2.GetValue("Version")?.ToString();
                if (!string.IsNullOrEmpty(ver)) return ver;
            }

            // Another fallback: Direct3D version from registry
            using var key3 = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Direct3D");
            if (key3 != null)
            {
                var ver = key3.GetValue("Version")?.ToString();
                if (!string.IsNullOrEmpty(ver)) return ver;
            }

            return "Unknown";
        }
        catch
        {
            return "Unknown";
        }
    }
}