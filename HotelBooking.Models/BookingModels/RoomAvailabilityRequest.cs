using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models.BookingModels
{
    public class RoomAvailabilityRequest
    {
        public Guid BookingNumber { get; set; }
        public int HotelId { get; set; }
    }
}
