// NuGet: DinkToPdf
using DinkToPdf;
using DinkToPdf.Contracts;

namespace FreeDotNetPdf.DinkToPdf;

/// <summary>
/// Use case: convert an HTML string to a PDF byte[] (e.g. to stream straight back from a
/// web API endpoint — no temp files). Inject the shared IConverter (see ConverterRegistration).
/// </summary>
public static class HtmlStringToPdf
{
    public static byte[] Convert(IConverter converter, string html)
    {
        var doc = new HtmlToPdfDocument
        {
            GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
            Objects = { new ObjectSettings { HtmlContent = html } }
        };

        return converter.Convert(doc);
    }
}
