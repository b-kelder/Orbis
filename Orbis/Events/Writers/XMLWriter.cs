using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Orbis.Events.Writers
{
    class XMLWriter : DeviceWriterHelper, ILogWriter
    {
        public async void Write(List<Log> logs)
        {
            // Pick folder and handle cancel actions
            bool folderPicked = await PickFolder();
            if (folderPicked)
            {
                StorageFile currentFile = await CreateFile("Orbis Log", "xml", CreationCollisionOption.GenerateUniqueName);

                using (IRandomAccessStream writeStream = await currentFile.OpenAsync(FileAccessMode.ReadWrite))
                {
                    Stream s = writeStream.AsStreamForWrite();
                    System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings
                    {
                        Async = true,
                        Indent = true,
                    };

                    using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(s, settings))
                    {
                        writer.WriteStartElement("Logs");

                        foreach (Log log in logs)
                        {
                            writer.WriteStartElement("Log");
                            writer.WriteElementString("Item", log.Item);
                            writer.WriteElementString("Type", log.Type);
                            writer.WriteElementString("Timestamp", log.Timestamp);
                            writer.WriteEndElement();
                        }
                        writer.Flush();
                        await writer.FlushAsync();
                    }
                }
            }
        }
    }
}
