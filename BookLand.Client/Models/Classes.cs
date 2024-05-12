namespace BookLand.Client.Models;

public class Classes
{
    public class MeuLivroDados : DBModels.Livro
    {
        public DBModels.Livro Livro { get; set; } = new();
        public DBModels.UsuarioLivro UsuarioLivro { get; set; } = new();

    }

    //WebScrapping
    //resposta do skoob
    public class LivroSkoob
    {
        public int livro_id { get; set; }
        public int id { get; set; }
        public string titulo { get; set; } = "";
        public string nome_portugues { get; set; } = "";
        public string subtitulo { get; set; } = "";
        public int qt_estantes { get; set; }
        public int qt_edicoes { get; set; }
        public int editoraid { get; set; }
        public string editora { get; set; } = "";
        public string autor { get; set; } = "";
        public int ano { get; set; }
        public int? mes { get; set; }
        public int paginas { get; set; }
        public string img_url { get; set; } = "";
        public string capa_micro { get; set; } = "";
        public string capa_pequena { get; set; } = "";
    }

    public class ResultSkoob
    {
        public List<LivroSkoob> results { get; set; } = new();
        public int total { get; set; }
        public int totalPages { get; set; }
        public int page { get; set; }
        public int limit { get; set; }
    }


    //resposta do google
    public class ReadingModesGoogle
    {
        public bool text { get; set; }
        public bool image { get; set; }
    }

    public class IndustryIdentifierGoogle
    {
        public string type { get; set; } = "";
        public string identifier { get; set; } = "";
    }

    public class PanelizationSummaryGoogle
    {
        public bool containsEpubBubbles { get; set; }
        public bool containsImageBubbles { get; set; }
    }

    public class VolumeInfoGoogle
    {
        public string title { get; set; } = "";
        public List<string> authors { get; set; } = new();
        public string publishedDate { get; set; } = "";
        public string description { get; set; } = "";
        public List<IndustryIdentifierGoogle> industryIdentifiers { get; set; } = new();
        public ReadingModesGoogle readingModes { get; set; } = new();
        public int pageCount { get; set; }
        public string printType { get; set; } = "";
        public List<string> categories { get; set; } = new();
        public string maturityRating { get; set; } = "";
        public bool allowAnonLogging { get; set; }
        public string contentVersion { get; set; } = "";
        public PanelizationSummaryGoogle panelizationSummary { get; set; } = new();
        public string language { get; set; } = "";
        public string previewLink { get; set; } = "";
        public string infoLink { get; set; } = "";
        public string canonicalVolumeLink { get; set; } = "";
    }

    public class SaleInfoGoogle
    {
        public string country { get; set; } = "";
        public string saleability { get; set; } = "";
        public bool isEbook { get; set; }
    }

    public class EpubGoogle
    {
        public bool isAvailable { get; set; }
    }

    public class PdfGoogle
    {
        public bool isAvailable { get; set; }
    }

    public class AccessInfoGoogle
    {
        public string country { get; set; } = "";
        public string viewability { get; set; } = "";
        public bool embeddable { get; set; }
        public bool publicDomain { get; set; }
        public string textToSpeechPermission { get; set; } = "";
        public EpubGoogle epub { get; set; } = new();
        public PdfGoogle pdf { get; set; } = new();
        public string webReaderLink { get; set; } = "";
        public string accessViewStatus { get; set; } = "";
        public bool quoteSharingAllowed { get; set; }
    }

    public class SearchInfoGoogle
    {
        public string textSnippet { get; set; } = "";
    }

    public class ItemGoogle
    {
        public string kind { get; set; } = "";
        public string id { get; set; } = "";
        public string etag { get; set; } = "";
        public string selfLink { get; set; } = "";
        public VolumeInfoGoogle volumeInfo { get; set; } = new();
        public SaleInfoGoogle saleInfo { get; set; } = new();
        public AccessInfoGoogle accessInfo { get; set; } = new();
        public SearchInfoGoogle searchInfo { get; set; } = new();
    }

    public class BooksApiResponseGoogle
    {
        public string kind { get; set; } = "";
        public int totalItems { get; set; }
        public List<ItemGoogle> items { get; set; } = new();
    }
}
