using System.Net.Http.Json;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");
    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        await _httpClient.PostAsJsonAsync("/auth/login", new { email, senha });)
    }
}
