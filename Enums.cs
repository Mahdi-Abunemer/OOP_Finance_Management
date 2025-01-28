using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abunemer_Project_2
{
    public class Enums
    {
        public enum Income_Type
        {
            Salary = 1,
            Scholarship,
            Other
        }
        public enum Expense_Type
        {
            food = 1,
            restaurants,
            medicine,
            sport,
            taxi,
            rent,
            investments,
            clothes,
            fun,
            other
        }
        public enum PasswordScore
        {
            Blank = 0,
            VeryWeek = 1,
            Week = 2,
            Medium = 3,
            Storng = 4,
            VeryStorng = 5,

        }
        public enum Currency
        {
            Dollar,
            Euro,
            Rubble,
        }

    }
}
