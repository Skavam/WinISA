using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using WinISA.Models;

namespace WinISA.Collectors;

public static class EventLogCollector
{
    public static List<EventLogEntryInfo> GetRecentErrors()
    {
        var list = new List<EventLogEntryInfo>();
        try
        {
            var logs = new[] { "System", "Application" };
            foreach (var logName in logs)
            {
                try
                {
                    var log = new EventLog(logName);
                    var entries = log.Entries.Cast<EventLogEntry>()
                        .Where(e => e.EntryType == EventLogEntryType.Error || e.EntryType == EventLogEntryType.Warning)
                        .OrderByDescending(e => e.TimeGenerated)
                        .Take(10);
                    foreach (var entry in entries)
                    {
                        list.Add(new EventLogEntryInfo
                        {
                            LogName = logName,
                            Time = entry.TimeGenerated.ToString("yyyy-MM-dd HH:mm:ss"),
                            Source = entry.Source,
                            EventID = entry.InstanceId,
                            Message = entry.Message.Length > 200 ? entry.Message.Substring(0, 200) + "..." : entry.Message
                        });
                    }
                }
                catch { /* ignore this log */ }
            }
        }
        catch { /* ignore */ }

        return list.Take(20).ToList(); // overall limit
    }
}