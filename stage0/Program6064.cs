using System;

namespace Targil0
{
    partial class Program
    {
        static void Main(string[] args)
        {
            Welcome6659();
            Welcome6064();
            Console.ReadKey();
        }

        static partial void Welcome6064();
        private static void Welcome6659()
        {
            Console.Write("Enter your name:");

            string name = Console.ReadLine()!;

            Console.WriteLine($"{name}, welcome to my first console application!");
        }
    }
}
