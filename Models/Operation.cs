using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Abunemer_Project_2.Enums;
namespace Abunemer_Project_2.Models
{
    public class Operation
    {
        public DateTime Date { get; set; }
        public Money Value { get; set; }
    }
    public class Income : Operation
    {
        public Currency CurrencyType { get; internal set; }
        public Income_Type Category { get; internal set; }
    }
    public class Expense : Operation
    {
        public Currency CurrencyType { get; internal set; }
        public Expense_Type Category { get; internal set; }
    }
}
