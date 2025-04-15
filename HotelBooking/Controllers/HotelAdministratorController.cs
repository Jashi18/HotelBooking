using HotelBooking.Application.Interfaces;
using HotelBooking.Models.AuthModels;
using HotelBooking.Models.HotelModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class HotelAdministratorController : ControllerBase
    {
        private readonly IHotelAdministratorService _hotelAdminService;
        private readonly IAuthService _authService;

        public HotelAdministratorController(
            IHotelAdministratorService hotelAdminService,
            IAuthService authService)
        {
            _hotelAdminService = hotelAdminService;
            _authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<List<HotelAdministratorResponse>>> GetAllHotelAdministrators()
        {
            var hotelAdmins = await _hotelAdminService.GetAllHotelAdministrators();
            return Ok(hotelAdmins);
        }

        [HttpGet("hotel/{hotelId}")]
        public async Task<ActionResult<List<HotelAdministratorResponse>>> GetAdministratorsByHotelId(int hotelId)
        {
            var hotelAdmins = await _hotelAdminService.GetAdministratorsByHotelId(hotelId);
            return Ok(hotelAdmins);
        }

        [HttpGet("user/{userId}/hotels")]
        public async Task<ActionResult<List<HotelResponse>>> GetManagedHotelsByUserId(int userId)
        {
            var hotels = await _hotelAdminService.GetManagedHotelsByUserId(userId);
            return Ok(hotels);
        }

        [HttpPost("assign")]
        public async Task<ActionResult<bool>> AssignHotelToAdministrator(AssignHotelAdministratorRequest request)
        {
            var result = await _hotelAdminService.AssignHotelToAdministrator(request);

            if (result)
                return Ok(true);

            return BadRequest("Failed to assign hotel to administrator");
        }

        [HttpDelete("{userId}/hotel/{hotelId}")]
        public async Task<ActionResult<bool>> RemoveHotelFromAdministrator(int userId, int hotelId)
        {
            var result = await _hotelAdminService.RemoveHotelFromAdministrator(userId, hotelId);

            if (result)
                return Ok(true);

            return NotFound("Hotel administrator assignment not found");
        }

        [HttpPost("create-hotel-administrator")]
        public async Task<ActionResult> CreateHotelAdministrator([FromBody] RegisterRequest request)
        {
            var result = await _authService.CreateHotelAdministrator(request);

            if (result.Success)
                return Ok(result);

            return BadRequest(result);
        }
    }
}