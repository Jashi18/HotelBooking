namespace HotelBooking.Models.RoomModels
{
    public class UpdateRoomTypeRequest
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
