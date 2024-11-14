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
        // Volunteer details
        string[] volunteerNames = {
        "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein",
        "Shira Israelof", "Yael Mizrahi", "Oren Shmuel", "Maya Katz",
        "Tomer Golan", "Lea Sharabi", "Moti Ben-David", "Yaakov Peretz",
        "Ruth Azulay", "Itzik Shalev", "Sara Bar", "Yonatan Ezra",
        "Noam Mizrahi","Yossi Tal", "Dana Asor","Rina Bar","Gal Yaakov",
        "Nave Shaked","Amit Cohen","Michal Dahary","Itamar Ben-Ami","Adi Raz"
    };
        string[] volunteerEmails = {
        "danilevy123@gmail.com", "eliamar456@gmail.com", "yaircohen789@outlook.com", "arielalevin234@gmail.com",
        "dinaklein567@gmail.com", "shiraisraelof890@gmail.com", "yaelmizrahi123@outlook.com",
        "orenshmuel456@gmail.com", "mayakatz789@gmail.com", "tomergolan234@gmail.com", "leasharabi567@gmail.com",
        "motibendavid890@outlook.com", "yaakovperetz123@gmail.com", "ruthazulay456@gmail.com",
        "itzikshalev789@outlook.com", "sarabar234@walla.com", "yonatanezra567@gmail.com","NoamM@gmail.com",
        "Yossi12@gmail.com","DanaAsor1@gmail.com","RinaB12@gmail.com","GalYaakov22@gmail.com",
        "NaveS11@walla.com","AmitCohen1@outlook.com","MichalDA@gmail.com","ItamarBenAMI@outlook.com","AdiRaz1@gmail.com"
    };
        string[] addresses = {
        "גרנדר קניון, David Tuviyahu Ave 125, Be'er Sheva",
        "דרך אליהו נאוי 28, Beersheba",
        "מתחם תחנת הדלק אלון, 31, Arad",
        "Mivtsa Nakhshon Street 60, Be'er Sheva",
        "Yitzhack I. Rager Boulevard 24, Be'er Sheva",
        "Tsvi Bornstein Street 312, Yeruham",
        "איזור תעשייה, הפועלים 5, דימונה",
        "Herzl Street 43, Be'er Sheva",
        "Derekh Hebron 21, Beersheba",
        "Derekh Hebron 4, Beersheba",
        "Kornmehl Farm M.E. Ramat Negev D.N. Halutza",
        "רסקו סנטר, HaTikva 8, Beersheba",
        "40, Tlalim",
        "קניון פרץ סנטר, דימונה",
        "פרץ סנטר - אזור תעשיה דרומי, Dimona"
    };
        double[] latitudes = {
        31.2506405f, 31.2467805f, 31.2497676f, 31.2605014f, 31.2473633f,
        30.988506f, 31.062823f, 31.2396368f, 31.2391961f, 31.2377661f,
        30.972753f, 31.2474244f, 30.9935689f, 31.0599323f, 31.0599323f
    };
        double[] longitudes = {
        34.7716625f, 34.8156994f, 35.1914546f, 34.7873239f, 34.7978134f,
        34.927951f, 35.0192015f, 34.7877478f, 34.7967932f, 34.7944248f,
        34.7755085f, 34.79862f, 34.7643578f, 35.0203137f, 35.0203137f
    };
        string[] strongPasswords = {
        "A3b9Kp5vL1", "Z8mQ7xW4rB", "L2dR9yC8zN", "J1fV6kH3tP", "B5gY4nM7jQ",
        "N6xP8cL2mV", "T3lH7wQ5rZ", "Y9kJ4bM8xL", "R7dF2yW6nP", "X3bZ9mQ5jT",
        "M1pV8yN6xL", "G7kJ2fT9mB", "H5qZ4pL7nV", "D9yX6kB3rQ", "C1mP7vJ8wZ"
    };

        List<string> phonePrefixes = new List<string> { "050", "053", "058", "052", "054" };

        for (int i = 0; i < volunteerNames.Length; i++)
        {
            // Assign each volunteer a unique strong password from the array
            string password = strongPasswords[i % strongPasswords.Length];

            // Generate unique ID for each volunteer
            int id;
            do
            {
                id = s_rand.Next(200000000, 400000000);
            } while (s_dalVolunteer!.Read(id) != null);

            // Generate random phone number and max distance
            string phone = $"{phonePrefixes[s_rand.Next(phonePrefixes.Count)]}-{s_rand.Next(1000000, 9999999)}";
            double maxDistance = s_rand.Next(1, 101);

            // Set role and active status
            Role role = (i == 0) ? Role.Admin : Role.GeneralVolunteer;
            bool isActive = (i != volunteerNames.Length - 1);
            DistanceType distanceType = DistanceType.Plane;

            // Create and add the volunteer to the database
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

            s_dalVolunteer!.Create(volunteer);
            Console.WriteLine($"Volunteer Created: {volunteer.Name}, {volunteer.Email}, {volunteer.Phone}, Role: {volunteer.Role}, Active: {volunteer.IsActive}, Max Distance: {volunteer.MaxDistance}, Distance Type: {volunteer.DistanceType}, Password: {volunteer.Password}");
        }
    }

    private static void CreateCalls()
    {
        // creat array of each sade of call
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
        CallType[] callTypes = new CallType[50];
        for (int i = 0; i < callTypes.Length; i++)
        {
            callTypes[i] = (CallType)s_rand.Next(Enum.GetValues(typeof(CallType)).Length);
        }
        string[] addresses = new[] {
    "גרנדר קניון, David Tuviyahu Ave 125, Be'er Sheva",
    "דרך אליהו נאוי 28, Beersheba",
    "מתחם תחנת הדלק אלון, 31, Arad",
    "Mivtsa Nakhshon Street 60, Be'er Sheva",
    "Yitzhack I. Rager Boulevard 24, Be'er Sheva",
    "Tsvi Bornstein Street 312, Yeruham",
    "איזור תעשייה, הפועלים 5, דימונה",
    "Herzl Street 43, Be'er Sheva",
    "Derekh Hebron 21, Beersheba",
    "Derekh Hebron 4, Beersheba",
    "Kornmehl Farm M.E. Ramat Negev D.N. Halutza",
    "רסקו סנטר, HaTikva 8, Beersheba",
    "40, Tlalim",
    "קניון פרץ סנטר, דימונה",
    "פרץ סנטר - אזור תעשיה דרומי, Dimona",
    "Tsvi Taub Street 7, Be'er Sheva",
    "Ha-Bdil Street 17, ב״ש",
    "ברוך קטינקא 2 ישפרו פלאנט, Beersheba",
    "MALL-7, הע\"ל, Be'er Sheva",
    "Kfar Hadarom Street 1, Be'er Sheva",
    "Kakal Street 55-79, Be'er Sheva",
    "HaAtsmaut Street 14, Be'er Sheva",
    "Kakal Street 79, Be'er Sheva",
    "Ha-Histadrut Street 22, Be'er Sheva",
    "דרום, Kakal Street 78, Be'er Sheva",
    "מתחם B7, Heil Handasa 2, Beersheba",
    "מתחם ביג, חיל ההנדסה 1, באר שבע",
    "Heil Handasa Street 6, Be'er Sheva",
    "Eliyahu Nawi Street, Be'er Sheva",
    "Kakal Street 195, Be'er Sheva",
    "מרכז ביג, Derekh Hebron 21, Be'er Sheva",
    "Heil Handasa Street 1, Be'er Sheva",
    "Derekh Hebron 66, Beersheba",
    "Herzl Street 55, Be'er Sheva",
    "Yitshak Ben Zvi Street 4, Be'er Sheva",
    "Yitshak Nafha Street 36 א', Be'er Sheva",
    "קניון עזריאלי הנגב, Be'er Sheva",
    "Eli'ezer Smoli Street 1, Be'er Sheva",
    "Sderot Johana Jabotinsky 11, Beersheba",
    "Yitshak Nafha Street 25, Be'er Sheva",
    "Ha-Tikva Street 8, Be'er Sheva",
    "Sderot Yerushalayim 2, Beersheba",
    "תחנת דלק פז, Sderot Yerushalayim 4, Beersheba",
    "מגדלי אביסרור, שדרות יצחק רגר 53, באר שבע",
    "David Tuviyahu Ave 125, Beersheba",
    "Derech Metsada 47, Beersheba",
    "מרכז הנגב דרך מצדה 6, Beersheba"
};
        double[] latitudes = new double[] {
31.2506405f, 31.2467805f, 31.2497676f, 31.2605014f, 31.2473633f, 30.98850599999999f, 31.062823f, 31.2396368f, 31.2391961f, 31.2377661f, 30.972753f, 31.2474244f, 30.9935689f, 31.0599323f, 31.0599323f, 31.22165469999999f, 31.2260083f, 31.2244175f, 31.2333045f, 31.2360614f, 31.2380911f, 31.2385546f, 31.2385105f, 31.2381019f, 31.2388346f, 31.243331f, 31.2447522f, 31.2443165f, 31.2492702f, 31.239762f, 31.2443691f, 31.2447522f, 31.244116f, 31.24109f, 31.2439462f, 31.2476115f, 31.243726f, 31.238365f, 31.238365f, 31.2386544f, 31.2461259f, 31.2474978f, 31.245456f, 31.2457355f, 31.2538143f, 31.2506405f, 31.2506405f, 31.2506405f, 31.2581979f, 31.2591166f
};
        double[] longitudes = new double[] {
34.7716625f, 34.8156994f, 35.1914546f, 34.7873239f, 34.7978134f, 34.927951f, 35.0192015f, 34.7877478f, 34.7967932f, 34.7944248f, 34.7755085f, 34.79862f, 34.7643578f, 35.0203137f, 35.0203137f, 34.8001505f, 34.8096236f, 34.8014475f, 34.7965166f, 34.7881834f, 34.7912054f, 34.7924147f, 34.7906793f, 34.7875611f, 34.7907967f, 34.812547f, 34.81256610000001f, 34.8127704f, 34.8178768f, 34.7889659f, 34.8114871f, 34.81256610000001f, 34.8060877f, 34.789058f, 34.7965666f, 34.8106433f, 34.7946189f, 34.7726193f, 34.7726193f, 34.7740828f, 34.8120535f, 34.7986387f, 34.7814817f, 34.7814879f, 34.7983284f, 34.7716625f, 34.7716625f, 34.7716625f, 34.792098f, 34.7955966f
};

        //get the time of the maarachet
        DateTime start = new DateTime(s_dalConfig.Clock.Year - 2, 1, 1);

        for (int i = 0; i < descriptions.Length; i++)
        {
            var startTime = start.AddDays(-s_rand.Next(1, 30)); // Random start time within the last month
            DateTime? deadline = s_rand.NextDouble() < 0.5 // 50% chance of having a deadline or to get null
                ? (DateTime?)startTime.AddHours(s_rand.Next(1, 72)) // Random deadline within 1-72 hours after startTime
                : null;

            //create the call  
            var call = new Call
            (
                Id: i + 1,
                CallType: callTypes[i],
                Address: addresses[i],
                Latitude: latitudes[i],
                Longitude: longitudes[i],
                StartTime: startTime,
                Description: descriptions[i],
                DeadLine: deadline
            );

            s_dalCall!.Create(call);
        }
    }

    private static void CreateAssignments()
    {
        List<Volunteer> volunteers = s_dalVolunteer!.ReadAll().ToList();
        List<Call> calls = s_dalCall!.ReadAll().ToList();
        int assignmentId = s_dalConfig!.NextAssignmentId;

        // Shuffle volunteers to create diverse assignment distribution
        List<Volunteer> shuffledVolunteers = volunteers.OrderBy(_ => s_rand.Next()).ToList();
        int totalVolunteers = shuffledVolunteers.Count;

        // Set up volunteers: 
        // - a few with no assignments
        // - some with one assignment
        // - others with multiple assignments
        int noAssignmentCount = Math.Max(1, totalVolunteers / 5); // 20% have no assignments
        int singleAssignmentCount = Math.Max(1, totalVolunteers / 3); // 30% have one assignment
        int multipleAssignmentCount = totalVolunteers - noAssignmentCount - singleAssignmentCount; // Remaining have multiple assignments

        // Assign calls to volunteers
        int callIndex = 0; // Tracks available calls

        for (int i = 0; i < totalVolunteers; i++)
        {
            Volunteer volunteer = shuffledVolunteers[i];
            int assignmentsForThisVolunteer;

            if (i < noAssignmentCount)
            {
                // Skip assignment for this volunteer
                continue;
            }
            else if (i < noAssignmentCount + singleAssignmentCount)
            {
                assignmentsForThisVolunteer = 1; // One assignment
            }
            else
            {
                assignmentsForThisVolunteer = s_rand.Next(2, 5); // Multiple assignments, between 2 and 4
            }

            for (int j = 0; j < assignmentsForThisVolunteer && callIndex < calls.Count; j++)
            {
                Call call = calls[callIndex++];
                DateTime startTime = call.StartTime.AddHours(s_rand.Next(1, 24));
                DateTime? endTime = null;
                Enum? endType = null;

                // Randomly decide if this assignment will be completed, canceled, or expired
                double outcomeChance = s_rand.NextDouble();

                if (outcomeChance < 0.3)
                {
                    // Completed within call's deadline
                    endTime = call.DeadLine.HasValue
                        ? call.StartTime.AddHours(s_rand.Next(1, (int)(call.DeadLine.Value - startTime).TotalHours + 1))
                        : startTime.AddHours(s_rand.Next(1, 72));
                    endType = EndType.Completed;
                }
                else if (outcomeChance < 0.5)
                {
                    // Self-canceled
                    endTime = startTime.AddHours(s_rand.Next(1, 48));
                    endType = EndType.SelfCanceled;
                }
                else if (outcomeChance < 0.7)
                {
                    // Admin-canceled
                    endTime = startTime.AddHours(s_rand.Next(1, 48));
                    endType = EndType.AdminCanceled;
                }
                else
                {
                    // Expired
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

                // Save the assignment
                s_dalAssignment!.Create(assignment);
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
