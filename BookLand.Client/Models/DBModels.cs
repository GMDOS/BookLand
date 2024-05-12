namespace BookLand.Client.Models;

public class DBModels
{
    public class Livro
    {
        public Guid Id { get; set; }
        public DateTime DataInsert { get; set; }
        public string ISBN { get; set; } = "";
        public string Titulo { get; set; } = "";
        public int QuantidadePaginas { get; set; }
        public int AnoPublicacao { get; set; }
        public string Descricao { get; set; } = "";
        public string? Capa { get; set; }
        public List<string>? Categorias { get; set; }
        public List<string>? Autores { get; set; }
        public List<string>? Imagens { get; set; }
    }

    public class UsuarioLivro
    {
        public Guid Id { get; set; }
        public Guid? IdLivro { get; set; }
        public Guid? IdUsuario { get; set; }
        public int PaginasLidas { get; set; }
    }

    public class Comentario
    {
        public Guid Id { get; set; }
        public Guid? IdLivro { get; set; }
        public Guid? IdUsuario { get; set; }
        public string Conteudo { get; set; } = "";
    }
    public class Usuario
    {
        public Guid Id { get; set; }
        public string NomeUsuario { get; set; } = "";
        public string Email { get; set; } = "";
        public string Senha { get; set; } = "";

    }
}
