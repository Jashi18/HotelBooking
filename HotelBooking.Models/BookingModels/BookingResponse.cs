using HotelBooking.Domain.Enums;

namespace HotelBooking.Models.BookingModels
{
    public class BookingResponse
    {
        public int Id { get; set; }
        public Guid BookingNumber { get; set; }
        public int RoomId { get; set; }
        public string RoomName { get; set; } = string.Empty;
        public string RoomNumber { get; set; } = string.Empty;
        public int HotelId { get; set; }
        public string HotelName { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public string StatusDescription => Status.ToString();
    }
}
