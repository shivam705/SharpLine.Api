using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharpLine.Api.Models
{
    public class Barber
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        [ForeignKey("Shop")]
        public int ShopId { get; set; }

        [JsonIgnore]
        public Shop? Shop { get; set; }

        // Link barber to an application user (barber's account)
        public string? UserId { get; set; }

        [JsonIgnore]
        public ApplicationUser? User { get; set; }

        // Indicates whether the barber is currently accepting bookings
        public bool IsAvailable { get; set; } = true;

        [JsonIgnore]
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
