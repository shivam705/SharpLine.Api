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
                var distance = HaversineDistanceKm(latitude, longitude, shop.Latitude, shop.Longitude);
                return distance <= radiusKm;
            });
        }

        public async Task<IEnumerable<Models.Dtos.ShopWithDistanceDto>> SearchShopsWithDistanceAsync(double latitude, double longitude, double radiusKm = 5)
        {
            var shops = await _shopRepo.GetAllAsync();
            var result = shops.Select(shop => new Models.Dtos.ShopWithDistanceDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                OwnerId = shop.OwnerId,
                DistanceKm = HaversineDistanceKm(latitude, longitude, shop.Latitude, shop.Longitude)
            })
            .Where(s => s.DistanceKm <= radiusKm)
            .OrderBy(s => s.DistanceKm);

            return result;
        }

        private double HaversineDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            double R = 6371; // Earth radius in km
            double dLat = (lat2 - lat1) * Math.PI / 180.0;
            double dLon = (lon2 - lon1) * Math.PI / 180.0;
            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                       Math.Cos(lat1 * Math.PI / 180.0) * Math.Cos(lat2 * Math.PI / 180.0) *
                       Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
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
