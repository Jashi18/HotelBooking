using HotelBooking.Application.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace HotelBooking.Application.Implementations
{
    public class PasswordService : IPasswordService
    {
        public void CreatePasswordHash(string password, out string passwordHash, out string salt)
        {
            using (var hmac = new HMACSHA512())
            {
                salt = Convert.ToBase64String(hmac.Key);
                passwordHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
            }
        }

        public bool VerifyPasswordHash(string password, string storedHash, string storedSalt)
        {
            var saltBytes = Convert.FromBase64String(storedSalt);

            using (var hmac = new HMACSHA512(saltBytes))
            {
                var computedHash = Convert.ToBase64String(
                    hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));

                return computedHash == storedHash;
            }
        }
    }
}
