using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using static BookLand.Client.Models.DBModels;
using static BookLand.Data.Pg;

namespace BookLand.Controllers;

[ApiController]
[Route("API")]
public class LivrosController : ControllerBase
{
    NpgsqlConnection sql;
    public LivrosController()
    {
        sql = ConectarAoBanco();
    }
    [HttpGet("ProcurarLivro/{isbn}")]
    public async Task<IActionResult?> ProcurarLivro(string isbn)
    {
        using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM livros WHERE isbn = @isbn", sql);
        cmd.Parameters.AddWithValue("@isbn", isbn);
        using NpgsqlDataReader reader = cmd.ExecuteReader();

        Livro? livro = await GetData<Livro>(reader);
        Debug.WriteLine("chamado");
        sql.Close();
        sql.Dispose();
        return Ok(livro);
    }
}
