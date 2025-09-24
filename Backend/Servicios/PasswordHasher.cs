using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Servicios
{
    public static class PasswordHasher
    {
        public static string Sha512Hex(string password)
        {
            using var sha = SHA512.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            // Esto devuelve el hash en hexadecimal (mayúsculas)
            return Convert.ToHexString(hash);
        }
    }
}
