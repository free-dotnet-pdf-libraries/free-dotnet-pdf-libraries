using System;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace FreeDotNetPdf.PdfPig;

/// <summary>
/// Use case: layout analysis for complex / multi-column documents. PdfPig ships
/// algorithms (Nearest Neighbour, Docstrum, Recursive XY Cut) that reconstruct
/// reading order where naive text extraction would scramble the output.
/// </summary>
public static class LayoutAnalysis
{
    public static void PrintBlocks(string pdfPath, int pageNumber = 1)
    {
        using var document = PdfDocument.Open(pdfPath);
        var page = document.GetPage(pageNumber);

        var words = NearestNeighbourWordExtractor.Instance.GetWords(page.Letters);
        var blocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);

        foreach (var block in blocks)
            Console.WriteLine(block.Text);
    }
}
