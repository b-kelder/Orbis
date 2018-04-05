using System;
using System.Collections.Generic;
using Orbis.Events.Helpers;

namespace Orbis.Events.Exporters
{
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
            // We parse all data to a var, which in the end gets writen to a file.
            data = Environment.NewLine + Environment.NewLine + "Exported at: " + DateTime.Now.ToString() + Environment.NewLine + Environment.NewLine;

            // Pick folder and handle cancel actions
            bool folderPicked = await PickFolder();
            if (folderPicked)
            {
                await CreateFile("Orbis Log");

                foreach (Log log in logs)
                {
                    data += log.ToString() + Environment.NewLine;
                }
                WriteToCurrentFile(data);
            }
        }
    }
}
