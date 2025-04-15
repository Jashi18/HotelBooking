using HotelBooking.AdminPanel.Models;
using HotelBooking.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HotelBooking.AdminPanel.Controllers
{
    [Authorize(Roles = "HotelAdministrator,Admin")]
    public class DashboardController : Controller
    {
        private readonly IHotelAdministratorService _hotelAdminService;
        private readonly IRoomService _roomService;
        private readonly IBookingService _bookingService;

        public DashboardController(
            IHotelAdministratorService hotelAdminService,
            IRoomService roomService,
            IBookingService bookingService)
        {
            _hotelAdminService = hotelAdminService;
            _roomService = roomService;
            _bookingService = bookingService;
        }

        public async Task<IActionResult> Index()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var username = User.FindFirstValue(ClaimTypes.Name) ?? User.FindFirstValue(ClaimTypes.Email);
            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();

            var viewModel = new DashboardViewModel
            {
                UserName = username,
                UserId = userId,
                Roles = roles
            };

            var hotels = await _hotelAdminService.GetManagedHotelsByUserId(userId);

            foreach (var hotel in hotels)
            {
                var rooms = await _roomService.GetRoomsByHotelId(hotel.Id);
                var bookings = await _bookingService.GetBookingsByHotelId(hotel.Id);

                var activeBookings = bookings.Count(b =>
                    b.Status == HotelBooking.Domain.Enums.BookingStatus.Confirmed ||
                    b.Status == HotelBooking.Domain.Enums.BookingStatus.CheckedIn ||
                    b.Status == HotelBooking.Domain.Enums.BookingStatus.Pending);

                viewModel.ManagedHotels.Add(new HotelViewModel
                {
                    Id = hotel.Id,
                    Name = hotel.Name,
                    Description = hotel.Description,
                    Address = hotel.Address,
                    City = hotel.City,
                    Country = hotel.Country,
                    StarRating = hotel.StarRating,
                    RoomCount = rooms.Count,
                    ActiveBookings = activeBookings
                });
            }

            return View(viewModel);
        }
    }
}
