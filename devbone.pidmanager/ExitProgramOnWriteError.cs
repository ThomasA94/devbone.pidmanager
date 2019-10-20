using System;
using System.Collections.Generic;
using System.Text;

namespace devbone.pidmanager
{
    public class ExitProgramOnWriteError : EventArgs
    {
        public string Path { get; }
        public int Pid { get; }

        public ExitProgramOnWriteError(string path, int pid)
        {
            this.Path = path;
            this.Pid = pid;
        }
    }
}
