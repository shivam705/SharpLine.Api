using AutoMapper;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;

namespace SharpLine.Api.Mappings
{
    public class BookingDtoToBookingConverter : ITypeConverter<BookingDto, Booking>
    {
        public Booking Convert(BookingDto source, Booking destination, ResolutionContext context)
        {
            if (source == null) return null!;

            var booking = new Booking
            {
                Id = source.Id,
                BarberId = source.BarberId,
                CustomerId = source.CustomerId,
                StartUtc = source.StartUtc,
                EndUtc = source.EndUtc,
                CreatedAtUtc = DateTime.UtcNow
            };

            if (!string.IsNullOrWhiteSpace(source.Status) && System.Enum.TryParse<BookingStatus>(source.Status, true, out var st))
            {
                booking.Status = st;
            }
            else
            {
                booking.Status = BookingStatus.Confirmed;
            }

            return booking;
        }
    }
}
