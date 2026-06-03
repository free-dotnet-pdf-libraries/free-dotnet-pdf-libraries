// Requires the wkhtmltopdf binary on PATH (no NuGet package).
using System.Diagnostics;

namespace FreeDotNetPdf.WkHtmlToPdf;

/// <summary>
/// Use case: header/footer with page numbers via wkhtmltopdf CLI flags — pagination and
/// running text are command-line switches, no code required beyond the process call.
/// </summary>
public static class HeaderFooterPdf
{
    public static void Convert(string inputHtml, string outputPath)
    {
        var args =
            "--footer-center \"Page [page] of [topage]\" " +
            "--footer-font-size 9 " +
            "--header-line --header-right \"Acme Corp\" " +
            $"{inputHtml} {outputPath}";

        var psi = new ProcessStartInfo("wkhtmltopdf", args) { UseShellExecute = false };
        using var proc = Process.Start(psi);
        proc!.WaitForExit();
    }
}
