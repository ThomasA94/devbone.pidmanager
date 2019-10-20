using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Serilog;

namespace devbone.pidmanager
{
    /// <summary>
    /// Handles the creation and the deletion of the pid file.
    /// </summary>
    public class PidManager
    {
        /// <summary>
        /// If true, the program will exit if a pid file write error occures.
        /// </summary>
        public bool ExitProgramOnPidFileWriteError { get; set; } = true;

        /// <summary>
        /// If false, nothing will be logged.
        /// </summary>
        public bool UseLogging { get; set; } = true;

        public delegate void ExitProgramOnWriteErrorHandler(object sender, ExitProgramOnWriteError eventArgs);

        /// <summary>
        /// Event is fired if the program will exit due to a write error of the pid file.
        /// </summary>
        public event ExitProgramOnWriteErrorHandler BeforeExitProgramOnWriteError;

        private readonly string _path;
        public string Path { get => this._path; }

        private readonly int _pid;
        public int Pid { get => this._pid; }

        /// <summary>
        /// Creates an instance of the <see cref="PidManager"/>
        /// </summary>
        public PidManager()
        {
            this._path = this.CreateFullPath(this.CreateDirectoryPath(), this.CreateFileName());
            this._pid = this.GetPid();

            this.Init();
        }


        public PidManager(string fullPathOrProgramName)
        {
            if (this.IsFullPath(fullPathOrProgramName))
            {
                this._path = fullPathOrProgramName;
            }
            else
            {
                this._path = this.CreateFullPath(this.CreateDirectoryPath(), this.CreateFileName(fullPathOrProgramName));
            }

            this._pid = this.GetPid();

            this.Init();
        }

        public PidManager(string fullPath, int pid)
        {
            this._path = fullPath;
            this._pid = pid;

            this.Init();
        }

        public PidManager(string directoryName, string programName)
        {
            this._path = this.CreateFullPath(directoryName, this.CreateFileName(programName));
            this._pid = this.GetPid();

            this.Init();
        }

        public PidManager(string directoryName, string programName, int pid)
        {
            this._path = this.CreateFullPath(directoryName, this.CreateFileName(programName));
            this._pid = pid;

            this.Init();
        }

        public PidManager(int pid, string programName)
        {
            this._path = this.CreateFullPath(this.CreateDirectoryPath(), this.CreateFileName(programName));
            this._pid = pid;

            this.Init();
        }



        private bool IsFullPath(string fullPathOrProgramName)
        {
            try
            {
                System.IO.Path.GetDirectoryName(fullPathOrProgramName);
                System.IO.Path.GetFileName(fullPathOrProgramName);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Writes the pid file and verifies the written value.
        /// </summary>
        /// <exception cref="PidException">Thrown when the verification was not successful.</exception>
        private void Init()
        {
            this.WritePidFile(this.Path, this.Pid);
            if (this.VerifyPidFile())
            {
                if (this.UseLogging) Log.Logger.Information("Successfully created PID file at " + this.Path);
            }
            else
            {
                string exceptionText = "Verification of pid file failed.";
                if (this.UseLogging) Log.Logger.Error(exceptionText);
                throw new PidException(exceptionText);
            }
        }

        ~PidManager()
        {
            this.DeletePidFile(this.Path);
        }

        private string CreateDirectoryPath()
        {
            string directory = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            directory = Directory.GetParent(directory).FullName;//TODO fix_pid path
            return directory;
        }

        /// <summary>
        /// Create the filename with the <see cref="System.AppDomain.CurrentDomain.FriendlyName"/> of this program and '.pid'.
        /// </summary>
        /// <returns></returns>
        private string CreateFileName()
        {
            return System.AppDomain.CurrentDomain.FriendlyName + ".pid";
        }

        /// <summary>
        /// Create the filename with the given <paramref name="programName"/> of this program and '.pid'.
        /// </summary>
        /// <param name="programName">The name which is used for the pid-filename.</param>
        /// <returns>The filename.</returns>
        private string CreateFileName(string programName)
        {
            return programName + ".pid";
        }

        /// <summary>
        /// Combines the given <paramref name="directory"/> and the given <paramref name="fileName"/>.
        /// </summary>
        /// <param name="directory">The directory of the pid file.</param>
        /// <param name="fileName">The name of the pid file.</param>
        /// <returns>The created path.</returns>
        private string CreateFullPath(string directory, string fileName)
        {
            return System.IO.Path.Combine(directory, fileName);
        }

        /// <summary>
        /// Returns the pid of the the current process.
        /// </summary>
        /// <returns>The pid of the the current process.</returns>
        private int GetPid()
        {
            int processId = System.Diagnostics.Process.GetCurrentProcess().Id;

            if (this.UseLogging) Log.Logger.Verbose("Found process id: " + processId, processId);

            return processId;
        }

        /// <summary>
        /// Write the pid id in the specified file.
        /// </summary>
        /// <param name="fullPath">The full path of the pid file.</param>
        /// <param name="pid">The process id.</param>
        /// <remarks>If <see cref="ExitProgramOnPidFileWriteError"/> is true, the program will exit on.</remarks>
        private void WritePidFile(string fullPath, int pid)
        {
            try
            {
                using (StreamWriter outputFile = new StreamWriter(fullPath))
                {
                    outputFile.Write(pid);
                }
                if (this.UseLogging) Log.Logger.Verbose("Wrote pid file with " + nameof(pid) + "='" + pid + "' to '" + fullPath + "'.");
            }
            catch
            {
                string msg = "Could not write pid file.";

                if (this.ExitProgramOnPidFileWriteError)
                {
                    if (this.BeforeExitProgramOnWriteError != null) this.BeforeExitProgramOnWriteError(this, new ExitProgramOnWriteError(this.Path, this.Pid));
                    if (this.UseLogging) Log.Logger.Fatal(msg + " Exit program.");
                    Environment.Exit(0);
                }
                else
                {
                    if (this.UseLogging) Log.Logger.Error(msg);
                }
            }
        }

        /// <summary>
        /// Reads the pidfile.
        /// </summary>
        /// <param name="fullPath">The full path to the pid file.</param>
        /// <returns>Returns the first line of the pid file.</returns>
        /// <remarks>All exceptions are ignored and only logged.</remarks>
        private string ReadPidFile(string fullPath)
        {
            string line = string.Empty;

            try
            {
                using (FileStream fileStream = new FileStream(fullPath, FileMode.Open))
                {
                    using (StreamReader reader = new StreamReader(fileStream))
                    {
                        line = reader.ReadLine();
                    }
                }
                if (this.UseLogging) Log.Logger.Verbose("Read pid file: '" + line + "'", line);
            }
            catch
            {
                if (this.UseLogging) Log.Logger.Warning("Could not read pid file.");
            }

            return line;
        }


        /// <summary>
        /// Deletes the pid file if it exists.
        /// </summary>
        /// <param name="fullPath">The full path to the pid file.</param>
        /// <remarks>All exceptions are ignored and only logged.</remarks>
        private void DeletePidFile(string fullPath)
        {
            try
            {
                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                }
                else
                {
                    if (this.UseLogging) Log.Logger.Debug("Could not find pid file. Ignoring exception...");
                }
            }
            catch
            {
                if (this.UseLogging) Log.Logger.Warning("Could not delete pid file. Ignoring exception...");
            }
        }


        /// <summary>
        /// Compares the current pid with the saved pid in the file. Returns true if they are identical.
        /// </summary>
        /// <returns>True if the current pid and the saved pid in the file are identical.</returns>
        public bool VerifyPidFile()
        {
            string readData = this.ReadPidFile(this.Path);
            if (readData == null || readData != this.Pid.ToString())
            {
                if (this.UseLogging) Log.Logger.Error("Pid file verification failed.");
                return false;
            }

            if (this.UseLogging) Log.Logger.Verbose("Pid file verification successfull.");
            return true;
        }

        /// <summary>
        /// Deletes the PID file in a clean way. Should be called before exiting the program.
        /// </summary>
        public void Close()
        {
            this.DeletePidFile(this.Path);
        }
    }
}
