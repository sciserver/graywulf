using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Sql.Schema
{
    /// <summary>
    /// Supports accessing the columns of a database objects
    /// </summary>
    public interface IColumns : IDatabaseObject
    {
        /// <summary>
        /// Gets the collection of columns
        /// </summary>
        ConcurrentDictionary<string, Column> Columns { get; }

        QuantityIndex Quantities { get; }
    }
}
