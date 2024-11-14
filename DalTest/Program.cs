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

        static void Main(string[] args)
        {
            try
            {
                RunApplication();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private static void RunApplication()
        {
            try
            {
                s_dAssignment?.Create(new Assignment());
                s_dalCall?.Create(new Call());
                s_daVolunteer?.Create(new Volunteer());
                s_dalConfig?.create(new Config());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in RunApplication: {ex.Message}");
            }
        }

        private static void CreateVolunteer()
        {
            try
            {
                s_daVolunteer?.Create(new Volunteer());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred in CreateVolunteer: {ex.Message}");
            }
        }

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

                if (int.TryParse(Console.ReadLine(), out int option))
                {
                    switch (option)
                    {
                        case 1:
                            continueRunning = false; // Exit
                            break;
                        case 2:
                            ShowAssignmentSubMenu(); // Show Submenu for Assignment
                            break;
                        case 3:
                            ShowCallSubMenu(); // Show Submenu for Call
                            break;
                        case 4:
                            ShowVolunteerSubMenu(); // Show Submenu for Volunteer
                            break;
                        case 5:
                            Initialization.Do(); // Initialize Data
                            break;
                        case 6:
                            DisplayAllData(); // Display All Data in the Database
                            break;
                        case 7:
                            ShowConfigSubMenu(); // Show Submenu for Configuration
                            break;
                        case 8:
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



        // Private method to display a submenu for a given entity
        private static void ShowEntityMenu(string entityName)
        {
            Console.WriteLine($"Submenu for {entityName}:");
            Console.WriteLine("1. Add a new object (Create)");
            Console.WriteLine("2. Show object by ID (Read)");
            Console.WriteLine("3. Show all objects (ReadAll)");
            Console.WriteLine("4. Update an object (Update)");
            Console.WriteLine("5. Delete an object (Delete)");
            Console.WriteLine("6. Delete all objects (DeleteAll)");
            Console.WriteLine("7. Go back");
        }

        // Private method to create a new entity
        private static void CreateEntity(string entityName)
        {
            try
            {
                Console.WriteLine($"Enter details to create a new {entityName}:");
                // Add logic here to collect entity data and create a new object
                if (entityName == "Assignment")
                {
                    s_dAssignment?.Create(new Assignment());
                }
                else if (entityName == "Call")
                {
                    s_dalCall?.Create(new Call());
                }
                else if (entityName == "Volunteer")
                {
                    s_daVolunteer?.Create(new Volunteer());
                }
                else if (entityName == "Config")
                {
                    s_dalConfig?.create(new Config());
                }
                Console.WriteLine($"{entityName} created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating {entityName}: {ex.Message}");
            }
        }

        // Private method to read an entity by ID
        private static void ReadEntity(string entityName)
        {
            try
            {
                Console.WriteLine($"Enter ID to read {entityName}:");
                string id = Console.ReadLine();
                // Add logic here to retrieve and display entity by ID
                Console.WriteLine($"{entityName} details for ID {id}:");
                // Example print (You need to replace it with actual fetching of entity)
                Console.WriteLine("Details of the entity will be displayed here.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading {entityName}: {ex.Message}");
            }
        }

        // Private method to update an entity
        private static void UpdateEntity(string entityName)
        {
            try
            {
                Console.WriteLine($"Enter ID to update {entityName}:");
                string id = Console.ReadLine();
                // Add logic here to update entity
                Console.WriteLine($"Updating {entityName} for ID {id}...");
                // Example of update
                Console.WriteLine("Entity updated successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating {entityName}: {ex.Message}");
            }
        }

        // Private method to delete an entity by ID
        private static void DeleteEntity(string entityName)
        {
            try
            {
                Console.WriteLine($"Enter ID to delete {entityName}:");
                string id = Console.ReadLine();
                // Add logic here to delete entity
                Console.WriteLine($"Deleting {entityName} with ID {id}...");
                Console.WriteLine($"{entityName} deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting {entityName}: {ex.Message}");
            }
        }

        // Private method to delete all entities of a given type
        private static void DeleteAllEntities(string entityName)
        {
            try
            {
                // Add logic here to delete all entities
                Console.WriteLine($"Deleting all {entityName}s...");
                Console.WriteLine($"All {entityName}s deleted successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all {entityName}s: {ex.Message}");
            }
        }

        // Private method for Config-specific actions (e.g., time adjustments)
        private static void ShowConfigMenu()
        {
            Console.WriteLine("Config Menu:");
            Console.WriteLine("1. Advance system clock by minute");
            Console.WriteLine("2. Advance system clock by hour");
            Console.WriteLine("3. Show current system clock value");
            Console.WriteLine("4. Set a new config value");
            Console.WriteLine("5. Reset config values");
            Console.WriteLine("6. Exit");
        }

        // Private method for advancing the system clock
        private static void AdvanceSystemClock()
        {
            Console.WriteLine("Enter number of minutes to advance the system clock:");
            int minutes = int.Parse(Console.ReadLine());
            // Logic to advance the clock by the given number of minutes
            Console.WriteLine($"System clock advanced by {minutes} minutes.");
        }

        // Private method for resetting all config values
        private static void ResetConfigValues()
        {
            s_dalConfig?.Reset();
            Console.WriteLine("Config values reset.");
        }
    }
}
