using System.Collections.Generic;

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

        public void AddLog(string item, string type = DEFAULT_TYPE)
        {
            log.Add(new Log(item, type));
        }

        public List<Log> GetLog()
        {
            return log;
        }

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
