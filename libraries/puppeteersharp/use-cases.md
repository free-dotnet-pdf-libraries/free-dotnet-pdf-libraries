# PuppeteerSharp — 3 Use Cases Where It Excels

PuppeteerSharp drives **real headless Chromium** via the DevTools Protocol. It is the only free .NET option that renders modern HTML/CSS — Flexbox, Grid, web fonts, JavaScript — with full browser fidelity. Its sweet spots are anything that needs *the browser's own rendering*: HTML→PDF, screenshots, and scraping of JS-heavy pages. The trade-off is operational (a ~150–300MB Chromium download and memory under load).

> Verified against the [hardkoded/puppeteer-sharp overview](https://deepwiki.com/hardkoded/puppeteer-sharp/1-overview) and [PuppeteerSharp PDF guidance](https://auth0.com/blog/pdf-generation-with-puppeteer-sharp/) (June 2026).

---

### 1. Modern HTML/CSS → PDF with header/footer templates

Render a Bootstrap/Tailwind invoice or report exactly as a browser would, with running headers, footers, and page numbers.

```csharp
using PuppeteerSharp;

await new BrowserFetcher().DownloadAsync();
await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
await using var page = await browser.NewPageAsync();
await page.SetContentAsync(htmlString);

await page.PdfAsync("report.pdf", new PdfOptions
{
    Format = PaperFormat.A4,
    PrintBackground = true,
    DisplayHeaderFooter = true,
    HeaderTemplate = "<div style='font-size:8px;width:100%;text-align:center'>Acme Report</div>",
    FooterTemplate = "<div style='font-size:8px;width:100%;text-align:center'>" +
                     "Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>",
    MarginOptions = new MarginOptions { Top = "60px", Bottom = "60px" }
});
```

### 2. Render a live URL or JavaScript SPA to PDF

Because it runs Chromium, it waits for scripts, charts, and async data to finish before capturing — impossible for static HTML parsers.

```csharp
using PuppeteerSharp;

await using var page = await browser.NewPageAsync();
await page.GoToAsync("https://dashboard.example.com/report/42", new NavigationOptions
{
    WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }   // let the SPA settle
});
await page.PdfAsync("dashboard.pdf", new PdfOptions { PrintBackground = true });
```

### 3. Full-page and element screenshots

Pixel-accurate image capture of a whole page or a single component — for thumbnails, visual regression, or social cards.

```csharp
using PuppeteerSharp;

await page.GoToAsync("https://example.com");
await page.ScreenshotAsync("full.png", new ScreenshotOptions { FullPage = true });

var chart = await page.QuerySelectorAsync(".chart-widget");
await chart.ScreenshotAsync("chart.png");        // just that element
```

> **Operational note:** download Chromium once at deploy time (not per request), and reuse a single `Browser` instance with a watchdog/restart strategy for long-running services. Generation only — no PDF editing, forms, or signing.
