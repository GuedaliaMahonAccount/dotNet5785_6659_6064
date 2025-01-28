using DalApi;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Net;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using BO;
using System.Globalization;
using BlImplementation;

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
        /// 
        /// exemple of correct adress
        /// "display_name": "מרכז אורן יצחק רגר, 185, Beer Sheva"
        /// "lat": "31.27042089999999"
        /// "lon": "34.7975837"
        /// 
        /// 
        /// </summary>
        //public static async Task<List<(double latitude, double longitude)>> GetCoordinatesFromAddressAsync(string address)
        //{
        //    if (string.IsNullOrWhiteSpace(address))
        //        return null;

        //    string apiKey = "pk.24d7295db243a75d8b3c688089250321";
        //    string url = $"https://api.locationiq.com/v1/search.php?key={apiKey}&q={WebUtility.UrlEncode(address)}&format=json";

        //    try
        //    {
        //        using (var client = new HttpClient())
        //        {
        //            var response = await client.GetStringAsync(url);
        //            var json = JsonDocument.Parse(response);
        //            var root = json.RootElement;

        //            if (root.GetArrayLength() > 0)
        //            {
        //                var coordinatesList = new List<(double latitude, double longitude)>();
        //                foreach (var item in root.EnumerateArray())
        //                {
        //                    var latitude = double.Parse(item.GetProperty("lat").GetString());
        //                    var longitude = double.Parse(item.GetProperty("lon").GetString());
        //                    coordinatesList.Add((latitude, longitude));
        //                }
        //                return coordinatesList;
        //            }
        //            return null; // No results
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error fetching coordinates: {ex.Message}");
        //        return null;
        //    }
        //}
        public static async Task<(double latitude, double longitude)?> GetCoordinatesFromAddressAsync(string address)
        {
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentNullException(nameof(address), "Address cannot be null or empty.");

            string apiKey = "pk.24d7295db243a75d8b3c688089250321";
            string url = $"https://api.locationiq.com/v1/search.php?key={apiKey}&q={WebUtility.UrlEncode(address)}&format=json";

            try
            {
                using var client = new HttpClient();
                var response = await client.GetStringAsync(url);
                var json = JsonDocument.Parse(response);
                var root = json.RootElement;

                if (root.GetArrayLength() > 0)
                {
                    // Get the first result (most relevant)
                    var firstResult = root[0];
                    var latitude = double.Parse(firstResult.GetProperty("lat").GetString(), CultureInfo.InvariantCulture);
                    var longitude = double.Parse(firstResult.GetProperty("lon").GetString(), CultureInfo.InvariantCulture);
                    return (latitude, longitude);
                }

                return null; // No results
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching coordinates: {ex.Message}");
                return null;
            }
        }
        public static async Task<bool> ValidAddressAsync(string address)
        {
            var coordinates = await GetCoordinatesFromAddressAsync(address);
            return coordinates != null; // If a valid pair of coordinates is returned, the address is valid.
        }

        /// <summary>
        /// new fonction to check coordinations
        /// </summary>
        /// <summary>
        /// Retrieves the closest coordinates to the target latitude and longitude from an address asynchronously.
        /// </summary>
        /// <param name="address">The address to search for coordinates.</param>
        /// <param name="targetLatitude">The target latitude to compare against.</param>
        /// <param name="targetLongitude">The target longitude to compare against.</param>
        /// <returns>The closest coordinates as a tuple or null if none are found within the tolerance.</returns>
        public static async Task<(double latitude, double longitude)?> GetClosestCoordinatesAsync(string address, double targetLatitude, double targetLongitude)
        {
            if (string.IsNullOrWhiteSpace(address))
                return null;

            string apiKey = "pk.24d7295db243a75d8b3c688089250321";
            string url = $"https://api.locationiq.com/v1/search.php?key={apiKey}&q={WebUtility.UrlEncode(address)}&format=json";

            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetStringAsync(url);
                    var json = JsonDocument.Parse(response);
                    var root = json.RootElement;

                    if (root.GetArrayLength() > 0)
                    {
                        const double tolerance = 0.05; // Approximately 5 km tolerance
                        double closestDistance = double.MaxValue;
                        (double lat, double lon)? closestCoordinates = null;

                        foreach (var item in root.EnumerateArray())
                        {
                            double latitude = double.Parse(item.GetProperty("lat").GetString(), CultureInfo.InvariantCulture);
                            double longitude = double.Parse(item.GetProperty("lon").GetString(), CultureInfo.InvariantCulture);

                            // Calculate the actual distance between the points
                            double distance = CalculateHaversineDistance(latitude, longitude, targetLatitude, targetLongitude);

                            if (distance < closestDistance)
                            {
                                closestDistance = distance;
                                closestCoordinates = (latitude, longitude);
                            }
                        }

                        // If the closest distance is within the tolerance
                        if (closestDistance <= tolerance * 111.32) // Convert degrees to km approximately
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

        /// <summary>
        /// Calculates the distance between two geographic points. he calculate by the choice of the volunteer wich type of ditance he want
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <param name="distanceType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, DistanceType distanceType)
        {
            const double R = 6371; // Earth's radius in kilometers

            switch (distanceType)
            {
                case DistanceType.Plane:
                case DistanceType.Helicopter:
                case DistanceType.Drone:
                    // Straight-line distance (Haversine formula)
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2);

                case DistanceType.Foot:
                case DistanceType.HikingTrail:
                case DistanceType.UrbanShortcuts:
                    // Walking distance approximation (increase Haversine distance by 30% for paths)
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2) * 1.3;

                case DistanceType.Car:
                case DistanceType.Bus:
                case DistanceType.OffRoadVehicle:
                    // Driving distance approximation (increase Haversine by 50%)
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2) * 1.5;

                case DistanceType.Bike:
                case DistanceType.BicycleShare:
                case DistanceType.Scooter:
                    // Biking distance approximation (increase Haversine by 40%)
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2) * 1.4;

                case DistanceType.PublicTransport:
                case DistanceType.Train:
                case DistanceType.Waterway:
                    // Assume a detour factor of 1.7 for routes with public transport or waterways
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2) * 1.7;

                case DistanceType.Horse:
                case DistanceType.Ski:
                case DistanceType.Snowmobile:
                case DistanceType.Rollerblade:
                case DistanceType.Skateboard:
                    // Increase Haversine by 20% for terrain-based travel
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2) * 1.2;

                case DistanceType.Boat:
                    // Assume minimal detour for water travel
                    return CalculateHaversineDistance(lat1, lon1, lat2, lon2) * 1.1;

                default:
                    throw new ArgumentException($"Unsupported DistanceType: {distanceType}");
            }
        }
        private static double CalculateHaversineDistance(double lat1, double lon1, double lat2, double lon2)
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

        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
        }



        public static bool IsRequesterAuthorizedToCancel(int requesterId, int volunteerId)
        {
            if (requesterId == volunteerId)
                return true;
            lock (AdminManager.BlMutex)
            {
                var requester = s_dal.Volunteer.Read(requesterId);
                if (requester != null && requester.Role == DO.Role.Admin)
                    return true;
            }
                return false;
        }


        //public static void PeriodicVolunteersUpdates(DateTime oldClock, DateTime newClock, TimeSpan MaxRange)
        //{
        //    try
        //    {
        //        bool volunteerUpdated = false; // Indicates if any volunteer has been updated

        //        var volunteersFromDal = s_dal.Volunteer.ReadAll();
        //        var assignmentsFromDal = s_dal.Assignment.ReadAll();

        //        var volunteersInBO = volunteersFromDal.Select(v => new BO.VolunteerInList
        //        {
        //            Id = v.Id,
        //            Name = v.Name,
        //            IsActive = v.IsActive,
        //            CompletedAssignmentsCount = assignmentsFromDal.Count(call => call.VolunteerId == v.Id),
        //            CancelledCallsCount = assignmentsFromDal.Count(call => call.VolunteerId == v.Id),
        //            ExpiredCallsCount = assignmentsFromDal.Count(call => call.VolunteerId == v.Id),
        //            CurrentCallId = assignmentsFromDal.Where(call => call.VolunteerId == v.Id).Select(call => call.CallId).FirstOrDefault(),
        //            CurrentCallType = assignmentsFromDal.Where(call => call.VolunteerId == v.Id).Select(call =>
        //            {
        //                switch ((BO.EndType)call.EndType)
        //                {
        //                    case BO.EndType.Completed:
        //                        return BO.CallType.Completed;
        //                    case BO.EndType.SelfCanceled:
        //                        return BO.CallType.SelfCanceled;
        //                    case BO.EndType.Expired:
        //                        return BO.CallType.Expired;
        //                    case BO.EndType.AdminCanceled:
        //                        return BO.CallType.AdminCanceled;
        //                    default:
        //                        throw new InvalidOperationException("Unknown EndType");
        //                }
        //            }).FirstOrDefault()
        //        }).ToList();

        //        lock (AdminManager.blMutex) // Synchronization to avoid conflicts
        //        {
        //            foreach (var volunteer in volunteersInBO)
        //            {
        //                var volunteerAssignments = assignmentsFromDal
        //                    .Where(a => a.VolunteerId == volunteer.Id)
        //                    .OrderByDescending(a => a.StartTime)
        //                    .ToList();

        //                if (volunteerAssignments.Any())
        //                {
        //                    var latestAssignment = volunteerAssignments.First();

        //                    // Calculate the time difference
        //                    var timeDifference = AdminManager.Now - latestAssignment.StartTime;

        //                    if (timeDifference >= MaxRange)
        //                    {
        //                        volunteerUpdated = true;
        //                        var updatedVolunteer = s_dal.Volunteer.Read(volunteer.Id);

        //                        // Create a new object with updated IsActive property
        //                        var updatedVolunteerWithInactive = updatedVolunteer with { IsActive = false };
        //                        s_dal.Volunteer.Update(updatedVolunteerWithInactive);

        //                        VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);
        //                    }
        //                }
        //                else
        //                {
        //                    // Delete the volunteer if no assignments exist
        //                    s_dal.Volunteer.Delete(volunteer.Id);
        //                    VolunteerManager.Observers.NotifyItemUpdated(volunteer.Id);
        //                }
        //            }
        //        }

        //        // Check if the year has changed
        //        bool yearChanged = oldClock.Year != newClock.Year;

        //        if (yearChanged || volunteerUpdated)
        //        {
        //            VolunteerManager.Observers.NotifyListUpdated();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("An error occurred while updating volunteers.", ex);
        //    }
        //}





        ///simulator

        private static readonly Random s_random = new();

        /// <summary>
        /// Simulates the routine activities of volunteers, including assigning calls and managing call statuses.
        /// </summary>
        public static void SimulateVolunteerActivity(bool isSimulator)
        {
            Thread.CurrentThread.Name = $"Simulator{Guid.NewGuid()}";

            // Retrieve active volunteers as a concrete list
            List<DO.Volunteer> activeVolunteers;
            lock (AdminManager.BlMutex)
            {
                activeVolunteers = s_dal.Volunteer.ReadAll(v => v.IsActive).ToList();
            }

            LinkedList<int> updatedCallIds = new();

            foreach (var volunteer in activeVolunteers)
            {
                // Check if the volunteer has an active call
                var activeAssignment = GetActiveAssignmentForVolunteer(volunteer.Id);

                if (activeAssignment == null)
                {
                    // Volunteer has no active call; consider assigning a new call with a probability of 20%
                    if (s_random.NextDouble() < 0.2)
                    {
                        AssignRandomCallToVolunteer(volunteer.Id, updatedCallIds, isSimulator);
                    }
                }
                else
                {
                    // Volunteer has an active call
                    HandleActiveCall(volunteer, activeAssignment, updatedCallIds);
                }
            }

            // Notify observers outside of the lock
            foreach (int callId in updatedCallIds)
            {
                Observers.NotifyItemUpdated(callId);
            }
            Observers.NotifyListUpdated();
        }

        /// <summary>
        /// Retrieves the active assignment for a volunteer.
        /// </summary>
        private static DO.Assignment GetActiveAssignmentForVolunteer(int volunteerId)
        {
            lock (AdminManager.BlMutex)
            {
                return s_dal.Assignment.ReadAll()
                    .FirstOrDefault(a => a.VolunteerId == volunteerId && a.EndTime == null);
            }
        }

        /// <summary>
        /// Assigns a random call to a volunteer from the pool of available calls.
        /// </summary>
        private static void AssignRandomCallToVolunteer(int volunteerId, LinkedList<int> updatedCallIds, bool isSimulator)
        {
            // Retrieve open calls with valid coordinates
            List<DO.Call> openCalls;
            lock (AdminManager.BlMutex)
            {
                openCalls = s_dal.Call.ReadAll(c =>
                    (CallManager.GetStatusCall(c.Id) == BO.Status.Open || CallManager.GetStatusCall(c.Id) == BO.Status.OpenAtRisk) &&
                    c.Latitude != 0 && c.Longitude != 0).ToList();
            }

            if (openCalls.Count > 0)
            {
                // Select a random call
                var selectedCall = openCalls[s_random.Next(openCalls.Count)];

                try
                {
                    // Get an instance of CallImplementation (or use a dependency-injected version)
                    var callManager = new CallImplementation();

                    // Use selectionCall from CallImplementation
                    callManager.selectionCall(volunteerId, selectedCall.Id, isSimulator);

                    // Add the selected call ID to the updated list
                    updatedCallIds.AddLast(selectedCall.Id);
                }
                catch (Exception ex)
                {
                    // Handle errors during assignment (e.g., if the call is already assigned)
                    Console.WriteLine($"Error assigning call ID {selectedCall.Id} to volunteer ID {volunteerId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Handles an active call for a volunteer, determining if the call is completed or canceled.
        /// </summary>
        private static void HandleActiveCall(DO.Volunteer volunteer, DO.Assignment activeAssignment, LinkedList<int> updatedCallIds)
        {
            var call = s_dal.Call.Read(activeAssignment.CallId);

            // Determine if enough time has passed to complete the call
            double distance = CalculateDistance(
                volunteer.Latitude ?? 0, volunteer.Longitude ?? 0,
                call.Latitude, call.Longitude);

            TimeSpan estimatedDuration = TimeSpan.FromMinutes(distance / 5); // Assume 5 km/h travel
            estimatedDuration += TimeSpan.FromMinutes(s_random.Next(10, 30)); // Add random buffer time

            if (DateTime.Now - activeAssignment.StartTime >= estimatedDuration)
            {
                // Mark the call as completed
                lock (AdminManager.BlMutex)
                {
                    var updatedAssignment = activeAssignment with
                    {
                        EndTime = DateTime.Now,
                        EndType = DO.EndType.Completed
                    };
                    s_dal.Assignment.Update(updatedAssignment);


                    updatedCallIds.AddLast(call.Id);
                    Observers.NotifyItemUpdated(call.Id);
                }
            }
            else if (s_random.NextDouble() < 0.1) // 10% probability of canceling the call
            {
                lock (AdminManager.BlMutex)
                {
                    var updatedAssignment = activeAssignment with
                    {
                        EndTime = DateTime.Now,
                        EndType = DO.EndType.SelfCanceled
                    };
                    s_dal.Assignment.Update(updatedAssignment);


                    updatedCallIds.AddLast(call.Id);
                    Observers.NotifyItemUpdated(call.Id);
                }
            }

            Observers.NotifyListUpdated();
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
            if (string.IsNullOrEmpty(str) || str.Length % 4 != 0 || str.Length <= 20)
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