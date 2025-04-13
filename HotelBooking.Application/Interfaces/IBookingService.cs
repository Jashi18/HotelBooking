using HotelBooking.Domain.Enums;
using HotelBooking.Models.BookingModels;

namespace HotelBooking.Application.Interfaces
{
    public interface IBookingService
    {
        Task<List<BookingResponse>> GetAllBookings();
        Task<List<BookingResponse>> GetBookingsByHotelId(int hotelId);
        Task<List<BookingResponse>> GetBookingsByRoomId(int roomId);
        Task<List<BookingResponse>> GetBookingsByStatus(BookingStatus status);
        Task<List<BookingResponse>> GetBookingsByGuestEmail(string email);
        Task<BookingResponse> GetBookingById(int id);
    }
}
