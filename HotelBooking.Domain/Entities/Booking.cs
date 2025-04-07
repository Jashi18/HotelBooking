using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities
{
    public class Booking : BaseEntity
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BookingReference { get; set; } = string.Empty;
        public string GuestName { get; set; } = string.Empty;
        public string GuestEmail { get; set; } = string.Empty;
        public string GuestPhone { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
        public string SpecialRequests { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }

        public Room? Room { get; set; }
    }
}
