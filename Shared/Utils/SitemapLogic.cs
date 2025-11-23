using System.Xml.Linq;

namespace GuiderBlazor.Shared.Utils
{
    public static class SitemapLogic
    {
        public static string Generate(string siteBaseUrl, IEnumerable<string> placeSlugs)
        {
            XNamespace xmlns = "http://www.sitemaps.org/schemas/sitemap/0.9";
            var root = new XElement(xmlns + "urlset");

            // 1. Главная страница
            root.Add(new XElement(xmlns + "url",
                new XElement(xmlns + "loc", $"{siteBaseUrl}/"),
                new XElement(xmlns + "changefreq", "daily"),
                new XElement(xmlns + "priority", "1.0")
            ));

            // 2. Страницы мест (Places)
            if (placeSlugs != null)
            {
                foreach (var slug in placeSlugs)
                {
                    root.Add(new XElement(xmlns + "url",
                        new XElement(xmlns + "loc", $"{siteBaseUrl}/place/{slug}"),
                        new XElement(xmlns + "changefreq", "weekly"),
                        new XElement(xmlns + "priority", "0.8")
                    ));
                }
            }

            var doc = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), root);
            return doc.Declaration + Environment.NewLine + doc.ToString();
        }
    }
}
