namespace DalTest
{
    using DalApi;
    using DO;
    using System;
    using System.Linq;
    using System.Collections.Generic;

    public static class Initialization
    {
        private static IDal? s_dal;
        private static readonly Random s_rand = new();

        private static void CreateVolunteers()
        {
            // Volunteer details
            string[] volunteerNames = {
                "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein",
                "Shira Israelof", "Yael Mizrahi", "Oren Shmuel", "Maya Katz",
                "Tomer Golan", "Lea Sharabi", "Moti Ben-David", "Yaakov Peretz",
                "Ruth Azulay", "Itzik Shalev", "Sara Bar", "Yonatan Ezra",
                "Noam Mizrahi", "Yossi Tal", "Dana Asor", "Rina Bar", "Gal Yaakov",
                "Nave Shaked", "Amit Cohen", "Michal Dahary", "Itamar Ben-Ami", "Adi Raz"
            };
            string[] volunteerEmails = {
                // Add volunteer emails here
            };
            string[] addresses = {
                // Add addresses here
            };
            double[] latitudes = { /* Latitude values */ };
            double[] longitudes = { /* Longitude values */ };
            string[] strongPasswords = { /* Passwords */ };
            List<string> phonePrefixes = new List<string> { "050", "053", "058", "052", "054" };

            for (int i = 0; i < volunteerNames.Length; i++)
            {
                string password = strongPasswords[i % strongPasswords.Length];
                int id;
                do
                {
                    id = s_rand.Next(200000000, 400000000);
                } while (s_dal!.Volunteer.Read(id) != null);

                string phone = $"{phonePrefixes[s_rand.Next(phonePrefixes.Count)]}-{s_rand.Next(1000000, 9999999)}";
                double maxDistance = s_rand.Next(1, 101);
                Role role = (i == 0) ? Role.Admin : Role.GeneralVolunteer;
                bool isActive = (i != volunteerNames.Length - 1);
                DistanceType distanceType = DistanceType.Plane;

                var volunteer = new Volunteer
                (
                    Id: id,
                    Name: volunteerNames[i],
                    Email: volunteerEmails[i],
                    Phone: phone,
                    Role: role,
                    IsActive: isActive,
                    MaxDistance: maxDistance,
                    DistanceType: distanceType,
                    Password: password,
                    Address: addresses.Length > i ? addresses[i] : null,
                    Latitude: latitudes.Length > i ? latitudes[i] : null,
                    Longitude: longitudes.Length > i ? longitudes[i] : null
                );

                s_dal.Volunteer.Create(volunteer);
                Console.WriteLine($"Volunteer Created: {volunteer.Name}");
            }
        }

        private static void CreateCalls()
        {
            string[] descriptions = { /* Call descriptions */ };
            CallType[] callTypes = new CallType[descriptions.Length];
            for (int i = 0; i < callTypes.Length; i++)
            {
                callTypes[i] = (CallType)s_rand.Next(Enum.GetValues(typeof(CallType)).Length);
            }
            string[] addresses = { /* Addresses */ };
            double[] latitudes = { /* Latitude values */ };
            double[] longitudes = { /* Longitude values */ };

            DateTime start = new DateTime(s_dal!.Config.Clock.Year - 2, 1, 1);

            for (int i = 0; i < descriptions.Length; i++)
            {
                var startTime = start.AddDays(-s_rand.Next(1, 30));
                DateTime? deadline = s_rand.NextDouble() < 0.5
                    ? (DateTime?)startTime.AddHours(s_rand.Next(1, 72))
                    : null;

                int callId = s_dal.Config.NextCallId;

                var call = new Call
                (
                    Id: callId,
                    CallType: callTypes[i],
                    Address: addresses[i],
                    Latitude: latitudes[i],
                    Longitude: longitudes[i],
                    StartTime: startTime,
                    Description: descriptions[i],
                    DeadLine: deadline
                );

                s_dal.Call.Create(call);
            }
        }

        private static void CreateAssignments()
        {
            // Use IEnumerable instead of List
            IEnumerable<Volunteer> volunteers = s_dal!.Volunteer.ReadAll();
            IEnumerable<Call> calls = s_dal.Call.ReadAll();

            int assignmentId = s_dal.Config.NextAssignmentId;

            // Shuffle volunteers using a temporary array
            Volunteer[] shuffledVolunteers = volunteers.OrderBy(_ => s_rand.Next()).ToArray();
            int totalVolunteers = shuffledVolunteers.Length;

            // Divide volunteers into categories
            int noAssignmentCount = Math.Max(1, totalVolunteers / 5);
            int singleAssignmentCount = Math.Max(1, totalVolunteers / 3);
            int multipleAssignmentCount = totalVolunteers - noAssignmentCount - singleAssignmentCount;

            // Use an enumerator to iterate through calls
            using var callEnumerator = calls.GetEnumerator();
            bool hasMoreCalls = callEnumerator.MoveNext();

            for (int i = 0; i < totalVolunteers; i++)
            {
                Volunteer volunteer = shuffledVolunteers[i];
                int assignmentsForThisVolunteer;

                // Assign no tasks to some volunteers
                if (i < noAssignmentCount) continue;
                // Assign one task to some volunteers
                else if (i < noAssignmentCount + singleAssignmentCount) assignmentsForThisVolunteer = 1;
                // Assign multiple tasks to the remaining volunteers
                else assignmentsForThisVolunteer = s_rand.Next(2, 5);

                for (int j = 0; j < assignmentsForThisVolunteer && hasMoreCalls; j++)
                {
                    // Get the current call
                    Call call = callEnumerator.Current;
                    hasMoreCalls = callEnumerator.MoveNext();

                    DateTime startTime = call.StartTime.AddHours(s_rand.Next(1, 24));
                    DateTime? endTime = null;
                    EndType? endType = null;

                    double outcomeChance = s_rand.NextDouble();

                    // Determine the outcome of the assignment
                    if (outcomeChance < 0.3)
                    {
                        endTime = call.DeadLine.HasValue
                            ? call.StartTime.AddHours(s_rand.Next(1, (int)(call.DeadLine.Value - startTime).TotalHours + 1))
                            : startTime.AddHours(s_rand.Next(1, 72));
                        endType = EndType.Completed;
                    }
                    else if (outcomeChance < 0.5)
                    {
                        endTime = startTime.AddHours(s_rand.Next(1, 48));
                        endType = EndType.SelfCanceled;
                    }
                    else if (outcomeChance < 0.7)
                    {
                        endTime = startTime.AddHours(s_rand.Next(1, 48));
                        endType = EndType.AdminCanceled;
                    }
                    else
                    {
                        endType = EndType.Expired;
                    }

                    // Create the assignment
                    var assignment = new Assignment
                    (
                        Id: assignmentId++,
                        CallId: call.Id,
                        VolunteerId: volunteer.Id,
                        StartTime: startTime,
                        EndTime: endTime,
                        EndType: endType
                    );

                    // Save the assignment in the database
                    s_dal.Assignment.Create(assignment);
                }
            }
        }

        public static void Do(IDal dal)
        {
            s_dal = dal;
            Console.WriteLine("Reset Configuration values and List values...");
            s_dal.ResetDB();
            CreateVolunteers();
            CreateCalls();
            CreateAssignments();
        }
    }
}
