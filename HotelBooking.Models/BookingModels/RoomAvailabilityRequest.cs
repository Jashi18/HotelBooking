using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models.BookingModels
{
    public class RoomAvailabilityRequest
    {
        [Required]
        public int HotelId { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 20)]
        public int NumberOfGuests { get; set; }
    }
}
