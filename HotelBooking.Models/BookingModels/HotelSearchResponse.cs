using HotelBooking.Models.HotelModels;

namespace HotelBooking.Models.BookingModels
{
    public class HotelSearchResponse
    {
        public int HotelId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public string CheckInTime { get; set; } = string.Empty;
        public string CheckOutTime { get; set; } = string.Empty;
        public List<ImageResponse> Images { get; set; } = new List<ImageResponse>();
        public int AvailableRoomCount { get; set; }
        public decimal LowestPrice { get; set; }
        public decimal HighestPrice { get; set; }
        public Guid BookingNumber { get; set; }
    }
}
