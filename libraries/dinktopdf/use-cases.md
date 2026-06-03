# DinkToPdf — 3 Use Cases Where It Excels

DinkToPdf is a thin C# wrapper over the **wkhtmltopdf** native library (P/Invoke, no CLI process). Its niche is *simple, CSS2-era HTML → PDF* inside an existing .NET service, with a strongly-typed API and `byte[]` output. It excels at legacy-friendly, low-ceremony generation — but inherits wkhtmltopdf's **archived/unmaintained** status, so treat it as maintenance-grade, not a new-project default.

> Verified against the [DinkToPdf repo](https://github.com/rdvojmoc/DinkToPdf) and its `GlobalSettings`/`ObjectSettings` source (June 2026).

---

### 1. HTML string → PDF byte array for web download

The most common pattern: render a Razor/HTML string to bytes and stream it straight back from an API endpoint — no temp files.

```csharp
using DinkToPdf;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument
{
    GlobalSettings = { PaperSize = PaperKind.A4, Orientation = Orientation.Portrait },
    Objects = { new ObjectSettings { HtmlContent = htmlString } }
};

byte[] pdf = converter.Convert(doc);     // return File(pdf, "application/pdf")
```

### 2. Headers, footers, and page numbers

DinkToPdf exposes wkhtmltopdf's header/footer settings with `[page]`/`[topage]` placeholders — handy for paginated reports.

```csharp
using DinkToPdf;

var settings = new ObjectSettings
{
    HtmlContent = htmlString,
    HeaderSettings = new HeaderSettings { FontSize = 9, Line = true,
                                          Right = "Page [page] of [topage]" },
    FooterSettings = new FooterSettings { FontSize = 9, Center = "Confidential" }
};
```

### 3. Thread-safe singleton inside ASP.NET Core DI

`SynchronizedConverter` serializes calls into the non-reentrant native library, so it is registered **once** as a singleton — the correct, crash-avoiding pattern for web apps.

```csharp
using DinkToPdf;
using DinkToPdf.Contracts;

// Program.cs — register the native converter a single time for the whole app
builder.Services.AddSingleton(typeof(IConverter),
    new SynchronizedConverter(new PdfTools()));

// then inject IConverter wherever you generate PDFs
```

> **Health warning:** wkhtmltopdf was archived in 2023 (no security patches), native binary loading is fragile on Alpine/Docker, and the Qt WebKit engine predates Flexbox/Grid. For new work prefer **PuppeteerSharp** (modern HTML) or **QuestPDF** (code-first).
