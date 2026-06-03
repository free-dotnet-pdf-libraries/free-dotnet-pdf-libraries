using System.Threading.Tasks;
using PuppeteerSharp;

namespace FreeDotNetPdf.PuppeteerSharp;

/// <summary>
/// Use case: pixel-accurate full-page and single-element screenshots — for thumbnails,
/// visual-regression testing, or social cards.
/// </summary>
public static class Screenshot
{
    public static async Task CaptureAsync(string url)
    {
        await new BrowserFetcher().DownloadAsync();
        await using var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
        await using var page = await browser.NewPageAsync();
        await page.GoToAsync(url);

        await page.ScreenshotAsync("full.png", new ScreenshotOptions { FullPage = true });

        var element = await page.QuerySelectorAsync(".chart-widget");
        if (element is not null)
            await element.ScreenshotAsync("chart.png");
    }
}
