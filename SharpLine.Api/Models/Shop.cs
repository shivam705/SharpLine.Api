using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using System.Threading;

namespace SharpLine.Api.Models
{
    public class Shop
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        // Shop owner (ApplicationUser Id)
        public string? OwnerId { get; set; }

        [JsonIgnore]
        public ApplicationUser? Owner { get; set; }

        [JsonIgnore] // Prevent cycle
        public ICollection<Barber> Barbers { get; set; } = new List<Barber>();
    }

}
