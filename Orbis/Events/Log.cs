using Orbis.Simulation;
using System;

namespace Orbis.Events
{
    class Log
    {
        public string Item { get; set; }
        public string Type { get; set; }
        public string Timestamp { get; set; }
        public string GameTimestamp { get; set; }

        /// <summary>
        /// Create a log object
        /// </summary>
        /// <param name="item">The item to log</param>
        /// <param name="type">The type of the log</param>
        public Log(string item, string type)
        {
            Item            = item;
            Type            = type;
            Timestamp       = DateTime.Now.ToString();
        }

        /// <summary>
        /// Create a log object with gametime
        /// </summary>
        /// <param name="item">The item to log</param>
        /// <param name="type">The type of the log</param>
        /// <param name="gameTime">The current gametime</param>
        public Log(string item, string type, DateTime gameTime)
        {
            Item = item;
            Type = type;
            Timestamp = DateTime.Now.ToString();
            GameTimestamp = gameTime.ToString("MMM yyyy");
        }

        /// <summary>
        /// Convert the log to string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            string gameTime = "";
            if (GameTimestamp != null)
            {
                gameTime = " | gametime:" + GameTimestamp;
            }
            return Timestamp + gameTime + " (" + Type + ") > " + Item;
        }
    }
}
