using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;
using System.Security.Claims;

namespace SharpLine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbersController : ControllerBase
    {
        private readonly IBarberService _barberService;
        private readonly IShopService _shopService;
        protected ResponseDto _response;


        public BarbersController(IBarberService barberService, IShopService shopService)
        {
            _barberService = barberService;
            _shopService = shopService;
            _response = new ResponseDto();
        }

        // Register current authenticated user as a barber for a shop
        [Authorize]
        [HttpPost("register-self")]
        public async Task<IActionResult> RegisterSelf([FromBody] BarberDto barberDto)
        {
            // take current user id
            var userId = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(userId))
            {
                _response.IsSuccess = false;
                _response.Message = "User not authenticated";
                return Unauthorized(_response);
            }

            barberDto.UserId = userId;
            var created = await _barberService.CreateBarberAsync(barberDto);
            _response.Result = created;
            return CreatedAtAction(nameof(GetBarberById), new { id = created.Id }, _response);
        }

        // Shop owner adds a staff barber
        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpPost("{shopId}/staff")]
        public async Task<IActionResult> AddStaff(int shopId, [FromBody] BarberDto barberDto)
        {
            if (barberDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Barber data required";
                return BadRequest(_response);
            }

            // Verify ownership for ShopOwner role
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User?.IsInRole("Admin") ?? false;
            if (!isAdmin)
            {
                var shop = await _shopService.GetShopByIdAsync(shopId);
                if (shop == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Shop not found";
                    return NotFound(_response);
                }

                if (shop.OwnerId != userId)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not authorized to manage staff for this shop";
                    return Forbid();
                }
            }

            barberDto.ShopId = shopId;
            var created = await _barberService.CreateBarberAsync(barberDto);
            _response.Result = created;
            return CreatedAtAction(nameof(GetBarberById), new { id = created.Id }, _response);
        }

        // Shop owner removes a staff barber
        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpDelete("{shopId}/staff/{barberId}")]
        public async Task<IActionResult> RemoveStaff(int shopId, int barberId)
        {
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var isAdmin = User?.IsInRole("Admin") ?? false;
            if (!isAdmin)
            {
                var shop = await _shopService.GetShopByIdAsync(shopId);
                if (shop == null)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Shop not found";
                    return NotFound(_response);
                }

                if (shop.OwnerId != userId)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Not authorized to manage staff for this shop";
                    return Forbid();
                }
            }

            var removed = await _barberService.RemoveBarberAsync(barberId);
            if (!removed)
            {
                _response.IsSuccess = false;
                _response.Message = "Failed to remove barber";
                return BadRequest(_response);
            }
            _response.Message = "Barber removed";
            return Ok(_response);
        }


        // CREATE a new barber
        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBarber([FromBody] BarberDto barberDto)
        {
            if (barberDto == null)
                return BadRequest("Barber data is required.");

            // Service works with DTOs directly
            var createdDto = await _barberService.CreateBarberAsync(barberDto);
            _response.Result = createdDto;
            return CreatedAtAction(nameof(GetBarberById), new { id = createdDto.Id }, _response);
        }

        // GET single barber by ID (used by CreatedAtAction)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBarberById(int id)
        {
            var barberDto = await _barberService.GetBarberByIdAsync(id);
            if (barberDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Barber not found";
                return NotFound(_response);
            }

            _response.Result = barberDto;
            return Ok(_response);
        }

        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpPost("availability")]
        public async Task<IActionResult> AddAvailability([FromBody] AvailabilityDto availabilityDto)
        {
            if (availabilityDto == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Availability data is required.";
                return BadRequest(_response);
            }

            var resultDto = await _barberService.AddAvailabilityAsync(availabilityDto);
            _response.Result = resultDto;
            return Ok(_response);
        }


        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpDelete("availability/{id}")]
        public async Task<IActionResult> RemoveAvailability(int id)
        {
            await _barberService.RemoveAvailabilityAsync(id);
            return NoContent();
        }

        // Check if barber is available for a given time range
        [Authorize]
        [HttpGet("{barberId}/is-available")]
        public async Task<IActionResult> IsAvailable(int barberId, [FromQuery] DateTime startUtc, [FromQuery] DateTime endUtc)
        {
            if (startUtc >= endUtc)
            {
                _response.IsSuccess = false;
                _response.Message = "Invalid time range";
                return BadRequest(_response);
            }

            var available = await _barberService.IsBarberAvailableAsync(barberId, startUtc, endUtc);
            _response.Result = new { BarberId = barberId, StartUtc = startUtc, EndUtc = endUtc, IsAvailable = available };
            return Ok(_response);
        }
    }
}




//{
//    "name": "Anil",
//  "shopId": 1,
//  "shop": {
//        "name": "Sarai Shop",
//    "address": "sarai",
//    "latitude": 90,
//    "longitude": 60
//  }
//}



//{
//    "barberId": 4,
//  "startUtc": "2025-10-25T10:00:00Z",
//  "endUtc": "2025-10-25T11:00:00Z"
//}