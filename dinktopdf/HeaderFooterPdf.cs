// NuGet: DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;

namespace FreeDotNetPdf.DinkToPdf;

/// <summary>
/// Use case: HTML to PDF with a header line and footer page numbers, using
/// wkhtmltopdf's [page]/[topage] placeholders exposed through DinkToPdf settings.
/// </summary>
public static class HeaderFooterPdf
{
    public static byte[] Convert(IConverter converter, string html)
    {
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = { PaperSize = PaperKind.A4 },
            Objects =
            {
                new ObjectSettings
                {
                    HtmlContent = html,
                    HeaderSettings = new HeaderSettings { FontSize = 9, Line = true, Right = "Page [page] of [topage]" },
                    FooterSettings = new FooterSettings { FontSize = 9, Center = "Confidential" }
                }
            }
        };

        return converter.Convert(doc);
    }
}
