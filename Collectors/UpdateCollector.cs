using System;
using System.Collections.Generic;
using WinISA.Models;
using System.Linq;

namespace WinISA.Collectors;

public static class UpdateCollector
{
    public static List<UpdateHistoryInfo> GetHistory()
    {
        var list = new List<UpdateHistoryInfo>();
        try
        {
            Type t = Type.GetTypeFromProgID("Microsoft.Update.Session");
            if (t == null) return list;

            dynamic session = Activator.CreateInstance(t);
            dynamic searcher = session.CreateUpdateSearcher();
            int count = searcher.GetTotalHistoryCount();
            if (count > 0)
            {
                dynamic history = searcher.QueryHistory(0, Math.Min(count, 50));
                foreach (var entry in history)
                {
                    list.Add(new UpdateHistoryInfo
                    {
                        Title = entry.Title,
                        Description = entry.Description,
                        Date = entry.Date.ToString("yyyy-MM-dd HH:mm"),
                        Operation = entry.Operation.ToString(),
                        ResultCode = entry.ResultCode.ToString()
                    });
                }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}