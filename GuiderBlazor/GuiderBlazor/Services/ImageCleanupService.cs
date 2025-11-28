using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace GuiderBlazor.Services
{
    public class ImageCleanupService : BackgroundService
    {
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ImageCleanupService> _logger;

        // SEO-Стратегия: Храним файлы 180 дней (полгода) с момента ПОСЛЕДНЕГО ПРОСМОТРА.
        // Это гарантирует, что редкие страницы не будут терять картинки слишком быстро,
        // но совсем мертвый контент со временем удалится.
        private readonly TimeSpan _retentionPeriod = TimeSpan.FromDays(180);

        // Проверка запускается раз в 24 часа
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public ImageCleanupService(IWebHostEnvironment env, ILogger<ImageCleanupService> logger)
        {
            _env = env;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("[ImageCleanup] Сервис очистки запущен. Период хранения: {Days} дней.", _retentionPeriod.TotalDays);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Ждем, чтобы не нагружать сервер сразу при старте приложения
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);

                    CleanupOldImages();
                }
                catch (OperationCanceledException)
                {
                    // Нормальная остановка сервиса
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[ImageCleanup] Ошибка в процессе очистки.");
                }

                // Ждем до следующего цикла (24 часа)
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private void CleanupOldImages()
        {
            // Путь к папке wwwroot/images
            var imagesPath = Path.Combine(_env.WebRootPath, "images");

            if (!Directory.Exists(imagesPath)) return;

            var files = Directory.GetFiles(imagesPath, "*.*", SearchOption.AllDirectories);

            // Дата отсечения: Текущее время минус 180 дней
            var thresholdDate = DateTime.UtcNow.Subtract(_retentionPeriod);
            var deletedCount = 0;
            var errorsCount = 0;

            foreach (var file in files)
            {
                var fileInfo = new FileInfo(file);

                // 1. Пропускаем плейсхолдеры или системные файлы, если есть
                if (fileInfo.Name.Contains("placeholder")) continue;

                // 2. Проверяем LastWriteTimeUtc.
                // Middleware будет обновлять это время при каждом просмотре картинки.
                if (fileInfo.LastWriteTimeUtc < thresholdDate)
                {
                    try
                    {
                        fileInfo.Delete();
                        deletedCount++;
                    }
                    catch (Exception ex)
                    {
                        // Файл может быть заблокирован системой
                        errorsCount++;
                    }
                }
            }

            if (deletedCount > 0)
            {
                _logger.LogInformation($"[ImageCleanup] Удалено {deletedCount} устаревших изображений.");
            }

            // Удаляем пустые папки, чтобы не мусорить в структуре
            CleanupEmptyDirectories(imagesPath);
        }

        private void CleanupEmptyDirectories(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                CleanupEmptyDirectories(directory);
                try
                {
                    if (Directory.GetFiles(directory).Length == 0 &&
                        Directory.GetDirectories(directory).Length == 0)
                    {
                        Directory.Delete(directory, false);
                    }
                }
                catch
                {
                    // Игнорируем ошибки удаления папок
                }
            }
        }
    }
}