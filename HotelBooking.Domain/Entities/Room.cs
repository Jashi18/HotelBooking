namespace HotelBooking.Domain.Entities
{
    public class Room : BaseEntity
    {
        public int Id { get; set; }
        public int HotelId { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal BasePrice { get; set; }
        public int MaxOccupancy { get; set; }
        public bool IsActive { get; set; }

        public Hotel? Hotel { get; set; }
        public RoomType? RoomType { get; set; }
        public ICollection<Image> Images { get; set; } = new List<Image>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
