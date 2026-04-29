using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Repositories;

namespace SharpLine.Api.Services
{
    public class ShopService : IShopService
    {
        private readonly ShopRepository _shopRepo;

        public ShopService(ShopRepository shopRepo)
        {
            _shopRepo = shopRepo;
        }

        public async Task<IEnumerable<Shop>> GetNearestShopsAsync(double latitude, double longitude, double radiusKm = 5)
        {
            var shops = await _shopRepo.GetAllAsync();

            return shops.Where(shop =>
            {
                double dLat = (shop.Latitude - latitude) * Math.PI / 180.0;
                double dLon = (shop.Longitude - longitude) * Math.PI / 180.0;
                double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                           Math.Cos(latitude * Math.PI / 180.0) * Math.Cos(shop.Latitude * Math.PI / 180.0) *
                           Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                double distance = 6371 * c; // radius of Earth in km
                return distance <= radiusKm;
            });
        }

        public async Task<Shop> CreateShopAsync(Shop shop)
        {
            await _shopRepo.AddAsync(shop);
            await _shopRepo.SaveChangesAsync();
            return shop;
        }

        public async Task<Shop?> GetShopByIdAsync(int id) => await _shopRepo.GetByIdAsync(id);
    }
}
