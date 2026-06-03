using System;
using System.Diagnostics;

namespace FreeDotNetPdf.WkHtmlToPdf;

/// <summary>
/// Use case: compose a cover page + auto-generated table of contents + chapters.
/// wkhtmltopdf builds a clickable TOC from your headings via positional objects
/// (cover &lt;file&gt;, toc, then the content pages).
/// </summary>
public static class CoverAndTocPdf
{
    public static void Build(string coverHtml, string[] chapters, string outputPath)
    {
        var args = $"cover {coverHtml} toc {string.Join(' ', chapters)} {outputPath}";

        var psi = new ProcessStartInfo("wkhtmltopdf", args) { UseShellExecute = false };
        using var proc = Process.Start(psi);
        proc!.WaitForExit();
    }
}
