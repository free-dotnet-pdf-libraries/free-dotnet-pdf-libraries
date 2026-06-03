# QuestPDF — 3 Use Cases Where It Excels

QuestPDF is a **code-first** generator: you describe documents with a fluent C# API and a layout engine built specifically for PDF. It shines when the *source of truth is C# data* (not HTML), when you need precise, paginated layout, and when throughput matters. It does **not** render HTML — pair it with PuppeteerSharp or iText for that.

> Verified against the [features overview](https://www.questpdf.com/features-overview.html), [charts API](https://www.questpdf.com/api-reference/charts.html), and [QuestPDF 2026.5.0 on NuGet](https://www.nuget.org/packages/QuestPDF/) (June 2026).

---

### 1. Data-driven invoices & statements with auto-paginating tables

Its strongest use case: structured business documents where rows come from a collection and the engine handles page breaks, repeating headers, and totals automatically.

```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;

QuestPDF.Settings.License = LicenseType.Community;

Document.Create(doc => doc.Page(page =>
{
    page.Size(PageSizes.A4);
    page.Margin(40);
    page.Header().Text("Invoice #1024").FontSize(20).Bold();

    page.Content().Table(table =>
    {
        table.ColumnsDefinition(c => { c.RelativeColumn(); c.ConstantColumn(90); });
        table.Header(h =>
        {
            h.Cell().Text("Item").Bold();
            h.Cell().AlignRight().Text("Amount").Bold();
        });
        foreach (var line in invoice.Lines)          // header repeats across pages automatically
        {
            table.Cell().Text(line.Name);
            table.Cell().AlignRight().Text($"${line.Total:N2}");
        }
    });

    page.Footer().AlignCenter().Text(t => { t.Span("Page "); t.CurrentPageNumber(); });
})).GeneratePdf("invoice.pdf");
```

### 2. High-throughput batch report generation

QuestPDF is pure managed code with no browser or native binary, so it generates **thousands of pages per second** and parallelizes cleanly — ideal for nightly statement runs or per-customer exports.

```csharp
using System.Threading.Tasks;
using QuestPDF.Fluent;

Parallel.ForEach(customers, customer =>
{
    Document.Create(doc => doc.Page(page =>
    {
        page.Margin(40);
        page.Content().Text($"Statement for {customer.Name}");
    }))
    .GeneratePdf($"out/{customer.Id}.pdf");
});
```

### 3. Analytics reports with native charts (ScottPlot → vector SVG)

QuestPDF embeds **SVG as crisp vector graphics**, so it pairs with ScottPlot/LiveCharts to drop charts straight into a report with no rasterization.

```csharp
using QuestPDF.Fluent;

var plot = new ScottPlot.Plot();
plot.Add.Bars(new double[] { 12, 19, 7, 23 });
string svg = plot.GetSvgXml(600, 300);

Document.Create(doc => doc.Page(page =>
{
    page.Margin(40);
    page.Header().Text("Q3 Revenue by Region").Bold();
    page.Content().Svg(svg);          // rendered as vector, not a bitmap
})).GeneratePdf("report.pdf");
```

> **License note:** MIT (Community) for companies under $1M annual revenue; a paid Professional/Enterprise license is required above that threshold. See the deep dive in this folder.
