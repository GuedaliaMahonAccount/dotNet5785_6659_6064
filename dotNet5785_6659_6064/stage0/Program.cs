using System;

namespace Targil0
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter your name:");

            // Read the user's name from the console
            string name = Console.ReadLine();

            // Display the welcome message with the user's name
            Console.WriteLine("{0}, welcome to my first console application!", name);
        }
    }
}
