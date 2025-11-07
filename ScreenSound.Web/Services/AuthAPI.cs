namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory httpClientFactory)
{
    private HttpClient httpClient = httpClientFactory.CreateClient("AuthAPI");
    public Task
}
