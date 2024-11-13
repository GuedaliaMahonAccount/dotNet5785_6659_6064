namespace DalTest;

using DalApi;
using DO;
using System;

public static class Initialization
{
    private static IVolunteer? s_dalVolunteer;
    private static ICall? s_dalCall;
    private static IAssignment? s_dalAssignment;
    private static IConfig? s_dalConfig;
    private static readonly Random s_rand = new();


    private static void CreateVolunteers()
    {
        string[] volunteerNames = {
        "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein",
        "Shira Israelof", "Yael Mizrahi", "Oren Shmuel", "Maya Katz",
        "Tomer Golan", "Lea Sharabi", "Moti Ben-David", "Yaakov Peretz",
        "Ruth Azulay", "Itzik Shalev", "Sara Bar", "Yonatan Ezra"
    };

        string[] volunteerEmails = {
        "danilevy123@gmail.com", "eliamar456@gmail.com", "yaircohen789@outlook.com", "arielalevin234@gmail.com",
        "dinaklein567@gmail.com", "shiraisraelof890@gmail.com", "yaelmizrahi123@outlook.com",
        "orenshmuel456@gmail.com", "mayakatz789@gmail.com", "tomergolan234@gmail.com", "leasharabi567@gmail.com",
        "motibendavid890@outlook.com", "yaakovperetz123@gmail.com", "ruthazulay456@gmail.com",
        "itzikshalev789@outlook.com", "sarabar234@walla.com", "yonatanezra567@gmail.com"
    };
        // Create a strong password
        string password = "";
        string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string lowerCase = "abcdefghijklmnopqrstuvwxyz";
        string digits = "0123456789";
        string allChars = upperCase + lowerCase + digits;


        List<string> phonePrefixes = new List<string> { "050", "053", "058", "052", "054" };
        Random random = new Random();

        for (int i = 0; i < volunteerNames.Length; i++)
        {
            string name = volunteerNames[i];
            string email = volunteerEmails[i];

            // Generate unique ID
            int id;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalVolunteer!.Read(id) != null);

            // Generate phone number
            string phone = $"{phonePrefixes[random.Next(phonePrefixes.Count)]}-{random.Next(1000000, 9999999)}";


            // Generate maximum distance for calls (randomly up to 100 km)
            int maxDistance = random.Next(1, 101);

            password += upperCase[random.Next(upperCase.Length)];   // Add one uppercase letter
            password += lowerCase[random.Next(lowerCase.Length)];  // Add one lowercase letter
            password += digits[random.Next(digits.Length)];       // Add one digit


            // Fill the rest of the password to make it at least 8 characters long
            for (int j = password.Length; j < 8; j++)
            {
                password += allChars[random.Next(allChars.Length)];
            }

            // Shuffle the password to ensure randomness
            password = new string(password.OrderBy(c => random.Next()).ToArray());

            // Create the volunteer and add to the database
            s_dalVolunteer!.Create(new Volunteer
            {
                Id = id,
                Name = name,
                Email = email,
                Phone = phone,
                MaxDistance = maxDistance,
                Password = password // Assuming Password is a property of the Volunteer class
            });

            Console.WriteLine($"Volunteer Created: {name}, {email}, {phone}, Max Distance: {maxDistance} km, Password: {password}");
        }
    }





    private static void CreateCalls()
    {
        string[] callTopics = { "Medical Assistance", "Food Delivery", "Emotional Support", "Shopping Help", "Technical Assistance" };
        foreach (var topic in callTopics)
        {
            int id;
            do
                id = s_rand.Next(1000, 9999);
            while (s_dalCall!.Read(id) != null);

            s_dalCall!.Create(new Call { Id = id, Topic = topic });
        }
    }

    private static void CreateAssignments()
    {
        var volunteerIds = s_dalVolunteer!.GetAllIds();
        var callIds = s_dalCall!.GetAllIds();

        foreach (var volunteerId in volunteerIds)
        {
            var callId = callIds[s_rand.Next(callIds.Count)];
            if (s_dalAssignment!.Read(volunteerId, callId) == null)
            {
                s_dalAssignment.Create(new Assignment { VolunteerId = volunteerId, CallId = callId });
            }
        }
    }

    public static void Do(IVolunteer? dalVolunteer, ICall? dalCall, IAssignment? dalAssignment, IConfig? dalConfig)
    {
        s_dalVolunteer = dalVolunteer ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalCall = dalCall ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalAssignment = dalAssignment ?? throw new NullReferenceException("DAL object can not be null!");
        s_dalConfig = dalConfig ?? throw new NullReferenceException("Config object can not be null!");

        Console.WriteLine("Resetting Configuration values and List values...");
        s_dalConfig.Reset();
        s_dalVolunteer.DeleteAll();
        s_dalCall.DeleteAll();
        s_dalAssignment.DeleteAll();

        Console.WriteLine("Initializing Volunteers list ...");
        CreateVolunteers();

        Console.WriteLine("Initializing Calls list ...");
        CreateCalls();

        Console.WriteLine("Initializing Assignments list ...");
        CreateAssignments();
    }
}
