using HotelBooking.Application.Interfaces;
using HotelBooking.Models.BookingModels;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookingProcessingController : ControllerBase
    {
        private readonly IBookingProcessingService _bookingProcessingService;

        public BookingProcessingController(IBookingProcessingService bookingProcessingService)
        {
            _bookingProcessingService = bookingProcessingService;
        }

        [HttpPost("search-hotels")]
        public async Task<IActionResult> SearchHotels(HotelSearchRequest request)
        {
            try
            {
                var results = await _bookingProcessingService.SearchHotels(request);
                return Ok(results);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("available-rooms")]
        public async Task<IActionResult> GetAvailableRooms(RoomAvailabilityRequest request)
        {
            try
            {
                var availableRooms = await _bookingProcessingService.GetAvailableRooms(request);
                return Ok(availableRooms);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPost("fill-personal-information")]
        public async Task<IActionResult> FillPersonalInformation(FillPersonalInformationRequest request)
        {
            try
            {
                var response = await _bookingProcessingService.FillPersonalInformation(request);
                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
