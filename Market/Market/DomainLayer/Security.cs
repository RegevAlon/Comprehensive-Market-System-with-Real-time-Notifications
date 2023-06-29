using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace Market.DomainLayer
{
    public class Security
    {
        public string HashPassword(string password)
        {
            byte[] salt = GenerateSalt();
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 10_000);
            byte[] hash = pbkdf2.GetBytes(24);

            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 12);
            Array.Copy(hash, 0, hashBytes, 12, 24);

            string hashedPassword = Convert.ToBase64String(hashBytes);
            return hashedPassword;
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(hashedPassword);
            byte[] salt = new byte[12];
            Array.Copy(hashBytes, 0, salt, 0, 12);

            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(passwordBytes, salt, 10_000);
            byte[] hash = pbkdf2.GetBytes(24);

            bool passwordMatch = hash.SequenceEqual(hashBytes.Skip(12));

            return passwordMatch;
        }

        private static byte[] GenerateSalt()
        {
            byte[] salt = new byte[12];
            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(salt);
            }
            return salt;
        }
    }
}
