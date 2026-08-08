using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WinISA.Models;

namespace WinISA.Collectors;

public static class TaskCollector
{
    public static List<ScheduledTaskInfo> GetTasks()
    {
        var list = new List<ScheduledTaskInfo>();
        try
        {
            // Use schtasks.exe to get detailed task info
            var psi = new ProcessStartInfo
            {
                FileName = "schtasks.exe",
                Arguments = "/query /fo csv /v",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };
            using var p = Process.Start(psi);
            if (p == null) return list;

            var output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();

            var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var line in lines.Skip(1).Take(100)) // skip header, take first 100
            {
                var parts = line.Split(',');
                if (parts.Length < 10) continue;

                var name = parts[0].Trim('"');
                var path = parts[1].Trim('"');
                var state = parts[3].Trim('"');
                var lastRun = parts[4].Trim('"');
                var nextRun = parts[5].Trim('"');
                var enabledText = parts[8].Trim('"');
                var description = parts[9].Trim('"');

                list.Add(new ScheduledTaskInfo
                {
                    Name = string.IsNullOrEmpty(name) ? null : name,
                    Path = string.IsNullOrEmpty(path) ? null : path,
                    State = string.IsNullOrEmpty(state) ? null : state,
                    Enabled = enabledText.Contains("Enabled"),
                    LastRunTime = string.IsNullOrEmpty(lastRun) ? null : lastRun,
                    NextRunTime = string.IsNullOrEmpty(nextRun) ? null : nextRun,
                    Description = string.IsNullOrEmpty(description) ? null : description
                });
            }
        }
        catch { /* ignore */ }

        return list;
    }
}