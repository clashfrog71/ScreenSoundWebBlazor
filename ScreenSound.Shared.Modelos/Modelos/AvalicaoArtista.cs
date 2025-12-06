using ScreenSound.Modelos;

namespace ScreenSound.Shared.Modelos.Modelos;

public class AvalicaoArtista
{
    public int ArtistaID { get; set; }
    public virtual Artista? Artista { get; set; } //virtual: carregamento preguiçoso, carrega quando pede
    public int UsuarioID { get; set; }
    public int Nota { get; set; }
}
