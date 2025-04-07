namespace HotelBooking.Models.HotelModels
{
    public class CreateHotelRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string PostalCode { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int StarRating { get; set; }
        public string CheckInTime { get; set; } = string.Empty;
        public string CheckOutTime { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
