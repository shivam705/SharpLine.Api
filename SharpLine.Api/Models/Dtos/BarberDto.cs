namespace SharpLine.Api.Models.Dtos
{
    public class BarberDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ShopId { get; set; }
        public string? UserId { get; set; }
    }
}
