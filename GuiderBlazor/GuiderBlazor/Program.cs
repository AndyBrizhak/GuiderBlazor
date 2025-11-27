using GuiderBlazor.Components;
using GuiderBlazor.Shared.Services;
using GuiderBlazor.Shared.Utils;
using Microsoft.AspNetCore.OutputCaching;
using SixLabors.ImageSharp.Web.DependencyInjection; // <--- ДОБАВЛЕНО (нужен этот using)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpContextAccessor();

// --- НАСТРОЙКА IMAGESHARP  ---
builder.Services.AddImageSharp()
    .SetRequestParser<SixLabors.ImageSharp.Web.Commands.QueryCollectionRequestParser>()
    .Configure<SixLabors.ImageSharp.Web.Middleware.ImageSharpMiddlewareOptions>(options =>
    {
        options.Configuration = SixLabors.ImageSharp.Configuration.Default;
        options.BrowserMaxAge = TimeSpan.FromDays(7);
        options.CacheMaxAge = TimeSpan.FromDays(365);
    })
    .ClearProviders()
    .AddProvider<SixLabors.ImageSharp.Web.Providers.PhysicalFileSystemProvider>()
    .SetCache<SixLabors.ImageSharp.Web.Caching.PhysicalFileSystemCache>()
    .Configure<SixLabors.ImageSharp.Web.Caching.PhysicalFileSystemCacheOptions>(options =>
    {
        options.CacheFolder = "cache";
    });
// ----------------------------------------------

var cacheSettings = builder.Configuration.GetSection("CacheSettings");
var duration = cacheSettings.GetValue<int>("PlaceDetailsDuration", 60);
var sizeLimit = cacheSettings.GetValue<long>("SizeLimitBytes", 500 * 1024 * 1024);
var maxBodySize = cacheSettings.GetValue<long>("MaximumBodySizeBytes", 64 * 1024);

builder.Services.AddOutputCache(options =>
{
    options.SizeLimit = sizeLimit;
    options.MaximumBodySize = maxBodySize;

    options.AddPolicy("PlaceDetails", builder =>
    {
        builder.Expire(TimeSpan.FromSeconds(duration));
        builder.SetVaryByRouteValue("url");
        builder.Tag("all-places");
    });
});

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri("https://localhost:8081/")
});

builder.Services.AddScoped<IPlacesService, PlacesService>();
builder.Services.AddScoped<ICitiesService, CitiesService>();
builder.Services.AddScoped<ITagsService, TagsService>();
builder.Services.AddScoped<IProvincesService, ProvincesService>();


builder.Services.AddScoped<GuiderBlazor.Shared.Services.ImageService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

// --- ПОДКЛЮЧЕНИЕ MIDDLEWARE (ВАЖЕН ПОРЯДОК: ДО StaticFiles) ---
// 1. Сначала зеркало (скачивание оригиналов)
app.UseMiddleware<GuiderBlazor.Middleware.ImageMirrorMiddleware>();

// 2. Затем обработка картинок (ImageSharp)
app.UseImageSharp();
// -------------------------------------------------------------

app.UseStaticFiles();
app.UseAntiforgery();
app.UseOutputCache();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(GuiderBlazor.Client._Imports).Assembly);

app.MapGet("/sitemap.xml", async () =>
{
    var apiBaseUrl = "https://localhost:8081/";
    var siteBaseUrl = "http://localhost:3000";
    List<string> slugs = new();
    using (var client = new HttpClient { BaseAddress = new Uri(apiBaseUrl) })
    {
        try
        {
            slugs = await client.GetFromJsonAsync<List<string>>("sitemap/places-slugs") ?? new();
        }
        catch { }
    }
    var xmlContent = SitemapLogic.Generate(siteBaseUrl, slugs);
    return Results.Text(xmlContent, "application/xml");
});

app.MapPost("/cache/invalidate", async (HttpContext context, IOutputCacheStore store, IConfiguration config) =>
{
    var secretKey = config["CacheSettings:CacheInvalidationKey"];
    var incomingKey = context.Request.Query["key"].ToString();

    if (string.IsNullOrEmpty(secretKey) || incomingKey != secretKey)
    {
        return Results.Unauthorized();
    }

    var tag = context.Request.Query["tag"].ToString();

    if (string.IsNullOrEmpty(tag))
    {
        await store.EvictByTagAsync("all-places", CancellationToken.None);
        return Results.Ok(new { message = "All places cache cleared" });
    }
    else
    {
        await store.EvictByTagAsync(tag, CancellationToken.None);
        return Results.Ok(new { message = $"Cache cleared for tag: {tag}" });
    }
});

app.Run();
