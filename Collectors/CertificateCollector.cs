using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using WinISA.Models;

namespace WinISA.Collectors;

public static class CertificateCollector
{
    public static List<CertificateInfo> GetCertificates()
    {
        var list = new List<CertificateInfo>();
        var locations = new[] { StoreLocation.CurrentUser, StoreLocation.LocalMachine };

        foreach (var loc in locations)
        {
            try
            {
                using var store = new X509Store(StoreName.My, loc);
                store.Open(OpenFlags.ReadOnly);
                foreach (var cert in store.Certificates)
                {
                    list.Add(new CertificateInfo
                    {
                        Subject = cert.Subject,
                        Issuer = cert.Issuer,
                        SerialNumber = cert.SerialNumber,
                        Thumbprint = cert.Thumbprint,
                        NotBefore = cert.NotBefore.ToString("yyyy-MM-dd HH:mm"),
                        NotAfter = cert.NotAfter.ToString("yyyy-MM-dd HH:mm"),
                        StoreLocation = loc.ToString()
                    });
                }
                store.Close();
            }
            catch { /* skip this location */ }
        }

        return list.Take(50).ToList();
    }
}