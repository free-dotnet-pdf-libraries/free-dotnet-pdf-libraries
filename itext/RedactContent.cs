// NuGet: itext7, itext7.pdfsweep
using iText.Kernel.Pdf;
using iText.PdfCleanup;
using iText.PdfCleanup.Autosweep;

namespace FreeDotNetPdf.IText;

/// <summary>
/// Use case: irreversible redaction with the pdfSweep add-on — truly removes content
/// (not just a black box over it) and can auto-target by regex. Right tool for scrubbing
/// PII/PHI before disclosure. Requires the iText.PdfCleanup (pdfSweep) package.
/// </summary>
public static class RedactContent
{
    public static void RedactSsns(string inputPath, string outputPath)
    {
        using var pdf = new PdfDocument(new PdfReader(inputPath), new PdfWriter(outputPath));

        var strategy = new RegexBasedCleanupStrategy(@"\d{3}-\d{2}-\d{4}"); // e.g. US SSNs
        PdfCleaner.AutoSweepCleanUp(pdf, strategy);
    }
}
