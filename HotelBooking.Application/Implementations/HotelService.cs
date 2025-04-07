using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Data;
using HotelBooking.Domain.Entities;
using HotelBooking.Models.HotelModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Implementations
{
    public class HotelService : IHotelService
    {
        private readonly ApplicationDbContext _context;
        public HotelService(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        public async Task<List<HotelResponse>> GetAllHotels()
        {
            var hotels = await _context.Hotels
                .Include(h => h.Images)
                .Where(h => h.DeleteDate == null)
                .ToListAsync();

            return hotels.Select(h => new HotelResponse
            {
                Id = h.Id,
                Name = h.Name,
                Description = h.Description,
                Address = h.Address,
                City = h.City,
                Country = h.Country,
                PostalCode = h.PostalCode,
                PhoneNumber = h.PhoneNumber,
                Email = h.Email,
                StarRating = h.StarRating,
                CheckInTime = h.CheckInTime,
                CheckOutTime = h.CheckOutTime,
                IsActive = h.IsActive,
                Images = h.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            }).ToList();
        }
        public async Task<HotelResponse> GetHotelById(int id)
        {
            var hotel = await _context.Hotels
                .Include(h => h.Images)
                .FirstOrDefaultAsync(h => h.Id == id && h.DeleteDate == null);

            if(hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found");

            return new HotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PostalCode = hotel.PostalCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                CheckInTime = hotel.CheckInTime,
                CheckOutTime = hotel.CheckOutTime,
                IsActive = hotel.IsActive,
                Images = hotel.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };
        }
        public async Task<HotelResponse> CreateHotel(CreateHotelRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Hotel name cannot be empty");

            var hotel = new Hotel
            {
                Name = request.Name,
                Description = request.Description,
                Address = request.Address,
                City = request.City,
                Country = request.Country,
                PostalCode = request.PostalCode,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email,
                StarRating = request.StarRating,
                CheckInTime = request.CheckInTime,
                CheckOutTime = request.CheckOutTime,
                IsActive = request.IsActive,
                CreateDate = DateTime.UtcNow
            };

            _context.Hotels.Add(hotel);
            await _context.SaveChangesAsync();

            return new HotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PostalCode = hotel.PostalCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                CheckInTime = hotel.CheckInTime,
                CheckOutTime = hotel.CheckOutTime,
                IsActive = hotel.IsActive,
                Images = new List<ImageResponse>()
            };
        }
        public async Task<HotelResponse> UpdateHotel(UpdateHotelRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Hotel name cannot be empty");

            var hotel = await _context.Hotels
                .Include(h => h.Images)
                .FirstOrDefaultAsync(h => h.Id == request.Id && h.DeleteDate == null);

            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {request.Id} not found");

            hotel.Name = request.Name;
            hotel.Description = request.Description;
            hotel.Address = request.Address;
            hotel.City = request.City;
            hotel.Country = request.Country;
            hotel.PostalCode = request.PostalCode;
            hotel.PhoneNumber = request.PhoneNumber;
            hotel.Email = request.Email;
            hotel.StarRating = request.StarRating;
            hotel.CheckInTime = request.CheckInTime;
            hotel.CheckOutTime = request.CheckOutTime;
            hotel.IsActive = request.IsActive;
            hotel.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new HotelResponse
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Description = hotel.Description,
                Address = hotel.Address,
                City = hotel.City,
                Country = hotel.Country,
                PostalCode = hotel.PostalCode,
                PhoneNumber = hotel.PhoneNumber,
                Email = hotel.Email,
                StarRating = hotel.StarRating,
                CheckInTime = hotel.CheckInTime,
                CheckOutTime = hotel.CheckOutTime,
                IsActive = hotel.IsActive,
                Images = hotel.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };
        }
        public async Task<bool> DeleteHotel(int id)
        {
            var hotel = await _context.Hotels.FindAsync(id);

            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {id} not found");

            hotel.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
