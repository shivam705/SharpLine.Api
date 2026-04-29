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

        [JsonIgnore]
        public ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

        [JsonIgnore]
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
