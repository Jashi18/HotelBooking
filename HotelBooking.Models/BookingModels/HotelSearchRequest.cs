using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Models.BookingModels
{
    public class HotelSearchRequest
    {
        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required]
        [Range(1, 20)]
        public int NumberOfGuests { get; set; }

        [Required]
        public string Country { get; set; } = string.Empty;

        public string? City { get; set; }
    }
}
