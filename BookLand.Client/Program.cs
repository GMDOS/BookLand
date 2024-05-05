using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

//builder.Services.AddScoped(sp => new HttpClient
//{
//    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
//});
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7266") });


await builder.Build().RunAsync();
