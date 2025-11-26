using GuiderBlazor.Components;
using GuiderBlazor.Shared.Services;
using GuiderBlazor.Shared.Utils;
using Microsoft.AspNetCore.OutputCaching;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
    //.AddInteractiveWebAssemblyComponents();


builder.Services.AddHttpContextAccessor();

// 1. Считываем настройки из appsettings.json (которые вы добавили ранее)
var cacheSettings = builder.Configuration.GetSection("CacheSettings");
var duration = cacheSettings.GetValue<int>("PlaceDetailsDuration", 60); // Если нет в конфиге, будет 60 сек
var sizeLimit = cacheSettings.GetValue<long>("SizeLimitBytes", 500 * 1024 * 1024); // Лимит кэша 500 МБ
var maxBodySize = cacheSettings.GetValue<long>("MaximumBodySizeBytes", 64 * 1024); // Лимит страницы 64 КБ

// 2. Регистрируем OutputCache с настройками и политикой
builder.Services.AddOutputCache(options =>
{
    // Глобальные лимиты памяти
    options.SizeLimit = sizeLimit;
    options.MaximumBodySize = maxBodySize;

    // 3. Создаем политику "PlaceDetails"
    options.AddPolicy("PlaceDetails", builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(duration));

        // Кэшируем разные версии для разных URL
        builder.SetVaryByRouteValue("url");

        // Общий тег, чтобы можно было сбросить ВЕСЬ кэш мест разом
        builder.Tag("all-places");
    });
});



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

// Включение middleware кэширования
// Важно: Это должно быть ПЕРЕД MapRazorComponents
app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    //.AddInteractiveWebAssemblyRenderMode()
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

// ---  Эндпоинт для инвалидации кеша ---
app.MapPost("/cache/invalidate", async (HttpContext context, IOutputCacheStore store, IConfiguration config) =>
{
    // 1. Проверяем секретный ключ (защита от хакеров)
    var secretKey = config["CacheSettings:CacheInvalidationKey"];
    var incomingKey = context.Request.Query["key"].ToString();

    if (string.IsNullOrEmpty(secretKey) || incomingKey != secretKey)
    {
        return Results.Unauthorized();
    }

    // 2. Смотрим, что нужно очистить
    var tag = context.Request.Query["tag"].ToString();

    if (string.IsNullOrEmpty(tag))
    {
        // Если тег не передан, очищаем ВСЁ (на случай глобальных изменений)
        await store.EvictByTagAsync("all-places", CancellationToken.None);
        return Results.Ok(new { message = "All places cache cleared" });
    }
    else
    {
        // Очищаем конкретный тег (например, place:some-url)
        await store.EvictByTagAsync(tag, CancellationToken.None);
        return Results.Ok(new { message = $"Cache cleared for tag: {tag}" });
    }
});
// ----------------------------------------------

app.Run();
