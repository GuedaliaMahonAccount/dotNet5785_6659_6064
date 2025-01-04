using DalApi;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using BO;
using System.Globalization;

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
            if (string.IsNullOrEmpty(phone))
                return false;

            phone = phone.Trim().Replace("-", "").Replace(" ", "");

            if (phone.Length != 10)
                return false;

            if (!phone.StartsWith("05"))
                return false;

            return phone.All(char.IsDigit);
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
        /// exemple of correct adress
        /// "display_name": "מרכז אורן יצחק רגר, 185, באר שבע"
        /// "lat": "31.27042089999999"
        /// "lon": "34.7975837"
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

        /// <summary>
        /// new fonction to check coordinations
        /// </summary>
        public static (double latitude, double longitude)? GetClosestCoordinates(string address, double targetLatitude, double targetLongitude)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            string apiKey = "pk.24d7295db243a75d8b3c688089250321";
            string url = $"https://api.locationiq.com/v1/search.php?key={apiKey}&q={WebUtility.UrlEncode(address)}&format=json";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetStringAsync(url).Result;
                    var json = JsonDocument.Parse(response);
                    var root = json.RootElement;

                    if (root.GetArrayLength() > 0)
                    {
                        const double tolerance = 0.05; // Augmenté à environ 5km
                        double closestDistance = double.MaxValue;
                        (double lat, double lon)? closestCoordinates = null;

                        foreach (var item in root.EnumerateArray())
                        {
                            double latitude = double.Parse(item.GetProperty("lat").GetString(), CultureInfo.InvariantCulture);
                            double longitude = double.Parse(item.GetProperty("lon").GetString(), CultureInfo.InvariantCulture);

                            // Calculer la distance réelle entre les points
                            double distance = CalculateDistance(latitude, longitude, targetLatitude, targetLongitude);

                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestCoordinates = (latitude, longitude);
                            }
                        }

                        // Si la distance la plus proche est dans la tolérance
                        if (closestDistance <= tolerance * 111.32) // Convertir degrés en km approximativement
                        {
                            return closestCoordinates;
                        }
                    }
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching coordinates: {ex.Message}");
                return null;
            }
        }
        public static double CalculateDistance(double? lat1, double? lon1, double? lat2, double? lon2)
        {
            if (!lat1.HasValue || !lon1.HasValue || !lat2.HasValue || !lon2.HasValue)
                throw new InvalidOperationException("Cannot calculate distance: one or more coordinates are null.");

            var R = 6371; // Rayon de la Terre en km
            var dLat = ToRad(lat2.Value - lat1.Value);
            var dLon = ToRad(lon2.Value - lon1.Value);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1.Value)) * Math.Cos(ToRad(lat2.Value)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
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


        public static void PeriodicVolunteersUpdates()
        {
            try
            {
                var volunteersFromDal = s_dal.Volunteer.ReadAll();
                var assignmentsFromDal = s_dal.Assignment.ReadAll();

                var volunteersInBO = volunteersFromDal.Select(v => new BO.VolunteerInList
                {
                    Id = v.Id,
                    Name = v.Name,
                    IsActive = v.IsActive,
                    CompletedAssignmentsCount = assignmentsFromDal.Count(call => call.VolunteerId == v.Id),
                    CancelledCallsCount = assignmentsFromDal.Count(call => call.VolunteerId == v.Id),
                    ExpiredCallsCount = assignmentsFromDal.Count(call => call.VolunteerId == v.Id),
                    CurrentCallId = assignmentsFromDal.Where(call => call.VolunteerId == v.Id).Select(call => call.CallId).FirstOrDefault(),
                    CurrentCallType = assignmentsFromDal.Where(call => call.VolunteerId == v.Id).Select(call =>
                    {
                        switch ((BO.EndType)call.EndType)
                        {
                            case BO.EndType.Completed:
                                return BO.CallType.Completed;
                            case BO.EndType.SelfCanceled:
                                return BO.CallType.SelfCanceled;
                            case BO.EndType.Expired:
                                return BO.CallType.Expired;
                            case BO.EndType.AdminCanceled:
                                return BO.CallType.AdminCanceled;
                            default:
                                throw new InvalidOperationException("Unknown EndType");
                        }
                    }).FirstOrDefault()
                }).ToList();

                var volunteersWithNullCallType = volunteersInBO.Where(v => v.CurrentCallType == null).ToList();

                foreach (var volunteer in volunteersWithNullCallType)
                {
                    try
                    {
                        var volunteerAssignments = assignmentsFromDal
                            .Where(a => a.VolunteerId == volunteer.Id)
                            .OrderByDescending(a => a.StartTime)
                            .ToList();

                        if (volunteerAssignments.Any())
                        {
                            var latestAssignment = volunteerAssignments.First();

                            if (latestAssignment.StartTime < DateTime.Now.AddYears(-5))
                            {
                                s_dal.Volunteer.Delete(volunteer.Id);

                                // Remove the observer
                                VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);
                            }
                        }
                        else
                        {
                            s_dal.Volunteer.Delete(volunteer.Id);

                            // Remove the observer
                            VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new BlDoesNotExistException($"Error processing volunteer with ID {volunteer.Id}.", ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while removing volunteers with no active call for 5 years.", ex);
            }
        }

    }


    public static class AesEncryptionHelper
    {
        private static readonly string Key = "YourSecureKey123";

        private static readonly string IV = "YourSecureIV1234";

        public static string Encrypt(string plainText)
        {
            if (IsBase64String(plainText))
            {
                return plainText;
            }
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

        private static bool IsBase64String(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length % 4 != 0)
                return false;

            try
            {
                Convert.FromBase64String(str);
                return true;
            }
            catch
            {
                return false;
            }
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