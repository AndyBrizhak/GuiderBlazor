using GuiderBlazor.Shared.Models;


namespace GuiderBlazor.Shared.Services
{
    public interface IPlacesService
    {
        Task<PlacesResponse?> GetPlacesAsync(PlaceFilterParams filters);
        Task<Place?> GetPlaceByIdAsync(string id);
    }
}
