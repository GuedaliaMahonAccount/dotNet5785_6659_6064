using System;
using System.Collections.Generic;
using BlApi;
using BO;

namespace BlTest
{

    internal class Program
    {
        static readonly IBl s_bl = Factory.Get();

        static void Main(string[] args)
        {
            try
            {
                OPTION option = showMainMenu();

                while (OPTION.EXIT != option)
                {
                    handleCRUDOptions(option);
                    option = showMainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static OPTION showMainMenu()
        {
            int choice;
            do
            {
                Console.WriteLine(@"
OPTION Options:
0 - Exit
1 - Volunteer
2 - Call
3 - Admin");
            }
            while (!int.TryParse(Console.ReadLine(), out choice));
            return (OPTION)choice;
        }

        private static CRUD showCrudMenu(OPTION entity)
        {
            int choice;
            do
            {
                Console.WriteLine(@$"
{entity} CRUD Options:
0 - Exit
1 - Create 
2 - Read
3 - ReadAll
4 - Update 
5 - Delete");
            }
            while (!int.TryParse(Console.ReadLine(), out choice));
            return (CRUD)choice;
        }

        private static void handleCRUDOptions(OPTION entity)
        {
            try
            {
                switch (showCrudMenu(entity))
                {
                    case CRUD.CREATE:
                        handleCreate(entity);
                        break;
                    case CRUD.READ:
                        handleRead(entity);
                        break;
                    case CRUD.READ_ALL:
                        handleReadAll(entity);
                        break;
                    case CRUD.UPDATE:
                        handleUpdate(entity);
                        break;
                    case CRUD.DELETE:
                        handleDelete(entity);
                        break;
                    default:
                        return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void handleCreate(OPTION entity)
        {
            switch (entity)
            {
                case OPTION.VOLUNTEER:
                    createVolunteer(out Volunteer volunteer);
                    s_bl.Volunteer.AddVolunteer(volunteer);
                    break;
                case OPTION.CALL:
                    createCall(out Call call);
                    s_bl.Call.AddCall(call);
                    break;
                case OPTION.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void handleRead(OPTION entity)
        {
            Console.WriteLine("Enter an id");
            if (!int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine("Wrong input");

            switch (entity)
            {
                case OPTION.VOLUNTEER:
                    Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(id));
                    break;
                case OPTION.CALL:
                    Console.WriteLine(s_bl.Call.GetCallDetails(id));
                    break;
                case OPTION.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void handleReadAll(OPTION entity)
        {
            switch (entity)
            {
                case OPTION.VOLUNTEER:
                    foreach (var item in s_bl.Volunteer.GetVolunteersList())
                        Console.WriteLine(item);
                    break;
                case OPTION.CALL:
                    foreach (var item in s_bl.Call.GetCallList(null, null, null))
                        Console.WriteLine(item);
                    break;
                case OPTION.ADMIN:
                    // Admin read all logic if needed
                    break;
                default:
                    break;
            }
        }

        private static void handleUpdate(OPTION entity)
        {
            Console.WriteLine("Enter an id");
            if (!int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine("Wrong input");

            switch (entity)
            {
                case OPTION.VOLUNTEER:
                    Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    updateVolunteer(volunteer);
                    s_bl.Volunteer.UpdateVolunteer(id, volunteer);
                    break;
                case OPTION.CALL:
                    Call call = s_bl.Call.GetCallDetails(id);
                    updateCall(call);
                    s_bl.Call.UpdateCall(call);
                    break;
                case OPTION.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void handleDelete(OPTION entity)
        {
            Console.WriteLine("Enter an id");
            if (!int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine("Wrong input");

            switch (entity)
            {
                case OPTION.VOLUNTEER:
                    s_bl.Volunteer.DeleteVolunteer(id);
                    break;
                case OPTION.CALL:
                    s_bl.Call.DeleteCall(id);
                    break;
                case OPTION.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void createVolunteer(out Volunteer volunteer)
        {
            Console.Write("Enter Name: ");
            string name = Console.ReadLine() ?? throw new FormatException("Wrong input");

            Console.Write("Enter Phone: ");
            string phone = Console.ReadLine() ?? throw new FormatException("Wrong input");

            Console.Write("Enter Email: ");
            string email = Console.ReadLine() ?? throw new FormatException("Wrong input");

            Console.Write("Enter IsActive (true/false): ");
            if (!bool.TryParse(Console.ReadLine(), out bool isActive))
                throw new FormatException("Wrong input");

            Console.Write("Enter Role: ");
            string roleInput = Console.ReadLine() ?? throw new FormatException("Wrong input");
            if (!Enum.TryParse(roleInput, out Role role))
                throw new FormatException("Invalid Role");

            Console.Write("Enter DistanceType: ");
            string distanceTypeInput = Console.ReadLine() ?? throw new FormatException("Wrong input");
            if (!Enum.TryParse(distanceTypeInput, out DistanceType distanceType))
                throw new FormatException("Invalid DistanceType");

            Console.Write("Enter Password: ");
            string password = Console.ReadLine() ?? throw new FormatException("Wrong input");

            Console.Write("Enter Address: ");
            string address = Console.ReadLine() ?? throw new FormatException("Wrong input");

            Console.Write("Enter Latitude: ");
            if (!double.TryParse(Console.ReadLine(), out double latitude))
                throw new FormatException("Wrong input");

            Console.Write("Enter Longitude: ");
            if (!double.TryParse(Console.ReadLine(), out double longitude))
                throw new FormatException("Wrong input");

            Console.Write("Enter MaxDistance: ");
            if (!double.TryParse(Console.ReadLine(), out double maxDistance))
                throw new FormatException("Wrong input");

            volunteer = new Volunteer
            {
                Name = name,
                Phone = phone,
                Email = email,
                IsActive = isActive,
                Role = role,
                DistanceType = distanceType,
                Password = password,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                MaxDistance = maxDistance
            };
        }

        private static void updateVolunteer(Volunteer volunteer)
        {
            Console.Write("Enter New Name: ");
            volunteer.Name = Console.ReadLine() ?? volunteer.Name;

            Console.Write("Enter New Phone: ");
            volunteer.Phone = Console.ReadLine() ?? volunteer.Phone;

            Console.Write("Enter New Email: ");
            volunteer.Email = Console.ReadLine() ?? volunteer.Email;

            Console.Write("Enter New IsActive (true/false): ");
            if (bool.TryParse(Console.ReadLine(), out bool isActive))
                volunteer.IsActive = isActive;

            Console.Write("Enter New Role: ");
            string roleInput = Console.ReadLine() ?? volunteer.Role.ToString();
            if (Enum.TryParse(roleInput, out Role role))
                volunteer.Role = role;

            Console.Write("Enter New DistanceType: ");
            string distanceTypeInput = Console.ReadLine() ?? volunteer.DistanceType.ToString();
            if (Enum.TryParse(distanceTypeInput, out DistanceType distanceType))
                volunteer.DistanceType = distanceType;

            Console.Write("Enter New Password: ");
            volunteer.Password = Console.ReadLine() ?? volunteer.Password;

            Console.Write("Enter New Address: ");
            volunteer.Address = Console.ReadLine() ?? volunteer.Address;

            Console.Write("Enter New Latitude: ");
            if (double.TryParse(Console.ReadLine(), out double latitude))
                volunteer.Latitude = latitude;

            Console.Write("Enter New Longitude: ");
            if (double.TryParse(Console.ReadLine(), out double longitude))
                volunteer.Longitude = longitude;

            Console.Write("Enter New MaxDistance: ");
            if (double.TryParse(Console.ReadLine(), out double maxDistance))
                volunteer.MaxDistance = maxDistance;
        }

        private static void createCall(out Call call)
        {
            Console.Write("Enter Call Type: ");
            string callTypeInput = Console.ReadLine() ?? throw new FormatException("Wrong input");
            if (!Enum.TryParse(callTypeInput, out CallType callType))
                throw new FormatException("Invalid Call Type");

            Console.Write("Enter Call Description: ");
            string description = Console.ReadLine() ?? throw new FormatException("Wrong input");

            call = new Call
            {
                CallType = callType,
                Description = description
            };
        }

        private static void updateCall(Call call)
        {
            Console.Write("Enter New Call Type: ");
            string callTypeInput = Console.ReadLine() ?? call.CallType.ToString();
            if (!Enum.TryParse(callTypeInput, out CallType callType))
                throw new FormatException("Invalid Call Type");

            call.CallType = callType;

            Console.Write("Enter New Call Description: ");
            call.Description = Console.ReadLine() ?? call.Description;
        }
    }
}