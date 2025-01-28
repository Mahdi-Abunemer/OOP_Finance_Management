using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Abunemer_Project_2.Models.Wallet; 
namespace Abunemer_Project_2.Models
{
    public class User
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public byte[] Salt { get; set; }
        public string BirthDay { get; private set; } 

        private List<Wallet> wallet = new List<Wallet>();

       

        public User(string email, string passwordHash, string birthDay, string name)
        {
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            BirthDay = birthDay;
        }

        public void AddWallet(Wallet w)
        {
            wallet.Add(w);
        }
        public void Del_Wallet(Wallet w)
        {
            wallet.Remove(w); 
        }
        public bool userWallets(Wallet w)
        {
            return (wallet.Contains(w)) ? true : false;  
        }
        public Wallet? Choose_Wallet(string name)
        {
    
            var wallet = GetAllWallets().FirstOrDefault(w => w.Name == name);
            if (wallet == null)
            {
                throw new ArgumentException("Wallet doesn't exist.");
            }
            return wallet;
        }

        public List<Wallet> GetAllWallets() => wallet;
    }
}
