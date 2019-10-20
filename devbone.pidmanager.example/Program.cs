using System;
using devbone.pidmanager;

namespace devbone.pidmanager.example
{
    class Program
    {
        static void Main(string[] args)
        {
            PidManager manager = new PidManager();
            manager.BeforeExitProgramOnWriteError += Manager_BeforeExitProgramOnWriteError;

            System.Threading.Thread.Sleep(2000);

            manager.Close();

            Console.ReadLine();
        }

        private static void Manager_BeforeExitProgramOnWriteError(object myObject, EventArgs myArgs)
        {
            Console.WriteLine("Stop program due to pid file write error.");
        }
    }
}
