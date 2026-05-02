namespace SharpLine.Api.Models.Dtos
{
    public class ShopDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Address { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? OwnerId { get; set; }
    }
}
