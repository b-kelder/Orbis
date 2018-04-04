using System;
using System.Collections.Generic;
using Orbis.Events.Helpers;

namespace Orbis.Events.Exporters
{
    class TextFileExporter : DeviceWriterHelper, ILogExporter
    {
        public async void Export(List<Log> logs)
        {
            // Pick folder and handle cancel actions
            bool folderPicked = await PickFolder();
            if (folderPicked)
            {
                await CreateFile("Orbis Log");

                foreach (Log log in logs)
                {
                    WriteToCurrentFile(log.ToString() + Environment.NewLine);
                }
            }
        }
    }
}
