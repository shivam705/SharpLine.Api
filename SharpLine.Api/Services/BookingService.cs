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

        public BookingService(BookingRepository bookingRepo, ApplicationDbContext context)
        {
            _bookingRepo = bookingRepo;
            _context = context;
        }

        public async Task<Booking> CreateBookingAsync(Booking booking)
        {
            var availability = await _context.Availabilities.FirstOrDefaultAsync(a =>
                a.BarberId == booking.BarberId &&
                booking.StartUtc >= a.StartUtc &&
                booking.EndUtc <= a.EndUtc);

            if (availability == null)
                throw new Exception("Barber not available for selected time.");

            bool overlap = await _context.Bookings.AnyAsync(b =>
                b.BarberId == booking.BarberId &&
                ((booking.StartUtc >= b.StartUtc && booking.StartUtc < b.EndUtc) ||
                 (booking.EndUtc > b.StartUtc && booking.EndUtc <= b.EndUtc)) &&
                b.Status == BookingStatus.Confirmed);

            if (overlap)
                throw new Exception("Time slot already booked.");

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
