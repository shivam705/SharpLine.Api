using AutoMapper;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;

namespace SharpLine.Api.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Barber, BarberDto>().ReverseMap();
            CreateMap<Shop, ShopDto>().ReverseMap();
            CreateMap<Availability, AvailabilityDto>().ReverseMap();
            CreateMap<Booking, BookingDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));

            CreateMap<BookingDto, Booking>().ConvertUsing<BookingDtoToBookingConverter>();

            // helper for parsing booking status
            static BookingStatus ParseBookingStatus(string? status)
            {
                if (string.IsNullOrWhiteSpace(status)) return BookingStatus.Confirmed;
                if (System.Enum.TryParse<BookingStatus>(status, true, out var st)) return st;
                return BookingStatus.Confirmed;
            }
            CreateMap<ApplicationUser, UserDto>().ReverseMap();
        }
    }
}
