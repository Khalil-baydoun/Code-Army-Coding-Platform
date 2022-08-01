using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Extensions.Options;
using System;
using System.Text;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations
{
    public class PBKDF2HashingService : IHashingService
    {
        public string Hash(string value, string userSalt)
        {
            string hashedValue = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: value,
                salt: Encoding.ASCII.GetBytes(userSalt),
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 256 / 8));
            return hashedValue;
        }
    }
}
