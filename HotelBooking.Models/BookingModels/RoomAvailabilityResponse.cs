using HotelBooking.Models.HotelModels;

namespace HotelBooking.Models.BookingModels
{
    public class RoomAvailabilityResponse
    {
        public int RoomId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoomTypeName { get; set; } = string.Empty;
        public int MaxOccupancy { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public List<ImageResponse> Images { get; set; } = new List<ImageResponse>();
        public Guid BookingNumber { get; set; }
    }
}
