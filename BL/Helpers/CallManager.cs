using DalApi;
using System.Net;
using System.Text.Json;
using BO;

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



        /// <summary>
        /// Updates all open calls whose deadlines have passed and closes them with the status "Expired".
        /// </summary>
        public static void UpdateExpiredCalls()
        {
            var systemTime = DateTime.Now;

            // Retrieve all calls from the DAL (assuming DO.Call objects)
            var calls = s_dal.Call.ReadAll();

            // Convert DO.Call objects to BO.Call (assuming a conversion helper exists)
            var boCalls = calls.Select(call => ConvertToBOCall(call)).ToList();

            // Iterate through calls whose deadline has passed
            foreach (var call in boCalls)
            {
                if (call.DeadLine.HasValue && call.DeadLine.Value < systemTime && !IsCallClosed(call))
                {
                    // Check if call has no assignment
                    if (call.Assignments == null || !call.Assignments.Any())
                    {
                        // Add a new assignment with "Expired Cancellation"
                        var newAssignment = new BO.CallAssignInList
                        {
                            VolunteerId = null,
                            VolunteerName = null,
                            StartTime = DateTime.MinValue,
                            EndTime = systemTime,
                            EndType = EndType.Expired // Assuming an enum value
                        };

                        call.Assignments?.Add(newAssignment);
                    }
                    else
                    {
                        // Update the last assignment if EndTime is null
                        var openAssignment = call.Assignments.LastOrDefault(a => a.EndTime == null);
                        var updatedAssignment = new BO.CallAssignInList
                        {
                            VolunteerId = openAssignment.VolunteerId,
                            VolunteerName = openAssignment.VolunteerName,
                            StartTime = openAssignment.StartTime,
                            EndTime = systemTime,
                            EndType = EndType.Expired
                        };

                        // Replace the existing assignment in the list
                        var index = call.Assignments.IndexOf(openAssignment);
                        if (index >= 0)
                        {
                            call.Assignments[index] = updatedAssignment;
                        }

                    }

                    // Convert BO.Call back to DO.Call
                    var updatedDOCall = ConvertToDOCall(call);

                    // Update the call in the DAL
                    s_dal.Call.Update(updatedDOCall);
                }
            }
        }

        /// <summary>
        /// Helper method to determine if a call is already closed.
        /// </summary>
        /// <param name="call">The call to check.</param>
        /// <returns>True if the call is closed, otherwise false.</returns>
        private static bool IsCallClosed(BO.Call call)
        {
            return call.Assignments != null && call.Assignments.Any(a => a.EndTime != null && a.EndType != null);
        }

        /// <summary>
        /// Converts a DO.Call object to a BO.Call object.
        /// </summary>
        private static BO.Call ConvertToBOCall(DO.Call doCall)
        {
            return new BO.Call
            {
                Id = doCall.Id,
                CallType = (BO.CallType)doCall.CallType,
                Address = doCall.Address,
                Latitude = doCall.Latitude,
                Longitude = doCall.Longitude,
                StartTime = doCall.StartTime,
                Description = doCall.Description,
                DeadLine = doCall.DeadLine,
                Assignments = new List<BO.CallAssignInList>() // Convert assignments if needed
            };
        }

        /// <summary>
        /// Converts a BO.Call object to a DO.Call object.
        /// </summary>
        private static DO.Call ConvertToDOCall(BO.Call boCall)
        {
            return new DO.Call
            {
                Id = boCall.Id,
                CallType = (DO.CallType)boCall.CallType,
                Address = boCall.Address,
                Latitude = boCall.Latitude ?? 0, // Assuming default value
                Longitude = boCall.Longitude ?? 0, // Assuming default value
                StartTime = boCall.StartTime,
                Description = boCall.Description,
                DeadLine = boCall.DeadLine
            };
        }

    }
}
