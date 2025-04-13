namespace HotelBooking.Domain.Enums
{
    public enum ImageType
    {
        Hotel = 1,
        Room = 2
    }

    public enum BookingStatus
    {
        Pending = 1,
        Draft = 2,
        Confirmed = 3,
        CheckedIn = 4,
        CheckedOut = 5,
        Cancelled = 6
    }
}
