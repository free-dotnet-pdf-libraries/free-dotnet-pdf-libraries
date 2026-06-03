// NuGet: PdfSharp
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

namespace FreeDotNetPdf.PdfSharp;

/// <summary>
/// Use case: stamp a diagonal watermark onto every page of an existing PDF.
/// NOTE (PdfSharp 6.2+): on Linux/containers, register a font resolver at startup
/// before drawing text, e.g. GlobalFontSettings.FontResolver = new MyFontResolver();
/// otherwise text rendering throws.
/// </summary>
public static class WatermarkStamp
{
    public static void Stamp(string inputPath, string outputPath, string text = "CONFIDENTIAL")
    {
        using var doc = PdfReader.Open(inputPath, PdfDocumentOpenMode.Modify);
        var font = new XFont("Arial", 48);

        foreach (var page in doc.Pages)
        {
            var gfx = XGraphics.FromPdfPage(page);
            gfx.RotateAtTransform(-40, new XPoint(page.Width / 2, page.Height / 2));
            gfx.DrawString(text, font,
                new XSolidBrush(XColor.FromArgb(50, 255, 0, 0)),
                new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
        }

        doc.Save(outputPath);
    }
}
