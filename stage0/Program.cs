using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome6659();
            Console.ReadKey();
        }

        private static void Welcome6659()
        {
            Console.Write("Enter your name:");

            string name = Console.ReadLine()!;

            Console.WriteLine($"{name}, welcome to my first console application!");
        }
    }
}
