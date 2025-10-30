using GuiderBlazor.Client.Models;
using System.Net.Http.Json;

namespace GuiderBlazor.Client.Services
{
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
                var response = await _httpClient.GetFromJsonAsync<PlacesResponse>(
                    $"places/filters?{queryString}");
                return response;
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

            if (!string.IsNullOrWhiteSpace(filters.SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(filters.SearchTerm)}");

            if (filters.Tags?.Any() == true)
            {
                foreach (var tag in filters.Tags)
                    queryParams.Add($"tags={Uri.EscapeDataString(tag)}");
            }

            queryParams.Add($"tagsMode={filters.TagsMode}");

            if (!string.IsNullOrWhiteSpace(filters.Category))
                queryParams.Add($"category={Uri.EscapeDataString(filters.Category)}");

            queryParams.Add($"page={filters.Page}");
            queryParams.Add($"perPage={filters.PerPage}");
            queryParams.Add($"sortField={filters.SortField}");
            queryParams.Add($"sortOrder={filters.SortOrder}");

            return string.Join("&", queryParams);
        }
    }
}
