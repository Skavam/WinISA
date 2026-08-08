using System.Globalization;
using WinISA.Models;

namespace WinISA.Collectors;

public static class LocaleCollector
{
    public static LocaleInfo? GetInfo()
    {
        try
        {
            var ci = CultureInfo.CurrentCulture;
            var ui = CultureInfo.CurrentUICulture;
            var installed = CultureInfo.InstalledUICulture;

            return new LocaleInfo
            {
                CurrentCulture = ci.Name,
                CurrentUICulture = ui.Name,
                InstalledUICulture = installed.Name,
                DisplayName = ci.DisplayName,
                EnglishName = ci.EnglishName
            };
        }
        catch
        {
            return null;
        }
    }
}