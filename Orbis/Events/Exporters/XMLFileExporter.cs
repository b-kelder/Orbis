using Orbis.Events.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Windows.Storage;
using Windows.Storage.Streams;

namespace Orbis.Events.Exporters
{
    /// <summary>
    /// Author: AukeM
    /// Exporter class for xml files
    /// </summary>
    class XMLFileExporter : DeviceWriterHelper, ILogExporter
    {
        /// <summary>
        /// Export a list of logs to an XML file
        /// </summary>
        /// <param name="logs">The list of logs that needs to be exported</param>
        public async void Export(List<Log> logs)
        {
            // Create a new file, if duplicate, create unique name
            StorageFile currentFile = await CreateFile("Orbis Log", "xml", CreationCollisionOption.GenerateUniqueName);
            using (IRandomAccessStream writeStream = await currentFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                // Create an output stream to write to
                Stream s = writeStream.AsStreamForWrite();

                // Create settings to write Async and use indent in file
                System.Xml.XmlWriterSettings settings = new System.Xml.XmlWriterSettings
                {
                    Async = true,
                    Indent = true,
                };

                // Create the actual writer with defined settings to write to file
                using (System.Xml.XmlWriter writer = System.Xml.XmlWriter.Create(s, settings))
                {
                    writer.WriteStartElement("Logs");
                    string timestamp = DateTime.Now.ToString();

                    // Write each log in a new Log node
                    foreach (Log log in logs.ToArray())
                    {
                        writer.WriteStartElement("Log");
                        writer.WriteAttributeString("exportedAt", timestamp);   // Timestamp of when the log got exported

                        // Write al log data dynamically
                        foreach (KeyValuePair<string, string> data in log.GetData())
                        {
                            writer.WriteElementString(data.Key, data.Value);
                        }
                        writer.WriteEndElement();
                    }
                    await writer.FlushAsync();
                }
            }
        }
    }
}
