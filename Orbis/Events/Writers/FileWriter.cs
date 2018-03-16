
namespace Orbis.Events.Writers
{
    class FileWriter : DeviceWriterHelper, ILogWriter
    {
        public async void Write(Log log)
        {
            // Pick folder and handle cancel actions
            bool folderPicked = await PickFolder();
            if (folderPicked)
            {
                await CreateFile("Orbis Log");
                WriteToCurrentFile(log.ToString());
            }
        }
    }
}
