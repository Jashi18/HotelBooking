namespace HotelBooking.Models.AuthModels
{
    public class AssignHotelAdministratorRequest
    {
        public int UserId { get; set; }
        public int HotelId { get; set; }
    }
}
