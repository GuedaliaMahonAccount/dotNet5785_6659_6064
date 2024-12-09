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
                Option Option = showMainMenu();

                while (Option.EXIT != Option)
                {
                    handleCRUDOptions(Option);
                    Option = showMainMenu();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static Option showMainMenu()
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

        private static Crud showCrudMenu(Option entity)
        {
            int choice;
            do
            {
                Console.WriteLine(@$"
{entity} Crud Options:
0 - Exit
1 - Create 
2 - Read
3 - ReadAll
4 - Update 
5 - Delete");
            }
            while (!int.TryParse(Console.ReadLine(), out choice));
            return (Crud)choice;
        }

        private static void handleCRUDOptions(Option entity)
        {
            try
            {
                switch (showCrudMenu(entity))
                {
                    case Crud.CREATE:
                        handleCreate(entity);
                        break;
                    case Crud.READ:
                        handleRead(entity);
                        break;
                    case Crud.READ_ALL:
                        handleReadAll(entity);
                        break;
                    case Crud.UPDATE:
                        handleUpdate(entity);
                        break;
                    case Crud.DELETE:
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

        private static void handleCreate(Option entity)
        {
            switch (entity)
            {
                case Option.VOLUNTEER:
                    createVolunteer(out Volunteer volunteer);
                    s_bl.Volunteer.AddVolunteer(volunteer);
                    break;
                case Option.CALL:
                    Call call = CreateCall();
                    s_bl.Call.AddCall(call);
                    break;
                case Option.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void handleRead(Option entity)
        {
            Console.WriteLine("Enter an id");
            if (!int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine("Wrong input");

            switch (entity)
            {
                case Option.VOLUNTEER:
                    Console.WriteLine(s_bl.Volunteer.GetVolunteerDetails(id));
                    break;
                case Option.CALL:
                    Console.WriteLine(s_bl.Call.GetCallDetails(id));
                    break;
                case Option.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void handleReadAll(Option entity)
        {
            switch (entity)
            {
                case Option.VOLUNTEER:
                    foreach (var item in s_bl.Volunteer.GetVolunteersList())
                        Console.WriteLine(item);
                    break;
                case Option.CALL:
                    foreach (var item in s_bl.Call.GetCallList(null, null, null))
                        Console.WriteLine(item);
                    break;
                case Option.ADMIN:
                    // Admin read all logic if needed
                    break;
                default:
                    break;
            }
        }

        private static void handleUpdate(Option entity)
        {
            Console.WriteLine("Enter an id");
            if (!int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine("Wrong input");

            switch (entity)
            {
                case Option.VOLUNTEER:
                    Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(id);
                    updateVolunteer(volunteer);
                    s_bl.Volunteer.UpdateVolunteer(id, volunteer);
                    break;
                case Option.CALL:
                    Call call = s_bl.Call.GetCallDetails(id);
                    UpdateCall(call);
                    s_bl.Call.UpdateCall(call);
                    break;
                case Option.ADMIN:
                    break;
                default:
                    break;
            }
        }

        private static void handleDelete(Option entity)
        {
            Console.WriteLine("Enter an id");
            if (!int.TryParse(Console.ReadLine(), out int id))
                Console.WriteLine("Wrong input");

            switch (entity)
            {
                case Option.VOLUNTEER:
                    s_bl.Volunteer.DeleteVolunteer(id);
                    break;
                case Option.CALL:
                    s_bl.Call.DeleteCall(id);
                    break;
                case Option.ADMIN:
                    break;
                default:
                    break;
            }
        }


        //vounteer
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


        //calls
        private static Call CreateCall()
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

        private static void UpdateCall(Call call)
        {
            // Display current CallType and prompt for update
            Console.WriteLine($"Current Call Type: {call.CallType}");
            while (true)
            {
                Console.Write("Enter New Call Type (press Enter to keep current): ");
                string callTypeInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(callTypeInput))
                {
                    // Keep the existing CallType if no input is provided
                    break;
                }

                if (Enum.TryParse(callTypeInput, out CallType callType))
                {
                    call.CallType = callType;
                    break;
                }

                Console.WriteLine("Invalid Call Type. Please try again.");
            }

            // Prompt for updated description
            Console.WriteLine($"Current Description: {call.Description}");
            Console.Write("Enter New Call Description (press Enter to keep current): ");
            string descriptionInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(descriptionInput))
            {
                call.Description = descriptionInput;
            }

            // Prompt for updated address
            Console.WriteLine($"Current Address: {call.Address}");
            Console.Write("Enter New Address (press Enter to keep current): ");
            string addressInput = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(addressInput))
            {
                call.Address = addressInput;
            }

            // Prompt for updated Latitude and Longitude
            Console.WriteLine($"Current Latitude: {call.Latitude}, Current Longitude: {call.Longitude}");
            while (true)
            {
                Console.Write("Enter New Latitude (press Enter to keep current): ");
                string latitudeInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(latitudeInput))
                {
                    // Keep existing Latitude if no input is provided
                    break;
                }

                if (double.TryParse(latitudeInput, out double latitude))
                {
                    call.Latitude = latitude;
                    break;
                }

                Console.WriteLine("Invalid Latitude. Please try again.");
            }

            while (true)
            {
                Console.Write("Enter New Longitude (press Enter to keep current): ");
                string longitudeInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(longitudeInput))
                {
                    // Keep existing Longitude if no input is provided
                    break;
                }

                if (double.TryParse(longitudeInput, out double longitude))
                {
                    call.Longitude = longitude;
                    break;
                }

                Console.WriteLine("Invalid Longitude. Please try again.");
            }

            // Prompt for updated StartTime
            Console.WriteLine($"Current Start Time: {call.StartTime}");
            while (true)
            {
                Console.Write("Enter New Start Time (yyyy-MM-dd HH:mm, press Enter to keep current): ");
                string startTimeInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(startTimeInput))
                {
                    // Keep existing StartTime if no input is provided
                    break;
                }

                if (DateTime.TryParse(startTimeInput, out DateTime startTime))
                {
                    call.StartTime = startTime;
                    break;
                }

                Console.WriteLine("Invalid Start Time. Please try again.");
            }

            // Prompt for updated Deadline
            Console.WriteLine($"Current Deadline: {call.DeadLine}");
            while (true)
            {
                Console.Write("Enter New Deadline (yyyy-MM-dd HH:mm, press Enter to keep current): ");
                string deadlineInput = Console.ReadLine();

                if (string.IsNullOrWhiteSpace(deadlineInput))
                {
                    // Keep existing Deadline if no input is provided
                    break;
                }

                if (DateTime.TryParse(deadlineInput, out DateTime deadline))
                {
                    call.DeadLine = deadline;
                    break;
                }

                Console.WriteLine("Invalid Deadline. Please try again.");
            }

            Console.WriteLine("Call updated successfully!");
        }

    }
}