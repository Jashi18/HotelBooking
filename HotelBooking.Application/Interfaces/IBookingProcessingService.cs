using HotelBooking.Models.BookingModels;

namespace HotelBooking.Application.Interfaces
{
    public interface IBookingProcessingService
    {
        Task<List<HotelSearchResponse>> SearchHotels(HotelSearchRequest request);
        Task<List<RoomAvailabilityResponse>> GetAvailableRooms(RoomAvailabilityRequest request);

    }
}
