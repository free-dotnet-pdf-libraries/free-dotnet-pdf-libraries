// NuGet: PdfSharp
using System.Collections.Generic;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FreeDotNetPdf.PdfSharp;

/// <summary>
/// Use case: merge several PDFs into one — combine cover sheets, statements and attachments.
/// </summary>
public static class MergePdfs
{
    public static void Merge(IEnumerable<string> inputPaths, string outputPath)
    {
        var output = new PdfDocument();
        foreach (var file in inputPaths)
        {
            using var input = PdfReader.Open(file, PdfDocumentOpenMode.Import);
            foreach (var page in input.Pages)
                output.AddPage(page);
        }
        output.Save(outputPath);
    }
}
