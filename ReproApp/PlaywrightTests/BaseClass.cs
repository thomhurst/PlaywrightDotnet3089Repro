using System.Net.Http.Headers;
using TUnit.Playwright;

namespace PlaywrightTests;

public abstract class BaseClass : PageTest
{
    static BaseClass()
    {
        Environment.SetEnvironmentVariable("PWDEBUG", "1");
    }
    
    [ClassDataSource<TestApplicationFactory>(Shared = SharedType.PerTestSession)]
    public required TestApplicationFactory TestApplicationFactory { get; init; }
    
    protected IEnumerable<KeyValuePair<string,string>>? MapHeaders(HttpResponseHeaders responseHeaders)
    {
        return responseHeaders.Select(x => new KeyValuePair<string,string>(x.Key, string.Join("; ", x.Value)));
    }
}