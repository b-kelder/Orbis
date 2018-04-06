using System;
using System.Collections.Generic;

namespace Orbis.Events
{
    class Logger
    {
        private const string DEFAULT_TYPE = "normal";   // Default log type
        private List<Log> log;                          // List that contains all the logs
        private static Logger logger;

        private Logger()
        {
            log = new List<Log>();
        }

        /// <summary>
        /// Get instance of logger
        /// </summary>
        /// <returns></returns>
        public static Logger GetInstance()
        {
            if (logger == null)
            {
                logger = new Logger();
            }
            return logger;
        }


        /// <summary>
        /// Add log with item and type
        /// </summary>
        /// <param name="item">The text to log</param>
        /// <param name="type">The type of log</param>
        public void Add(string item, string type = DEFAULT_TYPE)
        {
            log.Add(new Log(item, type));
        }

        /// <summary>
        /// Add log with gametimestamp
        /// </summary>
        /// <param name="item"></param>
        /// <param name="gameTime"></param>
        /// <param name="type"></param>
        public void AddWithGameTime(string item, DateTime gameTime, string type = DEFAULT_TYPE)
        {
            string[] data = { item, gameTime.ToString("MMM yyyy"), type, item };
            log.Add(new Log(data, "{3} | gametime: {0}: ({1}) > {2}"));
        }



        /// <summary>
        /// Fetch the current log
        /// </summary>
        /// <returns>The log</returns>
        public List<Log> GetLog()
        {
            return log;
        }
    }
}
