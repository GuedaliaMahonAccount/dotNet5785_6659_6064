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
        string[] descriptions = new[]
{
    "Preparing nutritious meals for soldiers stationed at remote bases.",
    "Delivering fresh food and supplies to soldiers on active duty.",
    "Collecting uniforms and other clothing items from soldiers for cleaning.",
    "Organizing and washing laundry for soldiers in the field.",
    "Providing transportation for soldiers moving between training sites.",
    "Repairing essential equipment used by soldiers in field operations.",
    "Supplying soldiers with needed clothing and protective gear.",
    "Helping set up temporary shelters for soldiers during training exercises.",
    "Cleaning facilities used by soldiers to maintain hygiene and morale.",
    "Assisting with basic medical care and first aid for injured soldiers.",
    "Providing mental health support for soldiers dealing with stress.",
    "Offering training in first aid and basic survival skills to new recruits.",
    "Organizing recreational activities to boost soldier morale.",
    "Preparing hot drinks and snacks for soldiers on cold night shifts.",
    "Collecting food and clothing donations from the community for soldiers.",
    "Performing vehicle maintenance for transport vehicles used by soldiers.",
    "Tutoring soldiers in subjects they need help with for future studies.",
    "Distributing hygiene kits including soap, shampoo, and other essentials.",
    "Helping coordinate logistical support for upcoming training exercises.",
    "Setting up basic technology and communication equipment for soldiers.",
    "Translating important documents for soldiers in bilingual units.",
    "Assisting soldiers with completing essential paperwork and filing forms.",
    "Responding to urgent calls for transportation of supplies or soldiers.",
    "Delivering personalized care packages from families to the front lines.",
    "Preparing meals specifically tailored to meet dietary needs of soldiers.",
    "Transporting heavy equipment needed for soldier training operations.",
    "Collecting and distributing warm clothing for soldiers in colder areas.",
    "Assisting in setting up recreation areas for downtime between exercises.",
    "Offering counseling services for soldiers dealing with homesickness.",
    "Planning group activities to help soldiers relax and build camaraderie.",
    "Delivering hot drinks to soldiers on long patrols in winter weather.",
    "Organizing community drives to gather essential supplies for soldiers.",
    "Assisting with maintenance of field equipment essential for missions.",
    "Providing mentorship to young soldiers adjusting to military life.",
    "Supplying individual hygiene kits to soldiers returning from exercises.",
    "Coordinating supply logistics for a large training mission.",
    "Assisting soldiers with technical issues in their communication gear.",
    "Translating training materials for soldiers in multilingual units.",
    "Helping soldiers complete required paperwork after basic training.",
    "Responding to emergency calls for urgent medical supplies.",
    "Distributing care packages from local organizations to active soldiers.",
    "Preparing food packages for quick delivery to soldiers on night shifts.",
    "Transporting medical equipment to field locations where soldiers train.",
    "Repairing damaged uniforms and gear for soldiers in remote areas.",
    "Setting up sleeping areas in temporary camps for training exercises.",
    "Sanitizing and organizing common areas in field bases.",
    "Providing basic first aid support to soldiers in rugged environments.",
    "Helping soldiers practice self-care with hygiene products.",
    "Preparing and delivering nutritious meals for soldiers on duty.",
    "Delivering donated blankets and winter gear for soldiers in cold areas.",
    "Organizing group therapy sessions to support soldier well-being."
};
        CallType[] callTypes = new[]
{
    CallType.PrepareFood,               // Preparing nutritious meals for soldiers stationed at remote bases.
    CallType.DeliverSupplies,           // Delivering fresh food and supplies to soldiers on active duty.
    CallType.CollectLaundry,            // Collecting uniforms and other clothing items from soldiers for cleaning.
    CallType.WashLaundry,               // Organizing and washing laundry for soldiers in the field.
    CallType.Transport,                 // Providing transportation for soldiers moving between training sites.
    CallType.RepairEquipment,           // Repairing essential equipment used by soldiers in field operations.
    CallType.ProvideClothing,           // Supplying soldiers with needed clothing and protective gear.
    CallType.SetupCamp,                 // Helping set up temporary shelters for soldiers during training exercises.
    CallType.CleanFacilities,           // Cleaning facilities used by soldiers to maintain hygiene and morale.
    CallType.ProvideMedicalAid,         // Assisting with basic medical care and first aid for injured soldiers.
    CallType.MentalHealthSupport,       // Providing mental health support for soldiers dealing with stress.
    CallType.OfferTraining,             // Offering training in first aid and basic survival skills to new recruits.
    CallType.OrganizeRecreationalEvent, // Organizing recreational activities to boost soldier morale.
    CallType.PrepareHotDrinks,          // Preparing hot drinks and snacks for soldiers on cold night shifts.
    CallType.CollectDonations,          // Collecting food and clothing donations from the community for soldiers.
    CallType.PerformVehicleMaintenance, // Performing vehicle maintenance for transport vehicles used by soldiers.
    CallType.TutorOrMentor,             // Tutoring soldiers in subjects they need help with for future studies.
    CallType.SupplyHygieneKits,         // Distributing hygiene kits including soap, shampoo, and other essentials.
    CallType.OrganizeLogistics,         // Helping coordinate logistical support for upcoming training exercises.
    CallType.ProvideITSupport,          // Setting up basic technology and communication equipment for soldiers.
    CallType.TranslateDocuments,        // Translating important documents for soldiers in bilingual units.
    CallType.AssistWithPaperwork,       // Assisting soldiers with completing essential paperwork and filing forms.
    CallType.EmergencyResponse,         // Responding to urgent calls for transportation of supplies or soldiers.
    CallType.DistributeCarePackages,    // Delivering personalized care packages from families to the front lines.
    CallType.PrepareFood,               // Preparing meals specifically tailored to meet dietary needs of soldiers.
    CallType.Transport,                 // Transporting heavy equipment needed for soldier training operations.
    CallType.ProvideClothing,           // Collecting and distributing warm clothing for soldiers in colder areas.
    CallType.OrganizeRecreationalEvent, // Assisting in setting up recreation areas for downtime between exercises.
    CallType.MentalHealthSupport,       // Offering counseling services for soldiers dealing with homesickness.
    CallType.OrganizeRecreationalEvent, // Planning group activities to help soldiers relax and build camaraderie.
    CallType.PrepareHotDrinks,          // Delivering hot drinks to soldiers on long patrols in winter weather.
    CallType.CollectDonations,          // Organizing community drives to gather essential supplies for soldiers.
    CallType.PerformVehicleMaintenance, // Assisting with maintenance of field equipment essential for missions.
    CallType.TutorOrMentor,             // Providing mentorship to young soldiers adjusting to military life.
    CallType.SupplyHygieneKits,         // Supplying individual hygiene kits to soldiers returning from exercises.
    CallType.OrganizeLogistics,         // Coordinating supply logistics for a large training mission.
    CallType.ProvideITSupport,          // Assisting soldiers with technical issues in their communication gear.
    CallType.TranslateDocuments,        // Translating training materials for soldiers in multilingual units.
    CallType.AssistWithPaperwork,       // Helping soldiers complete required paperwork after basic training.
    CallType.EmergencyResponse,         // Responding to emergency calls for urgent medical supplies.
    CallType.DistributeCarePackages,    // Distributing care packages from local organizations to active soldiers.
    CallType.PrepareFood,               // Preparing food packages for quick delivery to soldiers on night shifts.
    CallType.Transport,                 // Transporting medical equipment to field locations where soldiers train.
    CallType.ProvideClothing,           // Repairing damaged uniforms and gear for soldiers in remote areas.
    CallType.SetupCamp,                 // Setting up sleeping areas in temporary camps for training exercises.
    CallType.CleanFacilities,           // Sanitizing and organizing common areas in field bases.
    CallType.ProvideMedicalAid,         // Providing basic first aid support to soldiers in rugged environments.
    CallType.SupplyHygieneKits,         // Helping soldiers practice self-care with hygiene products.
    CallType.PrepareFood,               // Preparing and delivering nutritious meals for soldiers on duty.
    CallType.DeliverSupplies,           // Delivering donated blankets and winter gear for soldiers in cold areas.
    CallType.OrganizeRecreationalEvent, // Organizing group therapy sessions to support soldier well-being.
};
        string[] addresses = new[]
        {
    "Ino Shaki 6",
    "Perets Bernstein 2",
    "Ben Yehuda 13",
    "Dizengoff 99",
    "Herzl 45",
    "Weizmann 17",
    "Jabotinsky 35",
    "King George 20",
    "Rothschild 16",
    "Aluf David Elazar 10",
    "Sokolov 22",
    "Yafo 32",
    "Allenby 72",
    "Hillel Yaffe 3",
    "Rabbi Akiva 5",
    "Ben Gurion 50",
    "Yigal Alon 98",
    "Keren Hayesod 21",
    "Hertzel 8",
    "Shaul Hamelech 22",
    "Menachem Begin 6",
    "Nahalat Binyamin 25",
    "Bialik 15",
    "Sderot Yerushalayim 10",
    "Shenkar 12",
    "David HaMelech 7",
    "Moshe Dayan 33",
    "Hankin 40",
    "Balfour 28",
    "Levi Eshkol 4",
    "Herzl 15",
    "Hagolan 12",
    "HaPalmach 2",
    "HaHistadrut 20",
    "HaYarkon 200",
    "Ruppin 19",
    "HaAliyah 11",
    "Neot Peres 7",
    "HaMasger 17",
    "Mish'ol HaNesharim 5",
    "Rabbi Meir 3",
    "Herzl 89",
    "Pinsker 10",
    "HaShomer 3",
    "Nili 8",
    "Sderot Rothschild 9",
    "Trumpeldor 7",
    "Bergen Street 4",
    "Even Gvirol 19",
    "Giora Yoseftal 2",
    "Ben Tzvi 5"
};






        foreach (var topic in descriptions)
        {


            s_dalCall!.Create(new Call { });
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
