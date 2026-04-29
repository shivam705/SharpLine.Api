using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;

namespace SharpLine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;


        public ShopsController(IShopService shopService)
        {
            _shopService = shopService;
        }


        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearest(double lat, double lon)
        {
            var shops = await _shopService.GetNearestShopsAsync(lat, lon);
            return Ok(new
            {
                status = "200",
                message = "Nearest Shops Successfully.",
                data = shops
            });
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] Shop shop)
        {
            var result = await _shopService.CreateShopAsync(shop);
            return Ok(new
            {
                status = "201",
                message = "Shop Created Successfully.",
                data = result
            });
        }
    }
}


//{
//    "name": "Shivam",
//  "address": "Rasulpur",
//  "latitude": 80,
//  "longitude": 80,
//  "barbers": []
//}
