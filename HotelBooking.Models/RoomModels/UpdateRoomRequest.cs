namespace HotelBooking.Models.RoomModels
{
    public class UpdateRoomRequest
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsActive { get; set; }
        public int? RoomTypeId { get; set; }
    }
}
