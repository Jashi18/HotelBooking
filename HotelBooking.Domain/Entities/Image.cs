using HotelBooking.Domain.Enums;

namespace HotelBooking.Domain.Entities
{
    public class Image : BaseEntity
    {
        public int Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Caption { get; set; } = string.Empty;
        public bool IsPrimary { get; set; }
        public int DisplayOrder { get; set; }

        public ImageType ImageType { get; set; }
    }
}
