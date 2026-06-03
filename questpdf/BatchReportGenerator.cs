using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FreeDotNetPdf.QuestPdf;

/// <summary>
/// Use case: high-throughput batch generation. QuestPDF is pure managed code with no
/// browser/native binary, so it generates thousands of pages/sec and parallelizes cleanly.
/// </summary>
public sealed record Customer(string Id, string Name);

public static class BatchReportGenerator
{
    public static void GenerateAll(IEnumerable<Customer> customers, string outputDir)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        Directory.CreateDirectory(outputDir);

        Parallel.ForEach(customers, customer =>
        {
            Document.Create(doc => doc.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.Content().Text($"Statement for {customer.Name}");
            }))
            .GeneratePdf(Path.Combine(outputDir, $"{customer.Id}.pdf"));
        });
    }
}
