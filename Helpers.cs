using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text.RegularExpressions;
using static Abunemer_Project_2.Enums;

namespace Abunemer_Project_2
{
    internal class Helpers
    {
        public static class SecurityHelper
        {
            public static byte[] GenerateSalt()
            {
                var salt = new byte[16]; // 128-bit salt
                using (var rng = RandomNumberGenerator.Create())
                {
                    rng.GetBytes(salt);
                }
                return salt;
            }
            public static string HashPassword(string password, byte[] salt)
            {
                return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
            }

        }
        public class PasswordAdvisor
        {
            public static PasswordScore CheckStrength(string password)
            {
                int score = 0;
                if (password.Length < 1)
                {
                    return PasswordScore.Blank;
                }
                if (password.Length < 4)
                    return PasswordScore.VeryWeek;
                if (password.Length >= 5)
                    score++;
                if (password.Length >= 7)
                    score++;
                Regex Valid_Regex = new Regex
                   ("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%&*-]).{8,}$");
                if (Valid_Regex.IsMatch(password))
                {
                    score += 2;
                }
                return (PasswordScore)score;
            }
        }
    }
   

}
