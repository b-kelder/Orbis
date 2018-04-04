using System.Collections.Generic;
using System.Diagnostics;

namespace Orbis.Events.Exporters
{
    class ConsoleExporter : ILogExporter
    {
        public void Export(List<Log> logs)
        {
            foreach (Log log in logs)
            {
                Debug.WriteLine(log.ToString());
            }
        }
    }
}
