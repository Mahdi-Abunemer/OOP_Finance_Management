using Abunemer_Project_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Abunemer_Project_2
{
    
    public class Storage
    {
        private static Storage _instance;

        private List<User> Users { get; set; }
        public User? ActiveUser { get; private set; }
        public Wallet? ActiveWallet { get; private set; } // Active wallet for the user

        private Storage()
        {
            Users = new List<User>();
        }

        public Storage(User? activeUser)
        {
            ActiveUser = activeUser;
        }

        public static Storage GetInstance()
        {
            return _instance ??= new Storage();
        }

        public void SetActiveWallet(Wallet w)
        {
            if (ActiveUser == null)
            {
                throw new ArgumentException("There is no active user to set a wallet.");
            }
            if (!ActiveUser.userWallets(w))
            {
                throw new ArgumentException("This wallet does not belong to the user.");
            }
            ActiveWallet = w;
        }

        public void AddUser(User user)
        {
            if (Users.Any(x => x.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new ArgumentException("A user with the same email already exists.");
            }
            Users.Add(user);
        }

        public User FindUserByEmail(string email)
        {
            var user = Users.FirstOrDefault(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
            if (user == null)
            {
                throw new KeyNotFoundException($"User with email {email} not found.");
            }
            return user;
        }

        public void SetActiveUser(string email)
        {
            ActiveUser = FindUserByEmail(email);
        }

        public User? GetActiveUser()
        {
            return ActiveUser;
        }

        public Wallet? GetActiveWallet()
        {
            return ActiveWallet;
        }

        public string LogOut()
        {
            ActiveUser = null;
            ActiveWallet = null; 
            return "You have been logged out.";
        }
    }
}
