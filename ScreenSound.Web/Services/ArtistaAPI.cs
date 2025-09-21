using ScreenSound.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;

namespace ScreenSound.Web.Services
{
    public class ArtistaAPI
    {
        private readonly HttpClient httpClient;
        public ArtistaAPI(IHttpClientFactory factory)
        {
            httpClient = factory.CreateClient("API");
        }
        public async Task<ICollection<ArtistaResponse>?> GetArtistasAsync()
        {
            return await
                  httpClient.GetFromJsonAsync<ICollection<ArtistaResponse>>("artistas");
        }
        public async Task DeletarArtistasAsync(int id)
        {
            await httpClient.DeleteAsync($"artistas/{id}");
        }
    }
}
