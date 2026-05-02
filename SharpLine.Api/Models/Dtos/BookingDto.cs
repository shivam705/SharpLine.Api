namespace SharpLine.Api.Models.Dtos
{
    public class BookingDto
    {
        public int Id { get; set; }
        public int BarberId { get; set; }
        public string CustomerId { get; set; } = null!;
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public string Status { get; set; } = "Confirmed";
    }
}
