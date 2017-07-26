using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace SampleApplication_SRCL_sealed
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var srcl = new SRCL.Init())

            {
                // Your main function code goes here...
                //creating a process

                Thread.Sleep(15000);
                Console.WriteLine("Please enter the process to be created:");
                string prname;
                prname = Console.ReadLine();
                Console.WriteLine("Starting process: " + prname);

                Process process = System.Diagnostics.Process.Start(prname);
                process.WaitForExit();
                Thread.Sleep(8000);

            }
        }
    }
}
