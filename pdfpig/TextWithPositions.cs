// NuGet: PdfPig
using System;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace FreeDotNetPdf.PdfPig;

/// <summary>
/// Use case: precise text extraction — every word exposes its bounding box,
/// so you get not just what the text says but exactly where it sits on the page.
/// </summary>
public static class TextWithPositions
{
    public static void Dump(string pdfPath)
    {
        using var document = PdfDocument.Open(pdfPath);
        foreach (var page in document.GetPages())
        {
            foreach (Word word in page.GetWords())
            {
                var box = word.BoundingBox;
                Console.WriteLine($"{word.Text} @ ({box.Left:F0},{box.Bottom:F0})");
            }
        }
    }
}
