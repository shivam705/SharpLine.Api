using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;

namespace SharpLine.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BarbersController : ControllerBase
    {
        private readonly IBarberService _barberService;


        public BarbersController(IBarberService barberService)
        {
            _barberService = barberService;
        }


        // CREATE a new barber
        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateBarber([FromBody] Barber barber)
        {
            if (barber == null)
                return BadRequest("Barber data is required.");

            var created = await _barberService.CreateBarberAsync(barber);
            return CreatedAtAction(nameof(GetBarberById), new { id = created.Id }, created);
        }

        // GET single barber by ID (used by CreatedAtAction)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBarberById(int id)
        {
            var barber = await _barberService.GetBarberByIdAsync(id);
            if (barber == null)
                return NotFound();
            return Ok(barber);
        }

        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpPost("availability")]
        public async Task<IActionResult> AddAvailability([FromBody] Availability availability)
        {
            var result = await _barberService.AddAvailabilityAsync(availability);
            return Ok(result);
        }


        [Authorize(Roles = "ShopOwner,Admin")]
        [HttpDelete("availability/{id}")]
        public async Task<IActionResult> RemoveAvailability(int id)
        {
            await _barberService.RemoveAvailabilityAsync(id);
            return NoContent();
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