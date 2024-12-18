using DalApi;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using BO;
using System.Security.Cryptography;
using System.Text;
using System.IO;

namespace Helpers
{
    internal static class VolunteerManager
    {
        internal static ObserverManager Observers = new();
        private static IDal s_dal = Factory.Get;

        /// <summary>
        /// function to check validity
        /// </summary>
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
            string decryptedPassword;
            try
            {
                decryptedPassword = AesEncryptionHelper.Decrypt(password);
            }
            catch
            {
                decryptedPassword = password;
            }

            // Password requirements:
            // - Minimum 8 characters
            // - At least one uppercase letter
            // - At least one lowercase letter
            // - At least one number
            return Regex.IsMatch(decryptedPassword,
                @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$");
        }

        public static bool ValidIsActive(bool isActive)
        {
            // Just ensures it's a boolean
            return true;
        }


        /// <summary>
        /// Retrieves coordinates from an address using LocationIQ API.
        /// 
        /// exemple of correct adress
        /// "display_name": "Tiltan, Ramla, Ramla Subdistrict, District centre, 7135275, Israël"
        /// "lat": "31.9290114"
        /// "lon": "34.8737578"
        /// 
        /// 
        /// </summary>
        public static List<(double latitude, double longitude)> GetCoordinatesFromAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            string apiKey = "pk.24d7295db243a75d8b3c688089250321";
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
                        // Extract all coordinates
                        var coordinatesList = new List<(double latitude, double longitude)>();
                        foreach (var item in root.EnumerateArray())
                        {
                            var latitude = double.Parse(item.GetProperty("lat").GetString());
                            var longitude = double.Parse(item.GetProperty("lon").GetString());
                            coordinatesList.Add((latitude, longitude));
                        }
                        return coordinatesList;
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
            var coordinatesList = GetCoordinatesFromAddress(address);
            return coordinatesList != null && coordinatesList.Count > 0; // If at least one result is returned, the address is valid.
        }
        public static bool AreCoordinatesMatching(string address, double latitude, double longitude)
        {
            var resolvedCoordinates = GetCoordinatesFromAddress(address);
            if (resolvedCoordinates == null || resolvedCoordinates.Count == 0)
                return false;

            const double tolerance = 0.001; // Ajustez si nécessaire.

            // Check if any of the returned coordinates match
            foreach (var (resolvedLatitude, resolvedLongitude) in resolvedCoordinates)
            {
                if (Math.Abs(resolvedLatitude - latitude) <= tolerance &&
                    Math.Abs(resolvedLongitude - longitude) <= tolerance)
                {
                    return true; // Found a match
                }
            }
            return false; // No match found
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

    }

    public static class AesEncryptionHelper
    {
        private static readonly string Key = "YourSecureKey123";

        private static readonly string IV = "YourSecureIV1234";

        public static string Encrypt(string plainText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using MemoryStream ms = new();
            using CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            using (StreamWriter writer = new(cs))
            {
                writer.Write(plainText);
            }

            return Convert.ToBase64String(ms.ToArray());
        }

        public static string Decrypt(string encryptedText)
        {
            using Aes aes = Aes.Create();
            aes.Key = Encoding.UTF8.GetBytes(Key);
            aes.IV = Encoding.UTF8.GetBytes(IV);

            using MemoryStream ms = new(Convert.FromBase64String(encryptedText));
            using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
            using StreamReader reader = new(cs);
            {
                return reader.ReadToEnd();
            }
        }
    }

    public static class PasswordUtils
    {
        public static string ReadAndEncryptPassword()
        {
            StringBuilder plainPassword = new StringBuilder();
            Console.Write("Enter password: ");

            while (true)
            {
                var keyInfo = Console.ReadKey(intercept: true);

                if (keyInfo.Key == ConsoleKey.Enter)
                {
                    Console.WriteLine(); 
                    break;
                }

                if (keyInfo.Key == ConsoleKey.Backspace && plainPassword.Length > 0)
                {
                    plainPassword.Remove(plainPassword.Length - 1, 1);
                    Console.Write("\b \b"); 
                }
                else if (keyInfo.Key != ConsoleKey.Backspace)
                {
                    plainPassword.Append(keyInfo.KeyChar);
                    Console.Write("*"); 
                }
            }

            return AesEncryptionHelper.Encrypt(plainPassword.ToString());
        }
    }
}