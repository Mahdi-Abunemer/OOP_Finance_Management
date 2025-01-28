using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using static Abunemer_Project_2.Enums; 

namespace Abunemer_Project_2.Models
{   
    public class Money : IEquatable<Money>, IComparable<Money>
    {
        private char sign;
        private uint intpart;
        private byte frcpart;

        private Currency currency;

        public Currency CurrencyType
        {
            get { return currency; }
            set { currency = value; }
        }
        public char Sign
        {
            get { return sign; }
            set
            {
                if (value == '-' || value == '+')
                    sign = value;
                else
                    throw new ArgumentException("Sign  must be \"+\" OR \"-\".");
            }
        }
        public uint Intpart
        {
            get { return intpart; }
            set { intpart = value; }
        }
        public byte Frcpart
        {
            get { return frcpart; }
            set
            {
                if (value >= 0 && value <= 99)
                    frcpart = value;
                else
                    throw new ArgumentException("Fractional part must be between 0 and 99.");
            }
        }


        public string GetCurrencySymbol()
        {
            switch (currency)
            {
                case Currency.Dollar:
                    return "$";
                case Currency.Euro:
                    return "EUR";
                case Currency.Rubble:
                    return "RUB";
                default:
                    return "";

            }
        }
        //Method for displaying whole number in string format
        public string Display()
        {
            return $"{sign}{intpart}.{frcpart:D2} {GetCurrencySymbol()}";
        }
        public void Setting_input(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput))
            {
                throw new ArgumentException("Input string cannot be null or empty.");
            }

            var match = Regex.Match(strInput, @"^(?<sign>[+-])(?<intpart>\d+)\.(?<frcpart>\d{1,2})\s*(?<currency>[$A-Z]+)$");

            if (!match.Success)
            {
                throw new ArgumentException("Invalid input format.");
            }

            Sign = match.Groups["sign"].Value[0];

            Intpart = uint.Parse(match.Groups["intpart"].Value);

            // Extract and assign fractional part, ensuring it has 2 digits
            string fractionalString = match.Groups["frcpart"].Value;
            Frcpart = byte.Parse(fractionalString.PadRight(2, '0'));

            string currencySymbol = match.Groups["currency"].Value;
            currency = currencySymbol switch
            {
                "$" => Currency.Dollar,
                "EUR" => Currency.Euro,
                "RUB" => Currency.Rubble,
                _ => throw new ArgumentException("Unknown currency symbol.")
            };
        }

        // 4- constructor 
        public Money()
        { // Randomly initialize
            Random rand = new Random();

            intpart = (uint)rand.Next(0, 101);
            frcpart = (byte)(rand.NextDouble() * 0.99);
            sign = rand.Next(0, 2) == 0 ? '+' : '-';

            Array values = Enum.GetValues(typeof(Currency));
            currency = (Currency)values.GetValue(rand.Next(values.Length));
        }

        // -5-
        public Money(char s, uint i, byte frc, string curr_user)
        {
            Sign = s;
            Intpart = i;
            Frcpart = frc;
            if (!Enum.TryParse(curr_user, true, out Currency parsedCurrency))//12/10 i changed
            {
                throw new ArgumentException("Invalid currency type");
            }
            currency = parsedCurrency;
        }
        public Money(Money acopy)
        {
            Sign = acopy.sign;
            Intpart = acopy.intpart;
            Frcpart = acopy.frcpart;

            currency = acopy.currency;
        }
        public Money(string StrInput)
        {
            Setting_input(StrInput);
        }
        //6- 
        public string AddAmount(char sign, uint integerPart, byte fractionalPart)
        {

            int currentSign = Sign == '+' ? 1 : -1;
            int inputSign = sign == '+' ? 1 : -1;

            // Calculate the result based on both signs
            int fres = currentSign * Frcpart + inputSign * fractionalPart;
            long res = currentSign * Intpart + inputSign * integerPart;

            if (fres >= 100)
            {
                fres -= 100;
                res++;
            }
            else if (fres < 0)
            {
                fres += 100;
                res--;
            }

            if (res < 0 || res == 0 && fres < 0)
            {
                res = Math.Abs(res);
                fres = Math.Abs(fres);
                Sign = '-';
            }
            else
            {
                Sign = '+';
            }

            return $"{Sign}{res}.{fres:D2} {GetCurrencySymbol()}";
        }
        //7-
        public string AddAccounting(Money other)
        {
            return AddAmount(other.Sign, other.Intpart, other.Frcpart);
        }

        //8 
        public string SubtractAmount(char sign, uint integerPart, byte fractionalPart)
        {
            if (sign == '+')
                return AddAmount('-', integerPart, fractionalPart);

            else
                return AddAmount('+', integerPart, fractionalPart);
        }
        //9
        public string SubtractAccounting(Money other)
        {
            return SubtractAmount(other.Sign, other.Intpart, other.Frcpart);
        }
        //10
        public bool Equals(Money? other)
        {
            if (other == null)
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return Sign == other.Sign &&
                   Intpart == other.Intpart &&
                   Math.Abs(Frcpart - other.Frcpart) < 100 &&
                   currency == other.currency;
        }

        // override 
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return Equals(obj as Money);
        }

        // Override GetHashCode() to ensure consistent behavior with Equals
        public override int GetHashCode()
        {
            return HashCode.Combine(Sign, Intpart, Frcpart, currency);
        }

        //11
        public int CompareTo(Money? other)
        {
            // Handle null comparison
            if (other == null) return 1;

            // Compare the signs
            if (Sign != other.Sign)
            {
                // If this object is positive and other is negative, this is greater.
                return Sign == '+' ? 1 : -1;
            }

            // Both have the same sign; now compare Intpart
            int intPartComparison = Intpart.CompareTo(other.Intpart);
            if (intPartComparison != 0)
            {
                return Sign == '+' ? intPartComparison : -intPartComparison;
            }
            int frcPartComparison = Frcpart.CompareTo(other.Frcpart);
            if (frcPartComparison != 0)
            {
                return Sign == '+' ? frcPartComparison : -frcPartComparison;
            }

            //  compare currencies if everything else is the same
            return currency.CompareTo(other.currency);
        }
        //overload      
        public static bool operator >(Money a, Money b)
        {
            return a.CompareTo(b) > 0;
        }
        public static bool operator >=(Money a, Money b)
        {
            return a.CompareTo(b) >= 0;
        }
        public static bool operator <(Money a, Money b)
        {
            return a.CompareTo(b) < 0;
        }
        public static bool operator <=(Money a, Money b)
        {
            return a.CompareTo(b) <= 0;
        }
        //14
        public void Conv(Currency targetCurrency)
        {
            // Define conversion rates
            Dictionary<(Currency, Currency), float> conversionRates = new Dictionary<(Currency, Currency), float>
    {
        { (Currency.Dollar, Currency.Euro), 0.85f },
        { (Currency.Dollar, Currency.Rubble), 95.0f },
        { (Currency.Euro, Currency.Dollar), 1.18f },
        { (Currency.Euro, Currency.Rubble), 88.0f },
        { (Currency.Rubble, Currency.Dollar), 0.013f },
        { (Currency.Rubble, Currency.Euro), 0.011f },
    };

            // If no conversion is needed
            if (currency == targetCurrency)
            {
                return;
            }

            // Get the conversion rate
            if (!conversionRates.TryGetValue((currency, targetCurrency), out float conversionRate))
            {
                throw new ArgumentException("No conversion rate available for the selected currencies.");
            }

            // Calculate total amount
            float totalAmount = Intpart + Frcpart / 100f;

            // Convert total amount to the target currency
            float convertedAmount = totalAmount * conversionRate;


            // Split into integer and fractional parts
            Intpart = (uint)convertedAmount;
            Frcpart = (byte)((convertedAmount - Intpart) * 100);


            // Update the currency to the target currency
            currency = targetCurrency;

        }
        //12 
        public Money MSum(Money other)
        {
            if (other == null)
            {
                return new Money(Sign, Intpart, Frcpart, CurrencyType.ToString());
            }

            if (currency != other.currency)
            {
                Conv(other.currency);
            }

            string resultString = AddAccounting(other);
            Money result = new Money(resultString);
            return result;
        }
        //13
        public Money MDif(Money other)
        {
            if (other == null)
            {
                return new Money(Sign, Intpart, Frcpart, CurrencyType.ToString());
            }

            if (currency != other.currency)
            {
                Conv(other.currency);
            }

            string resultString = SubtractAccounting(other);
            Money result = new Money(resultString);
            return result;
        }
    }
}