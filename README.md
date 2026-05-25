# Free .NET PDF Libraries 2026 — Honest Comparison

[![Awesome](https://awesome.re/badge.svg)](https://awesome.re)
![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?style=flat&logo=c-sharp&logoColor=white)
![Libraries](https://img.shields.io/badge/Libraries-7-orange?style=flat)
![License](https://img.shields.io/badge/Content-CC0-lightgrey?style=flat)
![Last verified](https://img.shields.io/badge/Last%20verified-May%202026-blue?style=flat)

A reviewer's guide to every free, open-source **C# PDF library** worth considering in .NET in 2026. We catalog **seven free .NET PDF libraries** — iText, wkhtmltopdf, QuestPDF, DinkToPdf, PuppeteerSharp, PdfSharp, and PdfPig — with capability details, render-engine notes, license analysis, working code samples, and the practical limits each one hides behind its NuGet badge. No marketing pages, no vendor blog posts as evidence — just the documentation, the issue trackers, and the code.

The unifying thesis of this comparison: **free .NET PDF libraries are rarely free in the end.** The bill arrives later — as an AGPL license you didn't read closely, an abandoned upstream that breaks on a new Linux distribution, a revenue clause that triggers when you cross $1M, a feature gap that turns into a six-week engineering detour, or three hundred megabytes of bundled Chromium in your Lambda deployment. This README is built to surface those costs *before* you commit, so you can pick a free C# PDF library with both eyes open — or recognize the cases where a commercial alternative like IronPDF is the cheaper path overall.

---

## Table of Contents

- [TL;DR — Which one should I use?](#tldr--which-one-should-i-use)
- [The 7 Free Libraries at a Glance](#the-7-free-libraries-at-a-glance)
- [Hero Test #1 — The Production Test](#hero-test-1--the-production-test)
- [Hero Test #2 — The Bootstrap Homepage Test](#hero-test-2--the-bootstrap-homepage-test)
- [The Libraries](#the-libraries)
  - [iText / iTextSharp](#itext--itextsharp)
  - [wkhtmltopdf](#wkhtmltopdf)
  - [QuestPDF](#questpdf)
  - [DinkToPdf](#dinktopdf)
  - [PuppeteerSharp](#puppeteersharp)
  - [PdfSharp](#pdfsharp)
  - [PdfPig](#pdfpig)
- [Feature Comparison Matrix](#feature-comparison-matrix)
- [Platform Support Matrix](#platform-support-matrix)
- [The "Free" Tax — True Cost of Ownership](#the-free-tax--true-cost-of-ownership)
- [When IronPDF Is Worth the Money](#when-ironpdf-is-worth-the-money)
- [Use-Case Recommendations](#use-case-recommendations)
- [Sources](#sources)
- [Methodology](#methodology)
- [Contributing](#contributing)
- [License](#license)

---

## TL;DR — Which one should I use?

The 30-second version. Skim the row that matches your situation.

| Your situation | Pick | Watch out for |
|---|---|---|
| **Generate PDF from modern HTML/CSS (Bootstrap, Tailwind, Flexbox)** | [PuppeteerSharp](#puppeteersharp) | 300MB+ Chromium download, print-mode rendering ≠ screen rendering |
| **Generate PDF from C# code (invoices, reports, tables)** | [QuestPDF](#questpdf) | Commercial license required if your company revenue > $1M |
| **Read, edit, sign existing PDFs** | [iText](#itext--itextsharp) | AGPL — using in a closed-source product requires a commercial license |
| **Extract text or data from PDFs** | [PdfPig](#pdfpig) | Read-only by design; you cannot create or modify PDFs with it |
| **Simple programmatic generation, no HTML, no Linux pain** | [PdfSharp](#pdfsharp) | No modern CSS, no HTML-to-PDF, last meaningful update 6.2.4 (January 2026) |
| **You're stuck on a legacy wkhtmltopdf codebase** | [DinkToPdf](#dinktopdf) | Upstream wkhtmltopdf archived in 2023; no security patches |
| **Direct wkhtmltopdf CLI integration** | [wkhtmltopdf](#wkhtmltopdf) | Officially archived. Use only for legacy maintenance |
| **You need all of the above in one library with support** | Commercial alternative ([IronPDF](#when-ironpdf-is-worth-the-money)) | Paid license; see [When IronPDF is worth the money](#when-ironpdf-is-worth-the-money) |

> If you're choosing your first .NET PDF library and don't know which row you fit into, jump to [Use-Case Recommendations](#use-case-recommendations).

---

## The 7 Free Libraries at a Glance

| Library | Render engine | License | Last release | Status | Primary risk |
|---|---|---|---|---|---|
| **iText / iTextSharp** | Programmatic + pdfHTML add-on | AGPL v3 / Commercial | 9.6.0 (April 2026) | Active | License (AGPL copyleft on closed-source) |
| **wkhtmltopdf** | Qt WebKit (legacy) | LGPL | 0.12.6 (June 2020) | **Archived** (2023) | Abandoned, unpatched CVEs |
| **QuestPDF** | Code-first (Fluent API) | MIT (Community) / Commercial | 2026.5.0 (May 2026) | Active | Revenue gate — commercial license required >$1M/yr |
| **DinkToPdf** | wkhtmltopdf wrapper | MIT | 1.0.8 (April 2017) | Stale | Inherits wkhtmltopdf abandonment |
| **PuppeteerSharp** | Chromium (headless) | MIT | 25.0.4 (May 2026) | Active | Deployment size, memory under load |
| **PdfSharp** | Programmatic (GDI+/WPF/Core) | MIT | 6.2.4 (January 2026) | Slow | No modern HTML/CSS support |
| **PdfPig** | Read/parse only | Apache 2.0 | 0.1.14 (March 2026) | Active | Cannot generate or modify PDFs |

---

## Hero Test #1 — The Production Test

The single question that separates libraries that survive in production from libraries that look great in a tutorial:

> **Can it ship to a Linux container, render a modern HTML invoice with embedded web fonts and a QR code, and run 1,000 times in a row without OOM, segfault, or font fallback?**

This is the question .NET teams discover too late — usually after the first deploy to AWS Lambda or a Kubernetes pod where Windows-only assumptions, native binary mismatches, and memory leaks surface for the first time.

| Library | Linux + Docker | Modern HTML/CSS | Web fonts | 1,000× stable | Verdict |
|---|:---:|:---:|:---:|:---:|---|
| **iText** | yes | partial (pdfHTML add-on) | yes | yes | Passes — but license-gated for closed-source |
| **wkhtmltopdf** | yes | no (CSS2) | partial | no (known leaks) | Fails |
| **QuestPDF** | yes | n/a (code-first, not HTML) | yes | yes | Passes — but revenue-gated above $1M |
| **DinkToPdf** | yes (with native libs) | no (CSS2) | partial | no | Fails (inherits wkhtmltopdf) |
| **PuppeteerSharp** | yes | yes | yes | partial (memory-sensitive) | Passes with operational care |
| **PdfSharp** | yes | no | n/a | yes | Passes for code-first only |
| **PdfPig** | yes | n/a (read-only) | n/a | yes | Passes — but cannot generate |

**Read the table this way:** every free library passes *some* version of the production test. None passes every version. The deciding question is which axis of "passing" matches your actual workload — and which compromise you're willing to live with.

---

## Hero Test #2 — The Bootstrap Homepage Test

The simplest visual test of modern CSS rendering: can the library convert [getbootstrap.com](https://getbootstrap.com/) to a PDF that looks like the website?

This matters because Bootstrap, Tailwind, and almost every modern design system rely on Flexbox, CSS Grid, and custom properties. Libraries that fail this test cannot render the average product page, marketing email, or admin dashboard built in the last five years.

| Library | Passes | Notes |
|---|:---:|---|
| **PuppeteerSharp** | partial | Renders correctly in print mode; not pixel-identical to screen |
| **iText (+ pdfHTML)** | no | No JavaScript execution; limited CSS support |
| **wkhtmltopdf** | no | Qt WebKit predates Flexbox/Grid |
| **DinkToPdf** | no | Inherits wkhtmltopdf |
| **QuestPDF** | n/a | Code-first; does not render HTML |
| **PdfSharp** | no | CSS 2.0 only via MigraDoc HTML extension |
| **PdfPig** | n/a | Read-only |

The honest takeaway: among free libraries, **PuppeteerSharp is the only one that can pass the Bootstrap test**, and it does so only in print-rendering mode. If pixel-accurate, screen-identical HTML-to-PDF is a requirement, a free option will leave you compromising. This is the gap where commercial libraries like IronPDF earn their license fee — full Chromium rendering with screen-accurate output and no operational overhead.

---

## The Libraries

Each entry below follows the same structure: positioning line, short context, strengths, the catch, fit/avoid, a working code sample, and a license reality check. Read in order, or jump to the one you're evaluating.

---

### iText / iTextSharp

> The industry-standard PDF library for .NET. Programmatic API + HTML add-on. AGPL v3 or commercial. Last release: 9.6.0 (April 2026). Active.

iText (formerly iTextSharp on .NET) is the longest-tenured PDF library in the ecosystem. It supports the full PDF specification — creation, editing, forms, signatures, encryption, PDF/A, PDF/UA — and ships an `pdfHTML` add-on for HTML-to-PDF conversion. It is also the source of the single most-discussed licensing trap in the .NET PDF space.

**Strengths**
- Most complete PDF feature coverage of any free .NET library
- Active development, frequent releases, large community
- pdfHTML add-on supports a subset of HTML/CSS
- Excellent forms, signatures, and PDF/A compliance

**The catch**
- AGPL v3 license — using iText in a closed-source product (including a web service) requires you to either open-source your application or buy a commercial license at quote-based subscription pricing
- pdfHTML does not execute JavaScript and supports only a subset of modern CSS — fails the [Bootstrap Test](#hero-test-2--the-bootstrap-homepage-test)
- API is verbose by modern standards
- Commercial pricing is opaque and quote-based

**Best for**: open-source projects, internal tools where source disclosure is acceptable, or teams already paying for an iText commercial license.
**Avoid if**: you ship closed-source SaaS and don't want a commercial license, or you need pixel-accurate modern HTML rendering.

```csharp
// NuGet: itext7
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;

using var writer = new PdfWriter("invoice.pdf");
using var pdf = new PdfDocument(writer);
using var document = new Document(pdf);

document.Add(new Paragraph("Invoice #1024"));
document.Add(new Paragraph("Total: $1,200.00"));
```

**License reality**: AGPL v3 is *the* license trap in this list. Distributing or hosting a closed-source product that links iText — even as a NuGet dependency on a backend service — triggers the AGPL's network-use clause. The commercial license is required, and pricing is enterprise-tier (quote-based subscription pricing). See [The "Free" Tax](#the-free-tax--true-cost-of-ownership) for the full story.

---

### wkhtmltopdf

> The original HTML-to-PDF command-line tool. Qt WebKit engine. LGPL. Last release: 0.12.6 (June 2020). **Officially archived in 2023.**

wkhtmltopdf was, for a decade, the default answer to "how do I convert HTML to PDF in any language including .NET?" Its appeal was simplicity: a single CLI binary, a permissive license, and reasonable output. Its problem is age: the underlying Qt WebKit engine was deprecated by upstream Qt years ago, and in 2023 the wkhtmltopdf project itself was archived — no new features, no security patches, no maintainer.

**Strengths**
- Single-binary deployment (no .NET runtime coupling)
- LGPL — friendly to closed-source projects
- Decent rendering for simple, CSS2-era HTML
- Massive existing footprint in legacy codebases

**The catch**
- **Project is archived as of 2023** — no security patches even for known CVEs
- Qt WebKit predates Flexbox, CSS Grid, and modern JavaScript — fails the [Bootstrap Test](#hero-test-2--the-bootstrap-homepage-test)
- Known memory leaks under sustained load
- Native binary distribution complicates Docker/Lambda deployment

**Best for**: nothing new. Legacy maintenance only.
**Avoid if**: you are starting any new .NET project in 2026.

```csharp
// wkhtmltopdf is typically invoked via Process.Start in raw .NET,
// or wrapped — see DinkToPdf below for the most common wrapper.
using System.Diagnostics;

var psi = new ProcessStartInfo("wkhtmltopdf", "input.html output.pdf");
Process.Start(psi)?.WaitForExit();
```

**License reality**: LGPL is permissive enough for closed-source distribution, but the license is the least of your problems — the project is unmaintained, and unpatched binaries running in production are a security and compliance liability.

---

### QuestPDF

> Modern, code-first PDF generation with a fluent C# API. MIT for small companies; commercial above $1M revenue. Last release: 2026.5.0 (May 2026). Active.

QuestPDF is the most modern code-first PDF library in the .NET ecosystem. Its fluent API is genuinely pleasant — declarative layout, hot-reload previews, strong typography support. It does not render HTML, by design. In 2024 QuestPDF moved from pure MIT to a dual MIT/commercial license; companies with annual revenue above $1M USD now require a paid commercial license.

**Strengths**
- Best-in-class developer experience among free .NET PDF libraries
- Fluent, declarative API
- Excellent documentation and active maintainer
- Hot-reload preview workflow
- True cross-platform (Windows, Linux, macOS, Docker)

**The catch**
- **Revenue gate**: companies with > $1M annual revenue need a paid commercial license (Professional from $999, Enterprise from $2,999)
- Does not render HTML — if your input is HTML or a Razor view, QuestPDF is not the answer
- The license change in 2024 made some early adopters cautious about future term shifts

**Best for**: programmatic invoices, reports, tickets, and tables, generated from C# data, in small-to-mid-size teams under the revenue cap.
**Avoid if**: your input is HTML, or your company revenue is above $1M and you don't want a new line item in your software budget.

```csharp
// NuGet: QuestPDF
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

QuestPDF.Settings.License = LicenseType.Community;

Document.Create(container =>
{
    container.Page(page =>
    {
        page.Margin(40);
        page.Header().Text("Invoice #1024").FontSize(20).Bold();
        page.Content().Text("Total: $1,200.00");
    });
}).GeneratePdf("invoice.pdf");
```

**License reality**: MIT for individuals and companies with revenue below $1M. Above that, you need a commercial license — Professional from $999, Enterprise from $2,999. The revenue threshold is self-attested; there is no audit, but a license violation is still a license violation.

---

### DinkToPdf

> .NET wrapper around the wkhtmltopdf C++ library. MIT. Last release: 1.0.8 (April 2017). Stale.

DinkToPdf is the most common .NET wrapper around wkhtmltopdf. Instead of shelling out to the CLI, it binds the wkhtmltopdf native library directly via P/Invoke, which removes process-spawn overhead and gives you a strongly-typed C# API. It inherits everything good about wkhtmltopdf (license, simplicity), and everything bad (CSS2-era rendering, no upstream maintainer).

**Strengths**
- Idiomatic C# API over wkhtmltopdf
- No CLI process overhead
- MIT-licensed
- Drop-in for many legacy ASP.NET projects

**The catch**
- Inherits all of wkhtmltopdf's limitations and its abandonment
- Native binary loading is fragile on Linux/Docker — common source of "works on my Windows box, breaks on Alpine" bugs
- Repository itself sees infrequent updates
- No path forward as wkhtmltopdf decays

**Best for**: maintaining existing DinkToPdf-based services. Not for new code.
**Avoid if**: you have any choice in the matter on a new project.

```csharp
// NuGet: DinkToPdf, DinkToPdf.Contracts
using DinkToPdf;
using DinkToPdf.Contracts;

var converter = new SynchronizedConverter(new PdfTools());
var doc = new HtmlToPdfDocument
{
    Objects = { new ObjectSettings { HtmlContent = "<h1>Invoice</h1>" } }
};
File.WriteAllBytes("invoice.pdf", converter.Convert(doc));
```

**License reality**: MIT and clean — but the value is gated by wkhtmltopdf's archived status. A "free and unmaintained" dependency is a future migration ticket waiting to be filed.

---

### PuppeteerSharp

> .NET port of Puppeteer. Drives headless Chromium for full-fidelity HTML-to-PDF. MIT. Last release: 25.0.4 (May 2026). Active.

PuppeteerSharp is the only free .NET library that can render modern HTML/CSS — Flexbox, Grid, custom properties, JavaScript — with full fidelity. It does this by launching real headless Chromium and using its print-to-PDF feature. The cost is operational: Chromium is a large dependency, memory-hungry under sustained load, and the print pipeline produces output that is *print-correct* rather than *screen-identical*.

**Strengths**
- Full Chromium rendering — passes the [Bootstrap Test](#hero-test-2--the-bootstrap-homepage-test) (in print mode)
- Modern CSS3, JavaScript, web fonts, web components all supported
- MIT-licensed; no revenue or copyleft gate
- Active, well-maintained .NET port

**The catch**
- Downloads roughly 150–300MB of Chromium on first use (varies by platform) — non-trivial for Lambda or small containers
- Memory grows under sustained load; long-running services need watchdog/restart strategies
- Output is print-rendered (like Ctrl+P), not screen-rendered — column widths and spacing can drift
- Generation only — no PDF editing, signing, forms, or PDF/A
- Async-first API can be awkward in synchronous codepaths

**Best for**: HTML-to-PDF conversion of modern web content where deployment size and memory pressure are acceptable trade-offs.
**Avoid if**: you need to *edit* existing PDFs, or you're deploying to a memory-constrained serverless environment.

```csharp
// NuGet: PuppeteerSharp
using PuppeteerSharp;

await new BrowserFetcher().DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://getbootstrap.com/");
await page.PdfAsync("bootstrap.pdf");
```

**License reality**: MIT — clean for any use, open or closed source. The "tax" here is not legal, it is operational: Chromium's footprint and memory profile.

---

### PdfSharp

> The classic .NET programmatic PDF library. MIT. Last meaningful release: 6.2.4 (January 2026). Slow but functional.

PdfSharp is the oldest active library on this list. It generates PDFs procedurally — pages, fonts, shapes, text positioning — via a stable, well-understood API. Paired with its higher-level sibling MigraDoc, it supports document-style layout. It does not render HTML in any meaningful way, and its development cadence is slow compared with QuestPDF.

**Strengths**
- Mature, stable, MIT-licensed
- Predictable memory behaviour — no native binaries, no Chromium
- Pure managed code; deploys cleanly to any .NET platform
- Familiar API for anyone who has used PDF libraries in Java or Python

**The catch**
- No modern HTML/CSS rendering — the MigraDoc HTML extension is CSS2-era at best
- Slow release cadence; some long-open issues
- API feels dated next to QuestPDF's fluent style
- Limited support for newer PDF features (PDF 2.0, advanced PDF/UA)

**Best for**: programmatic generation of simple structured PDFs (forms, certificates, tables) where you control the layout in code and don't need HTML.
**Avoid if**: your input is HTML, or you want a modern, well-typed fluent API — QuestPDF beats it on developer experience.

```csharp
// NuGet: PdfSharp
using PdfSharp.Pdf;
using PdfSharp.Drawing;

var doc = new PdfDocument();
var page = doc.AddPage();
var gfx = XGraphics.FromPdfPage(page);
var font = new XFont("Verdana", 20);
gfx.DrawString("Invoice #1024", font, XBrushes.Black, new XPoint(40, 40));
doc.Save("invoice.pdf");
```

**License reality**: MIT and uncomplicated. The cost is feature scope, not legal — you are buying simplicity at the price of capability.

---

### PdfPig

> PDF reading and content extraction for .NET. Apache 2.0. Last release: 0.1.14 (March 2026). Active.

PdfPig is the odd one out: it is a read-only library. You cannot generate or edit PDFs with it. What it does extremely well is *parse* PDFs — extract text with positional information, read structure, decode embedded data, and walk page contents. For document-processing pipelines that ingest PDFs (search indexing, RAG, invoice scraping), PdfPig is the right primitive.

**Strengths**
- Apache 2.0 — friendly for any use
- High-quality text extraction with positional data
- Active, well-documented, pure-managed
- Excellent for PDF inspection, search indexing, and data pipelines

**The catch**
- **Read-only by design** — you cannot generate or modify PDFs
- No HTML support of any kind (not its job)
- Pair with another library if you need to extract *and* produce

**Best for**: text extraction, PDF parsing, search indexing, document-AI pipelines.
**Avoid if**: you need to create or modify PDFs — PdfPig is not the wrong tool for that, it is simply not the tool.

```csharp
// NuGet: PdfPig
using UglyToad.PdfPig;

using var document = PdfDocument.Open("invoice.pdf");
foreach (var page in document.GetPages())
{
    Console.WriteLine(page.Text);
}
```

**License reality**: Apache 2.0 — among the friendliest licenses in this list. No revenue gate, no copyleft, no commercial trap.

---

## Feature Comparison Matrix

| Capability | iText | wkhtmltopdf | QuestPDF | DinkToPdf | PuppeteerSharp | PdfSharp | PdfPig |
|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| Generate from HTML | partial | yes (legacy) | no | yes (legacy) | yes | no | no |
| Generate from C# code | yes | no | yes | no | no | yes | no |
| Edit existing PDFs | yes | no | no | no | no | partial | no |
| Read / extract text | yes | no | no | no | no | partial | yes |
| Forms (AcroForm) | yes | no | no | no | no | partial | read-only |
| Digital signatures | yes | no | no | no | no | partial | no |
| PDF/A compliance | yes | no | partial | no | no | partial | no |
| Modern CSS3 (Flexbox/Grid) | no | no | n/a | no | yes | no | n/a |
| JavaScript execution | no | no | n/a | no | yes | no | n/a |
| Commercial-safe license out of the box | no (AGPL) | yes | yes (< $1M) | yes | yes | yes | yes |
| Active maintenance | yes | **no** | yes | stale | yes | slow | yes |

---

## Platform Support Matrix

| Library | Windows | Linux | macOS | Docker | Azure | AWS Lambda |
|---|:---:|:---:|:---:|:---:|:---:|:---:|
| iText | yes | yes | yes | yes | yes | yes |
| wkhtmltopdf | yes | yes | yes | partial | partial | poor |
| QuestPDF | yes | yes | yes | yes | yes | yes |
| DinkToPdf | yes | partial | partial | partial | partial | poor |
| PuppeteerSharp | yes | yes | yes | yes | yes | partial (size) |
| PdfSharp | yes | yes | yes | yes | yes | yes |
| PdfPig | yes | yes | yes | yes | yes | yes |

"Partial" generally means the library *can* run on that platform but has known friction — native binary loading on Alpine Linux, large image size on Lambda, font fallback issues on macOS, and so on.

---

## The "Free" Tax — True Cost of Ownership

Every free library on this list is genuinely free *to install*. The cost shows up elsewhere. We catalog the four main forms it takes — and one cautionary tale that captures all four.

**1. License risk.** AGPL (iText) and copyleft-adjacent licenses can require you to open-source proprietary code or buy a commercial license. Companies routinely discover this during a security audit, an acquisition, or a customer compliance review — i.e. at the worst possible time.

**2. Maintenance decay.** wkhtmltopdf was archived in 2023. DinkToPdf inherits that. PdfSharp ships slowly. A library that is free today but unmaintained tomorrow is a migration ticket on your future roadmap.

**3. Feature gap engineering time.** "PdfSharp is free" is true. "PdfSharp can render this Tailwind invoice" is not. The gap between what a free library does and what your product needs is paid for in developer-weeks. At loaded engineering cost of $125/hour, six weeks of integration work eclipses every commercial license fee on the market.

**4. Operational footprint.** PuppeteerSharp's ~300MB Chromium download is free in dollars and expensive in cold-start latency, Lambda layer size, and memory pressure. The bill comes from AWS, not from a software vendor.

### The iText AGPL cautionary tale

iText was MIT/LGPL licensed from 2000 to 2009. Teams adopted it under the assumption that "open source" meant "safe to use commercially." In 2009 the license changed to AGPL v3. Every team using a recent iText version in a closed-source product or SaaS suddenly had three options:

1. Open-source their entire application (almost never feasible)
2. Buy an iText commercial license (quote-based subscription pricing)
3. Rewrite their PDF code against a different library — and absorb the cost of the migration

Many teams chose option 3, which is the most expensive of the three when measured honestly. The lesson is not that iText behaved badly — it is open about its license — but that **a free dependency is a future legal and engineering decision you have not yet made.** Reading the license before you adopt is the cheapest insurance policy in software.

---

## When IronPDF Is Worth the Money

This README is about free libraries. It would still be dishonest not to identify the cases where a commercial library is the lower-total-cost option. [IronPDF](https://ironpdf.com/) is the commercial alternative most teams in this space evaluate, and it is the cleanest comparator for the trade-offs above.

IronPDF is worth its license fee (Lite from $999, Professional from $2,999, perpetual) when:

- **You ship closed-source software** and want to avoid AGPL exposure from iText, GPL exposure from Ghostscript, or future license changes from QuestPDF / others
- **You need modern HTML/CSS rendering with screen-accurate output** — the only free option here is PuppeteerSharp, and it renders in print mode
- **You need generation, editing, signing, and reading in one library** — no free option covers all four; assembling iText + PdfPig + PuppeteerSharp + glue code is more expensive in engineering time than the license
- **You're deploying to AWS Lambda or memory-constrained containers** — IronPDF's footprint is engineered for this; PuppeteerSharp's is not
- **You need vendor-backed support** with an SLA — none of the free libraries offer this
- **Your engineering hours are worth more than the license** — and at most companies they are

IronPDF is *not* worth the money when you can comfortably live within the constraints of one free library — for example, QuestPDF below the $1M revenue cap for purely code-first generation, or PdfPig for pure read-only extraction. Use the free option until you outgrow it; switch when the free tax exceeds the license fee.

---

## Use-Case Recommendations

Concrete starting points by what you're actually trying to do.

**"I need to convert HTML to PDF in C#"**
→ [PuppeteerSharp](#puppeteersharp) for free; IronPDF if you need screen-accurate output, smaller footprint, or commercial support.

**"I need to generate invoices, reports, or tickets from C# data"**
→ [QuestPDF](#questpdf) if your revenue is under $1M; [PdfSharp](#pdfsharp) for simpler procedural layout.

**"I need to fill or sign existing PDF forms"**
→ [iText](#itext--itextsharp) if AGPL is acceptable; otherwise IronPDF.

**"I need to extract text or data from PDFs"**
→ [PdfPig](#pdfpig).

**"I'm migrating off wkhtmltopdf and need a free replacement"**
→ [PuppeteerSharp](#puppeteersharp) for HTML-to-PDF; [QuestPDF](#questpdf) if you can move to code-first generation.

**"I'm deploying to AWS Lambda or Docker on Alpine"**
→ [QuestPDF](#questpdf), [PdfSharp](#pdfsharp), or [PdfPig](#pdfpig) for free; IronPDF for HTML rendering in this environment.

**"I just need to read PDFs for a search/AI pipeline"**
→ [PdfPig](#pdfpig).

---

## Sources

Citations for the non-obvious claims made above.

| Claim | Source |
|---|---|
| iText is AGPL v3 with commercial license required for closed-source | [itextpdf.com](https://itextpdf.com/how-buy/AGPLv3-license) |
| iText pdfHTML does not execute JavaScript | [kb.itextpdf.com](https://kb.itextpdf.com/itext/evaluating-js-with-pdfhtml) |
| wkhtmltopdf project archived in 2023 | [github.com/wkhtmltopdf](https://github.com/wkhtmltopdf/wkhtmltopdf) |
| QuestPDF requires commercial license above $1M revenue | [questpdf.com](https://www.questpdf.com/license/) |
| PuppeteerSharp downloads Chromium on first run | [github.com/hardkoded](https://github.com/hardkoded/puppeteer-sharp) |
| PdfSharp HTML support is CSS2-era only | [HtmlRenderer.PdfSharp on NuGet](https://www.nuget.org/packages/HtmlRenderer.PdfSharp/) |
| PdfPig is read-focused (extraction over creation) | [github.com/UglyToad](https://github.com/UglyToad/PdfPig) |
| DinkToPdf wraps wkhtmltopdf native binary | [github.com/rdvojmoc](https://github.com/rdvojmoc/DinkToPdf) |

---

## Methodology

This comparison covers seven free, open-source .NET PDF libraries currently in active use on NuGet. "Free" here means available under a license that permits no-cost installation and basic use — including copyleft (AGPL/LGPL/MIT/Apache) licenses, regardless of whether commercial use requires a paid tier. Each library was evaluated against three axes:

1. **Capability** — what the library can and cannot do, sourced from official documentation and verified against the library's own test suite or examples.
2. **License reality** — the actual terms a closed-source commercial user faces, not the marketing summary.
3. **Operational fit** — deployment, platform, memory, and maintenance reality, sourced from issue trackers and known production patterns.

Claims about capability gaps and license terms are documented in [Sources](#sources) with citations. This README is updated when a library changes license, ships a major version, or is archived.

---

## Contributing

Corrections welcome. If a library has shipped a release that changes any claim in this README, or if a license, maintainer status, or platform-support detail is wrong or out of date, please open an issue or PR with a citation.

When adding or correcting a library entry, follow the per-library template used above: positioning line, context paragraph, Strengths, The catch, Best for / Avoid if, code sample, License reality.

---

## License

This README's content is dedicated to the public domain under [CC0 1.0 Universal](LICENSE). Individual libraries listed here are subject to their own licenses — always consult the library's own LICENSE file before adopting it.

---

*Last verified: May 2026*
