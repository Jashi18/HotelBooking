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
    }
}
