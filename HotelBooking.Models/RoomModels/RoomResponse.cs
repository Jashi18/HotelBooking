using HotelBooking.Models.HotelModels;

namespace HotelBooking.Models.RoomModels
{
    public class RoomResponse
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsActive { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public RoomTypeResponse? RoomType { get; set; }
        public List<ImageResponse> Images { get; set; } = new List<ImageResponse>();
    }
}
