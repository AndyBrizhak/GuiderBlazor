using System.Text.RegularExpressions;

namespace GuiderBlazor.Middleware
{
    public class ImageMirrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;

        // Регулярка: ловит запросы вида /images/.../.../file.jpg (или png/webp)
        private static readonly Regex ImagePathRegex = new Regex(@"^/images/[^/]+/[^/]+/.+\.(jpg|jpeg|png|webp)$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public ImageMirrorMiddleware(RequestDelegate next, IWebHostEnvironment env)
        {
            _next = next;
            _env = env;
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(15);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            // 1. Если запрос похож на путь к картинке
            if (path != null && ImagePathRegex.IsMatch(path))
            {
                // Превращаем URL путь в физический путь на диске (wwwroot/images/...)
                var localFilePath = Path.Combine(_env.WebRootPath, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                // 2. Если файла НЕТ на диске
                if (!File.Exists(localFilePath))
                {
                    // Проверяем, указан ли источник (?source=http...)
                    var sourceUrl = context.Request.Query["source"].ToString();

                    if (!string.IsNullOrEmpty(sourceUrl) && Uri.TryCreate(sourceUrl, UriKind.Absolute, out var uri))
                    {
                        try
                        {
                            await DownloadAndSaveImageAsync(sourceUrl, localFilePath);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"[ImageMirror] Ошибка скачивания {sourceUrl}: {ex.Message}");
                            // Не ломаем запрос, пусть вернется 404, если не скачалось
                        }
                    }
                }
            }

            // Передаем управление дальше (ImageSharp подхватит файл, если он теперь есть)
            await _next(context);
        }

        private async Task DownloadAndSaveImageAsync(string sourceUrl, string localPath)
        {
            // Создаем папки (province/place), если их нет
            var directory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Качаем и сохраняем
            var imageBytes = await _httpClient.GetByteArrayAsync(sourceUrl);
            await File.WriteAllBytesAsync(localPath, imageBytes);
        }
    }
}
