using System;
using System.Collections.Generic;
using Orbis.Events.Exporters;
using Orbis.Events.Helpers;

namespace Orbis.Events
{
    class LogExporter : DeviceWriterHelper
    {
        private List<ILogExporter> exporters;

        public LogExporter()
        {
            exporters = new List<ILogExporter>();

            // Set all the default exporter
            SetDefaultExporters();
        }

        /// <summary>
        /// Export a list of logs
        /// </summary>
        /// <param name="logs">The logs to export</param>
        public void Export(List<Log> logs)
        {
            // We need exporters to export data
            if (exporters == null || exporters.Count <= 0)
            {
                throw new Exception("Attempted to export without any configuered exporters.");
            }

            // Make sure we have something to write to.
            if (logs != null && logs.Count > 0)
            {
                try
                {
                    // Export in all export formats
                    foreach (ILogExporter exporter in exporters)
                    {
                        exporter.Export(logs);
                    }
                }
                catch(Exception ex)
                {
                    // TODO: Handle exception
                }
            }
        }

        /// <summary>
        /// Export a log
        /// </summary>
        /// <param name="log"></param>
        public void Export(Log log)
        {
            // To avoid duplication, put the log in a list, and use the list export method
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

        /// <summary>
        /// Set default exporters the system always will use.
        /// </summary>
        private void SetDefaultExporters()
        {
            // Add an exporter to console, txt and xml
            exporters.Add(new ConsoleExporter());       // Console Exporter
            exporters.Add(new TextFileExporter());      // Text File (.txt) Eporter
           // exporters.Add(new XMLFileExporter());       // XML exporter
        }
    }
}
