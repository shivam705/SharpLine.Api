using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharpLine.Api.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }

        public int BarberId { get; set; }

        [JsonIgnore] // Prevent cycles
        public Barber Barber { get; set; } = null!;

        public string CustomerId { get; set; } = null!;

        [JsonIgnore] // Optional: if ApplicationUser has navigation properties back to Booking
        public ApplicationUser Customer { get; set; } = null!;

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public BookingStatus Status { get; set; } = BookingStatus.Confirmed;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }

}
