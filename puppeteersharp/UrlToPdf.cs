using System.Threading.Tasks;
using PuppeteerSharp;

namespace FreeDotNetPdf.PuppeteerSharp;

/// <summary>
/// Use case: render a live URL or JavaScript SPA to PDF. Because it runs Chromium it waits
/// for scripts, charts and async data to settle (Networkidle0) before capturing.
/// </summary>
public static class UrlToPdf
{
    public static async Task RenderAsync(string url, string outputPath)
    {
        await new BrowserFetcher().DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();

        await page.GoToAsync(url, new NavigationOptions
        {
            WaitUntil = new[] { WaitUntilNavigation.Networkidle0 }
        });

        await page.PdfAsync(outputPath, new PdfOptions { PrintBackground = true });
    }
}
