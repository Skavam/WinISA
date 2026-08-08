using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using WinISA.Models;

namespace WinISA.Collectors;

public static class ServiceCollector
{
    public static List<ServiceInfo> GetInfo()
    {
        var list = new List<ServiceInfo>();
        try
        {
            var services = ServiceController.GetServices();
            foreach (var svc in services.OrderBy(s => s.DisplayName))
            {
                try
                {
                    list.Add(new ServiceInfo
                    {
                        DisplayName = svc.DisplayName,
                        ServiceName = svc.ServiceName,
                        Status = svc.Status.ToString(),
                        StartType = svc.StartType.ToString()
                    });
                }
                catch { /* skip this service */ }
            }
        }
        catch { /* ignore */ }

        return list;
    }
}