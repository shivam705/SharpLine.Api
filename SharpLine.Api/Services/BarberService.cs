using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Data;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;
using SharpLine.Api.Repositories;

namespace SharpLine.Api.Services
{
    public class BarberService : IBarberService
    {
        private readonly BarberRepository _barberRepo;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public BarberService(BarberRepository barberRepo, ApplicationDbContext context, IMapper mapper)
        {
            _barberRepo = barberRepo;
            _context = context;
            _mapper = mapper;
        }

        public async Task<BarberDto> CreateBarberAsync(BarberDto barberDto)
        {
            var barber = _mapper.Map<Barber>(barberDto);

            // Ensure shop is attached if provided
            if (barber.ShopId != 0)
            {
                // nothing to attach via Shop object since we only have ShopId
            }

            _context.Barbers.Add(barber);
            await _context.SaveChangesAsync();

            return _mapper.Map<BarberDto>(barber);
        }

        public async Task<BarberDto?> GetBarberByIdAsync(int id)
        {
            var barber = await _context.Barbers
                .Include(b => b.Availabilities)
                .Include(b => b.Bookings)
                .FirstOrDefaultAsync(b => b.Id == id);

            return barber == null ? null : _mapper.Map<BarberDto>(barber);
        }

        public async Task<IEnumerable<BarberDto>> GetBarbersByShopAsync(int shopId)
        {
            var items = await _context.Barbers
                .Include(b => b.Availabilities)
                .Where(b => b.ShopId == shopId)
                .ToListAsync();

            return items.Select(b => _mapper.Map<BarberDto>(b));
        }

        public async Task<AvailabilityDto> AddAvailabilityAsync(AvailabilityDto availabilityDto)
        {
            var barberExists = await _context.Barbers.AnyAsync(b => b.Id == availabilityDto.BarberId);
            if (!barberExists)
                throw new ArgumentException($"Barber with ID {availabilityDto.BarberId} does not exist.");

            var availability = _mapper.Map<Availability>(availabilityDto);
            _context.Availabilities.Add(availability);
            await _context.SaveChangesAsync();
            return _mapper.Map<AvailabilityDto>(availability);
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

        public async Task<bool> RemoveBarberAsync(int barberId)
        {
            var barber = await _context.Barbers.FindAsync(barberId);
            if (barber == null) return false;
            _context.Barbers.Remove(barber);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetBarberBusyStatusAsync(int barberId, bool isAvailable)
        {
            var barber = await _context.Barbers.FindAsync(barberId);
            if (barber == null) return false;
            barber.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetBarberBusyStatusByUserAsync(string userId, bool isAvailable)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;
            var barber = await _context.Barbers.FirstOrDefaultAsync(b => b.UserId == userId);
            if (barber == null) return false;
            barber.IsAvailable = isAvailable;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AvailabilityDto>> GetAvailabilitiesAsync(int barberId, DateTime fromUtc, DateTime toUtc)
        {
            var items = await _context.Availabilities
                .Where(a => a.BarberId == barberId && a.StartUtc >= fromUtc && a.EndUtc <= toUtc)
                .ToListAsync();

            return items.Select(a => _mapper.Map<AvailabilityDto>(a));
        }

        public async Task<bool> IsBarberAvailableAsync(int barberId, DateTime startUtc, DateTime endUtc)
        {
            var availability = await _context.Availabilities.FirstOrDefaultAsync(a =>
                a.BarberId == barberId && startUtc >= a.StartUtc && endUtc <= a.EndUtc);

            if (availability == null) return false;

            bool overlap = await _context.Bookings.AnyAsync(b =>
                b.BarberId == barberId && b.Status == BookingStatus.Confirmed &&
                ((startUtc >= b.StartUtc && startUtc < b.EndUtc) || (endUtc > b.StartUtc && endUtc <= b.EndUtc)));

            return !overlap;
        }
    }
}
