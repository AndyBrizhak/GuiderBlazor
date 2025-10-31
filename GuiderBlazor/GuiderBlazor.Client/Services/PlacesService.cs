using GuiderBlazor.Client.Models;
using System.Net.Http.Json;

namespace GuiderBlazor.Client.Services;

public class PlacesService : IPlacesService
{
    private readonly HttpClient _httpClient;

    public PlacesService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<PlacesResponse?> GetPlacesAsync(PlaceFilterParams filters)
    {
        try
        {
            var queryString = BuildQueryString(filters);

            // Отправляем запрос
            var response = await _httpClient.GetAsync($"places/filters?{queryString}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: {response.StatusCode}");
                return null;
            }

            // Читаем заголовок X-Total-Count
            long totalCount = 0;
            if (response.Headers.TryGetValues("X-Total-Count", out var totalCountValues))
            {
                long.TryParse(totalCountValues.FirstOrDefault(), out totalCount);
            }

            // Читаем тело ответа (массив мест)
            var places = await response.Content.ReadFromJsonAsync<List<Place>>();

            if (places == null)
            {
                return null;
            }

            // Формируем ответ
            var result = new PlacesResponse
            {
                Places = places,
                TotalCount = totalCount,
                Page = filters.Page,
                PerPage = filters.PerPage,
                TotalPages = (int)Math.Ceiling((double)totalCount / filters.PerPage)
            };

            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching places: {ex.Message}");
            return null;
        }
    }

    public async Task<Place?> GetPlaceByIdAsync(string id)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<Place>($"places/{id}");
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching place: {ex.Message}");
            return null;
        }
    }

    private string BuildQueryString(PlaceFilterParams filters)
    {
        var queryParams = new List<string>();

        // Текстовый поиск
        if (!string.IsNullOrWhiteSpace(filters.Q))
            queryParams.Add($"q={Uri.EscapeDataString(filters.Q)}");

        // Географические фильтры
        if (!string.IsNullOrWhiteSpace(filters.Province))
            queryParams.Add($"province={Uri.EscapeDataString(filters.Province)}");

        if (!string.IsNullOrWhiteSpace(filters.City))
            queryParams.Add($"city={Uri.EscapeDataString(filters.City)}");

        // Основные фильтры
        if (!string.IsNullOrWhiteSpace(filters.Name))
            queryParams.Add($"name={Uri.EscapeDataString(filters.Name)}");

        if (!string.IsNullOrWhiteSpace(filters.Url))
            queryParams.Add($"url={Uri.EscapeDataString(filters.Url)}");

        if (!string.IsNullOrWhiteSpace(filters.Category))
            queryParams.Add($"category={Uri.EscapeDataString(filters.Category)}");

        if (!string.IsNullOrWhiteSpace(filters.Status))
            queryParams.Add($"status={Uri.EscapeDataString(filters.Status)}");

        // Теги (список через запятую)
        if (filters.Tags?.Any() == true)
        {
            var tagsString = string.Join(",", filters.Tags);
            queryParams.Add($"tags={Uri.EscapeDataString(tagsString)}");
        }

        queryParams.Add($"tagsMode={filters.TagsMode}");

        // Геопространственный поиск
        if (filters.Latitude.HasValue)
            queryParams.Add($"latitude={filters.Latitude.Value}");

        if (filters.Longitude.HasValue)
            queryParams.Add($"longitude={filters.Longitude.Value}");

        if (filters.Distance.HasValue)
            queryParams.Add($"distance={filters.Distance.Value}");

        // Фильтр по времени работы
        if (filters.IsOpen.HasValue)
            queryParams.Add($"isOpen={filters.IsOpen.Value.ToString().ToLower()}");

        // Пагинация и сортировка
        queryParams.Add($"page={filters.Page}");
        queryParams.Add($"perPage={filters.PerPage}");
        queryParams.Add($"sortField={filters.SortField}");
        queryParams.Add($"sortOrder={filters.SortOrder}");

        // ВАЖНО: API также принимает параметры _sort и _order (из кода контроллера)
        // Добавляем их для совместимости
        queryParams.Add($"_sort={filters.SortField}");
        queryParams.Add($"_order={filters.SortOrder}");

        return string.Join("&", queryParams);
    }
}