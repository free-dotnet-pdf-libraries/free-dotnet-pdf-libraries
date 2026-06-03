// NuGet: PdfSharp
using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FreeDotNetPdf.PdfSharp;

/// <summary>
/// Use case: burst a large PDF into one file per page — useful for ingestion and
/// per-recipient splitting.
/// </summary>
public static class SplitPdf
{
    public static void SplitPerPage(string inputPath, string outputDir)
    {
        Directory.CreateDirectory(outputDir);
        using var input = PdfReader.Open(inputPath, PdfDocumentOpenMode.Import);

        for (int i = 0; i < input.PageCount; i++)
        {
            var doc = new PdfDocument();
            doc.AddPage(input.Pages[i]);
            doc.Save(Path.Combine(outputDir, $"page-{i + 1}.pdf"));
        }
    }
}
