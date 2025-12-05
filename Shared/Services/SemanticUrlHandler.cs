using System.Text.RegularExpressions;

namespace GuiderBlazor.Shared.Services
{

    /// <summary>
    /// Обработчик семантических URL с явными префиксами для исключения коллизий
    /// Структура: /category/{cat}/province/{prov}/city/{city}
    /// </summary>
    public class SemanticUrlHandler
    {
        private static readonly HashSet<string> ValidCategories = new(StringComparer.OrdinalIgnoreCase)
    {
        "services", "to-eat", "adventures", "shops"
    };

        /// <summary>
        /// Парсит семантический URL с префиксами в параметры фильтрации
        /// Примеры:
        /// /category/services -> Category=services
        /// /province/guanacaste -> Province=guanacaste
        /// /city/playa-del-coco -> City=playa-del-coco
        /// /category/services/province/guanacaste -> Category=services, Province=guanacaste
        /// /category/services/city/playa-del-coco -> Category=services, City=playa-del-coco (без провинции)
        /// /province/guanacaste/city/playa-del-coco -> Province=guanacaste, City=playa-del-coco
        /// /category/services/province/guanacaste/city/playa-del-coco -> все три
        /// </summary>
        public static (string? Category, string? Province, string? City) ParsePath(string path)
        {
            path = path.Trim('/');

            if (string.IsNullOrEmpty(path))
                return (null, null, null);

            string? category = null;
            string? province = null;
            string? city = null;

            var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < segments.Length - 1; i += 2)
            {
                var prefix = segments[i].ToLowerInvariant();
                var value = segments[i + 1];

                switch (prefix)
                {
                    case "category":
                        if (IsCategory(value))
                            category = NormalizeSlug(value);
                        break;
                    case "province":
                        province = NormalizeSlug(value);
                        break;
                    case "city":
                        city = NormalizeSlug(value);
                        break;
                }
            }

            return (category, province, city);
        }

        /// <summary>
        /// Строит семантический путь из параметров фильтрации с явными префиксами
        /// </summary>
        public static string BuildPath(string? category, string? province, string? city)
        {
            var segments = new List<string>();

            if (!string.IsNullOrEmpty(category))
            {
                segments.Add("category");
                segments.Add(ToUrlSlug(category));
            }

            if (!string.IsNullOrEmpty(province))
            {
                segments.Add("province");
                segments.Add(ToUrlSlug(province));
            }

            if (!string.IsNullOrEmpty(city))
            {
                segments.Add("city");
                segments.Add(ToUrlSlug(city));
            }

            return segments.Count == 0 ? "/" : $"/{string.Join("/", segments)}";
        }

        /// <summary>
        /// Строит полный URL с query-параметрами
        /// </summary>
        public static string BuildFullUrl(
            string? category = null,
            string? province = null,
            string? city = null,
            string? q = null,
            List<string>? tags = null,
            int? page = null,
            int? perPage = null,
            double? latitude = null,
            double? longitude = null,
            double? distance = null,
            bool? isOpen = null,
            string? sortField = null,
            string? sortOrder = null)
        {
            var path = BuildPath(category, province, city);
            var queryParts = new List<string>();

            // Query-параметры
            if (!string.IsNullOrEmpty(q))
                queryParts.Add($"Q={Uri.EscapeDataString(q)}");

            if (tags != null && tags.Any())
            {
                foreach (var tag in tags)
                {
                    if (!string.IsNullOrEmpty(tag))
                        queryParts.Add($"Tags={Uri.EscapeDataString(tag)}");
                }
            }

            if (page.HasValue && page.Value > 1)
                queryParts.Add($"Page={page.Value}");

            if (perPage.HasValue && perPage.Value != 4)
                queryParts.Add($"PerPage={perPage.Value}");

            if (latitude.HasValue)
                queryParts.Add($"Latitude={latitude.Value}");

            if (longitude.HasValue)
                queryParts.Add($"Longitude={longitude.Value}");

            if (distance.HasValue)
                queryParts.Add($"Distance={distance.Value}");

            if (isOpen == true)
                queryParts.Add("IsOpen=true");

            if (!string.IsNullOrEmpty(sortField))
                queryParts.Add($"SortField={Uri.EscapeDataString(sortField)}");

            if (!string.IsNullOrEmpty(sortOrder))
                queryParts.Add($"SortOrder={Uri.EscapeDataString(sortOrder)}");

            return queryParts.Count == 0
                ? path
                : $"{path}?{string.Join("&", queryParts)}";
        }

        /// <summary>
        /// Проверяет, является ли строка валидной категорией
        /// </summary>
        private static bool IsCategory(string segment)
        {
            return ValidCategories.Contains(segment);
        }

        /// <summary>
        /// Преобразует строку в URL-slug (kebab-case)
        /// "Playa del Coco" -> "playa-del-coco"
        /// "To Eat" -> "to-eat"
        /// </summary>
        public static string ToUrlSlug(string input)
        {
            if (string.IsNullOrEmpty(input))
                return string.Empty;

            var slug = input.ToLowerInvariant();
            slug = Regex.Replace(slug, @"[\s_]+", "-");
            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = Regex.Replace(slug, @"-+", "-");
            slug = slug.Trim('-');

            return slug;
        }

        /// <summary>
        /// Нормализует slug для отправки в API
        /// API принимает как "playa-del-coco", так и "Playa del Coco"
        /// </summary>
        public static string NormalizeSlug(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return string.Empty;

            // Оставляем slug как есть - API нормализует сам
            return slug;
        }

        /// <summary>
        /// Преобразует slug обратно в человекочитаемый формат
        /// "playa-del-coco" -> "Playa Del Coco"
        /// </summary>
        public static string SlugToTitle(string slug)
        {
            if (string.IsNullOrEmpty(slug))
                return string.Empty;

            var title = slug.Replace("-", " ");
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(title);
        }

        /// <summary>
        /// Генерирует route pattern для Blazor @page директивы
        /// </summary>
        public static class RoutePatterns
        {
            public const string Root = "/";
            public const string Category = "/category/{CategorySlug}";
            public const string Province = "/province/{ProvinceSlug}";
            public const string City = "/city/{CitySlug}";
            public const string CategoryProvince = "/category/{CategorySlug}/province/{ProvinceSlug}";
            public const string CategoryCity = "/category/{CategorySlug}/city/{CitySlug}";
            public const string ProvinceCity = "/province/{ProvinceSlug}/city/{CitySlug}";
            public const string Full = "/category/{CategorySlug}/province/{ProvinceSlug}/city/{CitySlug}";
        }
    }

}
