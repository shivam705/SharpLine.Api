using Microsoft.EntityFrameworkCore;
using SharpLine.Api.Data;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Repositories;

namespace SharpLine.Api.Services
{
    public class BookingService : IBookingService
    {
        private readonly BookingRepository _bookingRepo;
        private readonly ApplicationDbContext _context;
        private readonly IBarberService _barberService;

        public BookingService(BookingRepository bookingRepo, ApplicationDbContext context, IBarberService barberService)
        {
            _bookingRepo = bookingRepo;
            _context = context;
            _barberService = barberService;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            // Validate using BarberService
            var isAvailable = await _barberService.IsBarberAvailableAsync(booking.BarberId, booking.StartUtc, booking.EndUtc);
            if (!isAvailable)
                throw new Services.Exceptions.BookingConflictException("Barber not available for selected time or time slot already booked.");

            await _bookingRepo.AddAsync(booking);
            await _bookingRepo.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking?> GetBookingByIdAsync(int id) => await _bookingRepo.GetByIdAsync(id);

        public async Task<IEnumerable<Booking>> GetBookingsByCustomerAsync(string customerId)
        {
            return await _context.Bookings
                .Include(b => b.Barber)
                .Where(b => b.CustomerId == customerId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetBookingsByBarberAsync(int barberId)
        {
            return await _context.Bookings
                .Include(b => b.Customer)
                .Where(b => b.BarberId == barberId)
                .ToListAsync();
        }

        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            var booking = await _context.Bookings.FindAsync(bookingId);
            if (booking == null) return false;

            booking.Status = BookingStatus.Cancelled;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
