using HotelBooking.Models.AuthModels;
using HotelBooking.Models.HotelModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelBooking.Application.Interfaces
{
    public interface IHotelAdministratorService
    {
        Task<List<HotelResponse>> GetManagedHotelsByUserId(int userId);
        Task<List<HotelAdministratorResponse>> GetAllHotelAdministrators();
        Task<List<HotelAdministratorResponse>> GetAdministratorsByHotelId(int hotelId);
        Task<bool> AssignHotelToAdministrator(AssignHotelAdministratorRequest request);
        Task<bool> RemoveHotelFromAdministrator(int userId, int hotelId);
    }
}
