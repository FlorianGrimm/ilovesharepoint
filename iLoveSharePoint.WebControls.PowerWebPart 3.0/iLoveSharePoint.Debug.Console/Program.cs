using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.Threading;

namespace iLoveSharePoint.Debug
{
    class Program
    {
        internal static AutoResetEvent waitHandle;

        static void Main(string[] args)
        {
            Console.Title = "iLove SharePoint - Debug Console 1.0";

            ServiceHost debugService = null;
            try
            {
                Console.BackgroundColor = ConsoleColor.White;        
                Console.Clear();       
                
                Console.BufferWidth = 150;
                Console.BufferHeight = 400;

                if(Console.LargestWindowWidth >= 150)
                    Console.WindowWidth = 150;
                if (Console.LargestWindowHeight >= 50)
                    Console.WindowHeight = 50;
                
                debugService = new ServiceHost(typeof(DebugConsole));
                debugService.Open();
                Console.TreatControlCAsInput = false;

                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine("iLove SharePoint - Debug Console 1.0");
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("Press Ctrl+C to exit...");
                Console.ForegroundColor = ConsoleColor.DarkBlue;
                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkBlue;

                waitHandle = new AutoResetEvent(false);
                waitHandle.WaitOne();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
            }
            finally
            {
                debugService.Close();
                debugService = null;
            }
       
        }
    }
}
