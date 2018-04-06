using System;

namespace Orbis.Events
{
    class Log
    {
        private const string DEFAULT_FORMAT = "{0}: ({1}) > {2}";
        private string format;
        private string[] data;

        /// <summary>
        /// Create a log object
        /// </summary>
        /// <param name="item">The item to log</param>
        /// <param name="type">The type of the log</param>
        public Log(string item, string type, string format = DEFAULT_FORMAT)
        {
            data[0]         = item;
            data[1]         = type;
            data[2]         = DateTime.Now.ToString();
            this.format     = format;
        }

        /// <summary>
        /// Log a string array
        /// </summary>
        /// <param name="data">The data to log</param>
        /// <param name="format">The format to use</param>
        public Log(string[] data, string format = DEFAULT_FORMAT)
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
            return String.Format(format, data);
        }
    }
}
