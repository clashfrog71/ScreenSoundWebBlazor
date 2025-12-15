using Microsoft.AspNetCore.Mvc;
using ScreenSound.API.Requests;
using ScreenSound.API.Response;
using ScreenSound.Banco;
using ScreenSound.Modelos;
using ScreenSound.Shared.Dados.Modelos;

namespace ScreenSound.API.Endpoints;

public static class ArtistasExtensions
{
    public static void AddEndPointsArtistas(this WebApplication app)
    {
        var groupBuilder = app.MapGroup("artistas")
            .RequireAuthorization()
            .WithTags("Artistas");

        #region Endpoint Artistas
        groupBuilder.MapGet("", ([FromServices] DAL<Artista> dal) =>
        {
            var listaDeArtistas = dal.Listar();
            if (listaDeArtistas is null)
            {
                return Results.NotFound();
            }
            var listaDeArtistaResponse = EntityListToResponseList(listaDeArtistas);
            return Results.Ok(listaDeArtistaResponse);
        }).RequireAuthorization();

        groupBuilder.MapGet("{nome}", ([FromServices] DAL<Artista> dal, string nome) =>
        {
            var artista = dal.RecuperarPor(a => a.Nome.ToUpper().Equals(nome.ToUpper()));
            if (artista is null)
            {
                return Results.NotFound();
            }
            return Results.Ok(EntityToResponse(artista));

        });

        groupBuilder.MapPost("", async ([FromServices] IHostEnvironment env, [FromServices] DAL<Artista> dal, [FromBody] ArtistaRequest artistaRequest) =>
        {

            var nome = artistaRequest.nome.Trim();
            var imagemArtista = DateTime.Now.ToString("ddMMyyyyhhss") + "." + nome + ".jpg";

            var path = Path.Combine(env.ContentRootPath,
                "wwwroot", "FotosPerfil", imagemArtista);

            using MemoryStream ms = new MemoryStream(Convert.FromBase64String(artistaRequest.fotoPerfil!));
            using FileStream fs = new(path, FileMode.Create);
            await ms.CopyToAsync(fs);

            var artista = new Artista(artistaRequest.nome, artistaRequest.bio) { FotoPerfil = $"/FotosPerfil/{imagemArtista}" };

            dal.Adicionar(artista);
            return Results.Ok();
        });

        groupBuilder.MapDelete("{id}", ([FromServices] DAL<Artista> dal, int id) =>
        {
            var artista = dal.RecuperarPor(a => a.Id == id);
            if (artista is null)
            {
                return Results.NotFound();
            }
            dal.Deletar(artista);
            return Results.NoContent();

        });

        groupBuilder.MapPut("", ([FromServices] DAL<Artista> dal, [FromBody] ArtistaRequestEdit artistaRequestEdit) =>
        {
            var artistaAAtualizar = dal.RecuperarPor(a => a.Id == artistaRequestEdit.Id);
            if (artistaAAtualizar is null)
            {
                return Results.NotFound();
            }
            artistaAAtualizar.Nome = artistaRequestEdit.nome;
            artistaAAtualizar.Bio = artistaRequestEdit.bio;
            dal.Atualizar(artistaAAtualizar);
            return Results.Ok();
        });
        groupBuilder.MapPost("avaliacao", (
            HttpContext context,
            [FromServices] DAL<PessoaComAcesso> dalPessoa,
            [FromServices] DAL<Artista> artista,
            [FromBody] AvaliacaoArtistaRequest artistaRequest
            ) =>
        {
            var Dalartista = artista.RecuperarPor(a => a.Id == artistaRequest.artistaId); if (Dalartista is null)
            {
                return Results.NotFound("Artista não encontrado");
            }
            var pessoa = dalPessoa.RecuperarPor(p => p.UserName == context.User.Identity!.Name); if (pessoa is null) { throw new Exception("Usuário não encontrado"); }
            var avaliacoes = Dalartista.AvaliacoesArtista.FirstOrDefault(a => a.ArtistaID == Dalartista.Id && a.UsuarioID == pessoa.Id);
            if (avaliacoes is null)
            {
                Dalartista.AdicionarNota(artistaRequest.nota, pessoa.Id);
            }
            else
            {
                avaliacoes.Nota = artistaRequest.nota;
            }
            artista.Atualizar(Dalartista);
            return Results.Ok();
        });
        groupBuilder.MapGet("{id}/avaliacao", (
            int id,
            HttpContext context,
            [FromServices] DAL < PessoaComAcesso > dalPessoa,
            [FromServices] DAL < Artista > Dalartista
            ) =>
        {
            var artista = Dalartista.RecuperarPor(a => a.Id == id) if (artista is null) { return Results.NotFound("Artista não encontrado"); }
            var pessoa = dalPessoa.RecuperarPor(p => p.UserName == context.User.Identity!.Name); if (pessoa is null) { throw new Exception("Usuário não encontrado"); }
            var avaliacao = artista.AvaliacoesArtista.FirstOrDefault(a => a.ArtistaID == artista.Id && a.UsuarioID == pessoa.Id); if (avaliacao is null) { return Results.Ok(new AvaliacaoArtistaResponse(id,0); }
        });

    }
    #endregion


    private static ICollection<ArtistaResponse> EntityListToResponseList(IEnumerable<Artista> listaDeArtistas)
    {
        return listaDeArtistas.Select(a => EntityToResponse(a)).ToList();
    }

    private static ArtistaResponse EntityToResponse(Artista artista)
    {
        return new ArtistaResponse(artista.Id, artista.Nome, artista.Bio, artista.FotoPerfil);
    }


}
