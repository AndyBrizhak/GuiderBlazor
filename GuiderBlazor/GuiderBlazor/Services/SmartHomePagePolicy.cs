using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace GuiderBlazor.Services;

public sealed class SmartHomePagePolicy : IOutputCachePolicy
{
    public ValueTask CacheRequestAsync(OutputCacheContext context, CancellationToken cancellation)
    {
        var request = context.HttpContext.Request;
        var query = request.Query;

        // 1. BYPASS (Пропуск кэширования для поиска, гео, тегов и открытых мест)
        if (query.ContainsKey("Q") || query.ContainsKey("Tags") ||
            query.ContainsKey("Latitude") || query.ContainsKey("Longitude") ||
            query.ContainsKey("IsOpen"))
        {
            context.EnableOutputCaching = false;
            return ValueTask.CompletedTask;
        }

        if (query.ContainsKey("SortField") && query["SortField"] != "name")
        {
            context.EnableOutputCaching = false;
            return ValueTask.CompletedTask;
        }

        if (query.TryGetValue("Page", out var pageVal) && int.TryParse(pageVal, out int pageNum))
        {
            if (pageNum > 3)
            {
                context.EnableOutputCaching = false;
                return ValueTask.CompletedTask;
            }
        }

        // --- 2. РАЗРЕШАЕМ КЭШИРОВАНИЕ ---
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = true;
        context.AllowCacheStorage = true;
        context.AllowLocking = true;

        // Настройка ключей Query
        context.CacheVaryByRules.QueryKeys = new[]
        {
            "Category",
            "Province",
            "City",
            "Page"
        };

        // === ИСПРАВЛЕНИЕ ДЛЯ ТЕМЫ (Cookie) ===
        // Вместо несуществующего метода используем словарь VaryByValues
        var themeCookie = request.Cookies["theme"];

        // Добавляем пару "ключ-значение" в словарь правил
        context.CacheVaryByRules.VaryByValues["theme_key"] = themeCookie ?? "default";
        // =====================================


        // --- 3. ДИНАМИЧЕСКИЙ ТАЙМЕР (TTL) ---
        bool hasFilters = query.ContainsKey("Category") ||
                          query.ContainsKey("Province") ||
                          query.ContainsKey("City") ||
                          (query.ContainsKey("Page") && query["Page"] != "1");

        if (!hasFilters)
        {
            context.ResponseExpirationTimeSpan = TimeSpan.FromHours(24);
        }
        else
        {
            context.ResponseExpirationTimeSpan = TimeSpan.FromHours(1);
        }

        return ValueTask.CompletedTask;
    }

    public ValueTask ServeFromCacheAsync(OutputCacheContext context, CancellationToken cancellation)
        => ValueTask.CompletedTask;

    public ValueTask ServeResponseAsync(OutputCacheContext context, CancellationToken cancellation)
        => ValueTask.CompletedTask;
}
