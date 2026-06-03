# wkhtmltopdf — 3 Use Cases Where It Excels

wkhtmltopdf is a **single-binary command-line tool** (Qt WebKit engine) that converts HTML/URLs to PDF. In .NET you invoke it via `Process`. Its strengths are language-agnostic simplicity, a permissive LGPL license, and built-in niceties like TOC and cover pages. **It was officially archived in 2023** — no features, no security patches — so these use cases are for legacy maintenance and CSS2-era content only.

> Verified against the [wkhtmltopdf project (archived)](https://github.com/wkhtmltopdf/wkhtmltopdf) and its CLI documentation (June 2026).

---

### 1. Convert a URL or HTML file to PDF from .NET

Shell out to the binary — no library dependency, works identically across languages and CI scripts.

```csharp
using System.Diagnostics;

var psi = new ProcessStartInfo("wkhtmltopdf", "https://example.com out.pdf")
{
    RedirectStandardError = true,
    UseShellExecute = false
};
using var proc = Process.Start(psi);
proc!.WaitForExit();
```

### 2. Headers, footers, and page numbers via CLI flags

Pagination and running text are command-line switches — no code, just arguments.

```bash
wkhtmltopdf \
  --footer-center "Page [page] of [topage]" \
  --footer-font-size 9 \
  --header-line \
  --header-right "Acme Corp" \
  input.html output.pdf
```

### 3. Cover page + auto-generated table of contents

wkhtmltopdf composes multi-part documents from positional "objects" (`cover`, `toc`, then pages), building a clickable TOC from your headings.

```bash
wkhtmltopdf \
  cover cover.html \
  toc \
  chapter1.html chapter2.html \
  book.pdf
```

> **Health warning:** archived since 2023 with known unpatched CVEs and memory leaks under sustained load; Qt WebKit cannot render Flexbox/Grid or run modern JavaScript. Use only to keep existing pipelines alive — for anything new, choose **PuppeteerSharp** (modern HTML→PDF) or **QuestPDF** (code-first). The `DinkToPdf` wrapper in this collection is the common in-process .NET binding.
