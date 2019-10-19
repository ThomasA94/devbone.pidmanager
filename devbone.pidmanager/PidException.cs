using System;
using System.Collections.Generic;
using System.Text;

namespace devbone.pidmanager
{
    /// <summary>
    /// Handles the exceptions of for the <see cref="PidManager"/>.
    /// </summary>
    public class PidException : Exception
    {
        /// <summary>
        /// Creates a new instance of the <see cref="PidException"/>.
        /// </summary>
        public PidException() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="PidException"/>.
        /// </summary>
        /// <param name="message">A description of the exception.</param>
        public PidException(string message) : base(message)
        {

        }
    }
}
