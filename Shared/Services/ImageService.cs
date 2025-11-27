using System.Text.RegularExpressions;
using System.Net;

namespace GuiderBlazor.Shared.Services
{
    /// <summary>
    /// Сервис для генерации правильных SEO-ссылок на изображения.
    /// Находится в Shared, чтобы и клиент, и сервер знали, как строить пути.
    /// </summary>
    public class ImageService
    {
        public string GetStructuredImageUrl(string originalUrl, string placeName, string? province)
        {
            // Заглушка, если ссылки нет
            if (string.IsNullOrEmpty(originalUrl)) return "/images/placeholder.jpg";

            // Если ссылка уже локальная (начинается со слеша), не трогаем её
            if (originalUrl.StartsWith("/")) return originalUrl;

            // 1. Нормализация (очистка) названий для путей
            var safeProvince = NormalizeSlug(province ?? "general");
            var safePlace = NormalizeSlug(placeName);

            // Получаем имя файла
            string fileName;
            try
            {
                var uri = new Uri(originalUrl);
                fileName = Path.GetFileName(uri.LocalPath);
            }
            catch
            {
                // Если URL битый, возвращаем как есть или заглушку
                return originalUrl;
            }

            var safeFileName = NormalizeSlug(Path.GetFileNameWithoutExtension(fileName)) + Path.GetExtension(fileName);

            // 2. Формируем "виртуальный" путь: /images/province/place/file.jpg
            var virtualPath = $"/images/{safeProvince}/{safePlace}/{safeFileName}";

            // 3. Добавляем параметр source (откуда качать оригинал, если его нет)
            var encodedSource = WebUtility.UrlEncode(originalUrl);

            // Итоговый URL
            return $"{virtualPath}?source={encodedSource}";
        }

        private string NormalizeSlug(string text)
        {
            if (string.IsNullOrEmpty(text)) return "unknown";

            text = text.ToLowerInvariant();
            text = text.Replace(" ", "-");
            // Оставляем только a-z, 0-9, дефис и точку
            text = Regex.Replace(text, @"[^a-z0-9\-\.]", "");
            text = Regex.Replace(text, @"-+", "-");

            return text;
        }
    }
}
