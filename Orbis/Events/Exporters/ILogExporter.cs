using System.Collections.Generic;

namespace Orbis.Events.Exporters
{
    /// <summary>
    /// Author: AukeM
    /// Interface used for exporter handlers.
    /// </summary>
    interface ILogExporter
    {
        /// <summary>
        /// Export a list of logs to a specific format
        /// </summary>
        /// <param name="logs">The logs to export</param>
        void Export(List<Log> logs);
    }
}
