using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace SampleApplication
{
    class Program
    {
        static void Main(string[] args)
        {
                //creating a process

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
    