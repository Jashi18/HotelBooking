namespace HotelBooking.Models.BookingModels
{
    public class FillPersonalInformationRequest
    {
        public Guid BookingNumber { get; set; }
        public int RoomId { get; set; }
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public string SpecialRequests { get; set; } = string.Empty;
    }
}
