using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using TechnicalAssessment.Utility;

namespace TechnicalAssessment.Tests;

[TestFixture]
public class PerformanceTest
{
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private IPlaywright _playwright;

    [SetUp]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions { Headless = true });
        var context = await _browser.NewContextAsync();
        _page = await context.NewPageAsync();
    }

    [Test]
    public async Task MeasurePageLoadTime()
    {
        // Start navigation and wait for full load
        await _page.GotoAsync(TestData.BaseURL, new PageGotoOptions { WaitUntil = WaitUntilState.Load });

        // Capture navigation timing
        var timing = await _page.EvaluateAsync(@"() => {
                const t = performance.timing;
                return {
                    navigationStart: t.navigationStart,
                    loadEventEnd: t.loadEventEnd
                };
            }");

        var navStart = timing.Value.GetProperty("navigationStart").GetInt64();
        var loadEnd = timing.Value.GetProperty("loadEventEnd").GetInt64();
        var loadTime = loadEnd - navStart;

        Console.WriteLine($"[Performance] Home page load time: {loadTime} ms");

        // Assert page load time threshold
        Assert.Less(loadTime, 5000, "Home page should load in under 5 seconds");
    }

    [TearDown]
    public async Task Teardown()
    {
        await _browser.CloseAsync();
        _playwright.Dispose();
    }
}

