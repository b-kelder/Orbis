using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            Log logData = new Log(item, type);
            log.Add(logData);
            Debug.WriteLine(logData.ToString());
        }

        public List<Log> GetLog()
        {
            return log;
        }
    }
}
