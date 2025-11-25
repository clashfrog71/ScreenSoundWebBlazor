using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Graph.Models;
using ScreenSound.Web.Response;
using System.Net.Http.Json;
using System.Security.Claims;

namespace ScreenSound.Web.Services;

public class AuthAPI(IHttpClientFactory httpClientFactory, bool autenticado) : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("API");
    bool autenticado = false;

    public override Task<AuthenticationState> GetAuthenticationStateAsync()

    {
        autenticado = false;
        var pessoa = new ClaimsPrincipal();
        var response = await _httpClient.GetAsync("auth/manage/info");

        if (response.IsSuccessStatusCode)
        {
            var info = await response.Content.ReadFromJsonAsync<InfoPessoaResponse>();
            Claim[] dados =
            [
                new Claim(ClaimTypes.Name, info.Email),
                new Claim(ClaimTypes.Email, info.Email)
            ];

            var identity = new ClaimsIdentity(dados, "" +
                "Cookies");
            pessoa = new ClaimsPrincipal(identity);
            autenticado = true;
        }

        return new AuthenticationState(pessoa);
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
    public async Task LogoutAsync()
    {
        await _httpClient.PostAsync("/auth/logout", null);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
    public async Task<bool> VerificaAutenticado()
    {
        await GetAuthenticationStateAsync();
        return autenticado;
    }
}
