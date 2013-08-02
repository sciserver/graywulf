using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Supports accessing the indexes of a database object
    /// </summary>
    public interface IIndexes
    {
        /// <summary>
        /// Gets the collection of indexes
        /// </summary>
        ConcurrentDictionary<string, Index> Indexes { get; }
    }
}
