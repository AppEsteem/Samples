using System;
using System.Threading;
using System.Diagnostics;

namespace SampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Please enter the process to be created:");
            string prname;
            prname = Console.ReadLine();
            Console.WriteLine("Starting process: " + prname);

            Process process = Process.Start(prname);
            process.WaitForExit();
            Thread.Sleep(8000);
        }
    }
}
