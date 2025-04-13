using HotelBooking.Application.Interfaces;
using HotelBooking.Domain.Data;
using HotelBooking.Domain.Entities;
using HotelBooking.Domain.Enums;
using HotelBooking.Models.HotelModels;
using HotelBooking.Models.RoomModels;
using Microsoft.EntityFrameworkCore;

namespace HotelBooking.Application.Implementations
{
    public class RoomService : IRoomService
    {
        private readonly ApplicationDbContext _context;

        public RoomService(ApplicationDbContext dbContext)
        {
            _context = dbContext;
        }

        #region Room Operations
        public async Task<List<RoomResponse>> GetAllRooms()
        {
            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .Where(r => r.DeleteDate == null && r.Hotel.DeleteDate == null)
                .ToListAsync();

            return rooms.Select(r => new RoomResponse
            {
                Id = r.Id,
                HotelId = r.HotelId,
                RoomNumber = r.RoomNumber,
                Name = r.Name,
                Description = r.Description,
                BasePrice = r.BasePrice,
                MaxOccupancy = r.MaxOccupancy,
                IsActive = r.IsActive,
                HotelName = r.Hotel?.Name ?? string.Empty,
                RoomType = r.RoomType != null ? new RoomTypeResponse
                {
                    Id = r.RoomType.Id,
                    Name = r.RoomType.Name,
                    Description = r.RoomType.Description
                } : null,
                Images = r.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            }).ToList();
        }
        public async Task<List<RoomResponse>> GetRoomsByHotelId(int hotelId)
        {
            var rooms = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .Where(r => r.HotelId == hotelId && r.DeleteDate == null)
                .ToListAsync();

            return rooms.Select(r => new RoomResponse
            {
                Id = r.Id,
                HotelId = r.HotelId,
                RoomNumber = r.RoomNumber,
                Name = r.Name,
                Description = r.Description,
                BasePrice = r.BasePrice,
                MaxOccupancy = r.MaxOccupancy,
                IsActive = r.IsActive,
                HotelName = r.Hotel?.Name ?? string.Empty,
                RoomType = r.RoomType != null ? new RoomTypeResponse
                {
                    Id = r.RoomType.Id,
                    Name = r.RoomType.Name,
                    Description = r.RoomType.Description
                } : null,
                Images = r.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            }).ToList();
        }
        public async Task<RoomResponse> GetRoomById(int id)
        {
            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == id && r.DeleteDate == null);

            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            return new RoomResponse
            {
                Id = room.Id,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                Name = room.Name,
                Description = room.Description,
                BasePrice = room.BasePrice,
                MaxOccupancy = room.MaxOccupancy,
                IsActive = room.IsActive,
                HotelName = room.Hotel?.Name ?? string.Empty,
                RoomType = room.RoomType != null ? new RoomTypeResponse
                {
                    Id = room.RoomType.Id,
                    Name = room.RoomType.Name,
                    Description = room.RoomType.Description
                } : null,
                Images = room.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };
        }
        public async Task<RoomResponse> CreateRoom(CreateRoomRequest request)
        {
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == request.HotelId && h.DeleteDate == null);

            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {request.HotelId} not found");

            if (request.RoomTypeId.HasValue)
            {
                var roomType = await _context.RoomTypes
                    .FirstOrDefaultAsync(rt => rt.Id == request.RoomTypeId.Value && rt.DeleteDate == null);

                if (roomType == null)
                    throw new KeyNotFoundException($"Room type with ID {request.RoomTypeId} not found");
            }

            if (string.IsNullOrEmpty(request.RoomNumber))
                throw new ArgumentException("Room number cannot be empty");

            var existingRoom = await _context.Rooms
                .FirstOrDefaultAsync(r => r.HotelId == request.HotelId &&
                                         r.RoomNumber == request.RoomNumber &&
                                         r.DeleteDate == null);

            if (existingRoom != null)
                throw new InvalidOperationException($"Room number {request.RoomNumber} already exists in this hotel");

            var room = new Room
            {
                HotelId = request.HotelId,
                RoomNumber = request.RoomNumber,
                Name = request.Name,
                Description = request.Description,
                BasePrice = request.BasePrice,
                MaxOccupancy = request.MaxOccupancy,
                IsActive = request.IsActive,
                CreateDate = DateTime.UtcNow
            };

            if (request.RoomTypeId.HasValue)
            {
                var roomType = await _context.RoomTypes.FindAsync(request.RoomTypeId.Value);
                room.RoomType = roomType;
            }

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var createdRoom = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == room.Id);

            return new RoomResponse
            {
                Id = createdRoom!.Id,
                HotelId = createdRoom.HotelId,
                RoomNumber = createdRoom.RoomNumber,
                Name = createdRoom.Name,
                Description = createdRoom.Description,
                BasePrice = createdRoom.BasePrice,
                MaxOccupancy = createdRoom.MaxOccupancy,
                IsActive = createdRoom.IsActive,
                HotelName = createdRoom.Hotel?.Name ?? string.Empty,
                RoomType = createdRoom.RoomType != null ? new RoomTypeResponse
                {
                    Id = createdRoom.RoomType.Id,
                    Name = createdRoom.RoomType.Name,
                    Description = createdRoom.RoomType.Description
                } : null,
                Images = createdRoom.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };
        }
        public async Task<RoomResponse> UpdateRoom(UpdateRoomRequest request)
        {
            var hotel = await _context.Hotels
                .FirstOrDefaultAsync(h => h.Id == request.HotelId && h.DeleteDate == null);

            if (hotel == null)
                throw new KeyNotFoundException($"Hotel with ID {request.HotelId} not found");

            if (request.RoomTypeId.HasValue)
            {
                var roomType = await _context.RoomTypes
                    .FirstOrDefaultAsync(rt => rt.Id == request.RoomTypeId.Value && rt.DeleteDate == null);

                if (roomType == null)
                    throw new KeyNotFoundException($"Room type with ID {request.RoomTypeId} not found");
            }

            if (string.IsNullOrEmpty(request.RoomNumber))
                throw new ArgumentException("Room number cannot be empty");

            var room = await _context.Rooms
                .Include(r => r.Hotel)
                .Include(r => r.RoomType)
                .Include(r => r.Images)
                .FirstOrDefaultAsync(r => r.Id == request.Id && r.DeleteDate == null);

            if (room == null)
                throw new KeyNotFoundException($"Room with ID {request.Id} not found");

            if (room.RoomNumber != request.RoomNumber)
            {
                var existingRoom = await _context.Rooms
                    .FirstOrDefaultAsync(r => r.HotelId == request.HotelId &&
                                             r.RoomNumber == request.RoomNumber &&
                                             r.Id != request.Id &&
                                             r.DeleteDate == null);

                if (existingRoom != null)
                    throw new InvalidOperationException($"Room number {request.RoomNumber} already exists in this hotel");
            }

            room.HotelId = request.HotelId;
            room.RoomNumber = request.RoomNumber;
            room.Name = request.Name;
            room.Description = request.Description;
            room.BasePrice = request.BasePrice;
            room.MaxOccupancy = request.MaxOccupancy;
            room.IsActive = request.IsActive;
            room.UpdateDate = DateTime.UtcNow;

            if (request.RoomTypeId.HasValue)
            {
                var roomType = await _context.RoomTypes.FindAsync(request.RoomTypeId.Value);
                room.RoomType = roomType;
            }
            else
            {
                room.RoomType = null;
            }

            await _context.SaveChangesAsync();

            return new RoomResponse
            {
                Id = room.Id,
                HotelId = room.HotelId,
                RoomNumber = room.RoomNumber,
                Name = room.Name,
                Description = room.Description,
                BasePrice = room.BasePrice,
                MaxOccupancy = room.MaxOccupancy,
                IsActive = room.IsActive,
                HotelName = room.Hotel?.Name ?? string.Empty,
                RoomType = room.RoomType != null ? new RoomTypeResponse
                {
                    Id = room.RoomType.Id,
                    Name = room.RoomType.Name,
                    Description = room.RoomType.Description
                } : null,
                Images = room.Images.Select(i => new ImageResponse
                {
                    Id = i.Id,
                    ImageUrl = i.ImageUrl,
                    Caption = i.Caption,
                    IsPrimary = i.IsPrimary
                }).ToList()
            };
        }
        public async Task<bool> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
                throw new KeyNotFoundException($"Room with ID {id} not found");

            var hasActiveBookings = await _context.Bookings
                .AnyAsync(b => b.RoomId == id &&
                              b.DeleteDate == null &&
                              (b.Status == BookingStatus.Pending ||
                               b.Status == BookingStatus.Confirmed ||
                               b.Status == BookingStatus.CheckedIn));

            if (hasActiveBookings)
                throw new InvalidOperationException("Cannot delete room with active bookings");

            room.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion

        #region RoomType Operations
        public async Task<List<RoomTypeResponse>> GetAllRoomTypes()
        {
            var roomTypes = await _context.RoomTypes
                .Where(rt => rt.DeleteDate == null)
                .ToListAsync();

            return roomTypes.Select(rt => new RoomTypeResponse
            {
                Id = rt.Id,
                Name = rt.Name,
                Description = rt.Description
            }).ToList();
        }
        public async Task<RoomTypeResponse> GetRoomTypeById(int id)
        {
            var roomType = await _context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == id && rt.DeleteDate == null);

            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found");

            return new RoomTypeResponse
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            };
        }
        public async Task<RoomTypeResponse> CreateRoomType(CreateRoomTypeRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Room type name cannot be empty");

            var existingRoomType = await _context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Name.ToLower() == request.Name.ToLower() &&
                                          rt.DeleteDate == null);

            if (existingRoomType != null)
                throw new InvalidOperationException($"Room type with name '{request.Name}' already exists");

            var roomType = new RoomType
            {
                Name = request.Name,
                Description = request.Description,
                CreateDate = DateTime.UtcNow
            };

            _context.RoomTypes.Add(roomType);
            await _context.SaveChangesAsync();

            return new RoomTypeResponse
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            };
        }
        public async Task<RoomTypeResponse> UpdateRoomType(UpdateRoomTypeRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
                throw new ArgumentException("Room type name cannot be empty");

            var roomType = await _context.RoomTypes
                .FirstOrDefaultAsync(rt => rt.Id == request.Id && rt.DeleteDate == null);

            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {request.Id} not found");

            if (roomType.Name.ToLower() != request.Name.ToLower())
            {
                var existingRoomType = await _context.RoomTypes
                    .FirstOrDefaultAsync(rt => rt.Name.ToLower() == request.Name.ToLower() &&
                                              rt.Id != request.Id &&
                                              rt.DeleteDate == null);

                if (existingRoomType != null)
                    throw new InvalidOperationException($"Room type with name '{request.Name}' already exists");
            }

            roomType.Name = request.Name;
            roomType.Description = request.Description;
            roomType.UpdateDate = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return new RoomTypeResponse
            {
                Id = roomType.Id,
                Name = roomType.Name,
                Description = roomType.Description
            };
        }
        public async Task<bool> DeleteRoomType(int id)
        {
            var roomType = await _context.RoomTypes.FindAsync(id);

            if (roomType == null)
                throw new KeyNotFoundException($"Room type with ID {id} not found");

            var roomsUsingType = await _context.Rooms
                .AnyAsync(r => r.RoomType!.Id == id && r.DeleteDate == null);

            if (roomsUsingType)
                throw new InvalidOperationException("Cannot delete room type that is in use by rooms");

            roomType.DeleteDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
        #endregion
    }
}
