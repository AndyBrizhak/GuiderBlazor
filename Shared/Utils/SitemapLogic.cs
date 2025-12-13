using System.Text.Json; // Для JsonElement
using System.Xml.Linq;

namespace GuiderBlazor.Shared.Utils
{
    public static class SitemapLogic
    {
        // Принимаем JsonElement, так как API отдает массив объектов
        public static string Generate(string siteBaseUrl, JsonElement items)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            // 1. Главная страница
            root.Add(new XElement(xmlns + "url",
                new XElement(xmlns + "loc", $"{siteBaseUrl}/"),
                new XElement(xmlns + "changefreq", "daily"),
                new XElement(xmlns + "priority", "1.0"),
                new XElement(xmlns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            ));

            // 2. Страницы мест
            if (items.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in items.EnumerateArray())
                {
                    // Парсим JSON объект без создания классов
                    // Ожидаем формат: { "url": "slug", "lastMod": "2024-12-13" }

                    string slug = item.GetProperty("url").GetString() ?? "";
                    string date = item.GetProperty("lastMod").GetString() ?? DateTime.UtcNow.ToString("yyyy-MM-dd");

                    if (!string.IsNullOrEmpty(slug))
                    {
                        var urlElement = new XElement(xmlns + "url",
                            new XElement(xmlns + "loc", $"{siteBaseUrl}/place/{slug}"),
                            new XElement(xmlns + "changefreq", "weekly"),
                            new XElement(xmlns + "priority", "0.8"),
                            new XElement(xmlns + "lastmod", date) // <-- Вставляем дату из базы
                        );
                        root.Add(urlElement);
                    }
                }
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return doc.Declaration + Environment.NewLine + doc.ToString();
        }
    }
}