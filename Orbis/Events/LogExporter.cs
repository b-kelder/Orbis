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
        /// Export a list of logs
        /// </summary>
        /// <param name="logs">The logs to export</param>
        public void Export(List<Log> logs)
        {
            // Make sure we have something to write to
            if (exporters != null && exporters.Count > 0)
            {
                foreach (ILogExporter exporter in exporters)
                {
                    exporter.Export(logs);
                }
            }
            else
            {
                throw new Exception("Exporter exception: LogExporter called without any configuered exporters.");
            }
        }

        /// <summary>
        /// Export a log
        /// </summary>
        /// <param name="log"></param>
        public void Export(Log log)
        {
            Export
            (
                new List<Log> { log }
            );
        }

        /// <summary>
        /// Add an exporter
        /// </summary>
        /// <param name="exporter"></param>
        public void AddExporter(ILogExporter exporter)
        {
            if (!exporters.Contains(exporter))
            {
                exporters.Add(exporter);
            }
        }

        /// <summary>
        /// Remove an exporter
        /// </summary>
        /// <param name="exporter"></param>
        public void RemoveExporter(ILogExporter exporter)
        {
            if (exporters.Contains(exporter))
            {
                exporters.Remove(exporter);
            }
        }
    }
}
