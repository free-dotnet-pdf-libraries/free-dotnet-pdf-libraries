// NuGet: QuestPDF, ScottPlot
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FreeDotNetPdf.QuestPdf;

/// <summary>
/// Use case: analytics report embedding a ScottPlot chart as crisp vector SVG
/// (QuestPDF renders SVG as vector graphics, not a rasterized bitmap).
/// Requires the ScottPlot NuGet package alongside QuestPDF.
/// </summary>
public static class ChartReport
{
    public static void Generate(string outputPath, double[] values)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        var plot = new ScottPlot.Plot();
        plot.Add.Bars(values);
        string svg = plot.GetSvgXml(600, 300);

        Document.Create(doc => doc.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.Header().Text("Quarterly Revenue").Bold();
            page.Content().Svg(svg);
        }))
        .GeneratePdf(outputPath);
    }
}
