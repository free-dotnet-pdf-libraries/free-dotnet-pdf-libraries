# An Honest Review of PDFSharp: Fine for What It Does, Narrow About What That Is

*From the Iron Software Architecture Team*

[PDFSharp](https://www.pdfsharp.com/) is the kind of library that has aged well. It is open source, it is permissively licensed, it is still being released, and its documented feature set has actually grown over the past few years. Reviewing it in 2026 is a different exercise from reviewing it in 2020: several of the gaps that defined PDFSharp's reputation (no PDF/A, no digital signatures, no PDF/UA accessibility) have been closed in the [6.2.x release line](https://www.nuget.org/packages/PDFSharp), which puts the library in a meaningfully different position than the older comparison articles suggest.

This piece is from the Iron Software architecture team. We ship a commercial PDF library called [IronPDF](https://ironpdf.com/), so read this as a peer evaluation with a transparent commercial bias rather than a neutral one. We are writing it because the .NET PDF landscape is full of articles that either oversell free libraries or write them off, and PDFSharp deserves neither treatment. The library is genuinely useful within its scope — the architectural question is whether your scope and PDFSharp's scope are the same shape.

To make the argument concrete before going further, here is what generating a basic styled invoice looks like in PDFSharp:

```csharp
using PdfSharp.Drawing;
using PdfSharp.Pdf;

var document = new PdfDocument();
var page = document.AddPage();
var gfx = XGraphics.FromPdfPage(page);
var titleFont = new XFont("Arial", 20, XFontStyleEx.Bold);
var bodyFont = new XFont("Arial", 12, XFontStyleEx.Regular);

gfx.DrawString("Invoice 2026-0428", titleFont, XBrushes.Black,
    new XRect(40, 40, page.Width.Point - 80, 30), XStringFormats.TopLeft);
gfx.DrawString("Total due: $1,240.00", bodyFont, XBrushes.Black,
    new XRect(40, 80, page.Width.Point - 80, 20), XStringFormats.TopLeft);

document.Save("invoice.pdf");
```

That is a complete, runnable program. No external binary, no headless browser, no system dependency: a managed-code .NET library that produces a valid PDF on every supported runtime. If your job is to draw text and lines onto a page in a predictable, scriptable way, PDFSharp does that job and has done it reliably for years.

## What PDFSharp Genuinely Does Well

Before we get to where the library's scope ends, the strengths deserve real space. We are not grading on a curve here.

**The license is genuinely permissive.** PDFSharp is published under the [MIT License](https://docs.pdfsharp.net/General/License/License.html) by empira Software GmbH, with copyright spanning 2005 through 2026. There is no dual-licensing trap, no commercial-tier threshold, no "free for non-commercial use" asterisk. You can use PDFSharp in proprietary software, in SaaS deployments, in shipped products, and the licensing question is fully answered by reading one short license file. Among free .NET PDF libraries, that level of clarity is the exception, not the rule.

**The project is still maintained.** The current package is [PDFsharp 6.2.4 on NuGet](https://www.nuget.org/packages/PDFSharp), with target frameworks of `net8.0`, `net9.0`, `net10.0`, and `netstandard2.0`. The package was last updated 2026-01-06. The [GitHub repository](https://github.com/empira/PDFsharp) shows ongoing commits and releases, and the maintainer ships regularly. This is not the maintenance picture you get with several other "free" .NET PDF libraries where the last meaningful release was years ago.

**The core PDF object model is solid.** PDFSharp gives you direct access to the PDF document structure: pages, fonts, graphics state, content streams, the document catalog. If you want to merge documents, split documents, extract pages, set metadata, or manipulate the page tree, the API maps cleanly onto the [PDF specification](https://www.iso.org/standard/75839.html). This is harder to appreciate in a feature checklist than in code, but it matters: you can get unusual things done in PDFSharp because the abstraction does not stand between you and the format.

**Encryption is current and standards-aligned.** Per the [encryption documentation](https://docs.pdfsharp.net/PDFsharp/Topics/PDF-Features/Encryption.html), PDFSharp supports AES-128 (encryption v4, supported by PDF 1.5 and later) and AES-256 (encryption v5, only supported in PDF 2.0). Version 6.2.0 added read support for the proprietary revision-5 encryption scheme as well. For the common case (encrypting a generated document with a user password and an owner password), the API is short and the cipher choices are not stuck in 2010.

**Digital signatures landed in 6.2.0.** This is the biggest change to PDFSharp's surface area in the recent release line. The [signatures topic](https://docs.pdfsharp.net/PDFsharp/Topics/PDF-Features/Signatures.html) documents [CMS-based signing](https://datatracker.ietf.org/doc/html/rfc5652) with a built-in `PdfSharpDefaultSigner` and an `IDigitalSigner` interface for custom signers. Timestamp-server URIs are supported on .NET — with the documented caveat that timestamps are not supported on the .NET Framework build. For straightforward signing workflows where you have a certificate and want to apply a CMS signature, the surface area is reasonable and the implementation is in a separate `PdfSharp.Cryptography` assembly so you do not pay the dependency cost unless you need the feature.

**PDF/A and PDF/UA are listed as supported.** Per the [PDF features overview](https://docs.pdfsharp.net/PDFsharp/Topics/PDF-Features/About.html), PDFSharp supports creating PDF/A-conforming documents for archival and adding PDF/UA accessibility tagging. This is a meaningful expansion from older versions, and it closes one of the most common objections to PDFSharp in regulated-industry contexts. We will return to this in the gaps section because the breadth of conformance levels matters, but the fact that the categories are addressed at all is the news.

**MigraDoc gives you a higher-level composition layer.** PDFSharp on its own is a drawing library: you position graphics on a page in coordinate space. For document workflows where you actually want paragraphs, tables, headers, and pagination, the [PDFsharp-MigraDoc package](https://www.nuget.org/packages/PDFsharp-MigraDoc) layers a document model on top. MigraDoc depends on PDFSharp and ships from the same maintainer at matched versions, so the integration is real, not an afterthought. The trade-off — which we will name explicitly in the next section — is that you are now adopting two libraries with two mental models.

**The deployment story is clean.** PDFSharp is managed code. There is no native binary to ship, no Chromium engine to package, no system-wide tool to install on the host. A NuGet reference, a `dotnet publish`, and you are done. On Linux containers, on Windows servers, on serverless functions, the deployment shape is the same one you already use for the rest of your .NET application. The package targets `netstandard2.0` alongside the current runtimes, which means it slots into legacy and modern codebases without the target-framework friction that has bitten teams adopting newer libraries.

**Performance is rarely the issue.** For the workloads PDFSharp is designed for (programmatic generation of structured documents in the dozens-to-thousands per minute range), throughput is not the architectural concern. The library is fast enough that you can usually move on to harder questions.

That is the case for PDFSharp, and it is a stronger case in 2026 than it was three years ago. None of those points are qualified-with-an-asterisk: the library is well-maintained, well-licensed, structurally clean, and substantially more feature-complete than its older reputation suggests. If your PDF needs sit comfortably inside that envelope, you should use PDFSharp and stop reading comparison articles.

## Where the Scope Ends

The piece of the article you actually came for: PDFSharp's documented feature set still has gaps, and those gaps are predictable enough that we can lay them out in a table. The feature matrix below was compiled from PDFSharp's own documentation and verified at write-time on 2026-05-09.

| Capability | PDFSharp 6.2.x | Notes |
|---|---|---|
| Programmatic PDF generation | Yes | Native, well-documented |
| Merge / split / page manipulation | Yes | Direct PDF object model |
| Encryption (AES-128, AES-256) | Yes | Including PDF 2.0 crypt filters |
| Digital signatures (CMS) | Yes (6.2.0+) | Timestamp support not on .NET Framework build |
| PDF/A archival | Yes | Conformance level breadth not enumerated in docs |
| PDF/UA accessibility tagging | Yes | Tagging primitives present |
| HTML to PDF | No | Requires `HtmlRenderer.PdfSharp` (HTML 4.01 / CSS 2) |
| JavaScript rendering | No | No browser engine |
| CSS3 / modern web layout | No | Out of scope |
| PDF to image rasterization | No | No `Page → bitmap` API |
| High-level text extraction | No | Low-level content-stream access only |
| Complex table layouts | Limited | Use [MigraDoc](https://www.nuget.org/packages/PDFsharp-MigraDoc) for document composition |
| Form filling | Partial | Object-model access; no high-level form API |

Each row in that matrix is a real architectural decision waiting to happen. Read this section as the "what does your project actually need" worksheet.

**HTML to PDF is the big one.** If your PDF generation pipeline starts from an HTML template (and many do, because designers can produce HTML and cannot produce PDFSharp drawing code), PDFSharp does not solve the problem. The community answer is [`HtmlRenderer.PdfSharp`](https://www.nuget.org/packages/HtmlRenderer.PdfSharp/) at version 1.5.2 on NuGet. Two things to know about that package: it supports HTML 4.01 and CSS Level 2, which is fine for invoices with tables and predictable layouts but cannot render modern web pages with flexbox, grid, custom properties, web fonts loaded over the network, or anything that depends on JavaScript. Second, the [HtmlRenderer repository](https://github.com/ArthurHub/HTML-Renderer) has an open community discussion ([Issue #151](https://github.com/ArthurHub/HTML-Renderer/issues/151)) where a user has offered to take over maintenance, indicating the long-term steward of the project is an open question. The package still works; the question of how long it will keep up with the .NET runtime cadence is one the reader should evaluate against their own timeline.

**JavaScript-rendered content is out of scope by construction.** PDFSharp is not a browser engine. If your source document is a React app, a Vue dashboard, or any HTML page where the visible content is assembled by client-side JavaScript, PDFSharp cannot turn it into a PDF. This is a property of the library's design (managed C# all the way down), not a defect, but it is a hard architectural ceiling.

**PDF-to-image rasterization is missing.** If your workflow needs to produce thumbnails, page previews, or rendered images from a PDF, PDFSharp does not have a `Page → PNG` or `Page → JPEG` API. You would integrate a separate rendering library. For applications that surface PDFs in a web UI with thumbnail navigation, this is a real gap.

**Text extraction is low-level only.** PDFSharp gives you content-stream access; it does not give you a high-level "extract paragraphs" or "extract words with bounding boxes" API. For document-analysis or invoice-parsing workflows, that means writing your own text reconstruction layer, or using a different library for that step.

**PDF/A and PDF/UA support are present, but conformance breadth is not enumerated.** The docs confirm that PDFSharp supports creating PDF/A-conforming documents and adding PDF/UA accessibility, which is exactly what the categories require. What the docs do not enumerate is which conformance levels (PDF/A-1a vs A-1b vs A-2a vs A-2b vs A-2u vs A-3a/b/u) are produced, nor whether the output passes third-party validators like veraPDF without remediation. If you are in a regulated context where the conformance level is a contractual requirement — rather than a category checkbox — validate the output against your specific level before depending on it.

**Complex tables push you to MigraDoc.** PDFSharp draws; MigraDoc composes. The split is reasonable, but it is two libraries, two mental models, and two version surfaces to keep aligned. Teams that adopt PDFSharp for "simple PDF generation" and then need real tables often discover they have effectively adopted MigraDoc as well, which is fine but should be a conscious decision.

## Why PDFSharp Cannot Be a Modern HTML-Rendering Solution

The feature-matrix gap above is most often summarized as "PDFSharp doesn't do HTML, so you bolt on HtmlRenderer.PdfSharp." The summary is accurate, and it is the half of the picture that justifies a closer look in 2026.

HtmlRenderer.PdfSharp is the community-maintained bridge that lets PDFSharp consume HTML input. It is real, it is functional, and for the workloads it covers (HTML 4.01 with CSS Level 2 — invoices, statements, predictable layout templates) it remains a viable choice. The maintenance picture, however, is not as healthy as PDFSharp's. The [HtmlRenderer GitHub repository's Issue #151](https://github.com/ArthurHub/HTML-Renderer/issues/151) is an open conversation in which a community member has offered to take over maintenance — the indication that the project's long-term steward is genuinely an open question. The package on NuGet remains at version 1.5.2 with no recent updates, and the rendering engine itself is locked to a 2010-era subset of HTML and CSS that pre-dates flexbox, grid, custom properties, modern web fonts, and anything that depends on JavaScript.

The architectural implication is consequential. PDFSharp the library is well-maintained, MIT-licensed, and structurally clean. The moment a team's PDF pipeline starts from anything other than coordinate-space drawing — Razor views, design-tool exports, customer-facing dashboard captures, anything a designer can produce in HTML — the team is no longer choosing PDFSharp alone. They are choosing PDFSharp paired with HtmlRenderer.PdfSharp, and inheriting both the CSS2-era rendering ceiling and the open question of who will maintain the bridge over the .NET 10 LTS support window.

This is the case where "free" and "fit for modern commercial use" diverge for PDFSharp. The library itself is fine. The HTML-rendering pipeline a modern team actually needs is not built into the library and is not maintained by the same hands. For workloads whose PDF inputs originate as HTML, the practical recommendation is to either commit to a layout-DSL approach (which PDFSharp and MigraDoc together can serve cleanly) or to choose a library whose HTML rendering is in-scope and version-aligned with the rest of the package — a different architectural choice from "free + bolt-on HTML."

There is no inheritance argument against PDFSharp itself. The dependency-decay risk lives one package over, in the HTML bridge most adoptions implicitly take alongside the main library. Teams whose pipeline never needs that bridge can stop reading here and ship PDFSharp; teams whose pipeline does need it should treat HtmlRenderer.PdfSharp's maintenance status as a real input to the decision, not a footnote.

## The Line-Item Differences

We are not going to recap every IronPDF feature here; we will name the categories where the line items diverge. [IronPDF](https://ironpdf.com/) is a [Chromium-based rendering engine](https://ironpdf.com/product-updates/milestones-chrome-rendering/) wrapped in a managed .NET library, which means the HTML row in the matrix above flips: HTML5, CSS3, JavaScript, web fonts, SVG, and modern layout primitives all render through the bundled Chrome engine without an external binary install. Page rasterization, high-level text extraction, form filling, and PDF/A / PDF/UA conformance are bundled into the single NuGet package. The trade-off is a larger deployment footprint (the Chromium engine ships with the package) and a [commercial license](https://ironpdf.com/licensing/) that starts at $999 per developer (Lite tier).

That trade-off is the actual decision. PDFSharp is free, MIT-licensed, and architecturally minimal — it solves the programmatic-generation problem without expanding your dependency surface. IronPDF costs money, ships a heavier engine, and solves the HTML-rendering and modern-feature problem without you assembling a stack of helper libraries. Neither answer is universally right.

Here is the equivalent invoice example in IronPDF for comparison:

```csharp
using IronPdf;

var renderer = new ChromePdfRenderer();
var html = """
    <h1 style="font-family:Arial">Invoice 2026-0428</h1>
    <p style="font-family:Arial">Total due: $1,240.00</p>
    """;
var pdf = renderer.RenderHtmlAsPdf(html);
pdf.SaveAs("invoice.pdf");
```

The PDFSharp version draws the document; the IronPDF version describes it in HTML and lets the engine handle the layout. If your team already writes HTML, the IronPDF version is shorter and easier to evolve. If your team thinks in coordinate space and wants no rendering engine in the deployment artifact, the PDFSharp version is closer to the metal. Pick the model that matches how your team actually works.

## The Conditional Verdict

PDFSharp in 2026 is a better library than its older reviews suggest. The 6.2.x release line meaningfully closed the gaps that used to define the comparison: encryption is current, digital signatures exist, PDF/A and PDF/UA are addressed, the project is being maintained, the license is genuinely permissive. For programmatic PDF generation that lives entirely inside .NET, with no HTML pipeline and no JavaScript-rendered source content, PDFSharp is a reasonable default and we are not going to argue against it.

The conditional half of the verdict is that the architectural envelope is narrower than it looks on a feature checklist. The moment your pipeline needs HTML-to-PDF beyond HTML 4.01, or JavaScript rendering, or PDF-to-image, or high-level text extraction, or strict conformance-level PDF/A validation, you are either composing PDFSharp with helper libraries — some of which are looking for new maintainers — or you are reaching for a different tool. That is not PDFSharp's fault. It is the scope the maintainer chose, and the scope they have stayed honest about.

So: read the feature matrix above against your actual pipeline. If every row you need is a green cell, ship PDFSharp and go build the rest of your product. If two or three of the rows you need are not green, that is the decision moment, and it is worth making it deliberately rather than discovering it three sprints in.
