using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Data;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Repositories;

namespace SharpLine.Api.Services
{
    public class BarberService : IBarberService
    {
        private readonly BarberRepository _barberRepo;
        private readonly ApplicationDbContext _context;

        public BarberService(BarberRepository barberRepo, ApplicationDbContext context)
        {
            _barberRepo = barberRepo;
            _context = context;
        }

        public async Task<Barber> CreateBarberAsync(Barber barber)
        {
            if (barber.Shop != null)
            {
                _context.Attach(barber.Shop);
            }

            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();
            return barber;
        }

        public async Task<Barber?> GetBarberByIdAsync(int id)
        {
            return await _context.Barbers
                .Include(b => b.Availabilities)
                .Include(b => b.Bookings)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        
        public async Task<IEnumerable<Barber>> GetBarbersByShopAsync(int shopId)
        {
            return await _context.Barbers
                .Include(b => b.Availabilities)
                .Where(b => b.ShopId == shopId)
                .ToListAsync();
        }

        public async Task<Availability> AddAvailabilityAsync(Availability availability)
        {
            var barberExists = await _context.Barbers.AnyAsync(b => b.Id == availability.BarberId);
            if (!barberExists)
                throw new ArgumentException($"Barber with ID {availability.BarberId} does not exist.");

            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
            return availability;
        }

        public async Task RemoveAvailabilityAsync(int availabilityId)
        {
            var entity = await _context.Availabilities.FindAsync(availabilityId);
            if (entity != null)
            {
                _context.Availabilities.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
