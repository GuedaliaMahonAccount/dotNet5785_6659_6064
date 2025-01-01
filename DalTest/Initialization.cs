using System.Security.Cryptography;
using System.Text;
using DalApi;
using DO;

namespace DalTest
{
    public static class Initialization
    {
        private static IDal? s_dal;
        private static readonly Random s_rand = new();

        private static void CreateVolunteers()
        {
            List<int> s_preGeneratedIds = new()
        {
            322766064, 200000016, 200000024, 200000032, 200000040,
            200000057, 200000065, 200000073, 200000081, 200000099,
            200000107, 200000115, 200000123, 200000131, 200000149
        };

            string[] volunteerNames = {
                "Dani Levy", "Eli Amar", "Yair Cohen", "Ariela Levin", "Dina Klein",
                "Shira Israelof", "Yael Mizrahi", "Oren Shmuel", "Maya Katz",
                "Tomer Golan", "Lea Sharabi", "Moti Ben David", "Yaakov Peretz",
                "Ruth Azulay", "Itzik Shalev"
            };

            string[] volunteerEmails = {
                "danilevy123@gmail.com", "eliamar456@gmail.com", "yaircohen789@outlook.com", "arielalevin234@gmail.com",
                "dinaklein567@gmail.com", "shiraisraelof890@gmail.com", "yaelmizrahi123@outlook.com",
                "orenshmuel456@gmail.com", "mayakatz789@gmail.com", "tomergolan234@gmail.com", "leasharabi567@gmail.com",
                "motibendavid890@outlook.com", "yaakovperetz123@gmail.com", "ruthazulay456@gmail.com",
                "itzikshalev789@outlook.com"
            };

            string[] addresses = {
    "מרכז אורן יצחק רגר, 185, Beer Sheva",
    "מרכז הנגב דרך מצדה, 6, Beer Sheva",
    "תחנת דלק פז, 4, Beer Sheva",
    "איזור תעשייה, 5, Beer Sheva",
    "רסקו סנטר, 8, Beer Sheva",
    "ברוך קטינקא, 2, Beer Sheva",
    "מרכז ביג, 21, Beer Sheva",
    "קרן קיימת לישראל, 78, Beer Sheva",
    "מרכז אורן יצחק רגר, 185, Beer Sheva",
    "מתחם ביג, 1, Beer Sheva",
    "מרכז ביג, 21, Beer Sheva",
    "תחנת דלק פז, 4, Beer Sheva",
    "מגדלי אביסרור, 53, Beer Sheva",
    "מרכז הנגב דרך מצדה, 6, Beer Sheva",
    "מרכז אורן יצחק רגר, 185, Beer Sheva"
};

            float[] latitudes = {
31.27042089999999f, 31.2591166f, 31.2521018f, 31.2521018f, 31.2474244f, 31.2242536f, 31.2521018f, 31.2388346f, 31.27042089999999f, 31.2521018f, 31.2521018f, 31.2521018f, 31.2498359f, 31.2591166f, 31.27042089999999f
};

            float[] longitudes = {
34.7975837f, 34.7955966f, 34.7867691f, 34.7867691f, 34.79862f, 34.8010573f, 34.7867691f, 34.7907967f, 34.7975837f, 34.7867691f, 34.7867691f, 34.7867691f, 34.7739944f, 34.7955966f, 34.7975837f
};

            List<string> phonePrefixes = new List<string> { "050", "053", "058", "052", "054" };

            for (int i = 0; i < 15; i++)
            {
                string password = PasswordGenerator.GenerateStrongPassword();
                string encryptedPassword = AesEncryptionHelper.Encrypt(password);
                int id = s_preGeneratedIds[i];

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
                    Password: encryptedPassword,
                    Address: i < addresses.Length ? addresses[i] : null,
                    Latitude: i < latitudes.Length ? latitudes[i] : null,
                    Longitude: i < longitudes.Length ? longitudes[i] : null
                );

                s_dal.Volunteer.Create(volunteer);
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
            string[] addresses = {
    "שלמה אבן גבירול, 1, Israel",
    "בן יהודה, 121, Israel",
    "בן יהודה, 38, Israel",
    "זיסמן שלום, 14, Israel",
    "הירקון, 145, Israel",
    "הירקון, 250, Israel",
    "הירקון, 121, Israel",
    "מאפו, 9, Israel",
    "אליעזר פרי, 10, Israel",
    "טרומפלדור, 11, Israel",
    "בן יהודה, 35, Israel",
    "הירקון, 86, Israel",
    "בן יהודה, 87, Israel",
    "דיזנגוף, 43, Israel",
    "הירקון, 220, Israel",
    "הירקון, 99, Israel",
    "הירקון, 80, Israel",
    "הירקון, 287, Israel",
    "הירקון, 216, Israel",
    "רמת גן, 1, Israel",
    "בן יהודה, 77, Israel",
    "הירקון, 88, Israel",
    "בן יהודה, 3, Israel",
    "הירקון, 115, Israel",
    "הירקון, 78, Israel",
    "אליעזר פרי, 9, Israel",
    "זמנהוף, 2, Israel",
    "יונה הנביא, 41, Israel",
    "בני דן, 36, Israel",
    "בן יהודה, 14, Israel",
    "חולדה, 11, Israel",
    "בן סרוק, 13, Israel",
    "אליעזר קפלן, 4, Israel",
    "דיזנגוף, 130, Israel",
    "ארלוזורוב, 23, Israel",
    "בן יהודה, 84, Israel",
    "ירמיהו, 4, Israel",
    "חבקוק הנביא, 3, Israel",
    "הירקון, 155, Israel",
    "ויצמן, 1, Israel",
    "הירקון, 84, Israel",
    "ניסים אלוני, 17, Israel",
    "הירקון, 50, Israel",
    "הירקון, 19, Israel",
    "הירקון, 205, Israel",
    "זמנהוף, 1, Israel",
    "נס ציונה, 10, Israel",
    "בן יהודה, 28, Israel",
    "מנדלי מו״ס, 5, Israel",
    "שלמה אבן גבירול, 1, Israel"
};
            float[] latitudes = {
32.0713162f, 32.086167f, 32.0769348f, 32.0841733f, 32.0838289f, 32.0925088f, 32.0831292f, 32.08067f, 32.0865166f, 32.0755646f, 32.0771102f, 32.077606f, 32.082431f, 32.074634f, 32.0896504f, 32.0796764f, 32.0770334f, 32.09414f, 32.0892939f, 32.068424f, 32.0812592f, 32.0777623f, 32.0735194f, 32.0814839f, 32.0767003f, 32.0858688f, 32.0777646f, 32.0720763f, 32.0954946f, 32.07443130000001f, 31.833435f, 32.086776f, 32.0732517f, 32.0816336f, 32.0870125f, 32.0810562f, 32.0951275f, 32.0935645f, 32.0845749f, 32.0775347f, 32.077345f, 32.09000640000001f, 32.0731472f, 32.0704339f, 32.0893245f, 32.078094f, 31.932111f, 32.0758815f, 31.890267f, 32.0713162f
};
            float[] longitudes = {
34.7817176f, 34.7726997f, 34.7684819f, 34.7994752f, 34.7688932f, 34.77370270000001f, 34.7685983f, 34.7695164f, 34.7695971f, 34.7676861f, 34.7679725f, 34.767611f, 34.7710127f, 34.7760331f, 34.7724391f, 34.7677215f, 34.7673099f, 34.7737286f, 34.772373f, 34.824785f, 34.7704153f, 34.7675604f, 34.7681659f, 34.7682212f, 34.7674762f, 34.7697296f, 34.775037f, 34.7687876f, 34.7883626f, 34.7682201f, 34.886757f, 34.7833573f, 34.7827531f, 34.774047f, 34.7753159f, 34.7706016f, 34.7745326f, 34.7730239f, 34.7691515f, 34.7879046f, 34.7674017f, 34.79633159999999f, 34.7661283f, 34.7646432f, 34.7707709f, 34.774744f, 34.801327f, 34.7682341f, 35.010397f, 34.7817176f
};
            CallType[] callTypes = new CallType[descriptions.Length];
            for (int i = 0; i < callTypes.Length; i++)
            {
                callTypes[i] = (CallType)s_rand.Next(Enum.GetValues(typeof(CallType)).Length);
            }


            DateTime end = DateTime.Now;
            DateTime start = end.AddDays(-30);

            //in case that it s not exactly 50 data
            int minLength = new[] { descriptions.Length, addresses.Length, latitudes.Length, longitudes.Length }.Min();
            for (int i = 0; i < minLength; i++)
            {
                // Generate a random start date within the last 30 days
                var startTime = start
                    .AddDays(s_rand.Next(0, 30))            // Random days between 30 days ago and today
                    .AddHours(s_rand.Next(0, 24))          // Random hours
                    .AddMinutes(s_rand.Next(0, 60))        // Random minutes
                    .AddSeconds(s_rand.Next(0, 60));       // Random seconds

                // Generate a random deadline between 30 and 60 days after the start date
                DateTime? deadline = s_rand.NextDouble() < 0.5
                    ? (DateTime?)startTime
                        .AddDays(s_rand.Next(30, 60))      // Random days between 30 and 60 days after startTime
                        .AddHours(s_rand.Next(0, 24))      // Random hours
                        .AddMinutes(s_rand.Next(0, 60))    // Random minutes
                        .AddSeconds(s_rand.Next(0, 60))    // Random seconds
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
            IEnumerable<Volunteer> volunteers = s_dal!.Volunteer.ReadAll();
            IEnumerable<Call> calls = s_dal.Call.ReadAll();

            int assignmentId = s_dal.Config.NextAssignmentId;

            Volunteer[] shuffledVolunteers = volunteers.OrderBy(_ => s_rand.Next()).ToArray();
            int totalVolunteers = shuffledVolunteers.Length;

            int noAssignmentCount = Math.Max(1, totalVolunteers / 5);
            int singleAssignmentCount = Math.Max(1, totalVolunteers / 3);
            int multipleAssignmentCount = totalVolunteers - noAssignmentCount - singleAssignmentCount;

            using var callEnumerator = calls.GetEnumerator();
            bool hasMoreCalls = callEnumerator.MoveNext();

            for (int i = 0; i < totalVolunteers; i++)
            {
                Volunteer volunteer = shuffledVolunteers[i];
                int assignmentsForThisVolunteer;

                if (i < noAssignmentCount) continue;
                else if (i < noAssignmentCount + singleAssignmentCount) assignmentsForThisVolunteer = 1;
                else assignmentsForThisVolunteer = s_rand.Next(2, Math.Max(2, 5)); // Ensure valid range

                for (int j = 0; j < assignmentsForThisVolunteer && hasMoreCalls; j++)
                {
                    Call call = callEnumerator.Current;
                    hasMoreCalls = callEnumerator.MoveNext();

                    DateTime startTime = call.StartTime.AddHours(s_rand.Next(1, Math.Max(1, 24))); // Ensure valid range
                    DateTime? endTime = null;
                    EndType? endType = null;

                    double outcomeChance = s_rand.NextDouble();

                    if (outcomeChance < 0.3)
                    {
                        // Fix for calculating endTime when DeadLine is defined
                        if (call.DeadLine.HasValue && call.DeadLine.Value > startTime)
                        {
                            int maxHours = (int)(call.DeadLine.Value - startTime).TotalHours;

                            if (maxHours >= 1)
                            {
                                endTime = call.StartTime.AddHours(s_rand.Next(1, maxHours + 1));
                            }
                            else
                            {
                                Console.WriteLine($"Invalid maxHours={maxHours}, setting endTime to default range");
                                endTime = startTime.AddHours(s_rand.Next(1, 72));
                            }
                        }
                        else
                        {
                            endTime = startTime.AddHours(s_rand.Next(1, 72));
                        }
                        endType = EndType.Completed;
                    }
                    else if (outcomeChance < 0.5)
                    {
                        endTime = startTime.AddHours(s_rand.Next(1, 48)); // Ensure valid range
                        endType = EndType.SelfCanceled;
                    }
                    else if (outcomeChance < 0.7)
                    {
                        endTime = startTime.AddHours(s_rand.Next(1, 48)); // Ensure valid range
                        endType = EndType.AdminCanceled;
                    }
                    else
                    {
                        endType = EndType.Expired;
                    }

                    var assignment = new Assignment
                    {
                        Id = s_dal.Config.NextAssignmentId,
                        CallId = call.Id,
                        VolunteerId = volunteer.Id,
                        StartTime = startTime,
                        EndTime = endTime,
                        EndType = endType
                    };

                    s_dal.Assignment.Create(assignment);
                }
            }
        }

        public static void Do()
        {
            s_dal = DalApi.Factory.Get;
            Console.WriteLine("Reset Configuration values and List values...");
            s_dal.ResetDB();
            CreateVolunteers();
            CreateCalls();
            CreateAssignments();
        }
    }




    //password hash
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
    public static class PasswordGenerator
    {
        private static readonly Random s_rand = new();
        public static string GenerateStrongPassword()
        {
            const string upperCaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCaseLetters = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?";

            var passwordChars = new List<char>();

            // Ensure at least one character from each required set
            passwordChars.Add(upperCaseLetters[s_rand.Next(upperCaseLetters.Length)]);
            passwordChars.Add(lowerCaseLetters[s_rand.Next(lowerCaseLetters.Length)]);
            passwordChars.Add(digits[s_rand.Next(digits.Length)]);
            passwordChars.Add(specialChars[s_rand.Next(specialChars.Length)]);

            // Fill the rest of the password length with random characters from all sets
            string allChars = upperCaseLetters + lowerCaseLetters + digits + specialChars;
            for (int i = passwordChars.Count; i < 8; i++)
            {
                passwordChars.Add(allChars[s_rand.Next(allChars.Length)]);
            }

            // Shuffle the characters to ensure randomness
            var shuffledPassword = passwordChars.OrderBy(c => s_rand.Next()).ToArray();

            return new string(shuffledPassword);
        }
    }

}