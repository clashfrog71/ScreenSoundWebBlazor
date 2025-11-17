using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph.Models;
using ScreenSound.Web.Response;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory httpClientFactory) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var pessoa = new ClaimsPrincipal();
        var info = _httpClient.GetFromJsonAsync<pessoaAutenticada>("auth/manage/info");

        if(info is not null)
        {
            Claim[] dados = [
                new Claim(ClaimTypes.Name, info.Email),
                new Claim(ClaimTypes.Email, info.Email)
                ];

            var identity = new ClaimsIdentity(dados, "Cookies");
            pessoa = new ClaimsPrincipal(identity);
        }
    }

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
