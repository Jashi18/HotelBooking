namespace HotelBooking.AdminPanel.Models
{
    public class HotelViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public int RoomCount { get; set; }
        public int ActiveBookings { get; set; }
    }
}
