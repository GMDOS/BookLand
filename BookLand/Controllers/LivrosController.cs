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
    NpgsqlConnection conexao;
    public LivrosController()
    {
        conexao = ConectarAoBanco();
    }
    [HttpGet("ProcurarLivro/{isbn}")]
    public Livro? ProcurarLivro(string isbn)
    {

        using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM livros WHERE isbn = @isbn", conexao);
        cmd.Parameters.AddWithValue("@isbn", isbn);
        using NpgsqlDataReader reader = cmd.ExecuteReader();

        Livro? livro = GetData<Livro>(reader);
        Debug.WriteLine("chamado");
        conexao.Close();
        conexao.Dispose();
        return livro;
    }
}
