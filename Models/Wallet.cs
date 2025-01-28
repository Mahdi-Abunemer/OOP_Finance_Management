using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;
using static Abunemer_Project_2.Models.Money;
using static Abunemer_Project_2.Enums; 

namespace Abunemer_Project_2.Models
{

    public class Wallet
    {
        public string Name { get; set; }
        public List<Operation> _operations = new List<Operation>();
        public Money balance { get; set; }
        public Currency walletCurr { get; set;  } 
        
        public void chooesCurrencyWallet(string curr)
        {
            
            switch (curr)
            {
                case "$":
                    walletCurr = Currency.Dollar;
                    break;
                case "EUR":
                    walletCurr = Currency.Euro;
                    break;
                case "RUB":
                    walletCurr = Currency.Rubble;
                    break; 
                default:
                    return ;

            }
        } 
        public Wallet(Money startAmount , string nameWallet , string CurrencyWallet)
        {
            this.balance = startAmount;
            chooesCurrencyWallet(CurrencyWallet);
            this.balance.CurrencyType = walletCurr;
            this.Name = nameWallet; 
        }
        public void AddIncome(Income income)
        {
           // balance.CurrencyType = income.CurrencyType; 
            if (income == null)
            {
                throw new ArgumentException("Income cannot be null.");
            }
            if (balance != null)
            {
               var res = this.balance.MSum(income.Value);
                this.balance = res; 
            }
            else
            {
                this.balance = new Money(income.Value);
            }
        }

        public void AddExpense(Expense expense)
        {
            if (expense == null)
            {
                throw new ArgumentException("Expense cannot be null.");
            }
            if (balance != null)
            {
                if (balance >= expense.Value) 
                {
                    var res = this.balance.MDif(expense.Value);
                    this.balance = res;
                }
                else
                {
                    throw new InvalidOperationException("Insufficient balance for this expense.");
                }
            }
            else
            {
                throw new InvalidOperationException($"Currency {expense.CurrencyType} not found in wallet.");
            }
        }

        public string CollectStatistics(DateTime from, DateTime to , string cur)
        {
            // Filter operations by the given date range
            var stats = _operations.Where(op => op.Date >= from && op.Date <= to).ToList();

            if (stats.Count == 0)
            {
                throw new ArgumentException("No operations found for the given date range.");
               
            }

            var result = new Money('+', 0, 0, cur);

            foreach (var op in stats)
            {
                
                if (op is Income incomeOp)
                {
                   result = result.MSum(incomeOp.Value);
                }
                else if (op is Expense expenseOp)
                {
                   result =  result.MDif(expenseOp.Value);
                }
            }

           return ($"The total statistics from {from} to {to} in {cur}: {result.Display()}");
        }
    }

}
