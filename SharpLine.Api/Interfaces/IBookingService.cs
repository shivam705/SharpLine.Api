using SharpLine.Api.Models;

namespace SharpLine.Api.Interfaces
{
    public interface IBookingService
    {
        Task<Booking> CreateBookingAsync(Booking booking);
        Task<Booking?> GetBookingByIdAsync(int id);
        Task<IEnumerable<Booking>> GetBookingsByCustomerAsync(string customerId);
        Task<IEnumerable<Booking>> GetBookingsByBarberAsync(int barberId);
        Task<bool> CancelBookingAsync(int bookingId);
    }
}
