using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using static BookLand.Client.Models.Classes;
using static BookLand.Client.Models.DBModels;
using static BookLand.Data.Pg;
using static System.Net.WebRequestMethods;

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
        if (livro != null)
        {
            return Ok(livro);
        }
        livro = new();
        // Se não foi, começa a catar dados e salva no DB
        bool encontradoGoogle = false;

        var coverImageUrl = "";

        try
        {
            // Get JSON content from Google Books API
            var url = $"https://www.googleapis.com/books/v1/volumes?q=isbn:{isbn}";
            var client = new HttpClient();
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var jsonContent = await response.Content.ReadAsStringAsync();

            System.IO.File.WriteAllText("googlePesquisa.json", await response.Content.ReadAsStringAsync());
            var bookDataGoogle = JsonSerializer.Deserialize<BooksApiResponseGoogle>(jsonContent);
            if (bookDataGoogle == null)
            {
                Console.WriteLine("Falha ao ler dados do Google");
            } else
            {
                if (bookDataGoogle!.totalItems == 0)
                {
                    Console.WriteLine("Livro não encontrado no Google Books");
                    return NotFound(livro);
                } else
                {
                    //por algum motivo os dados que voltam da pesquisa do googleapis não são completos e as vezes estão errados
                    //com o selflink é possível buscar os dados atualizados..
                    encontradoGoogle = true;

                    var livroGoogle = await client.GetFromJsonAsync<ItemGoogle>(bookDataGoogle.items[0].selfLink);


                    livro.Titulo = livroGoogle!.volumeInfo.title;
                    livro.Autores = livroGoogle.volumeInfo.authors;
                    livro.Descricao = livroGoogle.volumeInfo.description;
                    livro.AnoPublicacao = Convert.ToDateTime(livroGoogle.volumeInfo.publishedDate).Year;
                    livro.QuantidadePaginas = Convert.ToInt32(livroGoogle.volumeInfo.pageCount);
                    livro.Categorias = livroGoogle.volumeInfo.categories;
                }
            }
        } catch (Exception exc)
        {
            Console.WriteLine($"Falha ao ler dados do Google {exc}");
            Console.WriteLine(exc.ToString());
        }


        var amazonURL = @$"https://www.amazon.com.br/s?k={isbn}";

        // Implement decompression methods to handle GZip and Deflate
        var handler = new HttpClientHandler()
        {
            AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip
        };

        using var clientAmazon = new HttpClient(handler);
        var amazonResponse = await clientAmazon.GetAsync(amazonURL);
        amazonResponse.EnsureSuccessStatusCode();
        using var stream = await amazonResponse.Content.ReadAsStreamAsync();
        using var sr = new StreamReader(stream);
        var docx = new HtmlDocument();
        docx.LoadHtml(await sr.ReadToEndAsync());

        System.IO.File.WriteAllText("amazon.html", docx.Text);
        var coverImageElement = docx.DocumentNode.SelectSingleNode("//img[@class='s-image']");
        if (coverImageElement != null)
        {
            coverImageUrl = coverImageElement.Attributes["src"].Value;
        }


        var clientCoverImage = new HttpClient();

        if (coverImageUrl != "")
        {
            using var coverImageResponse = await clientCoverImage.GetAsync(coverImageUrl);
            coverImageResponse.EnsureSuccessStatusCode();
            var coverImageData = await coverImageResponse.Content.ReadAsByteArrayAsync();
            var coverImageDataUri = $"data:{coverImageResponse.Content.Headers.ContentType?.MediaType};base64,{Convert.ToBase64String(coverImageData)}";
            livro.Capa = coverImageDataUri;
        }

        using NpgsqlCommand cmdInsert = new NpgsqlCommand(@"INSERT INTO livros (
                                                                             datainsert       
                                                                            ,isbn             
                                                                            ,titulo           
                                                                            ,quantidadepaginas
                                                                            ,anopublicacao    
                                                                            ,descricao        
                                                                            ,capa             
                                                                            ,categorias       
                                                                            ,autores          
                                                                            ,imagens)
                                                                VALUES(NOW()
                                                                      ,@isbn             
                                                                      ,@titulo           
                                                                      ,@quantidadepaginas
                                                                      ,@anopublicacao    
                                                                      ,@descricao        
                                                                      ,@capa             
                                                                      ,@categorias       
                                                                      ,@autores          
                                                                      ,@imagens)", sql);
        cmd.Parameters.AddWithValue("@isbn", isbn);
        cmd.Parameters.AddWithValue("@titulo", livro.Titulo);
        cmd.Parameters.AddWithValue("@quantidadepaginas", livro.QuantidadePaginas);
        cmd.Parameters.AddWithValue("@anopublicacao", livro.AnoPublicacao);
        cmd.Parameters.AddWithValue("@descricao", livro.Descricao);
        cmd.Parameters.AddWithValue("@capa", livro.Capa ?? "");
        cmd.Parameters.AddWithValue("@categorias", livro.Categorias ?? new());
        cmd.Parameters.AddWithValue("@autores", livro.Autores ?? new());
        cmd.Parameters.AddWithValue("@imagens", livro.Imagens ?? new());
        return Ok(livro);
    }
    [HttpGet("BuscarMeusLivros")]
    public async Task<IActionResult?> BuscarMeusLivros()
    {
        string? idUsuario = Auth.GetIdFromToken(Request.Cookies["Token"]!)!;
        if (idUsuario == null)
        {
            return Unauthorized();
        }
        List<MeuLivroDados> meusLivrosDados = new();
        using NpgsqlCommand cmd = new NpgsqlCommand("SELECT * FROM livros WHERE isbn = @isbn", sql);

        //cmd.Parameters.AddWithValue("@isbn", isbn);
        using NpgsqlDataReader reader = cmd.ExecuteReader();

        Livro? livro = await GetData<Livro>(reader);
        Debug.WriteLine("chamado");
        sql.Close();
        sql.Dispose();
        return Ok(livro);
    }
}
