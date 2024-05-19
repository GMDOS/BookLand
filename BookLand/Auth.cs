namespace BookLand;
public static class Auth
{
    // tokens possui um dicionário cuja chave é o token e a senha é o uuid do usuário
    private static Dictionary<string, string> tokens = new();
    public static string CreateNewToken(string idUsuario)
    {
        string token = Guid.NewGuid().ToString();
        tokens.Add(token, idUsuario);
        return token;
    }
    public static bool CheckToken(string token)
    {
        if (tokens.ContainsKey(token))
        {
            return true;
        }
        return false;
    }
    public static string? GetIdFromToken(string? token)
    {
        if (token == null)
        {
            return "94955ea6-7a3d-4c86-a3b6-71df083b0a73";
        }
        if (tokens.TryGetValue(token, out var idUsuario))
        {
            return idUsuario;
        }
        return null;
    }
}
public class AuthMiddleware
{

    private readonly RequestDelegate _next;
    public AuthMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        // Verifica se a solicitação é para uma página HTML
        bool isHtmlPageRequest = context.Request.Headers["Accept"].ToString().Contains("text/html");

        // Se o cookie não existir ou não for válido, e a solicitação for para uma página HTML, e não for a página de login ou de cadastro
        if ((!context.Request.Cookies.ContainsKey("Token") || !Auth.CheckToken(context.Request.Cookies["Token"]!))
            && isHtmlPageRequest
            && context.Request.Path != "/Login"
            && context.Request.Path != "/Cadastrar")
        {
            context.Response.Redirect("/Login");
            return;
        }

        // Se o cookie existir ou a solicitação não for para uma página HTML, continue com a próxima etapa no pipeline
        await _next(context);
    }

}
