using System;
using devbone.pidmanager;

namespace devbone.pidmanager.example
{
    class Program
    {
        private static PidManager manager;
        static void Main(string[] args)
        {
            manager = new PidManager();
            manager.BeforeExitProgramOnWriteError += Manager_BeforeExitProgramOnWriteError;
            Console.WriteLine(manager.Pid);
            Console.WriteLine(manager.Path);

            ExitProgram();
        }

        private static void Manager_BeforeExitProgramOnWriteError(object myObject, EventArgs myArgs)
        {
            Console.WriteLine("Stop program due to pid file write error.");
        }

        private static void ExitProgram()
        {
            manager.Close();

            Console.ReadLine();
            Environment.Exit(0);
        }
    }
}
