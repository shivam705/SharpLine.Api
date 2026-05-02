using System.Collections.Generic;
using System.Linq;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;

namespace SharpLine.Api.Mappings
{
    public static class DtoMappers
    {
        // Barber
        public static BarberDto ToDto(this Barber barber)
        {
            if (barber == null) return null!;
            return new BarberDto
            {
                Id = barber.Id,
                Name = barber.Name,
                ShopId = barber.ShopId,
                UserId = barber.UserId
            };
        }

        public static Barber ToModel(this BarberDto dto)
        {
            if (dto == null) return null!;
            return new Barber
            {
                Id = dto.Id,
                Name = dto.Name,
                ShopId = dto.ShopId,
                UserId = dto.UserId
            };
        }

        public static List<BarberDto> ToDtos(this IEnumerable<Barber> items) => items?.Select(x => x.ToDto()).ToList() ?? new List<BarberDto>();

        // Shop
        public static ShopDto ToDto(this Shop shop)
        {
            if (shop == null) return null!;
            return new ShopDto
            {
                Id = shop.Id,
                Name = shop.Name,
                Address = shop.Address,
                Latitude = shop.Latitude,
                Longitude = shop.Longitude,
                OwnerId = shop.OwnerId
            };
        }

        public static Shop ToModel(this ShopDto dto)
        {
            if (dto == null) return null!;
            return new Shop
            {
                Id = dto.Id,
                Name = dto.Name,
                Address = dto.Address,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                OwnerId = dto.OwnerId
            };
        }

        public static List<ShopDto> ToDtos(this IEnumerable<Shop> items) => items?.Select(x => x.ToDto()).ToList() ?? new List<ShopDto>();

        // Availability
        public static AvailabilityDto ToDto(this Availability availability)
        {
            if (availability == null) return null!;
            return new AvailabilityDto
            {
                Id = availability.Id,
                BarberId = availability.BarberId,
                StartUtc = availability.StartUtc,
                EndUtc = availability.EndUtc
            };
        }

        public static Availability ToModel(this AvailabilityDto dto)
        {
            if (dto == null) return null!;
            return new Availability
            {
                Id = dto.Id,
                BarberId = dto.BarberId,
                StartUtc = dto.StartUtc,
                EndUtc = dto.EndUtc
            };
        }

        public static List<AvailabilityDto> ToDtos(this IEnumerable<Availability> items) => items?.Select(x => x.ToDto()).ToList() ?? new List<AvailabilityDto>();

        // Booking
        public static BookingDto ToDto(this Booking booking)
        {
            if (booking == null) return null!;
            return new BookingDto
            {
                Id = booking.Id,
                BarberId = booking.BarberId,
                CustomerId = booking.CustomerId,
                StartUtc = booking.StartUtc,
                EndUtc = booking.EndUtc,
                Status = booking.Status.ToString()
            };
        }

        public static Booking ToModel(this BookingDto dto)
        {
            if (dto == null) return null!;
            return new Booking
            {
                Id = dto.Id,
                BarberId = dto.BarberId,
                CustomerId = dto.CustomerId,
                StartUtc = dto.StartUtc,
                EndUtc = dto.EndUtc,
                Status = System.Enum.TryParse<BookingStatus>(dto.Status, out var s) ? s : BookingStatus.Confirmed
            };
        }

        public static List<BookingDto> ToDtos(this IEnumerable<Booking> items) => items?.Select(x => x.ToDto()).ToList() ?? new List<BookingDto>();

        // ApplicationUser <-> UserDto
        public static UserDto ToDto(this ApplicationUser user)
        {
            if (user == null) return null!;
            return new UserDto
            {
                ID = user.Id,
                Email = user.Email ?? string.Empty,
                Name = user.Name ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty
            };
        }

        public static ApplicationUser ToModel(this UserDto dto)
        {
            if (dto == null) return null!;
            return new ApplicationUser
            {
                Id = dto.ID,
                Email = dto.Email,
                UserName = dto.Email,
                Name = dto.Name,
                PhoneNumber = dto.PhoneNumber
            };
        }
    }
}
