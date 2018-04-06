using System;
using System.Collections.Generic;

namespace Orbis.Events
{
    class Log
    {
        private const string DEFAULT_FORMAT = "{0}: ({1}) > {2}";
        private string format;
        private Dictionary<string, string> data;

        /// <summary>
        /// Create a standard log with format options
        /// </summary>
        /// <param name="item">The item to log</param>
        /// <param name="type">The type to log</param>
        /// <param name="format">Display format of the log object</param>
        public Log(string item, string type, string format = DEFAULT_FORMAT)
        {
            data = new Dictionary<string, string>
            {
                { "Timestamp",  DateTime.Now.ToString() },
                { "Type",       type },
                { "Item",       item }
            };
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
        /// Convert the log to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            // Variable to hold the value array of the data Dictionary
            string[] arrayData = new string[data.Count];

            // Copy data to array
            data.Values.CopyTo(arrayData, 0);

            // Return formatted string in configuered format
            return String.Format(format, arrayData);
        }
    }
}
