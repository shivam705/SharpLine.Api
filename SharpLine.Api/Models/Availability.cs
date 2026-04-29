using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SharpLine.Api.Models
{
    public class Availability
    {
        [Key]
        public int Id { get; set; }

        public int BarberId { get; set; }

        [JsonIgnore] // Prevent cycles
        public Barber? Barber { get; set; }

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
    }

}
