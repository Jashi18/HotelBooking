namespace HotelBooking.AdminPanel.Models
{
    public class DashboardViewModel
    {
        public string UserName { get; set; } = string.Empty;
        public int UserId { get; set; }
        public List<string> Roles { get; set; } = new List<string>();
        public List<HotelViewModel> ManagedHotels { get; set; } = new List<HotelViewModel>();
    }
}
