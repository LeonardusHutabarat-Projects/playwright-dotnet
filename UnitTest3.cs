using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PercyIO.Playwright;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest3 : PageTest
{
    [Test]
    public async Task ConsentWelcome()
    {
        //using var playwright = await Playwright.CreateAsync();
        var browser = await Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await browser.NewContextAsync();
        var page = await context.NewPageAsync();

        // Navigate to the website
        await Page.GotoAsync("https://automationexercise.com/");

        var logo = Page.Locator("img[alt='Website for automation practice']");
        await Expect(logo).ToBeVisibleAsync();

        // Take a Percy snapshot of the entire page
        Percy.Snapshot(Page, "Home Page");
        await logo.ScreenshotAsync(new LocatorScreenshotOptions { Path = "logo.png" });

        var consentButton = Page.Locator("button:has-text('Consent')");
        if (await consentButton.IsVisibleAsync())
        {
            await consentButton.ClickAsync();
        }

        // Now you can continue with your test steps
        // For example, check the homepage title
        Console.WriteLine(await Page.TitleAsync());

        await browser.CloseAsync();
    }
}