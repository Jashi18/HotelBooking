using HotelBooking.Models.RoomModels;

namespace HotelBooking.Application.Interfaces
{
    public interface IRoomService
    {
        Task<List<RoomResponse>> GetAllRooms();
        Task<List<RoomResponse>> GetRoomsByHotelId(int hotelId);
        Task<RoomResponse> GetRoomById(int id);
        Task<RoomResponse> CreateRoom(CreateRoomRequest request);
        Task<RoomResponse> UpdateRoom(UpdateRoomRequest request);
        Task<bool> DeleteRoom(int id);

        Task<List<RoomTypeResponse>> GetAllRoomTypes();
        Task<RoomTypeResponse> GetRoomTypeById(int id);
        Task<RoomTypeResponse> CreateRoomType(CreateRoomTypeRequest request);
        Task<RoomTypeResponse> UpdateRoomType(UpdateRoomTypeRequest request);
        Task<bool> DeleteRoomType(int id);
    }
}
