using HotelBooking.Application.Interfaces;
using HotelBooking.Models.RoomModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomsController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpGet]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _roomService.GetAllRooms();
            return Ok(rooms);
        }
        [HttpGet("hotel/{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotelId(int hotelId)
        {
            var rooms = await _roomService.GetRoomsByHotelId(hotelId);
            return Ok(rooms);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            try
            {
                var room = await _roomService.GetRoomById(id);
                return Ok(room);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost]
        public async Task<IActionResult> CreateRoom(CreateRoomRequest request)
        {
            try
            {
                var room = await _roomService.CreateRoom(request);
                return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, room);
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
        [HttpPut]
        public async Task<IActionResult> UpdateRoom(UpdateRoomRequest request)
        {
            try
            {
                var room = await _roomService.UpdateRoom(request);
                return Ok(room);
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
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            try
            {
                await _roomService.DeleteRoom(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
        [HttpGet("Get-RoomTypes")]
        public async Task<IActionResult> GetRoomTypes()
        {
            var roomTypes = await _roomService.GetAllRoomTypes();
            return Ok(roomTypes);
        }

        [HttpGet("Get-RoomType{id}")]
        public async Task<IActionResult> GetRoomType(int id)
        {
            try
            {
                var roomType = await _roomService.GetRoomTypeById(id);
                return Ok(roomType);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost("Create-RoomType")]
        public async Task<IActionResult> CreateRoomType(CreateRoomTypeRequest request)
        {
            try
            {
                var roomType = await _roomService.CreateRoomType(request);
                return CreatedAtAction(nameof(GetRoomType), new { id = roomType.Id }, roomType);
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

        [HttpPut("Update-RoomType")]
        public async Task<IActionResult> UpdateRoomType(UpdateRoomTypeRequest request)
        {
            try
            {
                var roomType = await _roomService.UpdateRoomType(request);
                return Ok(roomType);
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

        [HttpDelete("Delete-RoomType{id}")]
        public async Task<IActionResult> DeleteRoomType(int id)
        {
            try
            {
                await _roomService.DeleteRoomType(id);
                return NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
        }
    }
}
