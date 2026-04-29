using SharpLine.Api.Models;

namespace SharpLine.Api.Interfaces
{
    public interface IShopService
    {
        Task<IEnumerable<Shop>> GetNearestShopsAsync(double latitude, double longitude, double radiusKm = 5);
        Task<Shop> CreateShopAsync(Shop shop);
        Task<Shop?> GetShopByIdAsync(int id);
    }
}
