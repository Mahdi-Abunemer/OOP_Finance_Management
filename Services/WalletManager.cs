using Abunemer_Project_2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Abunemer_Project_2.Enums;
using static Abunemer_Project_2.Storage;
namespace Abunemer_Project_2.Services
{
    public class WalletManager
    {
        private Storage _storage;

        public WalletManager(Storage storage)
        {
            _storage = storage;
        }

        // Create a wallet and add it to the user
        public void CreateWallet(string name, Money startAmount,
            string CurrencyWallet)
        {
            if (_storage.ActiveUser == null)
            {
                throw new InvalidOperationException("No active user to create a wallet.");
            }

            if (_storage.ActiveUser.GetAllWallets().Any(w => w.Name == name))
            {
                throw new ArgumentException("Wallet with the same name already exists.");
            }

            var wallet = new Wallet(startAmount, name, CurrencyWallet);
            _storage.ActiveUser.AddWallet(wallet); // Add wallet to the active user
        }

        public void DeleteWallet(string name)  //(Wallet wallet)
        {
            if (_storage.ActiveUser == null)
            {
                throw new InvalidOperationException("No active user to create a wallet.");
            }

            if (_storage.ActiveUser.GetAllWallets().Any
                (w => w.Name != name))
            {
                throw new ArgumentException("Wallet doesn't exists.");
            }
            var wallet = GetAllWallets().FirstOrDefault(w => w.Name == name);
            _storage.ActiveUser.Del_Wallet(wallet);
        }
        // Add income to the active wallet
        public void Add_Income(Income income)
        {
            if (_storage.ActiveWallet == null)
            {
                throw new InvalidOperationException("No active wallet selected.");
            }

            _storage.ActiveWallet.AddIncome(income);
        }

        // Add expense to the active wallet
        public void Add_Expense(Expense expense)
        {
            if (_storage.ActiveWallet == null)
            {
                throw new InvalidOperationException("No active wallet selected.");
            }

            _storage.ActiveWallet.AddExpense(expense);
        }

        // Get all wallets associated with the active user
        public List<Wallet> GetAllWallets()
        {
            if (_storage.ActiveUser == null)
            {
                throw new InvalidOperationException("No active user.");
            }
            return _storage.ActiveUser.GetAllWallets();
        }
        public void ChooseWallet(string name)
        {
            if (_storage.ActiveUser == null)
            {
                throw new InvalidOperationException("No active user to create a wallet.");
            }
            _storage.SetActiveWallet(_storage.ActiveUser.Choose_Wallet(name));
        }
        public string collectStatistics(DateTime from, DateTime to)
        {

            if (_storage.ActiveWallet == null)
            {
                throw new InvalidOperationException("No active wallet to Calculate Statistics.");
            }
            string cur = Convert.ToString(_storage.ActiveWallet.walletCurr);
            var res = _storage.ActiveWallet.CollectStatistics(from, to, cur);
            return $"The result  of Statistics calculation for " +
                $": {res} ";
        }
        public bool AddOperation(Operation operation, Money value, int categoryNumber, DateTime date)
        {
            if (_storage.ActiveWallet == null)
            {
                throw new InvalidOperationException("No active wallet selected.");
            }
            if (_storage.ActiveWallet.balance.CurrencyType != value.CurrencyType)
            {
                throw new InvalidOperationException("The Currency Type of operation should be" +
                    " the same as currency Type of your wallet!");
            }
            if (value == null)
            {
                throw new ArgumentException("Invalid money value provided.");
            }

            operation.Value = value;
            operation.Date = date;
            _storage.ActiveWallet._operations.Add(operation);
            if (operation is Expense expense)
            {
                // Validate category number for Expense_Type
                if (!Enum.IsDefined(typeof(Expense_Type), categoryNumber))
                {
                    throw new ArgumentException("Invalid expense category.");
                }

                // Assign expense category
                Expense_Type category = (Expense_Type)categoryNumber;
                expense.Category = category; 
                expense.Value = value;
                Add_Expense(expense);
            }
            else if (operation is Income income)
            {
                // 
                if (!Enum.IsDefined(typeof(Income_Type), categoryNumber))
                {
                    throw new ArgumentException("Invalid income category.");
                }
                // Assign income category
                Income_Type category = (Income_Type)categoryNumber;
                 income.Category = category; 
                income.Value = value;
                Add_Income(income);
            }
            else
            {
                throw new ArgumentException("Operation type is not supported.");
            }

            return true;
        }
    }
}
