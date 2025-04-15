namespace HotelBooking.Domain.Entities
{
    public class HotelAdministrator : BaseEntity
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int HotelId { get; set; }

        public User User { get; set; } = null!;
        public Hotel Hotel { get; set; } = null!;
    }
}
