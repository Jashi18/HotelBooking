using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Data;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Models.BookingModels;
using HotelBooking.Models.HotelModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Implementations
{
    public class BookingProcessingService : IBookingProcessingService
    {
        private readonly ApplicationDbContext _context;

        public BookingProcessingService(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<HotelSearchResponse>> SearchHotels(HotelSearchRequest request)
        {
            if (request.CheckInDate < DateTime.Today)
                throw new ArgumentException("Check-in date cannot be in the past");

            if (request.CheckOutDate <= request.CheckInDate)
                throw new ArgumentException("Check-out date must be after check-in date");

            var draftBookingNumber = await GenerateDraftBooking(request);

            var nights = (request.CheckOutDate - request.CheckInDate).Days;

            var hotelsQuery = _context.Hotels
                .Include(h => h.Images)
                .Include(h => h.Rooms)
                .Where(h => h.DeleteDate == null &&
                           h.IsActive &&
                           h.Country.ToLower() == request.Country.ToLower());

            if (!string.IsNullOrEmpty(request.City))
            {
                hotelsQuery = hotelsQuery.Where(h => h.City.ToLower() == request.City.ToLower());
            }

            var hotels = await hotelsQuery.ToListAsync();

            var overlappingBookings = await _context.Bookings
                .Where(b => b.DeleteDate == null &&
                           b.RoomId != null &&
                           b.Room != null &&
                           b.Status != BookingStatus.Cancelled &&
                           (
                               (request.CheckInDate >= b.CheckInDate && request.CheckInDate < b.CheckOutDate) ||
                               (request.CheckOutDate > b.CheckInDate && request.CheckOutDate <= b.CheckOutDate) ||
                               (request.CheckInDate <= b.CheckInDate && request.CheckOutDate >= b.CheckOutDate)
                           ))
                .Select(b => new { b.RoomId, b.Room.HotelId })
                .ToListAsync();

            var result = new List<HotelSearchResponse>();

            foreach (var hotel in hotels)
            {
                var suitableRooms = hotel.Rooms
                    .Where(r => r.DeleteDate == null &&
                               r.IsActive &&
                               r.MaxOccupancy >= request.NumberOfGuests)
                    .ToList();

                if (!suitableRooms.Any())
                    continue;

                var bookedRoomIds = overlappingBookings
                    .Where(b => b.HotelId == hotel.Id)
                    .Select(b => b.RoomId)
                    .ToHashSet();

                var availableRooms = suitableRooms
                    .Where(r => !bookedRoomIds.Contains(r.Id))
                    .ToList();

                if (!availableRooms.Any())
                    continue;

                var lowestPrice = availableRooms.Min(r => r.BasePrice);
                var highestPrice = availableRooms.Max(r => r.BasePrice);

                result.Add(new HotelSearchResponse
                {
                    HotelId = hotel.Id,
                    Name = hotel.Name,
                    Description = hotel.Description,
                    Address = hotel.Address,
                    City = hotel.City,
                    Country = hotel.Country,
                    StarRating = hotel.StarRating,
                    CheckInTime = hotel.CheckInTime,
                    CheckOutTime = hotel.CheckOutTime,
                    Images = hotel.Images.Select(i => new ImageResponse
                    {
                        Id = i.Id,
                        ImageUrl = i.ImageUrl,
                        Caption = i.Caption,
                        IsPrimary = i.IsPrimary
                    }).ToList(),
                    AvailableRoomCount = availableRooms.Count,
                    LowestPrice = lowestPrice,
                    HighestPrice = highestPrice,
                    BookingNumber = draftBookingNumber
                });
            }

            return result
                .OrderByDescending(h => h.StarRating)
                .ThenBy(h => h.LowestPrice)
                .ToList();
        }
        private async Task<Guid> GenerateDraftBooking(HotelSearchRequest request)
        {
            var draftBooking = new Booking
            {
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                NumberOfGuests = request.NumberOfGuests,
                Status = BookingStatus.Draft,
                CreateDate = DateTime.UtcNow
            };

            _context.Bookings.Add(draftBooking);
            await _context.SaveChangesAsync();

            return draftBooking.BookingNumber;
        }
    }
}
