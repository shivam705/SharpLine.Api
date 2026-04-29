using SharpLine.Api.Data;
using SharpLine.Api.Models;

namespace SharpLine.Api.Repositories
{
    public class ShopRepository : Repository<Shop>
    {
        public ShopRepository(ApplicationDbContext context) : base(context) { }
    }
}
