
using System;
using System.Collections.Generic;

namespace Orbis.Events.Writers
{
    class TextFileWriter : DeviceWriterHelper, ILogWriter
    {
        public async void Write(List<Log> logs)
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
