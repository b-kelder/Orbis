using System;
using System.Collections.Generic;

namespace Orbis.Events
{
    /// <summary>
    /// Author: AukeM
    /// Log object used to log data
    /// </summary>
    class Log
    {
        private const string DEFAULT_FORMAT = "{0}: ({1}) > {2}";   // The default format to use when no format is given

        private string format;                                      // The current format of the log object (how will it be displayed in e.g: textfiles/console)
        private Dictionary<string, string> data;                    // The data the log object holds

        /// <summary>
        /// Create a standard log with format options
        /// </summary>
        /// <param name="item">The item to log</param>
        /// <param name="type">The type to log</param>
        /// <param name="format">Display format of the log object</param>
        public Log(string item, string type, string format = DEFAULT_FORMAT)
        {
            // Create the data for the log to hold
            data = new Dictionary<string, string>
            {
                { "Timestamp",  DateTime.Now.ToString() },
                { "Type",       type },
                { "Item",       item }
            };

            // Set the format of the log, if none given, fallback to: DEFAULT_FORMAT
            this.format = format;
        }

        /// <summary>
        /// Create a custom log object with option to format
        /// </summary>
        /// <param name="data">The data to log with key and value</param>
        /// <param name="format">Display format of the log object</param>
        public Log(Dictionary<string, string> data, string format = DEFAULT_FORMAT)
        {
            this.data   = data;
            this.format = format;
        }

        /// <summary>
        /// Returns the log object data
        /// </summary>
        /// <returns>The data</returns>
        public Dictionary<string, string> GetData()
        {
            return data;
        }

        /// <summary>
        /// Get the current configuered format used to format log objects
        /// </summary>
        /// <returns>The string format</returns>
        public string GetFormat()
        {
            return format;
        }

        /// <summary>
        /// Convert the log to string
        /// </summary>
        /// <returns>Formatted string</returns>
        public override string ToString()
        {
            // As String.Format needs a string array, we copy the Dictionary.Values into an string array.

            // Variable to hold the value array of the data Dictionary
            string[] arrayData = new string[data.Count];

            // Copy data to array
            data.Values.CopyTo(arrayData, 0);

            // Return formatted string in configuered format
            return String.Format(format, arrayData);
        }
    }
}
