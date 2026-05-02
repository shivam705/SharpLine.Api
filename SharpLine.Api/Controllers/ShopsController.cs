using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using System.Security.Claims;
using SharpLine.Api.Models.Dtos;
using SharpLine.Api.Services;

namespace SharpLine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ShopsController : ControllerBase
    {
        private readonly IShopService _shopService;
        protected ResponseDto _response;

        public ShopsController(IShopService shopService)
        {
            _shopService = shopService;
            _response = new ResponseDto();
        }


        [HttpGet("nearest")]
        public async Task<IActionResult> GetNearest(double lat, double lon)
        {
            var shops = await _shopService.GetNearestShopsAsync(lat, lon);
            var response = new ResponseDto { Result = shops };
            return Ok(response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(double lat, double lon, double radiusKm = 5)
        {
            var shops = await ((ShopService)_shopService).SearchShopsWithDistanceAsync(lat, lon, radiusKm);
            var response = new ResponseDto { Result = shops };
            return Ok(response);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateShop([FromBody] Shop shop)
        {
            // ensure authenticated user is set as owner
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized(new ResponseDto { IsSuccess = false, Message = "User not authenticated" });
            }

            shop.OwnerId = userId;

            var result = await _shopService.CreateShopAsync(shop);
            var response = new ResponseDto { Result = result };
            return Ok(response);
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
