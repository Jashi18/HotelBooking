using HotelBooking.Models.HotelModels;

namespace HotelBooking.Application.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelResponse>> GetAllHotels();
        Task<HotelResponse> GetHotelById(int id);
        Task<HotelResponse> CreateHotel(CreateHotelRequest request);
        Task<HotelResponse> UpdateHotel(UpdateHotelRequest request);
        Task<bool> DeleteHotel(int id);
    }
}
