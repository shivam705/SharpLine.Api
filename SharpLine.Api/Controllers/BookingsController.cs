using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using SharpLine.Api.Interfaces;
using SharpLine.Api.Models;
using SharpLine.Api.Models.Dtos;
using System.Security.Claims;
using SharpLine.Api.Services.Exceptions;

namespace SharpLine.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly IMapper _mapper;
        private readonly IBarberService _barberService;
        protected ResponseDto _response;


        public BookingsController(IBookingService bookingService, IMapper mapper, IBarberService barberService)
        {
            _bookingService = bookingService;
            _mapper = mapper;
            _response = new ResponseDto();
            _barberService = barberService;
        }


        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] BookingDto bookingDto)
        {
            try
            {
                var booking = _mapper.Map<Booking>(bookingDto);

                // Set CustomerId from authenticated user
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrWhiteSpace(userId))
                {
                    _response.IsSuccess = false;
                    _response.Message = "User not authenticated";
                    return Unauthorized(_response);
                }

                booking.CustomerId = userId;

                // Validate availability before creating
                var isAvailable = await _barberService.IsBarberAvailableAsync(booking.BarberId, booking.StartUtc, booking.EndUtc);
                if (!isAvailable)
                {
                    _response.IsSuccess = false;
                    _response.Message = "Barber not available for the selected time";
                    return Conflict(_response);
                }

                var result = await _bookingService.CreateBookingAsync(booking);
                var resultDto = _mapper.Map<BookingDto>(result);

                _response.Result = resultDto;
                return CreatedAtAction(nameof(GetBooking), new { id = resultDto.Id }, _response);
            }
            catch (BookingConflictException ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return Conflict(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Message = ex.Message;
                return BadRequest(_response);
            }
        }


        [Authorize(Roles = "User,ShopOwner,Admin")]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBooking(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
            {
                _response.IsSuccess = false;
                _response.Message = "Booking not found";
                return NotFound(_response);
            }

            var dto = _mapper.Map<BookingDto>(booking);
            _response.Result = dto;
            return Ok(_response);
        }
    }
}
