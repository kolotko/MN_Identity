using System.Net.Http.Json;

public class TestService
{
    private IHttpClientFactory _httpClientFactory;
    
    public TestService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> ExampleCall()
    {
        var httpClient = _httpClientFactory.CreateClient("Api");
        var user = await httpClient.GetFromJsonAsync<Dictionary<string,string>>("secure-endpoint");
        return user["id"];
    }
}