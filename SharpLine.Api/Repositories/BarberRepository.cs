using SharpLine.Api.Data;
using SharpLine.Api.Models;

namespace SharpLine.Api.Repositories
{
    public class BarberRepository : Repository<Barber>
    {
        public BarberRepository(ApplicationDbContext context) : base(context) { }
    }
}
