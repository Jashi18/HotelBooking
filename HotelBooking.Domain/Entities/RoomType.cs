namespace HotelBooking.Domain.Entities
{
    public class RoomType : BaseEntity
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public ICollection<Room> Rooms { get; set; }
    }
}
