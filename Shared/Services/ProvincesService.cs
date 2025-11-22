using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GuiderBlazor.Shared.Models;

namespace GuiderBlazor.Shared.Services
{
    public class ProvincesService : IProvincesService
    {
        private readonly HttpClient _httpClient;

        public ProvincesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<ProvinceDto>> GetActiveProvincesAsync(string? category = null)
        {
            try
            {
                // Формируем URL запроса
                var url = "provinces/active";

                if (!string.IsNullOrEmpty(category))
                {
                    // Не забываем экранировать параметры
                    url += $"?category={Uri.EscapeDataString(category)}";
                }

                // Blazor автоматически десериализует JSON array [ {name, slug}, ... ] в List<ProvinceDto>
                var result = await _httpClient.GetFromJsonAsync<List<ProvinceDto>>(url);

                return result ?? new List<ProvinceDto>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading active provinces: {ex.Message}");
                // Возвращаем пустой список, чтобы не "ронять" интерфейс
                return new List<ProvinceDto>();
            }
        }
    }
}
