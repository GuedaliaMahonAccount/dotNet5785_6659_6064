
using BlApi;
using BO;
using Helpers;

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
            Console.WriteLine("11 - Assign Call");
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
        ///
        //
        /// <summary>
        /// function to login
        /// </summary>
        private static void Login()
        {
            Console.Write("Enter username: ");
            string username = Console.ReadLine();

            string encryptedPassword = Helpers.PasswordUtils.ReadAndEncryptPassword(); ;

            try
            {
                string role = s_bl.Volunteer.Login(username, encryptedPassword);
                Console.WriteLine($"Login successful. Role: {role}");
            }
            catch (BO.BlDoesNotExistException ex)
            {
                Console.WriteLine($"Login failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }


        /// <summary>
        /// function to get the list of volunteers
        /// </summary>
        /// 
        private static void GetVolunteersList()
        {
            try
            {
                int choice;
                bool? isActive = null;
                BO.VolunteerInListSortFields? sortField = null;

                while (true)
                {
                    Console.WriteLine("\nVolunteers List Menu:");
                    Console.WriteLine("1. Get All Volunteers");
                    Console.WriteLine("2. Get Active Volunteers");
                    Console.WriteLine("3. Get Inactive Volunteers");
                    Console.WriteLine("4. Sort by ID");
                    Console.WriteLine("5. Sort by Name");
                    Console.WriteLine("6. Sort by Phone");
                    Console.WriteLine("7. Sort by Activity Status");
                    Console.WriteLine("8. Sort by Role");
                    Console.WriteLine("9. Sort by Latitude");
                    Console.WriteLine("10. Sort by Longitude");
                    Console.WriteLine("0. Return to Main Menu");

                    // Use TryParse for input validation
                    if (!int.TryParse(Console.ReadLine(), out choice))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    // Reset sorting and filtering for each iteration
                    isActive = null;
                    sortField = null;

                    switch (choice)
                    {
                        case 0:
                            return;
                        case 1: // All Volunteers
                            break;
                        case 2: // Active Volunteers
                            isActive = true;
                            break;
                        case 3: // Inactive Volunteers
                            isActive = false;
                            break;
                        case 4:
                            sortField = BO.VolunteerInListSortFields.Id;
                            break;
                        case 5:
                            sortField = BO.VolunteerInListSortFields.Name;
                            break;
                        case 6:
                            sortField = BO.VolunteerInListSortFields.Phone;
                            break;
                        case 7:
                            sortField = BO.VolunteerInListSortFields.IsActive;
                            break;
                        case 8:
                            sortField = BO.VolunteerInListSortFields.Role;
                            break;
                        case 9:
                            sortField = BO.VolunteerInListSortFields.Latitude;
                            break;
                        case 10:
                            sortField = BO.VolunteerInListSortFields.Longitude;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            continue;
                    }

                    // Ask the user if they want to filter volunteers before sorting
                    Console.WriteLine("\nDo you want to filter volunteers before sorting?");
                    Console.WriteLine("Press 0 to filter by inactive volunteers");
                    Console.WriteLine("Press 1 to filter by active volunteers");
                    Console.WriteLine("Press Enter if you are not interested in filtering");

                    string filterChoice = Console.ReadLine();

                    if (filterChoice == "0")
                    {
                        isActive = false;
                    }
                    else if (filterChoice == "1")
                    {
                        isActive = true;
                    }
                    else
                    {
                        isActive = null; // No filtering
                    }

                    // Retrieve and display volunteers
                    try
                    {
                        var volunteersList = s_bl.Volunteer.GetVolunteersList(isActive, sortField);

                        Console.WriteLine("\nVolunteers List:");
                        foreach (var volunteer in volunteersList)
                        {
                            Console.WriteLine(volunteer);
                        }
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        Console.WriteLine($"Volunteers do not exist: {ex.Message}");
                    }
                    catch (BO.BlNullPropertyException ex)
                    {
                        Console.WriteLine($"Null Property Error: {ex.Message}");
                    }
                    catch (BO.BlInvalidValueException ex)
                    {
                        Console.WriteLine($"Invalid Value: {ex.Message}");
                    }
                    catch (BO.BlArgumentNullException ex)
                    {
                        Console.WriteLine($"Argument Null Error: {ex.Message}");
                    }
                    catch (BO.LogicException ex)
                    {
                        Console.WriteLine($"Logic Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in Volunteers List Menu: {ex.Message}");
            }
        }

        /// <summary>
        /// function to get the details of a volunteer
        /// </summary>
        /// 
        private static void GetVolunteerDetails()
        {
            Console.Write("Enter volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                try
                {
                    BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);
                    Console.WriteLine(volunteer);
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    Console.WriteLine($"Error retrieving volunteer details: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid number.");
            }
        }

        /// <summary>
        /// function to update a volunteer
        /// </summary>
        private static void UpdateVolunteer()
        {
            Console.Write("Please enter the ID of the requester: ");
            int requesterId = int.Parse(Console.ReadLine());

            Console.Write("Please enter the ID of the volunteer you want to update: ");
            int volunteerId = int.Parse(Console.ReadLine());

            // Get the volunteer details from the BL
            BO.Volunteer volunteer = s_bl.Volunteer.GetVolunteerDetails(volunteerId);

            if (volunteer == null)
            {
                Console.WriteLine("Volunteer with ID {0} not found.", volunteerId);
                return;
            }

            // Get the requester details to check if they are an admin
            BO.Volunteer requester = s_bl.Volunteer.GetVolunteerDetails(requesterId);
            if (requester == null)
            {
                Console.WriteLine("Requester with ID {0} not found.", requesterId);
                return;
            }

            bool isAdmin = requester.Role == BO.Role.Admin;

            // Prompt the user to update the volunteer details
            Console.WriteLine("Enter new details for the volunteer (leave empty to keep current values):");

            Console.Write("Enter new name: ");
            string newName = Console.ReadLine();
            if (!string.IsNullOrEmpty(newName))
                volunteer.Name = newName;

            Console.Write("Enter new phone number: ");
            string newPhone = Console.ReadLine();
            if (!string.IsNullOrEmpty(newPhone))
                volunteer.Phone = newPhone;

            Console.Write("Enter new email: ");
            string newEmail = Console.ReadLine();
            if (!string.IsNullOrEmpty(newEmail))
                volunteer.Email = newEmail;

            Console.Write("Enter new password: ");
            string encryptedPassword = PasswordUtils.ReadAndEncryptPassword();
            if (!string.IsNullOrEmpty(encryptedPassword))
            {
                volunteer.Password = encryptedPassword;
            }


            Console.Write("Enter new address: ");
            string newAddress = Console.ReadLine();
            if (!string.IsNullOrEmpty(newAddress))
                volunteer.Address = newAddress;

            Console.Write("Enter new latitude (leave empty to keep current): ");
            string newLatitude = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLatitude))
            {
                if (double.TryParse(newLatitude, out double latitude))
                    volunteer.Latitude = latitude;
                else
                    Console.WriteLine("Invalid latitude value. Keeping current value.");
            }

            Console.Write("Enter new longitude (leave empty to keep current): ");
            string newLongitude = Console.ReadLine();
            if (!string.IsNullOrEmpty(newLongitude))
            {
                if (double.TryParse(newLongitude, out double longitude))
                    volunteer.Longitude = longitude;
                else
                    Console.WriteLine("Invalid longitude value. Keeping current value.");
            }

            // Only admin can change the Role
            Console.Write("Enter new role (Admin/Volunteer) - only admin can change this: ");
            string newRole = Console.ReadLine();
            if (!string.IsNullOrEmpty(newRole))
            {
                if (!isAdmin)
                {
                    Console.WriteLine("Only an admin can change the role. Keeping current role.");
                }
                else
                {
                    if (Enum.TryParse(newRole, out BO.Role role))
                        volunteer.Role = role;
                    else
                        Console.WriteLine("Invalid role value. Keeping current role.");
                }
            }

            Console.Write("Enter new IsActive status (true/false): ");
            string newIsActive = Console.ReadLine();
            if (!string.IsNullOrEmpty(newIsActive))
            {
                if (bool.TryParse(newIsActive, out bool isActive))
                    volunteer.IsActive = isActive;
                else
                    Console.WriteLine("Invalid IsActive value. Keeping current value.");
            }

            // Update the volunteer using the BL
            try
            {
                s_bl.Volunteer.UpdateVolunteerAsync(requesterId, volunteer);
                Console.WriteLine("Volunteer with ID {0} has been updated successfully.", volunteerId);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating volunteer: " + ex.Message);
            }
        }

        /// <summary>
        /// function to delete a volunteer
        /// </summary>
        private static void DeleteVolunteer()
        {
            Console.Write("Enter volunteer ID to delete: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                try
                {
                    s_bl.Volunteer.DeleteVolunteer(volunteerId);
                    Console.WriteLine("Volunteer deleted successfully.");
                }
                catch (BO.BlDeletionImpossibleException ex)
                {
                    Console.WriteLine($"Delete failed: {ex.Message}");
                }
                catch (BO.BlDoesNotExistException ex)
                {
                    Console.WriteLine($"Delete failed: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Unexpected error: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid ID. Please enter a valid number.");
            }
        }

        /// <summary>
        /// function to add a volunteer
        /// </summary>
        private static void AddVolunteer()
        {
            BO.Volunteer volunteer = new();

            Console.Write("Enter volunteer ID: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID.");
                return;
            }
            volunteer.Id = id;

            Console.Write("Enter name: ");
            volunteer.Name = Console.ReadLine();

            volunteer.Password = Helpers.PasswordUtils.ReadAndEncryptPassword(); ;

            Console.Write("Enter phone: ");
            volunteer.Phone = Console.ReadLine();

            Console.Write("Enter email: ");
            volunteer.Email = Console.ReadLine();

            Console.Write("Enter address: ");
            volunteer.Address = Console.ReadLine();

            Console.Write("Enter latitude: ");
            if (double.TryParse(Console.ReadLine(), out double latitude))
            {
                volunteer.Latitude = latitude;
            }
           
            Console.Write("Enter longitude: ");
            if (double.TryParse(Console.ReadLine(), out double longitude))
            {
                volunteer.Longitude = longitude;
            }

            try
            {
                s_bl.Volunteer.AddVolunteerAsync(volunteer);
                Console.WriteLine("Volunteer added successfully.");
            }
            catch (BO.BlInvalidValueException ex)
            {
                Console.WriteLine($"Add failed: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}");
            }
        }





        /// <summary>
        /// calls fonctions
        /// </summary>
        ///
        //
        /// <summary>
        /// Retrieves the quantities of calls grouped by type.
        /// </summary>
        private static void GetCallQuantities()
        {
            try
            {
                int[] callQuantities = s_bl.Call.GetCallQuantities();
                Console.WriteLine("Call Quantities by Type:");
                foreach (BO.CallType callType in Enum.GetValues(typeof(BO.CallType)))
                {
                    Console.WriteLine($"{callType}: {callQuantities[(int)callType]}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving call quantities: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves and displays the list of calls.
        /// </summary>
        private static void GetCallList()
        {
            try
            {
                Status? callType = null;
                BO.CallSortField? sortField = null;

                while (true)
                {
                    Console.WriteLine("\nCall List Menu:");
                    Console.WriteLine("1. Get All Calls");
                    Console.WriteLine("2. Get Open Calls");
                    Console.WriteLine("3. Get Completed Calls");
                    Console.WriteLine("4. Sort by Call Type");
                    Console.WriteLine("5. Sort by Start Time");
                    Console.WriteLine("6. Sort by Duration");
                    Console.WriteLine("0. Return to Main Menu");

                    Console.Write("Choose an option: ");
                    if (!int.TryParse(Console.ReadLine(), out int choice))
                    {
                        Console.WriteLine("Invalid input. Please enter a number.");
                        continue;
                    }

                    // Reset filters and sorting for each iteration
                    callType = null;
                    sortField = null;

                    switch (choice)
                    {
                        case 0:
                            return;
                        case 1: // Get All Calls
                            break;
                        case 2: // Open Calls
                            callType = Status.Open;
                            break;
                        case 3: // Closed Calls
                            callType = Status.Completed;
                            break;
                        case 4: // Sort by Call Type
                            sortField = BO.CallSortField.CallType;
                            break;
                        case 5: // Sort by Start Time
                            sortField = BO.CallSortField.StartTime;
                            break;
                        case 6: // Sort by Duration
                            sortField = BO.CallSortField.Duration;
                            break;
                        default:
                            Console.WriteLine("Invalid choice. Please try again.");
                            continue;
                    }

                    try
                    {
                        // Retrieve the filtered and sorted list
                        var callsList = s_bl.Call.GetCallList(callType, sortField);

                        // Display the calls
                        Console.WriteLine("\nCalls List:");
                        foreach (var call in callsList)
                        {
                            Console.WriteLine(call);
                        }
                    }
                    catch (BO.BlDoesNotExistException ex)
                    {
                        Console.WriteLine($"Calls do not exist: {ex.Message}");
                    }
                    catch (BO.BlNullPropertyException ex)
                    {
                        Console.WriteLine($"Null Property Error: {ex.Message}");
                    }
                    catch (BO.BlInvalidValueException ex)
                    {
                        Console.WriteLine($"Invalid Value: {ex.Message}");
                    }
                    catch (BO.BlArgumentNullException ex)
                    {
                        Console.WriteLine($"Argument Null Error: {ex.Message}");
                    }
                    catch (BO.LogicException ex)
                    {
                        Console.WriteLine($"Logic Error: {ex.Message}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Unexpected Error: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error in Calls List Menu: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves and displays detailed information about a specific call.
        /// </summary>
        private static void GetCallDetails()
        {
            Console.Write("Enter Call ID: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                try
                {
                    var call = s_bl.Call.GetCallDetails(callId);
                    Console.WriteLine($"Call Details for ID {call.Id}:\nType: {call.CallType}\nAddress: {call.Address}\nDescription: {call.Description}\nStart Time: {call.StartTime}\nDeadline: {call.DeadLine}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while retrieving call details: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid Call ID.");
            }
        }

        /// <summary>
        /// Updates an existing call's details.
        /// </summary>
        private static void UpdateCall()
        {
            Console.Write("Enter Call ID to Update: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                try
                {
                    var call = s_bl.Call.GetCallDetails(callId);
                    Console.Write("Enter New Address (leave blank to keep current): ");
                    string address = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(address))
                        call.Address = address;

                    Console.Write("Enter New Latitude: ");
                    if (double.TryParse(Console.ReadLine(), out double latitude))
                        call.Latitude = latitude;

                    Console.Write("Enter New Longitude: ");
                    if (double.TryParse(Console.ReadLine(), out double longitude))
                        call.Longitude = longitude;

                    Console.Write("Enter New Description (leave blank to keep current): ");
                    string description = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(description))
                        call.Description = description;

                    Console.Write("Enter New Start Time (yyyy-MM-dd HH:mm): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime startTime))
                        call.StartTime = startTime;

                    Console.Write("Enter New Deadline (yyyy-MM-dd HH:mm): ");
                    if (DateTime.TryParse(Console.ReadLine(), out DateTime deadline))
                        call.DeadLine = deadline;

                    s_bl.Call.UpdateCallAsync(call);
                    Console.WriteLine("Call updated successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while updating the call: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid Call ID.");
            }
        }

        /// <summary>
        /// Deletes a specific call from the system.
        /// </summary>
        private static void DeleteCall()
        {
            Console.Write("Enter Call ID to Delete: ");
            if (int.TryParse(Console.ReadLine(), out int callId))
            {
                try
                {
                    s_bl.Call.DeleteCall(callId);
                    Console.WriteLine("Call deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while deleting the call: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid Call ID.");
            }
        }

        /// <summary>
        /// Adds a new call to the system.
        /// </summary>
        private static void AddCall()
        {
            Call call = new()
            {
                CallType = CallType.None
            };

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

            s_bl.Call.AddCallAsync(call);
        }

        /// <summary>
        /// Retrieves and displays a list of closed calls.
        /// </summary>
        private static void GetClosedCalls()
        {
            try
            {
                Console.WriteLine("Enter Volunteer ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                int volunteerId = id;

                Console.WriteLine("Enter Call Type (or press Enter to skip):");
                string callTypeInput = Console.ReadLine();
                Enum? callType = (Enum?)(string.IsNullOrEmpty(callTypeInput)
                    ? null
                    : Enum.Parse(typeof(BO.CallType), callTypeInput, true));

                Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                string sortFieldInput = Console.ReadLine();
                Enum? sortField = (Enum?)(string.IsNullOrEmpty(sortFieldInput)
                    ? null
                    : Enum.Parse(typeof(BO.ClosedCallSortField), sortFieldInput, true));

                var closedCalls = s_bl.Call.GetClosedCalls(volunteerId, callType, sortField).ToList();

                if (!closedCalls.Any())
                {
                    Console.WriteLine("No closed calls found.");
                    return;
                }

                foreach (var call in closedCalls)
                {
                    Console.WriteLine($"ID: {call.Id}, Address: {call.Address}, Open Time: {call.OpenTime}, End Time: {call.EndTime}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving closed calls: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieves and displays a list of open calls.
        /// </summary>
        private static void GetOpenCalls()
        {
            try
            {
                Console.WriteLine("Enter Volunteer ID: ");
                if (!int.TryParse(Console.ReadLine(), out int id))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                }
                int volunteerId = id;

                Console.WriteLine("Enter Call Type (or press Enter to skip):");
                string callTypeInput = Console.ReadLine();
                Enum? callType = (Enum?)(string.IsNullOrEmpty(callTypeInput) ? null : Enum.Parse(typeof(BO.CallType), callTypeInput, true));

                Console.WriteLine("Enter Sort Field (or press Enter to skip):");
                string sortFieldInput = Console.ReadLine();
                Enum? sortField = (Enum?)(string.IsNullOrEmpty(sortFieldInput) ? null : Enum.Parse(typeof(BO.ClosedCallSortField), sortFieldInput, true));

                var openCalls = s_bl.Call.GetOpenCalls(volunteerId, callType, sortField);

                if (!openCalls.Any())
                {
                    Console.WriteLine("No open calls found.");
                    return;
                }

                foreach (var call in openCalls)
                {
                    Console.WriteLine($"ID: {call.Id}, Description: {call.Description}, Address: {call.Address}, Distance: {call.Distance} kilometers");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while retrieving open calls: {ex.Message}");
            }
        }

        /// <summary>
        /// Completes a specific call.
        /// </summary>
        private static void CompleteCall()
        {
            Console.Write("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                Console.Write("Enter Assignment ID: ");
                if (int.TryParse(Console.ReadLine(), out int assignmentId))
                {
                    try
                    {
                        s_bl.Call.CompleteCall(volunteerId, assignmentId);
                        Console.WriteLine("Call completed successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while completing the call: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Assignment ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Volunteer ID.");
            }
        }

        /// <summary>
        /// Cancels a specific call.
        /// </summary>
        private static void CancelCall()
        {
            Console.Write("Enter Requester ID: ");
            if (int.TryParse(Console.ReadLine(), out int requesterId))
            {
                Console.Write("Enter Assignment ID: ");
                if (int.TryParse(Console.ReadLine(), out int assignmentId))
                {
                    try
                    {
                        s_bl.Call.CancelCall(requesterId, assignmentId);
                        Console.WriteLine("Call canceled successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while canceling the call: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Assignment ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Requester ID.");
            }
        }

        /// <summary>
        /// Assigns a volunteer to a specific call.
        /// </summary>
        private static void SelectionCall()
        {
            Console.Write("Enter Volunteer ID: ");
            if (int.TryParse(Console.ReadLine(), out int volunteerId))
            {
                Console.Write("Enter Call ID: ");
                if (int.TryParse(Console.ReadLine(), out int callId))
                {
                    try
                    {
                        s_bl.Call.selectionCall(volunteerId, callId, true);
                        Console.WriteLine("Volunteer successfully assigned to call.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"An error occurred while assigning the call: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid Call ID.");
                }
            }
            else
            {
                Console.WriteLine("Invalid Volunteer ID.");
            }
        }



        /// <summary>
        /// admin fonctions
        /// </summary>
        /// 
        //
        /// <summary>
        /// Admin function to get the current time.
        /// </summary>
        private static void GetCurrentTime()
        {
            try
            {
                DateTime currentTime = s_bl.Admin.GetCurrentTime();
                Console.WriteLine($"Current Time: {currentTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting the current time: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin function to update the clock.
        /// </summary>
        private static void UpdateClock()
        {
            Console.WriteLine("Enter Time Unit to Update (MINUTE, HOUR, DAY, MONTH, YEAR): ");
            string input = Console.ReadLine();

            if (Enum.TryParse(input, true, out TimeUnit timeUnit))
            {
                try
                {
                    s_bl.Admin.UpdateClock(timeUnit);
                    Console.WriteLine($"Clock updated successfully by one {timeUnit}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while updating the clock: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid Time Unit. Please enter a valid option (MINUTE, HOUR, DAY, MONTH, YEAR).");
            }
        }

        /// <summary>
        /// Admin function to get the risk time range.
        /// </summary>
        private static void GetRiskTime()
        {
            try
            {
                TimeSpan riskTime = s_bl.Admin.GetRiskTime();
                Console.WriteLine($"Risk Time Range: {riskTime}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while getting the risk time: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin function to set the risk time range.
        /// </summary>
        private static void SetRiskTime()
        {
            Console.WriteLine("Enter Risk Time Range in format HH:MM:SS: ");
            string input = Console.ReadLine();

            if (TimeSpan.TryParse(input, out TimeSpan riskTime))
            {
                try
                {
                    s_bl.Admin.SetRiskTime(riskTime);
                    Console.WriteLine($"Risk Time successfully updated to {riskTime}.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while setting the risk time: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Invalid TimeSpan format. Please use HH:MM:SS.");
            }
        }

        /// <summary>
        /// Admin function to reset the database.
        /// </summary>
        private static void ResetDatabase()
        {
            try
            {
                s_bl.Admin.ResetDB();
                Console.WriteLine("Database reset successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while resetting the database: {ex.Message}");
            }
        }

        /// <summary>
        /// Admin function to initialize the database.
        /// </summary>
        private static void InitializeDatabase()
        {
            try
            {
                s_bl.Admin.InitializeDB();
                Console.WriteLine("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while initializing the database: {ex.Message}");
            }
        }


    }
}