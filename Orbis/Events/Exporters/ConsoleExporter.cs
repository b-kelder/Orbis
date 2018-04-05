using System;
using System.Collections.Generic;
using System.Diagnostics;

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
            Debug.WriteLine("\n\nExported at: " + DateTime.Now.ToString() + "\n\n");
            foreach (Log log in logs)
            {
                Debug.WriteLine(log.ToString());
            }
        }
    }
}
