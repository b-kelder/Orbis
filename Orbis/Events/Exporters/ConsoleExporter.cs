using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Orbis.Events.Exporters
{
    class ConsoleExporter : ILogExporter
    {
        /// <summary>
        /// Export a list of logs to the debug console
        /// </summary>
        /// <param name="logs">The list of logs that needs to be exported</param>
        public void Export(List<Log> logs)
        {
            Task.Run(() =>
            {
                Debug.WriteLine("\n\nExported at: " + DateTime.Now.ToString() + "\n\n");
                foreach (Log log in logs.ToArray())
                {
                    Debug.WriteLine(log.ToString());
                }
            });
        }
    }
}
