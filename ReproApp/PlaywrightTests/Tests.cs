using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Playwright;

namespace PlaywrightTests;

public class Tests : BaseClass
{
    [Test]
    public async Task Test1()
    {
        var client = TestApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("https://localhost")
        });
        
        await Page.Context.RouteAsync(_ => true, async route =>
        {
            var response = await client.GetAsync(route.Request.Url);

            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = response.StatusCode as int?,
                Headers = MapHeaders(response.Headers),
                BodyBytes = await response.Content.ReadAsByteArrayAsync()
            });
        });
            
        await Page.GotoAsync("https://localhost");

        // This never gets to the privacy page, because we fulfil a 302 redirect,
        // but Playwright can't access that `Location` because it needs to again go through our HttpClient
        
        await Assert.That(Page.Url).EndsWith("/Privacy");
    }
    
    [Test]
    public async Task Test2()
    {
        var client = TestApplicationFactory.CreateClient(new WebApplicationFactoryClientOptions
        {
            BaseAddress = new Uri("https://localhost")
        });
        
        await Page.Context.RouteAsync(_ => true, async route =>
        {
            var response = await client.GetAsync(route.Request.Url);

            await route.FulfillAsync(new RouteFulfillOptions
            {
                Status = response.StatusCode as int?,
                Headers = MapHeaders(response.Headers),
                BodyBytes = await response.Content.ReadAsByteArrayAsync()
            });
        });
            
        await Page.GotoAsync("https://localhost");

        // This follows redirects so does get to the privacy page
        // but because our client handled the redirect, there's no way of updating the URL in the browser
        // Playwright issues suggest returning a 302, but see the above test on why this doesn't work!
        
        await Assert.That(Page.Url).EndsWith("/Privacy");
    }
}