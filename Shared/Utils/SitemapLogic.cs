using System.Xml.Linq;
using GuiderBlazor.Shared.Models; // Подключите ваш DTO

namespace GuiderBlazor.Shared.Utils
{
    public static class SitemapLogic
    {
        public static string Generate(string siteBaseUrl, IEnumerable<SitemapItemDto> items)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            // 1. Главная страница (Всегда Priority 1.0)
            root.Add(new XElement(xmlns + "url",
                new XElement(xmlns + "loc", $"{siteBaseUrl}/"),
                new XElement(xmlns + "changefreq", "daily"),
                new XElement(xmlns + "priority", "1.0"),
                new XElement(xmlns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            ));

            // 2. Динамические страницы (Места + Фильтры)
            if (items != null)
            {
                foreach (var item in items)
                {
                    if (string.IsNullOrEmpty(item.Url)) continue;

                    string fullUrl;
                    string priority;
                    string changefreq;

                    // Убираем начальный слеш, если он вдруг пришел из API, чтобы не было двойных слешей
                    string cleanUrl = item.Url.TrimStart('/');

                    // Логика определения типа страницы
                    if (cleanUrl.StartsWith("?"))
                    {
                        // ЭТО ФИЛЬТР (Category, Province, City)
                        // Пример: http://localhost:5000/?Category=to-eat
                        fullUrl = $"{siteBaseUrl}/{cleanUrl}";

                        // Фильтры — это хабы, они важны для навигации бота
                        priority = "0.9";
                        changefreq = "daily";
                    }
                    else
                    {
                        // ЭТО МЕСТО (Place)
                        // API уже вернул нам "place/slug", просто клеим
                        // Пример: http://localhost:5000/place/some-place
                        fullUrl = $"{siteBaseUrl}/{cleanUrl}";

                        // Отдельные места чуть менее важны, чем разделы
                        priority = "0.8";
                        changefreq = "weekly";
                    }

                    // Дата: если пустая, ставим старую заглушку
                    var date = string.IsNullOrEmpty(item.LastMod)
                        ? "2024-01-01"
                        : item.LastMod;

                    root.Add(new XElement(xmlns + "url",
                        new XElement(xmlns + "loc", fullUrl),
                        new XElement(xmlns + "changefreq", changefreq),
                        new XElement(xmlns + "priority", priority),
                        new XElement(xmlns + "lastmod", date)
                    ));
                }
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return doc.Declaration + Environment.NewLine + doc.ToString();
        }
    }
}