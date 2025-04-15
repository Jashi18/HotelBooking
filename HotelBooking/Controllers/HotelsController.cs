using HotelBooking.Application.Interfaces;
using HotelBooking.Models.HotelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HotelsController : ControllerBase
    {
        private readonly IHotelService _hotelService;

        public HotelsController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotels()
        {
            var hotels = await _hotelService.GetAllHotels();
            return Ok(hotels);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotel = await _hotelService.GetHotelById(id);

            if (hotel == null)
                return NotFound();

            return Ok(hotel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateHotel(CreateHotelRequest request)
        {
            try
            {
                var hotel = await _hotelService.CreateHotel(request);
                return CreatedAtAction(nameof(GetHotel), new { id = hotel.Id }, hotel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpPut]
        public async Task<IActionResult> UpdateHotel(UpdateHotelRequest request)
        {
            try
            {
                var hotel = await _hotelService.UpdateHotel(request);
                return Ok(hotel);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            try
            {
                await _hotelService.DeleteHotel(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}