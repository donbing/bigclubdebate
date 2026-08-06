using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

namespace BigClubDebate.Tests;

public class PageLoadTests : IAsyncLifetime
{
    private DistributedApplication _app = null!;
    private IPage _page = null!;
    private IBrowser _browser = null!;
    private IPlaywright _playwright = null!;
    private string _baseUrl = null!;

    private const float DefaultTimeoutMs = 60_000f;

    [Fact]
    public async Task AllSectionsArePresentOnPageLoad()
    {
        // Act — navigate to the home page and wait for Blazor to render sections
        await _page.GotoAsync(_baseUrl, new PageGotoOptions
        {
            WaitUntil = WaitUntilState.NetworkIdle
        });

        // Wait for the known section headings to appear (Blazor Server renders them
        // after the SignalR circuit is established and OnAfterRenderAsync fires)
        var expectedSectionHeadings = new[]
        {
            "Head to Head",
            "Division 1 / Premiership",
            "League Cup",
            "FA Cup"
        };

        foreach (var heading in expectedSectionHeadings)
        {
            var locator = _page.Locator("h2").GetByText(heading);
            await locator.WaitForAsync(new LocatorWaitForOptions
            {
                Timeout = DefaultTimeoutMs,
                State = WaitForSelectorState.Visible
            });
        }

        // Also check the Big Game Points section (uses an id rather than h2)
        var bigGamePointsSection = _page.Locator("#bigGamePointsTotal");
        await bigGamePointsSection.WaitForAsync(new LocatorWaitForOptions
        {
            Timeout = DefaultTimeoutMs,
            State = WaitForSelectorState.Visible
        });

        // Assert — all section headings are present in the rendered page
        var allHeadings = await _page.Locator("h2").AllTextContentsAsync();
        foreach (var heading in expectedSectionHeadings)
        {
            Assert.Contains(heading, allHeadings.Select(h => h.Trim()));
        }

        var bigGamePointsHtml = await bigGamePointsSection.InnerHTMLAsync();
        Assert.Contains("big game points", bigGamePointsHtml, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task PageReturnsOkStatusCode()
    {
        // Act
        using var httpClient = _app.CreateHttpClient("web");
        using var response = await httpClient.GetAsync("/");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    public async Task InitializeAsync()
    {
        // --- Start the distributed application ---
        var appHost = await DistributedApplicationTestingBuilder
            .CreateAsync<Projects.BigClubDebate_AppHost>();

        appHost.Services.AddLogging(logging =>
        {
            logging.SetMinimumLevel(LogLevel.Warning);
            logging.AddFilter(appHost.Environment.ApplicationName, LogLevel.Warning);
            logging.AddFilter("Aspire.", LogLevel.Warning);
        });

        _app = await appHost.BuildAsync();
        await _app.StartAsync()
            .WaitAsync(TimeSpan.FromMilliseconds(DefaultTimeoutMs));

        await _app.ResourceNotifications.WaitForResourceHealthyAsync("web")
            .WaitAsync(TimeSpan.FromMilliseconds(DefaultTimeoutMs));

        // Resolve the web endpoint via CreateHttpClient — this gives us the correct base URI
        using var httpClient = _app.CreateHttpClient("web");
        _baseUrl = httpClient.BaseAddress!.ToString();

        // --- Start Playwright browser ---
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
        {
            Headless = true
        });

        var context = await _browser.NewContextAsync(new BrowserNewContextOptions
        {
            IgnoreHTTPSErrors = true
        });

        _page = await context.NewPageAsync();
    }

    public async Task DisposeAsync()
    {
        if (_page != null)
            await _page.CloseAsync();

        if (_browser != null)
            await _browser.CloseAsync();

        _playwright?.Dispose();

        if (_app != null)
            await _app.DisposeAsync();
    }
}
