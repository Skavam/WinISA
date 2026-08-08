using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Win32;
using WinISA.Models;

namespace WinISA.Collectors;

public static class StartupCollector
{
    public static List<StartupItemInfo> GetInfo()
    {
        var list = new List<StartupItemInfo>();

        try
        {
            // Registry Run keys (Local Machine)
            var regPaths = new (string path, string location)[]
            {
                (@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", "HKLM\\Run"),
                (@"SOFTWARE\Microsoft\Windows\CurrentVersion\RunOnce", "HKLM\\RunOnce"),
                (@"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Run", "HKLM\\Run (32-bit)"),
            };

            foreach (var (path, location) in regPaths)
            {
                using var key = Registry.LocalMachine.OpenSubKey(path);
                if (key == null) continue;
                foreach (var name in key.GetValueNames())
                {
                    var value = key.GetValue(name)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        list.Add(new StartupItemInfo
                        {
                            Name = name,
                            Command = value,
                            Location = location
                        });
                    }
                }
            }

            // Current User Run
            using var userKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run");
            if (userKey != null)
            {
                foreach (var name in userKey.GetValueNames())
                {
                    var value = userKey.GetValue(name)?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        list.Add(new StartupItemInfo
                        {
                            Name = name,
                            Command = value,
                            Location = "HKCU\\Run"
                        });
                    }
                }
            }

            // Startup folders (User and All Users)
            var startupFolders = new[]
            {
                Environment.GetFolderPath(Environment.SpecialFolder.Startup),
                Environment.GetFolderPath(Environment.SpecialFolder.CommonStartup)
            };

            foreach (var folder in startupFolders)
            {
                if (Directory.Exists(folder))
                {
                    foreach (var file in Directory.GetFiles(folder, "*.lnk"))
                    {
                        try
                        {
                            var name = Path.GetFileNameWithoutExtension(file);
                            list.Add(new StartupItemInfo
                            {
                                Name = name,
                                Command = file,
                                Location = "Startup Folder"
                            });
                        }
                        catch { /* skip this file */ }
                    }
                }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}