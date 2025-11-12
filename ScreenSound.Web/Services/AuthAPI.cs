using ScreenSound.Web.Response;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");
    public async Task<AuthResponse> LoginAsync(string email, string senha)
    {
        var Response = await _httpClient.PostAsJsonAsync("/auth/login", new { email, senha });
        if (Response.IsSuccessStatusCode)
        {
            return new AuthResponse
            {
                Sucesso = true,
                Erros = Array.Empty<string>()
            };
        }


        return new AuthResponse
        {
            Sucesso = false,
            Erros = ["senha incoreta"]
        };

    }
}
