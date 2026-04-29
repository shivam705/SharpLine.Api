using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;

namespace SharpLine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;


        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking booking)
        {
            var result = await _bookingService.CreateBookingAsync(booking);
            return Ok(result);
        }


        [Authorize(Roles = "User,ShopOwner,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            return Ok(booking);
        }
    }
}
