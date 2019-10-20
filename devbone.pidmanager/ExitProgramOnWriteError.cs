using System;
using System.Collections.Generic;
using System.Text;

namespace devbone.pidmanager
{
    /// <summary>
    /// Stores <see cref="EventArgs"/> data for ExitProgramOnWriteError events.
    /// </summary>
    public class ExitProgramOnWriteError : EventArgs
    {
        /// <summary>
        /// The path of the pid file.
        /// </summary>
        public string Path { get; }
        
        /// <summary>
        /// The process id.
        /// </summary>
        public int Pid { get; }

        /// <summary>
        /// Creates an instance of <see cref="ExitProgramOnWriteError"/>.
        /// </summary>
        /// <param name="path">The path of the pid file.</param>
        /// <param name="pid">The process id.</param>
        public ExitProgramOnWriteError(string path, int pid)
        {
            this.Path = path;
            this.Pid = pid;
        }
    }
}
