using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using static BookLand.Client.Models.DBModels;
using static BookLand.Data.Pg;
using static BookLand.Auth;
using Microsoft.Net.Http.Headers;
using System.Net;

namespace BookLand.Controllers;
[ApiController]
[Route("API")]
public class UsuariosController : ControllerBase
{
    NpgsqlConnection sql;
    public UsuariosController()
    {
        sql = ConectarAoBanco();
    }
    [HttpPost("Login")]
    public async Task<IActionResult?> Login([FromBody] Usuario usuarioReq)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand("SELECT Id, Senha FROM usuarios WHERE NomeUsuario = @NomeUsuario", sql);
        cmd.Parameters.AddWithValue("@NomeUsuario", usuarioReq.NomeUsuario);
        using NpgsqlDataReader reader = cmd.ExecuteReader();

        Usuario? usuarioRet = await GetData<Usuario>(reader);

        if (usuarioRet != null)
        {
            var result = new PasswordHasher<Usuario>().VerifyHashedPassword(usuarioReq, usuarioRet.Senha, usuarioReq.Senha);

            if (result == PasswordVerificationResult.Success)
            {
                string token = CreateNewToken(usuarioRet.Id.ToString());

                // Configurar o cookie
                var cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(2),
                    Path = "/",
                    HttpOnly = true, // Recomendado para segurança adicional
                    //Secure = true,   // Recomendado se estiver usando HTTPS
                };

                // Adicionar o cookie ao cabeçalho da resposta
                HttpContext.Response.Cookies.Append("Token", token, cookieOptions);

                //resp.Headers.AddCookies(new CookieHeaderValue[] { cookie });
                return Ok();
            }
        }

        return Unauthorized();
    }
    [HttpPost("Cadastrar")]
    public  async Task<IActionResult?> Cadastrar([FromBody]Usuario usuario)
    {      
        using NpgsqlCommand cmd = new NpgsqlCommand("INSERT INTO usuarios (NomeUsuario, Senha) VALUES (@NomeUsuario, @Senha)", sql);
        cmd.Parameters.AddWithValue("@NomeUsuario", usuario.NomeUsuario);
        cmd.Parameters.AddWithValue("@Senha", new PasswordHasher<Usuario>().HashPassword(usuario, usuario.Senha));
        await cmd.ExecuteNonQueryAsync();

        //Usuario? usuarioRet = GetData<Usuario>(reader);
        return Ok();
    }
}
