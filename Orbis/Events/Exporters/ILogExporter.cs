using System.Collections.Generic;

namespace Orbis.Events.Exporters
{
    interface ILogExporter
    {
        /// <summary>
        /// Export a list of logs to a specific format
        /// </summary>
        /// <param name="logs">The logs to export</param>
        void Export(List<Log> logs);
    }
}
