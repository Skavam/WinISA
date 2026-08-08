using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WinISA.Models;

namespace WinISA.Collectors;

public static class FontCollector
{
    public static List<string> GetFonts()
    {
        var fonts = new List<string>();
        try
        {
            var fontFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows), "Fonts");
            if (Directory.Exists(fontFolder))
            {
                var files = Directory.GetFiles(fontFolder, "*.ttf")
                    .Concat(Directory.GetFiles(fontFolder, "*.otf"))
                    .Select(Path.GetFileNameWithoutExtension)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .Take(100);
                fonts.AddRange(files);
            }
        }
        catch { /* ignore */ }
        return fonts;
    }
}