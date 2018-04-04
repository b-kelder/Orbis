using System;
using System.Collections.Generic;
using Orbis.Events.Exporters;

namespace Orbis.Events
{
    class LogExporter
    {
        private List<ILogExporter> exporters;

        public LogExporter()
        {
            exporters = new List<ILogExporter>();
        }

        /// <summary>
        /// Write a list of logs
        /// </summary>
        /// <param name="logs"></param>
        public void Write(List<Log> logs)
        {
            // Make sure we have something to write to
            if (exporters != null && exporters.Count > 0)
            {
                foreach (ILogExporter writer in exporters)
                {
                    writer.Export(logs);
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
        public void AddWriter(ILogExporter writer)
        {
            if (!exporters.Contains(writer))
            {
                exporters.Add(writer);
            }
        }

        /// <summary>
        /// Remove a writer
        /// </summary>
        /// <param name="writer"></param>
        public void RemoveWriter(ILogExporter writer)
        {
            if (exporters.Contains(writer))
            {
                exporters.Remove(writer);
            }
        }
    }
}
