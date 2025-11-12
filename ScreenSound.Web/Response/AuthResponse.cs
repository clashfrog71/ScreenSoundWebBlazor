using Microsoft.AspNetCore.Components.Web;

namespace ScreenSound.Web.Response
{
    public class AuthResponse()
    {

        public bool Sucesso { get; set; }
        public string[] Erros { get; set; }
    }
}
