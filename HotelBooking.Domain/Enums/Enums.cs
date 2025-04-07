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
        Confirmed = 2,
        CheckedIn = 3,
        CheckedOut = 4,
        Cancelled = 5,
        NoShow = 6
    }
}
