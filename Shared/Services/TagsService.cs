using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GuiderBlazor.Shared.Services
{
    public class TagsService : ITagsService
    {
        private readonly HttpClient _httpClient;

        public TagsService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetActiveTagsAsync(string? category, string? province, string? city, List<string>? selectedTags)
        {
            // Используем List<string> для динамической сборки всех частей запроса
            var queryParts = new List<string>();

            // Добавляем простые фильтры, если они есть
            if (!string.IsNullOrEmpty(category))
            {
                queryParts.Add($"category={Uri.EscapeDataString(category)}");
            }
            if (!string.IsNullOrEmpty(province))
            {
                queryParts.Add($"province={Uri.EscapeDataString(province)}");
            }
            if (!string.IsNullOrEmpty(city))
            {
                queryParts.Add($"city={Uri.EscapeDataString(city)}");
            }

            // Добавляем 'selectedTags'
            // API ожидает несколько параметров 'selectedTags=tag1&selectedTags=tag2'
            if (selectedTags != null && selectedTags.Any())
            {
                foreach (var tag in selectedTags)
                {
                    if (!string.IsNullOrEmpty(tag))
                    {
                        queryParts.Add($"selectedTags={Uri.EscapeDataString(tag)}");
                    }
                }
            }

            // Собираем итоговую строку запроса
            var queryString = string.Join("&", queryParts);

            // Определяем URL: либо базовый, либо с параметрами
            var apiUrl = string.IsNullOrEmpty(queryString) ? "tags/active" : $"tags/active?{queryString}";

            try
            {
                // Выполняем GET-запрос и десериализуем List<string>
                var tags = await _httpClient.GetFromJsonAsync<List<string>>(apiUrl);
                return tags ?? new List<string>();
            }
            catch (Exception ex)
            {
                // В случае ошибки API (например, 404 или 500) просто возвращаем пустой список
                Console.WriteLine($"Error loading active tags: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
