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
        public async Task<List<RoomAvailabilityResponse>> GetAvailableRooms(RoomAvailabilityRequest request)
        {
            var draftBooking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingNumber == request.BookingNumber &&
                                         b.Status == BookingStatus.Draft &&
                                         b.DeleteDate == null);

            if (draftBooking == null)
                throw new KeyNotFoundException($"Draft booking with number {request.BookingNumber} not found");

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == request.HotelId &&
                                        h.DeleteDate == null &&
                                        h.IsActive);

            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {request.HotelId} not found or is not active");

            var nights = (draftBooking.CheckOutDate - draftBooking.CheckInDate).Days;

            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .Where(r => r.HotelId == request.HotelId &&
                           r.IsActive &&
                           r.DeleteDate == null &&
                           r.MaxOccupancy >= draftBooking.NumberOfGuests)
                .ToListAsync();

            var overlappingBookingRoomIds = await _context.Bookings
                .Where(b => b.DeleteDate == null &&
                           b.Status != BookingStatus.Cancelled &&
                           b.Status != BookingStatus.Draft &&
                           b.RoomId != null &&
                           b.Room.HotelId == request.HotelId &&
                           (
                               (draftBooking.CheckInDate >= b.CheckInDate && draftBooking.CheckInDate < b.CheckOutDate) ||
                               (draftBooking.CheckOutDate > b.CheckInDate && draftBooking.CheckOutDate <= b.CheckOutDate) ||
                               (draftBooking.CheckInDate <= b.CheckInDate && draftBooking.CheckOutDate >= b.CheckOutDate)
                           ))
                .Select(b => b.RoomId.Value)
                .ToListAsync();

            var availableRooms = rooms
                .Where(r => !overlappingBookingRoomIds.Contains(r.Id))
                .ToList();

            return availableRooms.Select(r => new RoomAvailabilityResponse
            {
                RoomId = r.Id,
                RoomNumber = r.RoomNumber,
                RoomName = r.Name,
                Description = r.Description,
                RoomTypeName = r.RoomType?.Name ?? "Standard",
                MaxOccupancy = r.MaxOccupancy,
                BasePrice = r.BasePrice,
                TotalPrice = r.BasePrice * nights,
                HotelId = r.HotelId,
                HotelName = r.Hotel?.Name ?? string.Empty,
                Images = r.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList(),
                BookingNumber = draftBooking.BookingNumber
            })
            .OrderBy(r => r.TotalPrice)
            .ToList();
        }
        public async Task<FillPersonalInformationResponse> FillPersonalInformation(FillPersonalInformationRequest request)
        {
            var booking = await _context.Bookings
                .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
                .FirstOrDefaultAsync(b => b.BookingNumber == request.BookingNumber &&
                                        b.Status == BookingStatus.Draft &&
                                        b.DeleteDate == null);

            if (booking == null)
                throw new KeyNotFoundException($"Draft booking with number {request.BookingNumber} not found");

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .FirstOrDefaultAsync(r => r.Id == request.RoomId &&
                                        r.DeleteDate == null &&
                                        r.IsActive);

            if (room == null)
                throw new KeyNotFoundException($"Room with ID {request.RoomId} not found or is not active");

            if (string.IsNullOrWhiteSpace(request.GuestName))
                throw new ArgumentException("Guest name is required");

            if (string.IsNullOrWhiteSpace(request.GuestEmail))
                throw new ArgumentException("Guest email is required");

            if (string.IsNullOrWhiteSpace(request.GuestPhone))
                throw new ArgumentException("Guest phone is required");

            var isRoomAvailable = !await _context.Bookings
                .AnyAsync(b => b.RoomId == request.RoomId &&
                            b.Id != booking.Id &&
                            b.DeleteDate == null &&
                            b.Status != BookingStatus.Cancelled &&
                            b.Status != BookingStatus.Draft &&
                            (
                                (booking.CheckInDate >= b.CheckInDate && booking.CheckInDate < b.CheckOutDate) ||
                                (booking.CheckOutDate > b.CheckInDate && booking.CheckOutDate <= b.CheckOutDate) ||
                                (booking.CheckInDate <= b.CheckInDate && booking.CheckOutDate >= b.CheckOutDate)
                            ));

            if (!isRoomAvailable)
                throw new InvalidOperationException("The selected room is no longer available for the requested dates");

            var nights = (booking.CheckOutDate - booking.CheckInDate).Days;

            var siteFee = nights * room.BasePrice * 0.05m;

            var totalPrice = (room.BasePrice * nights) + siteFee;

            booking.RoomId = request.RoomId;
            booking.GuestName = request.GuestName;
            booking.GuestEmail = request.GuestEmail;
            booking.GuestPhone = request.GuestPhone;
            booking.SpecialRequests = request.SpecialRequests;
            booking.TotalPrice = totalPrice;
            booking.Status = BookingStatus.Pending;
            booking.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new FillPersonalInformationResponse
            {
                BookingNumber = booking.BookingNumber,
                BookingId = booking.Id,
                GuestName = booking.GuestName,
                GuestEmail = booking.GuestEmail,
                GuestPhone = booking.GuestPhone,
                SpecialRequests = booking.SpecialRequests,
                RoomId = room.Id,
                RoomName = room.Name,
                RoomNumber = room.RoomNumber,
                HotelId = room.HotelId,
                HotelName = room.Hotel?.Name ?? string.Empty,
                CheckInDate = booking.CheckInDate,
                CheckOutDate = booking.CheckOutDate,
                NumberOfGuests = booking.NumberOfGuests,
                TotalPrice = booking.TotalPrice,
                Status = booking.Status
            };
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
