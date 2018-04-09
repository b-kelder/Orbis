using System;
using System.Collections.Generic;
using Orbis.Events.Helpers;
using Windows.Storage;

namespace Orbis.Events.Exporters
{
    /// <summary>
    /// Author: AukeM
    /// Exporter for text file exporting
    /// </summary>
    class TextFileExporter : DeviceWriterHelper, ILogExporter
    {
        // Used to write all logs to, when all logs are written, export the data string
        private string data;

        /// <summary>
        /// Export a list of logs to a text file
        /// </summary>
        /// <param name="logs">The list of logs that needs to be exported</param>
        public async void Export(List<Log> logs)
        {
            data = Environment.NewLine + Environment.NewLine + "Exported at: " + DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine;
            
            await CreateFile("Orbis Log", "txt", CreationCollisionOption.GenerateUniqueName);

            // Copy to array (prevent modification while writing exception), write to text file
            foreach (Log log in logs.ToArray())
            {
                // We place all data in a var, which in the end gets writen to a file (so we do not have to call WriteToCurrentFile a lot of times)
                data += log.ToString() + Environment.NewLine;
            }
            WriteToCurrentFile(data);
        }
    }
}
