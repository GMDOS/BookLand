using BookLand.Client.Pages;
using BookLand.Components;
using static BookLand.Data.Pg;
using System.Text.Json;
using BookLand;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

builder.Services.AddAntiforgery();

// Precisa de httpclient por aqui também, visto que na primeira conexão o client usa ssr ao invés de csr
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["FrontendUrl"] ?? "https://localhost:7266")
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
} else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();
app.UseStaticFiles();


app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(BookLand.Client._Imports).Assembly);

app.MapControllers();

connectionString = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText("config.json"))?["connectionString"];

//app.UseMiddleware<AuthMiddleware>();
app.Run();
