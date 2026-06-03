// Requires the wkhtmltopdf binary on PATH (no NuGet package).
using System.Diagnostics;

namespace FreeDotNetPdf.WkHtmlToPdf;

/// <summary>
/// Use case: convert a URL or HTML file to PDF by invoking the wkhtmltopdf binary from .NET.
/// No library dependency — works identically across languages and CI scripts.
/// (wkhtmltopdf must be installed and on PATH.)
/// </summary>
public static class UrlToPdf
{
    public static void Convert(string source, string outputPath)
    {
        var psi = new ProcessStartInfo("wkhtmltopdf", $"{source} {outputPath}")
        {
            RedirectStandardError = true,
            UseShellExecute = false
        };

        using var proc = Process.Start(psi);
        proc!.WaitForExit();
    }
}
