using SharpLine.Api.Data;
using SharpLine.Api.Models;

namespace SharpLine.Api.Repositories
{
    public class BookingRepository : Repository<Booking>
    {
        public BookingRepository(ApplicationDbContext context) : base(context) { }
    }
}
