namespace HotelBooking.Models.RoomModels
{
    public class CreateRoomRequest
    {
        public int HotelId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsActive { get; set; } = true;
        public int? RoomTypeId { get; set; }
    }
}
