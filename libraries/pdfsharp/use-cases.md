# PdfSharp — 3 Use Cases Where It Excels

PdfSharp is mature, MIT-licensed, **pure managed** code — no native binaries, no Chromium, predictable memory. It does not render modern HTML, but it is excellent for programmatic generation and for **manipulating existing PDFs** (merge, split, stamp) with a tiny deployment footprint that runs anywhere .NET runs.

> Verified against the [PdfSharp/MigraDoc docs](https://docs.pdfsharp.net/) and community references (June 2026). **As of 6.2.0**, set a font resolver at startup before drawing text — see the note at the bottom.

---

### 1. Merge many PDFs into one

The classic batch-back-office task: combine generated statements, cover sheets, and attachments into a single deliverable.

```csharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

var output = new PdfDocument();
foreach (var file in new[] { "cover.pdf", "body.pdf", "appendix.pdf" })
{
    using var input = PdfReader.Open(file, PdfDocumentOpenMode.Import);
    foreach (var page in input.Pages)
        output.AddPage(page);
}
output.Save("merged.pdf");
```

### 2. Split / extract pages

Burst a large PDF into per-page (or per-range) files — useful for document-management ingestion and per-recipient splitting.

```csharp
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

using var input = PdfReader.Open("bundle.pdf", PdfDocumentOpenMode.Import);
for (int i = 0; i < input.PageCount; i++)
{
    var doc = new PdfDocument();
    doc.AddPage(input.Pages[i]);
    doc.Save($"page-{i + 1}.pdf");
}
```

### 3. Stamp a watermark onto existing pages

Open in `Modify` mode and draw over every page — the standard "CONFIDENTIAL / DRAFT" overlay without a heavyweight library.

```csharp
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

using var doc = PdfReader.Open("report.pdf", PdfDocumentOpenMode.Modify);
var font = new XFont("Arial", 48);
foreach (var page in doc.Pages)
{
    var gfx = XGraphics.FromPdfPage(page);
    gfx.RotateAtTransform(-40, new XPoint(page.Width / 2, page.Height / 2));
    gfx.DrawString("CONFIDENTIAL", font,
        new XSolidBrush(XColor.FromArgb(50, 255, 0, 0)),
        new XRect(0, 0, page.Width, page.Height), XStringFormats.Center);
}
doc.Save("stamped.pdf");
```

> **Font note (6.2+):** on Linux/containers or when drawing text, register fonts at startup — `GlobalFontSettings.FontResolver = new MyFontResolver();` — or text rendering will throw. For document-style layout (flowing text, tables) use its sibling **MigraDoc** on top of PdfSharp.
