using System;
using System.Collections.Generic;
using Orbis.Events.Writers;

namespace Orbis.Events
{
    class LogWriter
    {
        private List<ILogWriter> writers;

        public LogWriter()
        {
            writers = new List<ILogWriter>();
        }

        /// <summary>
        /// Write a list of logs
        /// </summary>
        /// <param name="logs"></param>
        public void Write(List<Log> logs)
        {
            // Make sure we have something to write to
            if (writers != null && writers.Count > 0)
            {
                foreach (ILogWriter writer in writers)
                {
                    writer.Write(logs);
                }
            }
            else
            {
                throw new Exception("Writer exception: LogWriter called without any configuered writers.");
            }
        }

        /// <summary>
        /// Write a log to configuered format
        /// </summary>
        /// <param name="log"></param>
        public void Write(Log log)
        {
            Write
            (
                new List<Log> { log }
            );
        }

        /// <summary>
        /// Add a writer
        /// </summary>
        /// <param name="writer"></param>
        public void AddWriter(ILogWriter writer)
        {
            if (!writers.Contains(writer))
            {
                writers.Add(writer);
            }
        }

        /// <summary>
        /// Remove a writer
        /// </summary>
        /// <param name="writer"></param>
        public void RemoveWriter(ILogWriter writer)
        {
            if (writers.Contains(writer))
            {
                writers.Remove(writer);
            }
        }
    }
}
