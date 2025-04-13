using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Data;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Models.BookingModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Implementations
{
    public class BookingService : IBookingService
    {
        private readonly ApplicationDbContext _context;
        public BookingService(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<BookingResponse>> GetAllBookings()
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.DeleteDate == null)
                .OrderByDescending(b => b.CreateDate)
                .ToListAsync();

            return MapBookingResponses(bookings);
        }
        public async Task<List<BookingResponse>> GetBookingsByHotelId(int hotelId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.Room.HotelId == hotelId && b.DeleteDate == null)
                .OrderByDescending(b => b.CreateDate)
                .ToListAsync();

            return MapBookingResponses(bookings);
        }
        public async Task<List<BookingResponse>> GetBookingsByRoomId(int roomId)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.RoomId == roomId && b.DeleteDate == null)
                .OrderByDescending(b => b.CreateDate)
                .ToListAsync();

            return MapBookingResponses(bookings);
        }
        public async Task<List<BookingResponse>> GetBookingsByStatus(BookingStatus status)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.Status == status && b.DeleteDate == null)
                .OrderByDescending(b => b.CreateDate)
                .ToListAsync();

            return MapBookingResponses(bookings);
        }
        public async Task<List<BookingResponse>> GetBookingsByGuestEmail(string email)
        {
            var bookings = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .Where(b => b.GuestEmail.ToLower() == email.ToLower() && b.DeleteDate == null)
                .OrderByDescending(b => b.CreateDate)
                .ToListAsync();

            return MapBookingResponses(bookings);
        }
        public async Task<BookingResponse> GetBookingById(int id)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefaultAsync(b => b.Id == id && b.DeleteDate == null);

            if (booking == null)
                throw new KeyNotFoundException($"Booking with ID {id} not found");

            return MapBookingResponse(booking);
        }

        private BookingResponse MapBookingResponse(Booking booking)
        {
            return new BookingResponse
            {
                Id = booking.Id,
                BookingNumber = booking.BookingNumber,
                RoomId = booking.RoomId ?? 0,
                RoomName = booking.Room?.Name ?? string.Empty,
                RoomNumber = booking.Room?.RoomNumber ?? string.Empty,
                HotelId = booking.Room?.HotelId ?? 0,
                HotelName = booking.Room?.Hotel?.Name ?? string.Empty,
                GuestName = booking.GuestName,
                GuestEmail = booking.GuestEmail,
                GuestPhone = booking.GuestPhone,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumberOfGuests = booking.NumberOfGuests,
                TotalPrice = booking.TotalPrice,
                SpecialRequests = booking.SpecialRequests,
                Status = booking.Status
            };
        }
        private List<BookingResponse> MapBookingResponses(List<Booking> bookings)
        {
            return bookings.Select(b => MapBookingResponse(b)).ToList();
        }
    }
}
