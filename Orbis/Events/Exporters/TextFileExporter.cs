using System;
using System.Collections.Generic;
using Orbis.Events.Helpers;

namespace Orbis.Events.Exporters
{
    class TextFileExporter : DeviceWriterHelper, ILogExporter
    {
        private string data;

        public async void Export(List<Log> logs)
        {
            // We parse all data to a var, which in the end gets writen to a file.
            data = "";

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
