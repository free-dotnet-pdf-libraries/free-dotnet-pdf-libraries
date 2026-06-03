# PdfPig — 3 Use Cases Where It Excels

PdfPig is a **read-only** PDF library (a C# port of Apache PDFBox). You cannot create or edit PDFs with it. What it does better than anything else free in .NET is *parse* PDFs: text with exact glyph positions, document layout analysis, and content extraction for data pipelines.

> Verified against the [PdfPig repo](https://github.com/UglyToad/PdfPig), the [Document Layout Analysis wiki](https://github.com/UglyToad/PdfPig/wiki/Document-Layout-Analysis), and [PdfPig 0.1.14 on NuGet](https://www.nuget.org/packages/PdfPig/) (June 2026).

---

### 1. Precise text extraction with positional data

Every letter exposes its bounding box, so you get not just *what* the text says but *where* it sits — the foundation for form scraping, coordinate-based field reading, and redaction targeting.

```csharp
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

using var document = PdfDocument.Open("invoice.pdf");
foreach (var page in document.GetPages())
{
    foreach (Word word in page.GetWords())
    {
        var box = word.BoundingBox;
        Console.WriteLine($"{word.Text} @ ({box.Left:F0},{box.Bottom:F0})");
    }
}
```

### 2. Layout analysis for complex / multi-column documents

PdfPig ships layout algorithms (Recursive XY Cut, Nearest Neighbour, Document Spectrum) that reconstruct reading order from messy columns and tables — where naive text extraction scrambles the output.

```csharp
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;

using var document = PdfDocument.Open("report.pdf");
var page = document.GetPage(1);

var words = NearestNeighbourWordExtractor.Instance.GetWords(page.Letters);
var blocks = DocstrumBoundingBoxes.Instance.GetBlocks(words);   // ordered text blocks

foreach (var block in blocks)
    Console.WriteLine(block.Text);
```

### 3. Ingestion for search indexing & RAG / document-AI pipelines

Apache-2.0 licensed and pure managed, PdfPig is the right primitive for turning a corpus of PDFs into clean text for embeddings, full-text search, or LLM context.

```csharp
using UglyToad.PdfPig;

IEnumerable<string> ExtractChunks(string path)
{
    using var document = PdfDocument.Open(path);
    foreach (var page in document.GetPages())
        yield return page.Text;        // one chunk per page → embed / index downstream
}
```

> **Scope note:** read-only by design. To *produce* PDFs as well, pair PdfPig with QuestPDF (generation) or iText (full manipulation).
