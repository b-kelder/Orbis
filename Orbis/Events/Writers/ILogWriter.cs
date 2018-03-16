
using System.Collections.Generic;

namespace Orbis.Events.Writers
{
    interface ILogWriter
    {
        /// <summary>
        /// Write a list of logs
        /// </summary>
        /// <param name="logs"></param>
        void Write(List<Log> logs);
    }
}
