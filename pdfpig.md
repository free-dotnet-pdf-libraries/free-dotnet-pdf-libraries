# PdfPig Review: The Rare Open-Source PDF Library That Does Exactly What It Says

Let's open with the conclusion, because that's the most useful thing we can do for you: [PdfPig](https://github.com/UglyToad/PdfPig) is good. We mean that without qualification, and we'll spend most of this article showing you why.

That probably sounds odd coming from a team that ships a commercial PDF library. We're the developer community at Iron Software, and yes, we make [IronPDF](https://ironpdf.com/), which competes in the broader .NET PDF space. So treat this as a peer-to-peer review with a transparent bias, not a neutral one. The reason we wanted to write this PdfPig review is that the .NET open-source PDF ecosystem has a lot of libraries that overpromise and underdeliver, and PdfPig is genuinely not one of them. Credit where it's due makes the rest of our reviews mean something.

Here's the thesis in one sentence: PdfPig is honest about its scope, and that honesty is the most underrated feature in any open-source library. The catch (and there is one) is that the scope is narrower than it looks at first glance, so the real question isn't "is it good?" It's "does its scope match what you actually need to do?"

To make that concrete before we go further, here is the entire setup ceremony for reading a PDF with PdfPig:

```csharp
using UglyToad.PdfPig;

using PdfDocument document = PdfDocument.Open("report.pdf");
Console.WriteLine($"Pages: {document.NumberOfPages}");
```

Two lines of using-directives, one to open, one to query. No factories, no DI registration, no async setup, no native binary to ship alongside. That minimalism is part of why we want to write about it honestly — it's a refreshing contrast to a lot of what's in the .NET PDF space.

## What PdfPig Is, In Plain Terms

PdfPig is a .NET port of [Apache PDFBox](https://pdfbox.apache.org/), the long-standing Java PDF library. It's licensed [Apache 2.0](https://www.apache.org/licenses/LICENSE-2.0): permissive, no copyleft, safe for commercial products without legal ambiguity (which, if we're being honest, is the bar a lot of "free" .NET PDF libraries quietly fail to clear). The package on NuGet has [over 21 million downloads](https://www.nuget.org/packages/PdfPig/) and a healthy community of contributors. Stars on the [GitHub repository](https://github.com/UglyToad/PdfPig) sit around 2,400, with active issue triage and a maintainer who actually merges PRs.

That last point matters more than star counts. We've seen plenty of OSS libraries with 10x the stars and zero recent commits. PdfPig's most recent push at the time of this writing was late April 2026, meaning the maintainer was active on the repo within the last two weeks. Releases over the last 18 months: [v0.1.11](https://github.com/UglyToad/PdfPig/releases/tag/v0.1.11) (July 2025), v0.1.12 (November 2025), v0.1.13 (December 2025), and the current stable [v0.1.14](https://github.com/UglyToad/PdfPig/releases) (March 2026). That's a roughly quarterly cadence, which is plenty for a library focused on a stable problem domain.

One real caveat on versioning before we go further: PdfPig is pre-1.0. The [README](https://github.com/UglyToad/PdfPig/blob/master/README.md) is upfront about this: minor versions can change the public API without [SemVer](https://semver.org/) guarantees until 1.0 is reached. If you're pinning a dependency in production, pin the patch version, not just the minor. The README's API-stability warning is worth taking literally.

## The Core API: Extracting Text Like It's 2026

Let's get to code, because that's why you're here. Here's the simplest possible thing PdfPig does well — pulling text out of a PDF in reading order:

```csharp
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

using PdfDocument document = PdfDocument.Open("invoice.pdf");

foreach (Page page in document.GetPages())
{
    string text = ContentOrderTextExtractor.GetText(page);
    Console.WriteLine($"Page {page.Number}: {text.Length} chars");
}
```

That's it. Open a document, iterate pages, get text. No service registration, no factory pattern, no fluent builder you have to learn. The class names are honest about what they do: `ContentOrderTextExtractor` extracts text in content order, `NearestNeighbourWordExtractor` extracts words by spatial proximity. There's a small set of layout-analysis tools and they each do one thing.

A gotcha that caught us off guard the first time: `page.Text` is *not* the property you want. It returns the raw content stream order, which is rarely the human reading order. The README is explicit about this: "you should not use `page.Text` directly, unless you know what you're doing." We've seen developers use it anyway because it's the obvious property name, then spend an afternoon wondering why their extracted invoice totals are showing up before the line items. Use the layout-analysis extractors instead. They exist for exactly this reason.

## Going Deeper: Words, Letters, Bounding Boxes

This is where PdfPig earns its place in our toolbelt. Most "extract text" libraries give you a string and call it a day. PdfPig gives you a tree: pages, then letters with positions, then words assembled by spatial algorithms, then text blocks via page segmenters, then optional reading-order detection.

Here's what that looks like for a richer extraction:

```csharp
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

using PdfDocument document = PdfDocument.Open("statement.pdf");
Page page = document.GetPage(1);

IReadOnlyList<Letter> letters = page.Letters;

var wordExtractor = NearestNeighbourWordExtractor.Instance;
IEnumerable<Word> words = wordExtractor.GetWords(letters);

var pageSegmenter = DocstrumBoundingBoxes.Instance;
IReadOnlyList<TextBlock> blocks = pageSegmenter.GetBlocks(words);

var readingOrder = UnsupervisedReadingOrderDetector.Instance;
IEnumerable<TextBlock> ordered = readingOrder.Get(blocks);

foreach (TextBlock block in ordered)
{
    Console.WriteLine($"[{block.BoundingBox}] {block.Text}");
}
```

If you've ever tried to do invoice parsing or statement parsing, you already know that "give us all the text" is the easy half of the problem. The hard half is "give us the text grouped by visual region, in the order a human would read it." PdfPig hands you the building blocks for that, including bounding box coordinates you can reason about geometrically. The [Docstrum algorithm](https://en.wikipedia.org/wiki/Document_layout_analysis) and the unsupervised reading-order detector aren't perfect, but they're real implementations of real research, not heuristics duct-taped together. These tools are well-suited to line-item extraction on supplier invoices and tabular data from multi-column PDF reports — exactly the kinds of workflows where regex-over-flat-string approaches collapse.

One more thing worth flagging in this PdfPig review: encrypted PDFs work fine if you have the password: pass it as the second argument to `PdfDocument.Open`. We've watched teams switch libraries because they assumed encrypted reading would be a paid feature. With PdfPig, it isn't.

Worth knowing on the runtime side: the current stable PdfPig package targets .NET Standard 2.0 and .NET Framework 4.6.2, which means it runs across current [.NET LTS releases](https://dotnet.microsoft.com/en-us/platform/support/policy/dotnet-core) (.NET 8 and .NET 10), older Framework apps, and anything in between. The same API surfaces in mixed environments without target-framework friction.

## The Creation Footnote — and Why We're Calling It a Footnote

PdfPig does support PDF creation through `PdfDocumentBuilder`. The README has an example: register a Standard 14 font, add a page, drop some text, write bytes. It works for what it's documented to do.

But the documentation itself is upfront about what it can't do, and these matter for production workflows. From the README:

> Document creation supports very limited changes to existing PDF documents. However it does not support any of the following: Editing forms, Copying or changing annotations, metadata or document structure data, Adding or removing text with existing fonts.

That's not a backhanded criticism — it's the maintainer being honest about what the creation API is for. It's there so you can produce simple debug PDFs (the wiki shows an example of overlaying bounding boxes on an extracted page for layout-analysis debugging, which is a great use case for this API). It's not there to be your invoice generator, your report builder, or your form-filling pipeline.

If your workflow is "read PDFs and extract data," PdfPig is excellent. If your workflow is "read PDFs, transform them, fill forms, sign them, and emit new PDFs," you're going to hit the wall on the creation side fast.

## Where PdfPig Stops

We want to be careful here, because the goal isn't to list everything PdfPig doesn't do. That's an unfair frame for any library. The goal is to map the boundary so you know whether your workflow lives inside it or outside it.

What PdfPig genuinely does not do, per its [own documentation](https://github.com/UglyToad/PdfPig/wiki):

- **HTML-to-PDF conversion.** Not its job. The wiki recommends other libraries for this.
- **PDF rendering to images.** Not its job either. The wiki points at [docnet](https://github.com/GowenGit/docnet) or PDFtoImage. There's a separate [`PdfPig.Rendering.Skia`](https://www.nuget.org/packages/PdfPig.Rendering.Skia) package that adds rendering, but it's a separate concern.
- **Form population.** Forms are readable but read-only: you cannot change values or add new form fields.
- **Digital signatures.** No signing, no signature validation in the way a compliance-driven workflow would need it.
- **PDF/A or PDF/UA compliance.** Out of scope.
- **Editing existing PDFs.** You can read them; the creation API can produce simple new pages but isn't intended for round-trip editing of arbitrary input PDFs.
- **Accessibility tagging.** Out of scope.

None of these are flaws. They're scope boundaries the maintainer drew on purpose, and the documentation tells you exactly where they are. That's the rare, valuable thing.

## When PdfPig Is the Right Tool

Concretely, here are workflows where we'd reach for PdfPig first:

- **Data extraction from PDFs.** Invoices, statements, regulatory filings, scientific papers — anything where the PDF is the input and structured data is the output. PdfPig's letter-level positioning and word/block extractors are specifically designed for this.
- **Search indexing.** Pulling text out of a PDF corpus to feed into a search engine. The text-extraction APIs are fast and stable.
- **Layout analysis and document understanding.** Research workflows where you need bounding boxes, reading order, and the geometric structure of the page. PdfPig is one of the few .NET options that exposes this layer.
- **Lightweight inspection tooling.** Debug viewers, validators, content audits. The basic creation API is good enough for overlays and annotations on extracted content.

If your job description includes the phrase "we need to read a PDF and pull data out of it," PdfPig should be on your shortlist before you look at anything paid.

## When PdfPig Leaves You Short

The flip side, also concretely:

- You need to render HTML or web content to PDF: PdfPig isn't your tool. You need a browser-engine-backed library or a layout engine.
- You need to fill in PDF forms and save them back: PdfPig can read the form fields, but it can't write them.
- You need to digitally sign PDFs for compliance, contracts, or audit trail purposes: out of scope.
- You need PDF/A archival format compliance for regulated industries: out of scope.
- You need to generate complex multi-page reports with tables, charts, and styled typography from a template: the basic creation API isn't built for this.
- You need to round-trip-edit existing PDFs (read in, modify, write out). The creation API explicitly doesn't support this for non-trivial documents.

For workflows that span both reading and writing — extract data from a PDF, generate a new PDF based on what you extracted, sign it, archive it — you'll need either a second library to handle the write half or a single library that covers both halves.

## The Workflow Tax: One Library Won't Cover Both Halves

The honest scope is exactly what PdfPig's maintainer has documented, and it is the property that makes PdfPig genuinely good at what it does. The implication for production planning is the one most readers leave the documentation without quite naming: any workflow that spans both reading and writing — extract from input PDFs, produce or modify output PDFs — commits the team to a second library, with its own license, its own dependency footprint, its own deployment surface, and its own maintenance trajectory.

This is the workflow tax. PdfPig is free in the literal sense — Apache 2.0, no commercial gating, no revenue threshold, no native binary to ship. PdfPig is not free in the architectural sense for any workflow that needs to write back. The bill comes due the moment the team's roadmap includes form filling, digital signatures, PDF/A archival output, template-driven PDF generation, round-trip editing, HTML-to-PDF rendering, or any of the categories the README is explicit about not covering.

The bill takes one of two forms. Either the team adopts a second PDF library (commercial or open-source) to cover the write half, and now pays a license fee, accepts another dependency's maintenance trajectory, and absorbs the integration cost of coordinating two libraries' object models. Or the team builds the missing functionality in-house, which means writing what is effectively a second PDF library at internal cost — engineer-quarters, not engineer-weeks, for any non-trivial scope.

For teams whose workflow is genuinely one-directional — search indexing, document understanding, invoice parsing, regulatory-filing analysis — PdfPig stands alone and the workflow tax is zero. For teams whose workflow is intake-then-emit — read an invoice, generate an audit document; parse a regulatory submission, produce a redacted version; extract data, sign and archive — the architectural picture is two libraries, and the question becomes not "is PdfPig free?" but "is the combined cost of PdfPig plus the second library lower than the cost of a single-library solution?"

The answer is workload-dependent. The cleaner the read/write split (different teams, different services, different infrastructure), the better the two-library composition. The tighter the integration (same service, shared models, frequent round-trips), the more attractive a single library that covers both halves becomes. Treating this as a workflow architecture decision rather than a free-vs-paid library decision is the framing the rest of this article exists to support.

## A Complementary Approach

This is where we think the honest framing matters most. PdfPig and a generation-capable library like [IronPDF](https://ironpdf.com/) aren't strictly competitors; they're often complementary. We've seen teams pair them: PdfPig for the extraction half of a workflow, a generation library for the rendering, signing, and form-filling half.

A toy version of that pattern:

```csharp
// Stage 1: PdfPig extracts data from the source document
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

string sourceText;
using (PdfDocument source = PdfDocument.Open("incoming-invoice.pdf"))
{
    var page = source.GetPage(1);
    sourceText = ContentOrderTextExtractor.GetText(page);
}

InvoiceData parsed = ParseInvoice(sourceText);

// Stage 2: A generation-capable library produces the audit PDF
// (IronPDF, QuestPDF, or another generation library would handle this stage)
string auditHtml = $"<h1>Audit for {parsed.InvoiceNumber}</h1>...";
// ... HTML-to-PDF conversion, signing, etc., happens here
```

You don't have to use IronPDF for the second stage — [QuestPDF](https://www.questpdf.com/) is another option. The point is that "PdfPig vs anything else" is often the wrong frame. PdfPig is the right tool for its half of the workflow. The architecture question is what tool covers the other half, and whether you want one library that does both halves or two libraries each doing what they do best.

## Final Honest Take

This is the part of a PdfPig review where the marketing instinct is to pivot to a sales pitch. We're going to resist that. If you're building data-extraction tooling and PdfPig fits your workflow, use PdfPig. The library is well-maintained, fairly licensed, technically solid, and refreshingly honest about its scope. We'd rather you use the right tool than the one we make.

If your workflow needs both reading and writing capabilities and you want to evaluate unified options, the [IronPDF documentation](https://ironpdf.com/docs/) is a reasonable place to look, and so is the [QuestPDF documentation](https://www.questpdf.com/) — read the licensing carefully on whatever you pick. The honest answer to "which library should we use?" almost always depends on your specific workflow, your team's constraints, and your budget. We'd rather give you the framing than the conclusion.

The reason we wrote this isn't to praise an "OSS competitor." It's that the .NET PDF space has too many libraries claiming to do everything and quietly failing at half of it, and PdfPig is the opposite — it claims to do a focused set of things, and it does them. That's worth saying out loud.
