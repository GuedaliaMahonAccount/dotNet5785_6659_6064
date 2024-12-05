using DalApi;
using System;
using System.IO;
using System.Net;
using System.Text.Json;

namespace Helpers
{
    internal static class CallManager
    {
        private static IDal s_dal = Factory.Get;

        /// <summary>
        /// Calculates the distance between two geographic points.
        /// </summary>
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
    }
}
