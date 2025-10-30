using GuiderBlazor.Client.Models;


namespace GuiderBlazor.Client.Services
{
    public interface IPlacesService
    {
        Task<PlacesResponse?> GetPlacesAsync(PlaceFilterParams filters);
        Task<Place?> GetPlaceByIdAsync(string id);
    }
}
