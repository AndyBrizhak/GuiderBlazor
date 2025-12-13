using GuiderBlazor.Shared.Models;
using System.Xml.Linq;

namespace GuiderBlazor.Shared.Utils
{
    public static class SitemapLogic
    {
        public static string Generate(string siteBaseUrl, List<SitemapPlaceDto> items)
        {
            // 🔴 БРЕЙКПОИНТ: Проверить items.Count

            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            root.Add(new XElement(xmlns + "url",
                new XElement(xmlns + "loc", $"{siteBaseUrl}/"),
                new XElement(xmlns + "changefreq", "daily"),
                new XElement(xmlns + "priority", "1.0"),
                new XElement(xmlns + "lastmod", DateTime.UtcNow.ToString("yyyy-MM-dd"))
            ));

            foreach (var item in items)
            {
                // 🔴 БРЕЙКПОИНТ: Проверить item.Url и item.LastMod

                if (!string.IsNullOrEmpty(item.Url))
                {
                    var urlElement = new XElement(xmlns + "url",
                        new XElement(xmlns + "loc", $"{siteBaseUrl}/place/{item.Url}"),
                        new XElement(xmlns + "changefreq", "weekly"),
                        new XElement(xmlns + "priority", "0.8"),
                        new XElement(xmlns + "lastmod", item.LastMod)
                    );
                    root.Add(urlElement);
                }
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return doc.Declaration + Environment.NewLine + doc.ToString();
        }
    }
}