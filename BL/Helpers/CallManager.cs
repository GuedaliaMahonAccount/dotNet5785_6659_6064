using DalApi;
using System.Net;
using System.Text.Json;
using System.Net.Mail;
using BO;
using System.Globalization;

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
        /// "display_name": "מרכז אורן יצחק רגר, 185, Beer Sheva"
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
        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371; // Rayon de la Terre en km
            var dLat = ToRad(lat2 - lat1);
            var dLon = ToRad(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }
        private static double ToRad(double degrees)
        {
            return degrees * (Math.PI / 180);
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
            lock (AdminManager.BlMutex)
            {
                var requester = s_dal.Volunteer.Read(requesterId);
                if (requester != null && requester.Role == DO.Role.Admin)
                    return true;
            }
            return false;

        }



        /// <summary>
        /// Updates all open calls whose deadlines have passed and closes them with the status "Expired".
        /// </summary>
        public static void UpdateExpiredCalls()
        {
            DateTime systemTime = DateTime.Now;

            // Fetch all calls and convert to a concrete list to avoid deferred execution
            List<DO.Call> callList;
            lock (AdminManager.BlMutex)
            {
                callList = s_dal.Call.ReadAll().ToList();
            }

            var updatedCallIds = new List<int>(); // Collect updated call IDs for notifications

            foreach (var call in callList)
            {
                bool callUpdated = false;

                // Convert DO.Call to BO.Call for processing
                BO.Call boCall = ConvertToBOCall(call);

                if (boCall.DeadLine.HasValue && boCall.DeadLine.Value < systemTime && !IsCallClosed(boCall))
                {
                    if (boCall.Assignments == null || !boCall.Assignments.Any())
                    {
                        // Create a new assignment with "Expired Cancellation"
                        var newAssignment = new BO.CallAssignInList
                        {
                            VolunteerId = 0, // No volunteer
                            VolunteerName = "System",
                            StartTime = DateTime.MinValue,
                            EndTime = systemTime,
                            EndType = EndType.Expired
                        };

                        boCall.Assignments ??= new List<BO.CallAssignInList>();
                        boCall.Assignments.Add(newAssignment);
                        callUpdated = true;
                    }
                    else
                    {
                        var openAssignment = boCall.Assignments.LastOrDefault(a => a.EndTime == null);
                        if (openAssignment != null)
                        {
                            var updatedAssignment = new BO.CallAssignInList
                            {
                                VolunteerId = openAssignment.VolunteerId,
                                VolunteerName = openAssignment.VolunteerName,
                                StartTime = openAssignment.StartTime,
                                EndTime = systemTime,
                                EndType = EndType.Expired
                            };

                            int index = boCall.Assignments.IndexOf(openAssignment);
                            if (index >= 0)
                            {
                                boCall.Assignments[index] = updatedAssignment;
                                callUpdated = true;
                            }
                        }
                    }

                    if (callUpdated)
                    {
                        // Update the call in the DAL
                        DO.Call updatedDOCall = ConvertToDOCall(boCall);
                        lock (AdminManager.BlMutex)
                        {
                            s_dal.Call.Update(updatedDOCall);
                        }

                        updatedCallIds.Add(call.Id);
                    }
                }
            }

            // Notify observers outside the lock
            foreach (int callId in updatedCallIds)
            {
                Observers.NotifyItemUpdated(callId);
            }
            Observers.NotifyListUpdated();
        }
        /// <summary>
        /// Updates the call type of all open calls whose deadlines are within the specified risk range.
        ///</summary>
        public static void UpdateRiskCall(DateTime oldClock, DateTime newClock, TimeSpan riskRange)
        {
            // Fetch all calls as a concrete list to avoid deferred execution
            List<DO.Call> callList;
            lock (AdminManager.BlMutex)
            {
                callList = s_dal.Call.ReadAll().ToList();
            }

            var updatedCallIds = new List<int>(); // Collect updated call IDs for notifications

            foreach (var call in callList)
            {
                if (call.DeadLine.HasValue)
                {
                    var timeLeft = call.DeadLine.Value - newClock;

                    DO.Call updatedCall = null;

                    if (timeLeft <= riskRange)
                    {
                        // Update calls to risk status if within range
                        switch (call.CallType)
                        {
                            case DO.CallType.Open:
                                updatedCall = call with { CallType = DO.CallType.OpenAtRisk };
                                break;
                            case DO.CallType.InTreatment:
                                updatedCall = call with { CallType = DO.CallType.InTreatmentAtRisk };
                                break;
                        }
                    }
                    else
                    {
                        // Revert calls to non-risk status if out of range
                        switch (call.CallType)
                        {
                            case DO.CallType.OpenAtRisk:
                                updatedCall = call with { CallType = DO.CallType.Open };
                                break;
                            case DO.CallType.InTreatmentAtRisk:
                                updatedCall = call with { CallType = DO.CallType.InTreatment };
                                break;
                        }
                    }

                    if (updatedCall != null)
                    {
                        lock (AdminManager.BlMutex)
                        {
                            s_dal.Call.Update(updatedCall);
                        }

                        updatedCallIds.Add(call.Id);
                    }
                }
            }

            // Notify observers outside the lock
            foreach (int callId in updatedCallIds)
            {
                Observers.NotifyItemUpdated(callId);
            }
            Observers.NotifyListUpdated();
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

        /// <summary>
        /// Notifies nearby volunteers of a new call.
        /// </summary>
        /// <param name="newCall">The new call to notify volunteers about.</param>
        /// <param name="radiusInKm">The radius within which volunteers will be notified.</param>
        public static void NotifyNearbyVolunteers(BO.Call newCall, double radiusInKm = 5.0)
        {
            lock (AdminManager.BlMutex)
            {
                // Retrieve all volunteers
                var volunteers = s_dal.Volunteer.ReadAll();

                // Filter volunteers within the specified radius
                var nearbyVolunteers = volunteers.Where(volunteer =>
                {
                    if (volunteer.Latitude.HasValue && volunteer.Longitude.HasValue)
                    {
                        double distance = CalculateDistance(
                            newCall.Latitude.Value, newCall.Longitude.Value,
                            volunteer.Latitude.Value, volunteer.Longitude.Value,
                            (BO.DistanceType)volunteer.DistanceType);
                        return distance <= radiusInKm;
                    }
                    return false;
                });

                // Send notification email to each nearby volunteer
                foreach (var volunteer in nearbyVolunteers)
                {
                    if (!string.IsNullOrWhiteSpace(volunteer.Email))
                    {
                        string subject = "New Call Alert: Assistance Needed!";
                        string body = $@"
                        <p>Dear {volunteer.Name},</p>
                        <p>A new call for assistance has been added in your area:</p>
                        <ul>
                            <li><strong>Address:</strong> {newCall.Address}</li>
                            <li><strong>Description:</strong> {newCall.Description}</li>
                            <li><strong>Deadline:</strong> {newCall.DeadLine?.ToString("f") ?? "N/A"}</li>
                        </ul>
                        <p>If you can help, please log in to the system and volunteer for the call.</p>
                        <p>Thank you for your support!</p>";

                        EmailService.SendEmail(volunteer.Email, subject, body);
                    }
                }
            }
        }






        ///simulator

        private static readonly Random s_random = new();

        /// <summary>
        /// Simulates the routine activities of volunteers, including assigning calls and managing call statuses.
        /// </summary>
        public static void SimulateVolunteerActivity()
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
                        AssignRandomCallToVolunteer(volunteer.Id, updatedCallIds);
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
        private static void AssignRandomCallToVolunteer(int volunteerId, LinkedList<int> updatedCallIds)
        {
            // Retrieve open calls with valid coordinates
            List<DO.Call> openCalls;
            lock (AdminManager.BlMutex)
            {
                // Remove the HasValue check since Latitude and Longitude are non-nullable
                openCalls = s_dal.Call.ReadAll(c =>
                    (c.CallType == DO.CallType.Open || c.CallType == DO.CallType.OpenAtRisk) &&
                    c.Latitude != 0 && c.Longitude != 0).ToList();

            }

            if (openCalls.Count > 0)
            {
                // Select a random call
                var selectedCall = openCalls[s_random.Next(openCalls.Count)];

                lock (AdminManager.BlMutex)
                {
                    // Create an assignment for the volunteer
                    var newAssignment = new DO.Assignment
                    (
                        Id: s_dal.Config.NextAssignmentId,
                        CallId: selectedCall.Id,
                        VolunteerId: volunteerId,
                        StartTime: DateTime.Now,
                        EndTime: null,
                        EndType: null
                    );
                    s_dal.Assignment.Create(newAssignment);

                    // Update the call status
                    var updatedCallType = selectedCall.CallType == DO.CallType.OpenAtRisk
                        ? DO.CallType.InTreatmentAtRisk
                        : DO.CallType.InTreatment;
                    var updatedCall = selectedCall with { CallType = updatedCallType };
                    s_dal.Call.Update(updatedCall);

                    updatedCallIds.AddLast(selectedCall.Id);
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

                    var updatedCall = call with { CallType = DO.CallType.Completed };
                    s_dal.Call.Update(updatedCall);

                    updatedCallIds.AddLast(call.Id);
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

                    var updatedCall = call with { CallType = DO.CallType.Open };
                    s_dal.Call.Update(updatedCall);

                    updatedCallIds.AddLast(call.Id);
                }
            }
        }

    }
}
