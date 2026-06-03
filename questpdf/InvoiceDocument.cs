using System.Collections.Generic;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FreeDotNetPdf.QuestPdf;

/// <summary>
/// Use case: data-driven invoice with an auto-paginating table.
/// The header row repeats automatically when the table spans multiple pages.
/// </summary>
public sealed record InvoiceLine(string Name, decimal Total);

public static class InvoiceDocument
{
    public static void Generate(string outputPath, string invoiceNumber, IEnumerable<InvoiceLine> lines)
    {
        QuestPDF.Settings.License = LicenseType.Community;

        Document.Create(doc => doc.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.Header().Text($"Invoice #{invoiceNumber}").FontSize(20).Bold();

            page.Content().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn();
                    columns.ConstantColumn(90);
                });

                table.Header(header =>
                {
                    header.Cell().Text("Item").Bold();
                    header.Cell().AlignRight().Text("Amount").Bold();
                });

                foreach (var line in lines)
                {
                    table.Cell().Text(line.Name);
                    table.Cell().AlignRight().Text($"${line.Total:N2}");
                }
            });

            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Page ");
                text.CurrentPageNumber();
            });
        }))
        .GeneratePdf(outputPath);
    }
}
