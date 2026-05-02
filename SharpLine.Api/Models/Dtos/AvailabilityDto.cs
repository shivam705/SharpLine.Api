namespace SharpLine.Api.Models.Dtos
{
    public class AvailabilityDto
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
    }
}
