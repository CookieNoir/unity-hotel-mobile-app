using System;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace Hotel.Validation
{
    public class DataValidation
    {
        public static bool IsLengthValid(string name, int minLength, int maxLength)
        {
            return name != null && name.Length >= minLength && name.Length <= maxLength;
        }

        public static bool IsEMailValid(string email)
        {
            if (email.Length == 0) return false;
            try
            {
                MailAddress m = new MailAddress(email);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        public static bool IsPhoneNumberValid(string phoneNumber)
        {
            return Regex.IsMatch(phoneNumber, @"^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$");
        }

        public static bool IsNaturalNumber(int number)
        {
            return number > 0;
        }
    }
}