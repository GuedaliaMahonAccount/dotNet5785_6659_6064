using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;

namespace Helpers
{
    internal static class VolunteerManager
    {
        private static IDal s_dal = Factory.Get;

        public static bool ValidId(string id)
        {
            // Check if ID is numeric
            if (!long.TryParse(id, out long numericId))
                return false;

            if (numericId < 200000000 || numericId > 400000000)
                return false;

            return ValidDigit(id);
        }

        private static bool ValidDigit(string id)
        {
            if (id.Length != 9)
                return false;

            int sum = 0;
            for (int i = 0; i < 8; i++)
            {
                int digit = int.Parse(id[i].ToString());
                int multiplier = (i % 2 == 0) ? 1 : 2;
                int result = digit * multiplier;
                sum += result > 9 ? result - 9 : result;
            }
            int checkDigit = int.Parse(id[8].ToString());
            int calculatedCheckDigit = (10 - (sum % 10)) % 10;
            return checkDigit == calculatedCheckDigit;
        }

        public static bool ValidName(string name)
        {
            // Check if name is not null or empty
            if (string.IsNullOrWhiteSpace(name))
                return false;

            // Check name length (adjust as needed)
            if (name.Length < 2 || name.Length > 50)
                return false;

            // Check if name contains only letters and spaces
            return Regex.IsMatch(name, @"^[A-Za-z\u0590-\u05FF\s]+$");
        }

        public static bool ValidPhone(string phone)
        {
            // Remove any non-digit characters
            string cleanedPhone = Regex.Replace(phone, @"[^\d]", "");

            // Israeli phone number validation
            // Mobile: 10 digits, starts with 05x
            // Landline: 9 digits
            return Regex.IsMatch(cleanedPhone, @"^(0)?5\d{8}$") ||
                   Regex.IsMatch(cleanedPhone, @"^\d{9}$");
        }

        public static bool ValidEmail(string email)
        {
            try
            {
                // Use MailAddress for initial validation
                var addr = new MailAddress(email);

                // Additional regex check for more thorough validation
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase);
            }
            catch
            {
                return false;
            }
        }

        public static bool ValidPassword(string password)
        {
            // Password requirements:
            // - Minimum 8 characters
            // - At least one uppercase letter
            // - At least one lowercase letter
            // - At least one number
            // - At least one special character
            return Regex.IsMatch(password,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
        }

        public static bool ValidIsActive(bool isActive)
        {
            // Just ensures it's a boolean
            return true;
        }

        public static bool ValidAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return false;

            // Check if address contains at least one valid street, city, or country name
            return Regex.IsMatch(address, @"\b(Street|Avenue|Road|Drive|Lane|Boulevard|City|Country)\b", RegexOptions.IgnoreCase);
        }

        public static bool ValidLongitude(double longitude)
        {
            return longitude >= -180 && longitude <= 180;
        }

        public static bool ValidLatitude(double latitude)
        {
            return latitude >= -90 && latitude <= 90;
        }
    }
}