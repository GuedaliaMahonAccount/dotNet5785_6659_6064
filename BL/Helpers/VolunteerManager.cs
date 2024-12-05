using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net;
using System.Text.Json;

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

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371; // Earth's radius in kilometers
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c; // Distance in kilometers
        }

        private static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }

        /// <summary>
        /// Retrieves coordinates from an address using LocationIQ API.
        /// </summary>
        public static (double latitude, double longitude)? GetCoordinatesFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            string apiKey = "67520387b42a2384305556nqrdb57df";
            string url = $"https://api.locationiq.com/v1/search.php?key={apiKey}&q={WebUtility.UrlEncode(address)}&format=json";

            try
            {
                var request = WebRequest.Create(url);
                request.Method = "GET";
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    var result = reader.ReadToEnd();

                    // Parse JSON using System.Text.Json
                    var json = JsonDocument.Parse(result);
                    var root = json.RootElement;

                    if (root.GetArrayLength() > 0)
                    {
                        var firstResult = root[0];
                        var latitude = double.Parse(firstResult.GetProperty("lat").GetString());
                        var longitude = double.Parse(firstResult.GetProperty("lon").GetString());
                        return (latitude, longitude);
                    }
                    return null; // No results
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching coordinates: {ex.Message}");
                return null;
            }
        }
        public static bool ValidAddress(string address)
        {
            var coordinates = GetCoordinatesFromAddress(address);
            return coordinates.HasValue; // If coordinates are returned, the address is real.
        }
        public static bool AreCoordinatesMatching(string address, double latitude, double longitude)
        {
            var resolvedCoordinates = GetCoordinatesFromAddress(address);
            if (!resolvedCoordinates.HasValue)
                return false;

            const double tolerance = 0.0001; // Adjust as necessary.
            return Math.Abs(resolvedCoordinates.Value.latitude - latitude) <= tolerance &&
                   Math.Abs(resolvedCoordinates.Value.longitude - longitude) <= tolerance;
        }

        public static bool IsRequesterAuthorizedToCancel(int requesterId, int volunteerId)
        {
            if (requesterId == volunteerId)
                return true;

            var requester = s_dal.Volunteer.Read(requesterId);
            if (requester != null && requester.Role == DO.Role.Admin)
                return true;

            return false;
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