using System;

namespace Orbis.Events
{
    class Log
    {
        public string Item { get; set; }
        public string Type { get; set; }
        public string Timestamp { get; set; }

        /// <summary>
        /// Create a log object
        /// </summary>
        /// <param name="item">The item to log</param>
        /// <param name="type">The type of the log</param>
        public Log(string item, string type)
        {
            Item        = item;
            Type        = type;
            Timestamp   = DateTime.Now.ToString();
        }

        /// <summary>
        /// Convert the log to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Timestamp + ":" + " (" + Type + ") > " + Item;
        }
    }
}
