using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orbis.UI
{
    /// <summary>
    ///     Exceptions thrown by Orbis UI.
    /// </summary>
    public class OrbisUIException : Exception
    {
        /// <summary>
        ///     Create a blank <see cref="OrbisUIException"/>.
        /// </summary>
        public OrbisUIException() { }

        /// <summary>
        ///     Create an <see cref="OrbisUIException"/> with the given message.
        /// </summary>
        /// <param name="message">
        ///     Info about the exception that occurred.
        /// </param>
        public OrbisUIException(string message) : base(message) { }

        /// <summary>
        ///     Create an <see cref="OrbisUIException"/> with the given message and inner exception.
        /// </summary>
        /// <param name="message">
        ///     Info about the exception that occurred.
        /// </param>
        /// <param name="inner">
        ///     The inner exception that was caught.
        /// </param>
        public OrbisUIException(string message, Exception inner) : base(message, inner) { }
    }
}
