using Dal;
using DalApi;
using DO;

namespace DalTest
{
    internal class Program
    {
        private static IAssignment? s_dAssignment = new AssignmentImplementation();
        private static ICall? s_dalCall = new CallImplementation();
        private static IVolunteer? s_daVolunteer = new VolunteerImplementation();
        private static IConfig? s_dalConfig = new ConfigImplementation();

        /// <summary>
        /// enums
        /// </summary>
        public enum MainMenuOption
        {
            ExitMainMenu=1,
            DisplaySubMenuAssignment,
            DisplaySubMenuCall,
            DisplaySubMenuVolunteer,
            InitializeData,
            DisplayAllData,
            DisplayConfigurationSubMenu,
            ResetDatabaseAndConfiguration
        }
        public enum ConfigSubMenuOption
        {
            ExitSubMenu,
            AdvanceSystemClockOneMinute,
            AdvanceSystemClockOneHour,
            DisplayCurrentSystemClockValue,
            SetNewConfigValue,
            DisplayCurrentConfigValue,
            ResetAllConfigValues
        }
         
        public enum selectedsubOption
        {
            Exit,                      // Exit the submenu
            Create,                    // Add a new object of the entity type to the list (Create)
            Read,                      // Display an object by ID (Read)
            ReadAll,                   // Display all objects of the entity type (ReadAll)
            Update,                    // Update an existing object's data (Update)
            Delete,                    // Delete an existing object from the list (Delete)
            DeleteAll                  // Delete all objects from the list (DeleteAll)
        }






        /// <summary>
        /// main
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            ShowMainMenu();
        }
        //main menu
        private static void ShowMainMenu()
        {
            bool continueRunning = true;
            while (continueRunning)
            {
                Console.Clear();
                Console.WriteLine("Main Menu:");
                Console.WriteLine("1. Exit");
                Console.WriteLine("2. Show Submenu for Assignment");
                Console.WriteLine("3. Show Submenu for Call");
                Console.WriteLine("4. Show Submenu for Volunteer");
                Console.WriteLine("5. Initialize Data");
                Console.WriteLine("6. Display All Data in the Database");
                Console.WriteLine("7. Show Submenu for Configuration");
                Console.WriteLine("8. Reset Database and Configuration Data");
                Console.Write("Please choose an option: ");

                if (int.TryParse(Console.ReadLine(), out int option) &&
                    Enum.IsDefined(typeof(MainMenuOption), option - 1))
                {
                    MainMenuOption selectedOption = (MainMenuOption)(option - 1);

                    switch (selectedOption)
                    {
                        case MainMenuOption.ExitMainMenu:
                            continueRunning = false; // Exit
                            break;
                        case MainMenuOption.DisplaySubMenuAssignment:
                            ShowSubMenu("Assignment");// Show Submenu for Assignment
                            break;
                        case MainMenuOption.DisplaySubMenuCall:
                            ShowSubMenu("Call"); // Show Submenu for Call
                            break; 
                        case MainMenuOption.DisplaySubMenuVolunteer:
                            ShowSubMenu("Volunteer"); // Show Submenu for Volunteer
                            break;
                        case MainMenuOption.InitializeData:
                            Initialization.Do();// Initialize Data
                            break;
                        case MainMenuOption.DisplayAllData:
                            DisplayAllData(); // Display All Data in the Database
                            break;
                        case MainMenuOption.DisplayConfigurationSubMenu:
                            ShowConfigSubMenu(); // Show Submenu for Configuration
                            break;
                        case MainMenuOption.ResetDatabaseAndConfiguration:
                            ResetDatabaseAndConfig(); // Reset Database and Configuration Data
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please choose again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose again.");
                }
            }
        }






        /// <summary>
        /// main menu option 
        /// </summary>
        //show general submenu for call, assignements, and volonteers
        public static void ShowSubMenu(string entityName)
        {
            bool continueRunning = true;
            while (continueRunning)
            {
                Console.Clear();
                ShowEntityMenu(entityName);
                Console.Write("Please choose an option: ");

                if (int.TryParse(Console.ReadLine(), out int option) &&
                    Enum.IsDefined(typeof(MainMenuOption), option - 1))
                {
                    MainMenuOption selectedOption = (MainMenuOption)(option - 1);

                    switch (selectedOption)
                    {
                        case MainMenuOption.ExitMainMenu:
                            continueRunning = false; // Exit
                            break;
                        case MainMenuOption.Create:
                            CreateEntity(entityName); // Create
                            break;
                        case MainMenuOption.Read:
                            ReadEntity(entityName); // Read
                            break;
                        case MainMenuOption.ReadAll:
                            DisplayAllData(); // ReadAll
                            break;
                        case MainMenuOption.Update:
                            UpdateEntity(entityName); // Update
                            break;
                        case MainMenuOption.Delete:
                            DeleteEntity(entityName); // Delete
                            break;
                        case MainMenuOption.DeleteAll:
                            DeleteAllEntities(entityName); // DeleteAll
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please choose again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose again.");
                }
            }
        }
        // ShowConfigSubMenu method with the options
        public static void ShowConfigSubMenu()
        {

            bool continueRunning = true;
            while (continueRunning)
            {
                Console.Clear();
                Console.WriteLine("Config Menu:");
                Console.WriteLine("1. Advance system clock by minute");
                Console.WriteLine("2. Advance system clock by hour");
                Console.WriteLine("3. Show current system clock value");
                Console.WriteLine("4. Set a new config value");
                Console.WriteLine("5. Reset config values");
                Console.WriteLine("6. Exit");
                Console.Write("Please choose an option: ");

                if (int.TryParse(Console.ReadLine(), out int option) &&
                    Enum.IsDefined(typeof(ConfigSubMenuOption), option))
                {
                    ConfigSubMenuOption selectedOption = (ConfigSubMenuOption)option;

                    switch (selectedOption)
                    {
                        case ConfigSubMenuOption.ExitSubMenu:
                            continueRunning = false;
                            break;
                        case ConfigSubMenuOption.AdvanceSystemClockOneMinute:
                            AdvanceSystemClockOneMinute();
                            break;
                        case ConfigSubMenuOption.AdvanceSystemClockOneHour:
                            AdvanceSystemClockOneHour();
                            break;
                        case ConfigSubMenuOption.DisplayCurrentSystemClockValue:
                            Console.WriteLine($"Current system clock value: {s_dalConfig?.Clock}");
                            break;
                        case ConfigSubMenuOption.SetNewConfigValue:
                            SetNewConfigValue();
                            break;
                        case ConfigSubMenuOption.DisplayCurrentConfigValue:
                            DisplayCurrentConfigValue();
                            break;
                        case ConfigSubMenuOption.ResetAllConfigValues:
                            ResetAllConfigValues();
                            break;
                        default:
                            Console.WriteLine("Invalid option. Please choose again.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid option. Please choose again.");
                }

                if (continueRunning) Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
        }
        //display all data
        public static void DisplayAllData()
        {
            try
            {

            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in DisplayAllData: {ex.Message}");
            }
        }
        //reset database and configuration
        public static void ResetDatabaseAndConfig()
        {
            try
            {
                s_dAssignment?.DeleteAll();
                s_dalCall?.DeleteAll();
                s_daVolunteer?.DeleteAll();
                s_dalConfig?.Reset();
                Console.WriteLine("Database and configuration data reset.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ResetDatabaseAndConfig: {ex.Message}");
            }
        }











        /// <summary>
        /// general submenu
        /// </summary>
        public static void CreateEntity(string entityName)
        {
            try
            {
                switch (entityName)
                {
                    case "Assignment":
                        var assignment = new Assignment();
                        assignment.SetDetailsFromUserInput();
                        s_dAssignment?.Add(assignment);
                        break;

                    case "Call":
                        var call = new Call();
                        call.SetDetailsFromUserInput();
                        s_dalCall?.Add(call);
                        break;

                    case "Volunteer":
                        var volunteer = new Volunteer();
                        volunteer.SetDetailsFromUserInput();
                        s_daVolunteer?.Add(volunteer);
                        break;

                    default:
                        break;
                }
                Console.WriteLine($"{entityName} created and added successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating {entityName}: {ex.Message}");
            }
        }





        /// <summary>
        /// configurw submenu
        /// </summary>
        // Private method to advance the system clock by one minute
        private static void AdvanceSystemClockOneMinute()
        {
            try
            {
                s_dalConfig?.Clock.AddMinutes(1);
                Console.WriteLine("System clock advanced by one minute.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in AdvanceSystemClockOneMinute: {ex.Message}");
            }
        }
        // Private method to advance the system clock by one hour
        private static void AdvanceSystemClockOneHour()
        {
            try
            {
                s_dalConfig?.Clock.AddHours(1);
                Console.WriteLine("System clock advanced by one hour.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in AdvanceSystemClockOneHour: {ex.Message}");
            }
        }
        // Private method to set a new configuration value
        private static void SetNewConfigValue()
        {
            bool continueConfiguring = true;
            while (continueConfiguring)
            {
                try
                {
                    Console.WriteLine("Please choose a configuration setting to update:");
                    Console.WriteLine("1. Update System Clock");
                    Console.WriteLine("2. Update Risk Range");
                    Console.WriteLine("3. Exit Configuration");

                    if (int.TryParse(Console.ReadLine(), out int choice))
                    {
                        switch (choice)
                        {
                            case 1:
                                Console.Write("Enter the new date and time for the Clock (format: yyyy-MM-dd HH:mm): ");
                                if (DateTime.TryParse(Console.ReadLine(), out DateTime newClock))
                                {
                                    s_dalConfig.Clock = newClock;
                                    Console.WriteLine("System clock updated successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid date and time format. Clock update failed.");
                                }
                                break;

                            case 2:
                                Console.Write("Enter the new Risk Range (in minutes): ");
                                if (int.TryParse(Console.ReadLine(), out int riskMinutes))
                                {
                                    s_dalConfig.RiskRange = TimeSpan.FromMinutes(riskMinutes);
                                    Console.WriteLine("Risk range updated successfully.");
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input. Risk range update failed.");
                                }
                                break;

                            case 3:
                                continueConfiguring = false; // Exit the configuration menu
                                break;

                            default:
                                Console.WriteLine("Invalid choice. Please choose again.");
                                break;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid choice. Please enter a valid number.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred in SetNewConfigValue: {ex.Message}");
                }
            }
        }
        // Private method to display the current configuration value
        private static void DisplayCurrentConfigValue()
        {
            try
            {
                Console.WriteLine("Current Configuration Values:");
                Console.WriteLine($"1. System Clock: {s_dalConfig.Clock}");
                Console.WriteLine($"2. Risk Range: {s_dalConfig.RiskRange.TotalMinutes} minutes");
                Console.WriteLine($"3. Next Call ID: {s_dalConfig.NextCallId}");
                Console.WriteLine($"4. Next Assignment ID: {s_dalConfig.NextAssignmentId}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in DisplayCurrentConfigValue: {ex.Message}");
            }
        }
        // Private method to reset all configuration values
        private static void ResetAllConfigValues()
        {
            try
            {
                s_dalConfig?.Reset();
                Console.WriteLine("All configuration values reset.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in ResetAllConfigValues: {ex.Message}");
            }
        }


    }
}
