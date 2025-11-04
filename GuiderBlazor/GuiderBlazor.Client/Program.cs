using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GuiderBlazor.Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Настройка HttpClient для прямого обращения к API
builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://api.guider.pro/")
});

// Регистрация сервиса
builder.Services.AddScoped<IPlacesService, PlacesService>();

await builder.Build().RunAsync();
