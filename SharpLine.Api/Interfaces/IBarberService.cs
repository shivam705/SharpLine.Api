using SharpLine.Api.Models;

namespace SharpLine.Api.Interfaces
{
    public interface IBarberService
    {
        Task<Barber> CreateBarberAsync(Barber barber);
        Task<Barber?> GetBarberByIdAsync(int id);
        Task<IEnumerable<Barber>> GetBarbersByShopAsync(int shopId);
        Task<Availability> AddAvailabilityAsync(Availability availability);
        Task RemoveAvailabilityAsync(int availabilityId);
    }
}
