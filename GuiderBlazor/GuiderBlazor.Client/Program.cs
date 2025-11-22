using GuiderBlazor.Shared.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
//using Shared.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Настройка HttpClient для прямого обращения к API
builder.Services.AddScoped(sp => new HttpClient
{
    //BaseAddress = new Uri("https://api.guider.pro/")
    BaseAddress = new Uri("https://localhost:8081/")
});

// Регистрация сервиса
builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.AddScoped<ICitiesService, CitiesService>();
builder.Services.AddScoped<ITagsService, TagsService>();
builder.Services.AddScoped<IProvincesService, ProvincesService>();

await builder.Build().RunAsync();
