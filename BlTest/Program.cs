using System;
using System.Collections.Generic;
using BlApi;
using BO;

namespace BlTest
{

    internal class Program
    {
        static readonly IBl s_bl = Factory.Get();


        /// <summary>
        /// main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            try
            {
                Option option = ShowMainMenu();

                while (option != Option.EXIT)
                {
                    switch (option)
                    {
                        case Option.VOLUNTEER:
                            HandleVolunteerOption();
                            break;

                        case Option.CALL:
                            HandleCallOption();
                            break;

                        case Option.ADMIN:
                            HandleAdminOption();
                            break;

                        default:
                            Console.WriteLine("Invalid option. Please try again.");
                            break;
                    }

                    option = ShowMainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
        private static Option ShowMainMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@"
Option Options:
0 - Exit
1 - Volunteer
2 - Call
3 - Admin");
            }
            while (!int.TryParse(Console.ReadLine(), out choice));
            return (Option)choice;
        }


        /// <summary>
        /// each entities Menu
        /// </summary>
        /// <param name="entity"></param>
        static void HandleAdminOption()
        {
            Console.WriteLine("Admin Menu:");
            Console.WriteLine("1 - Get Current Time");
            Console.WriteLine("2 - Update Clock");
            Console.WriteLine("3 - Get Risk Time");
            Console.WriteLine("4 - Set Risk Time");
            Console.WriteLine("5 - Reset Database");
            Console.WriteLine("6 - Initialize Database");
            Console.WriteLine("0 - Return to Main Menu");

            Console.Write("Choose an option: ");
            int choice = int.TryParse(Console.ReadLine(), out int input) ? input : -1;

            switch (choice)
            {
                case 1:
                    GetCurrentTime();
                    break;
                case 2:
                    UpdateClock();
                    break;
                case 3:
                    GetRiskTime();
                    break;
                case 4:
                    SetRiskTime();
                    break;
                case 5:
                    ResetDatabase();
                    break;
                case 6:
                    InitializeDatabase();
                    break;
                case 0:
                    Console.WriteLine("Returning to main menu...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        static void HandleCallOption()
        {
            Console.WriteLine("Call Menu:");
            Console.WriteLine("1 - Get Call Quantities");
            Console.WriteLine("2 - Get Call List");
            Console.WriteLine("3 - Get Call Details");
            Console.WriteLine("4 - Update Call");
            Console.WriteLine("5 - Delete Call");
            Console.WriteLine("6 - Add Call");
            Console.WriteLine("7 - Get Closed Calls");
            Console.WriteLine("8 - Get Open Calls");
            Console.WriteLine("9 - Complete Call");
            Console.WriteLine("10 - Cancel Call");
            Console.WriteLine("11 - Selection Call");
            Console.WriteLine("0 - Return to Main Menu");

            Console.Write("Choose an option: ");
            int choice = int.TryParse(Console.ReadLine(), out int input) ? input : -1;

            switch (choice)
            {
                case 1:
                    GetCallQuantities();
                    break;
                case 2:
                    GetCallList();
                    break;
                case 3:
                    GetCallDetails();
                    break;
                case 4:
                    UpdateCall();
                    break;
                case 5:
                    DeleteCall();
                    break;
                case 6:
                    AddCall();
                    break;
                case 7:
                    GetClosedCalls();
                    break;
                case 8:
                    GetOpenCalls();
                    break;
                case 9:
                    CompleteCall();
                    break;
                case 10:
                    CancelCall();
                    break;
                case 11:
                    SelectionCall();
                    break;
                case 0:
                    Console.WriteLine("Returning to main menu...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }
        static void HandleVolunteerOption()
        {
            Console.WriteLine("Volunteer Menu:");
            Console.WriteLine("1 - Login");
            Console.WriteLine("2 - Get Volunteers List");
            Console.WriteLine("3 - Get Volunteer Details");
            Console.WriteLine("4 - Update Volunteer");
            Console.WriteLine("5 - Delete Volunteer");
            Console.WriteLine("6 - Add Volunteer");
            Console.WriteLine("0 - Return to Main Menu");

            Console.Write("Choose an option: ");
            int choice = int.TryParse(Console.ReadLine(), out int input) ? input : -1;

            switch (choice)
            {
                case 1:
                    Login();
                    break;
                case 2:
                    GetVolunteersList();
                    break;
                case 3:
                    GetVolunteerDetails();
                    break;
                case 4:
                    UpdateVolunteer();
                    break;
                case 5:
                    DeleteVolunteer();
                    break;
                case 6:
                    AddVolunteer();
                    break;
                case 0:
                    Console.WriteLine("Returning to main menu...");
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }
        }





        /// <summary>
        /// vounteer fonctions
        /// </summary>
        private static void Login()
        {

        }
        private static void GetVolunteersList()
        {

        }
        private static void GetVolunteerDetails()
        {

        }
        private static void UpdateVolunteer()
        {

        }
        private static void DeleteVolunteer()
        {

        }
        private static void AddVolunteer()
        {

        }

        //private static void CreateVolunteer(out Volunteer volunteer)
        //{
        //    Console.Write("Enter Name: ");
        //    string name = Console.ReadLine() ?? throw new FormatException("Wrong input");

        //    Console.Write("Enter Phone: ");
        //    string phone = Console.ReadLine() ?? throw new FormatException("Wrong input");

        //    Console.Write("Enter Email: ");
        //    string email = Console.ReadLine() ?? throw new FormatException("Wrong input");

        //    Console.Write("Enter IsActive (true/false): ");
        //    if (!bool.TryParse(Console.ReadLine(), out bool isActive))
        //        throw new FormatException("Wrong input");

        //    Console.Write("Enter Role: ");
        //    string roleInput = Console.ReadLine() ?? throw new FormatException("Wrong input");
        //    if (!Enum.TryParse(roleInput, out Role role))
        //        throw new FormatException("Invalid Role");

        //    Console.Write("Enter DistanceType: ");
        //    string distanceTypeInput = Console.ReadLine() ?? throw new FormatException("Wrong input");
        //    if (!Enum.TryParse(distanceTypeInput, out DistanceType distanceType))
        //        throw new FormatException("Invalid DistanceType");

        //    Console.Write("Enter Password: ");
        //    string password = Console.ReadLine() ?? throw new FormatException("Wrong input");

        //    Console.Write("Enter Address: ");
        //    string address = Console.ReadLine() ?? throw new FormatException("Wrong input");

        //    Console.Write("Enter Latitude: ");
        //    if (!double.TryParse(Console.ReadLine(), out double latitude))
        //        throw new FormatException("Wrong input");

        //    Console.Write("Enter Longitude: ");
        //    if (!double.TryParse(Console.ReadLine(), out double longitude))
        //        throw new FormatException("Wrong input");

        //    Console.Write("Enter MaxDistance: ");
        //    if (!double.TryParse(Console.ReadLine(), out double maxDistance))
        //        throw new FormatException("Wrong input");

        //    volunteer = new Volunteer
        //    {
        //        Name = name,
        //        Phone = phone,
        //        Email = email,
        //        IsActive = isActive,
        //        Role = role,
        //        DistanceType = distanceType,
        //        Password = password,
        //        Address = address,
        //        Latitude = latitude,
        //        Longitude = longitude,
        //        MaxDistance = maxDistance
        //    };
        //}
        //private static void UpdateVolunteer(Volunteer volunteer)
        //{
        //    Console.Write("Enter New Name: ");
        //    volunteer.Name = Console.ReadLine() ?? volunteer.Name;

        //    Console.Write("Enter New Phone: ");
        //    volunteer.Phone = Console.ReadLine() ?? volunteer.Phone;

        //    Console.Write("Enter New Email: ");
        //    volunteer.Email = Console.ReadLine() ?? volunteer.Email;

        //    Console.Write("Enter New IsActive (true/false): ");
        //    if (bool.TryParse(Console.ReadLine(), out bool isActive))
        //        volunteer.IsActive = isActive;

        //    Console.Write("Enter New Role: ");
        //    string roleInput = Console.ReadLine() ?? volunteer.Role.ToString();
        //    if (Enum.TryParse(roleInput, out Role role))
        //        volunteer.Role = role;

        //    Console.Write("Enter New DistanceType: ");
        //    string distanceTypeInput = Console.ReadLine() ?? volunteer.DistanceType.ToString();
        //    if (Enum.TryParse(distanceTypeInput, out DistanceType distanceType))
        //        volunteer.DistanceType = distanceType;

        //    Console.Write("Enter New Password: ");
        //    volunteer.Password = Console.ReadLine() ?? volunteer.Password;

        //    Console.Write("Enter New Address: ");
        //    volunteer.Address = Console.ReadLine() ?? volunteer.Address;

        //    Console.Write("Enter New Latitude: ");
        //    if (double.TryParse(Console.ReadLine(), out double latitude))
        //        volunteer.Latitude = latitude;

        //    Console.Write("Enter New Longitude: ");
        //    if (double.TryParse(Console.ReadLine(), out double longitude))
        //        volunteer.Longitude = longitude;

        //    Console.Write("Enter New MaxDistance: ");
        //    if (double.TryParse(Console.ReadLine(), out double maxDistance))
        //        volunteer.MaxDistance = maxDistance;
        //}


        /// <summary>
        /// calls fonctions
        /// </summary>
        private static void GetCallQuantities()
        {

        }
        private static void GetCallList()
        {

        }
        private static void GetCallDetails()
        {

        }
        private static void UpdateCall()
        {

        }
        private static void DeleteCall()
        {

        }
        private static void AddCall()
        {
            Call call = new Call();


            call.CallType = CallType.Open;


            // Prompt for Address
            while (true)
            {
                Console.Write("Enter Address: ");
                string address = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(address))
                {
                    call.Address = address;
                    break;
                }

                Console.WriteLine("Address cannot be empty. Please try again.");
            }

            // Prompt for Latitude and Longitude
            while (true)
            {
                try
                {
                    Console.Write("Enter Latitude: ");
                    call.Latitude = double.Parse(Console.ReadLine() ?? throw new FormatException("Latitude is required."));

                    Console.Write("Enter Longitude: ");
                    call.Longitude = double.Parse(Console.ReadLine() ?? throw new FormatException("Longitude is required."));
                    break;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}. Please try again.");
                }
            }

            // Prompt for Description
            while (true)
            {
                Console.Write("Enter Call Description: ");
                string description = Console.ReadLine();

                if (!string.IsNullOrWhiteSpace(description))
                {
                    call.Description = description;
                    break;
                }

                Console.WriteLine("Description cannot be empty. Please try again.");
            }

            // Prompt for StartTime
            while (true)
            {
                try
                {
                    Console.Write("Enter Start Time (yyyy-MM-dd HH:mm): ");
                    call.StartTime = DateTime.Parse(Console.ReadLine() ?? throw new FormatException("Start Time is required."));
                    break;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}. Please try again.");
                }
            }

            // Prompt for DeadLine
            while (true)
            {
                try
                {
                    Console.Write("Enter Deadline (yyyy-MM-dd HH:mm): ");
                    call.DeadLine = DateTime.Parse(Console.ReadLine() ?? throw new FormatException("Deadline is required."));
                    break;
                }
                catch (FormatException ex)
                {
                    Console.WriteLine($"Invalid input: {ex.Message}. Please try again.");
                }
            }

            return call;
        }
        private static void GetClosedCalls()
        {

        }
        private static void GetOpenCalls()
        {

        }
        private static void CompleteCall()
        {

        }
        private static void CancelCall()
        {

        }
        private static void SelectionCall()
        {

        }


        /// <summary>
        /// admin fonctions
        /// </summary>
        private static void GetCurrentTime()
        {

        }
        private static void UpdateClock()
        {

        }
        private static void GetRiskTime()
        {

        }
        private static void SetRiskTime()
        {

        }
        private static void ResetDatabase()
        {

        }
        private static void InitializeDatabase()
        {

        }

    }
}