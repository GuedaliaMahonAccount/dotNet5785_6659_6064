using DalApi;
using System.Net;
using System.Text.Json;
using System.Net;
using System.Net.Mail;
using BO;

namespace Helpers
{
    internal static class CallManager
    {
        internal static ObserverManager Observers = new();
        private static IDal s_dal = Factory.Get;

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


        /// <summary>
        /// Retrieves coordinates from an address using LocationIQ API.
        /// 
        /// exemple of correct adress
        /// "display_name": "Tiltan, Ramla, Ramla Subdistrict, District centre, 7135275, Israël"
        /// "lat": "31.9290114"
        /// "lon": "34.8737578"
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
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

        /// <summary>
        /// function to check if he is autorized
        /// </summary>
        /// <param name="requesterId"></param>
        /// <param name="volunteerId"></param>
        /// <returns></returns>
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


        /// <summary>
        /// class to send email
        /// </summary>
        internal static class EmailService
        {
            private const string SmtpHost = "smtp.gmail.com"; 
            private const int SmtpPort = 587; 
            private const string SenderEmail = "guedalia.sebbah@gmail.com";
            private const string SenderPassword = "ujij qtrg kyrs cguv";
            public static void SendEmail(string recipientEmail, string subject, string body)
            {
                try
                {
                    using (var smtpClient = new SmtpClient(SmtpHost, SmtpPort))
                    {
                        smtpClient.Credentials = new NetworkCredential(SenderEmail, SenderPassword);
                        smtpClient.EnableSsl = true;

                        var mailMessage = new MailMessage
                        {
                            From = new MailAddress(SenderEmail),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true // Set to true if sending HTML emails
                        };

                        mailMessage.To.Add(recipientEmail);

                        smtpClient.Send(mailMessage);
                        Console.WriteLine($"Email sent to {recipientEmail} successfully.");
                    }
                }
                catch (SmtpException smtpEx)
                {
                    Console.WriteLine($"SMTP error while sending email to {recipientEmail}: {smtpEx.Message}");
                    Console.WriteLine($"Details: {smtpEx.StackTrace}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"General error while sending email to {recipientEmail}: {ex.Message}");
                    Console.WriteLine($"Details: {ex.StackTrace}");
                }
            }

        }
    }
}
