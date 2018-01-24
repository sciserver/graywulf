using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Schema
{
    /// <summary>
    /// Supports accessing the parameters of a database objects
    /// </summary>
    public interface IParameters
    {
        /// <summary>
        /// Gets the collection of parameters
        /// </summary>
        ConcurrentDictionary<string, Parameter> Parameters { get; }
    }
}
