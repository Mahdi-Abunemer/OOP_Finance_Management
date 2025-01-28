using Abunemer_Project_2.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using static Abunemer_Project_2.Enums;
using static Abunemer_Project_2.Helpers;

namespace Abunemer_Project_2.Services
{
    public interface IBusinessLogicServic
    {
        (bool, string) Register(string email, string password, string Birth_Date, string name);
        (bool, string) LogIn(string email, string password);
    }

    public class BusinessLogicServic : IBusinessLogicServic
    {
        private Storage _storage { get; }

        public BusinessLogicServic()
        {
            _storage = Storage.GetInstance();
        }

        public (bool, string) Register(string email, string password, string birthDate, string name)
        {
            try
            {
                var mail = new MailAddress(email);
            }
            catch (FormatException)
            {
                return (false, "Email is incorrect!\n");
            }

            var passwordScore = PasswordAdvisor.CheckStrength(password);
            if (passwordScore < PasswordScore.Medium)
            {
                return (false, "Your password is weak!\n");
            }

            if (!DateTime.TryParseExact(birthDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var birthDateParsed))
            {
                return (false, "Birth date has incorrect format!\n");
            }

            try
            {
                // Generate salt and hash the password
                var salt = SecurityHelper.GenerateSalt();
                var passwordHash = SecurityHelper.HashPassword(password, salt);

                var newUser = new User(email, passwordHash, birthDate, name)
                {
                    Salt = salt // Storing the salt for future use
                };
                _storage.AddUser(newUser);
            }
            catch (ArgumentException)
            {
                return (false, "A user with the same email already exists.\n");
            }

            // Log the user in
            _storage.SetActiveUser(email);
            return (true, "You have successfully registered! You will be automatically logged in.");
        }

        public (bool, string) LogIn(string email, string password)
        {
            try
            {
                var user = _storage.FindUserByEmail(email);
                var hashedPassword = SecurityHelper.HashPassword(password, user.Salt);
                if (user.PasswordHash == hashedPassword)
                {
                    _storage.SetActiveUser(email);
                    return (true, "You have successfully logged in!\n");
                }

                return (false, "Password is incorrect!\n");
            }
            catch (KeyNotFoundException)
            {
                return (false, "User with this email not found.");
            }
        }
    }
}
