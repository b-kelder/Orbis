﻿using System.Collections.Generic;

namespace Orbis.Events
{
    class Logger
    {
        private const string DEFAULT_TYPE = "normal";
        private List<Log> log;

        public Logger()
        {
            log = new List<Log>();
        }

        /// <summary>
        /// Add a new log
        /// </summary>
        /// <param name="item">The text to log</param>
        /// <param name="type">The type</param>
        public void AddLog(string item, string type = DEFAULT_TYPE)
        {
            log.Add(new Log(item, type));
        }

        /// <summary>
        /// Fetch the current log
        /// </summary>
        /// <returns>The log</returns>
        public List<Log> GetLog()
        {
            return log;
        }

        /// <summary>
        /// Get a log by type
        /// </summary>
        /// <param name="type">The type to fetch</param>
        /// <returns>List of logs</returns>
        public List<Log> GetLogByType(string type)
        {
            List<Log> tempLog = new List<Log>();

            foreach (var logItem in log)
            {
                if (logItem.Type == type)
                {
                    tempLog.Add(logItem);
                }
            }
            return tempLog;
        }
    }
}