using SharpLine.Api.Models.Dtos;

namespace SharpLine.Api.Interfaces
{
    public interface IBarberService
    {
        Task<BarberDto> CreateBarberAsync(BarberDto barber);
        Task<BarberDto?> GetBarberByIdAsync(int id);
        Task<IEnumerable<BarberDto>> GetBarbersByShopAsync(int shopId);
        Task<AvailabilityDto> AddAvailabilityAsync(AvailabilityDto availability);
        Task RemoveAvailabilityAsync(int availabilityId);
        Task<bool> RemoveBarberAsync(int barberId);
        Task<IEnumerable<AvailabilityDto>> GetAvailabilitiesAsync(int barberId, DateTime fromUtc, DateTime toUtc);
        Task<bool> IsBarberAvailableAsync(int barberId, DateTime startUtc, DateTime endUtc);
        Task<bool> SetBarberBusyStatusAsync(int barberId, bool isAvailable);
        // Toggle barber busy/available status at the barber-level
        Task<bool> SetBarberBusyStatusByUserAsync(string userId, bool isAvailable);
    }
}
