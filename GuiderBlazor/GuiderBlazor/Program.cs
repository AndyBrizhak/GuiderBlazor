using GuiderBlazor.Components;
using GuiderBlazor.Shared.Services;
using GuiderBlazor.Shared.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped(sp => new HttpClient
{
    //BaseAddress = new Uri("https://api.guider.pro/")
    BaseAddress = new Uri("https://localhost:8081/")

});
builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.AddScoped<ICitiesService, CitiesService>();
builder.Services.AddScoped<ITagsService, TagsService>();
builder.Services.AddScoped<IProvincesService, ProvincesService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(GuiderBlazor.Client._Imports).Assembly);


app.MapGet("/sitemap.xml", async () =>
{
    // 1. Настройки адресов
    // Адрес API (берем тот же, что у вас в builder)
    var apiBaseUrl = "https://localhost:8081/";
    // Адрес вашего сайта (для локальной разработки)
    var siteBaseUrl = "http://localhost:3000";

    List<string> slugs = new();

    // 2. Создаем отдельный HttpClient, так как мы вне scope обычного пользователя
    using (var client = new HttpClient { BaseAddress = new Uri(apiBaseUrl) })
    {
        try
        {
            // Запрашиваем у API только список слагов (строк)
            slugs = await client.GetFromJsonAsync<List<string>>("sitemap/places-slugs") ?? new();
        }
        catch
        {
            // Игнорируем ошибки API, чтобы хотя бы главная страница попала в sitemap
        }
    }

    // 3. Генерируем XML, вызывая статический метод из проекта Shared
    var xmlContent = SitemapLogic.Generate(siteBaseUrl, slugs);

    // 4. Отдаем XML
    return Results.Text(xmlContent, "application/xml");
});

app.Run();
