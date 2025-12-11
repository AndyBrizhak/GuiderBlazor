using System.Text.RegularExpressions;

namespace GuiderBlazor.Middleware
{
    public class ImageMirrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly HttpClient _httpClient;

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

            if (path != null && ImagePathRegex.IsMatch(path))
            {
                // Путь к физическому файлу
                var localFilePath = Path.Combine(_env.WebRootPath, path.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

                if (!File.Exists(localFilePath))
                {
                    // Файла НЕТ: качаем
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
                        }
                    }
                }
                else
                {
                    // --- Файл ЕСТЬ. Обновляем дату "посещения" УМНО ---
                    try
                    {
                        var lastWriteTime = File.GetLastWriteTimeUtc(localFilePath);

                        // Если файл не обновлялся более 10 дней, обновляем метку времени.
                        // ImageSharp пересоздаст кеш только 1 раз в 10 дней, а не каждый раз.
                        if (DateTime.UtcNow - lastWriteTime > TimeSpan.FromDays(10))
                        {
                            File.SetLastWriteTimeUtc(localFilePath, DateTime.UtcNow);
                        }
                    }
                    catch
                    {
                        // Игнорируем ошибки (файл занят и т.д.)
                    }
                }
            }

            await _next(context);
        }

        private async Task DownloadAndSaveImageAsync(string sourceUrl, string localPath)
        {
            var directory = Path.GetDirectoryName(localPath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var imageBytes = await _httpClient.GetByteArrayAsync(sourceUrl);
            await File.WriteAllBytesAsync(localPath, imageBytes);
        }
    }
}