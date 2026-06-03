// NuGet: PuppeteerSharp
using System.Threading.Tasks;
using PuppeteerSharp;
using PuppeteerSharp.Media;

namespace FreeDotNetPdf.PuppeteerSharp;

/// <summary>
/// Use case: render a modern HTML string (Bootstrap/Tailwind/Flexbox/Grid) to PDF with a
/// running header/footer and page numbers, using real headless Chromium.
/// </summary>
public static class HtmlToPdf
{
    public static async Task RenderAsync(string html, string outputPath)
    {
        await new BrowserFetcher().DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.SetContentAsync(html);

        await page.PdfAsync(outputPath, new PdfOptions
        {
            Format = PaperFormat.A4,
            PrintBackground = true,
            DisplayHeaderFooter = true,
            HeaderTemplate = "<div style='font-size:8px;width:100%;text-align:center'>Acme Report</div>",
            FooterTemplate = "<div style='font-size:8px;width:100%;text-align:center'>" +
                             "Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>",
            MarginOptions = new MarginOptions { Top = "60px", Bottom = "60px" }
        });
    }
}
