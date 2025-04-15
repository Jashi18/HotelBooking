using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Data;
using HotelBooking.Domain.Entities;
using HotelBooking.Models.AuthModels;
using HotelBooking.Models.HotelModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Implementations
{
    public class HotelAdministratorService : IHotelAdministratorService
    {
        private readonly ApplicationDbContext _context;
        private const string ADMIN_ROLE = "Admin";
        private const string HOTEL_ADMIN_ROLE = "HotelAdministrator";

        public HotelAdministratorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<HotelResponse>> GetManagedHotelsByUserId(int userId)
        {
            var isAdmin = await IsUserInRole(userId, ADMIN_ROLE);

            if (isAdmin)
            {
                var allHotels = await _context.Hotels
                    .Include(h => h.Images)
                    .Where(h => h.DeleteDate == null)
                    .ToListAsync();

                return allHotels.Select(h => MapHotelToResponse(h)).ToList();
            }

            var hotelIds = await _context.HotelAdministrators
                .Where(ha => ha.UserId == userId && ha.DeleteDate == null)
                .Select(ha => ha.HotelId)
                .ToListAsync();

            var hotels = await _context.Hotels
                .Include(h => h.Images)
                .Where(h => hotelIds.Contains(h.Id) && h.DeleteDate == null)
                .ToListAsync();

            return hotels.Select(h => MapHotelToResponse(h)).ToList();
        }
        public async Task<List<HotelAdministratorResponse>> GetAllHotelAdministrators()
        {
            var hotelAdmins = await _context.HotelAdministrators
                .Include(ha => ha.User)
                .Include(ha => ha.Hotel)
                .Where(ha => ha.DeleteDate == null &&
                            ha.User.DeleteDate == null &&
                            ha.Hotel.DeleteDate == null)
                .ToListAsync();

            return hotelAdmins.Select(ha => new HotelAdministratorResponse
            {
                Id = ha.Id,
                UserId = ha.UserId,
                UserEmail = ha.User.Email,
                UserName = $"{ha.User.FirstName} {ha.User.LastName}",
                HotelId = ha.HotelId,
                HotelName = ha.Hotel.Name
            }).ToList();
        }
        public async Task<List<HotelAdministratorResponse>> GetAdministratorsByHotelId(int hotelId)
        {
            var hotelAdmins = await _context.HotelAdministrators
                .Include(ha => ha.User)
                .Include(ha => ha.Hotel)
                .Where(ha => ha.HotelId == hotelId &&
                            ha.DeleteDate == null &&
                            ha.User.DeleteDate == null)
                .ToListAsync();

            return hotelAdmins.Select(ha => new HotelAdministratorResponse
            {
                Id = ha.Id,
                UserId = ha.UserId,
                UserEmail = ha.User.Email,
                UserName = $"{ha.User.FirstName} {ha.User.LastName}",
                HotelId = ha.HotelId,
                HotelName = ha.Hotel.Name
            }).ToList();
        }
        public async Task<bool> AssignHotelToAdministrator(AssignHotelAdministratorRequest request)
        {
            var isHotelAdmin = await IsUserInRole(request.UserId, HOTEL_ADMIN_ROLE);

            if (!isHotelAdmin)
                return false;

            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == request.HotelId && h.DeleteDate == null);

            if (hotel == null)
                return false;

            var existingAssignment = await _context.HotelAdministrators
                .FirstOrDefaultAsync(ha => ha.UserId == request.UserId &&
                                          ha.HotelId == request.HotelId &&
                                          ha.DeleteDate == null);

            if (existingAssignment != null)
                return true;

            var hotelAdmin = new HotelAdministrator
            {
                UserId = request.UserId,
                HotelId = request.HotelId,
                CreateDate = DateTime.UtcNow
            };

            _context.HotelAdministrators.Add(hotelAdmin);
            await _context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> RemoveHotelFromAdministrator(int userId, int hotelId)
        {
            var assignment = await _context.HotelAdministrators
                .FirstOrDefaultAsync(ha => ha.UserId == userId &&
                                          ha.HotelId == hotelId &&
                                          ha.DeleteDate == null);

            if (assignment == null)
                return false;

            assignment.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        private async Task<bool> IsUserInRole(int userId, string roleName)
        {
            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == userId &&
                               ur.Role.Name == roleName &&
                               ur.Role.DeleteDate == null);
        }
        private HotelResponse MapHotelToResponse(Hotel hotel)
        {
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
    }
}
