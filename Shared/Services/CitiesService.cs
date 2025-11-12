using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace GuiderBlazor.Shared.Services
{
    public class CitiesService : ICitiesService
    {
        private readonly HttpClient _httpClient;

        public CitiesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<string>> GetActiveCitiesAsync(string? category, string? province)
        {
            // Формируем query string для /cities/active
            var queryParams = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(category))
            {
                queryParams["category"] = category;
            }
            if (!string.IsNullOrEmpty(province))
            {
                queryParams["province"] = province;
            }

            var queryString = string.Join("&", queryParams
                .Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var apiUrl = string.IsNullOrEmpty(queryString) ? "cities/active" : $"cities/active?{queryString}";

            try
            {
                var cities = await _httpClient.GetFromJsonAsync<List<string>>(apiUrl);
                return cities ?? new List<string>();
            }
            catch (Exception ex)
            {
                // В случае ошибки API (например, 404 или 500) просто возвращаем пустой список
                Console.WriteLine($"Error loading active cities: {ex.Message}");
                return new List<string>();
            }
        }
    }
}
