using Orbis.Events.Exporters;
using Orbis.Events.Helpers;
using System;
using System.Collections.Generic;

namespace Orbis.Events
{
    /// <summary>
    /// Author: AukeM
    /// Log exporter to export logs to configuered formats
    /// </summary>
    class LogExporter : DeviceWriterHelper
    {
        private List<ILogExporter> exporters;

        public LogExporter()
        {
            exporters = new List<ILogExporter>();

            // Set all the default exporters
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

            // Check if the given list with logs are not null and contain at least 1 log object(.
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
                    System.Diagnostics.Debug.WriteLine("Export write issue: " + ex);
                }
            }
        }

        /// <summary>
        /// Export a log
        /// </summary>
        /// <param name="log">The log to export</param>
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
        /// <param name="exporter">The exporter to add</param>
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
        /// <param name="exporter">The exporter to remove</param>
        public void RemoveExporter(ILogExporter exporter)
        {
            if (exporters.Contains(exporter))
            {
                exporters.Remove(exporter);
            }
        }

        /// <summary>
        /// Set default exporters the system will always use
        /// </summary>
        private void SetDefaultExporters()
        {
            // Add an exporter to console, txt and xml
            exporters.Add(new ConsoleExporter());       // Console      (System.Diagnostics.Debug.WriteLine)
            exporters.Add(new TextFileExporter());      // Text File    (.txt) 
            exporters.Add(new XMLFileExporter());       // XML          (.xml)
        }
    }
}
