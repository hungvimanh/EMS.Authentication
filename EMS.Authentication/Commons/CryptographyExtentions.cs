using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TF.Authentication.Commons
{
    public static class CryptographyExtentions
    {
        /// <summary>
        /// Băm password với salt có sẵn.
        /// </summary>
        /// <param name="password">Password hashed</param>
        /// <param name="salt">Salt có sẵn</param>
        /// <returns>Password sau khi băm</returns>
        public static string HashHMACSHA256(this string password, string salt)
        {
            var realSalt = string.IsNullOrEmpty(salt) ? string.Empty : salt;
            return Hash(password, Encoding.ASCII.GetBytes(realSalt));
        }

        public static byte[] GenerateSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }

        //Generate Password ngẫu nhiên 10 kí tự
        public static string GeneratePassword()
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 10; i++)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            return res.ToString();
        }

        //mã hoá password
        private static string Hash(string password, byte[] salt)
        {
            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
        }
    }
}
