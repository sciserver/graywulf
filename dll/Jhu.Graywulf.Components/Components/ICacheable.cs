using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// Supports caching of database schema reflection classes
    /// </summary>
    public interface ICacheable
    {
        /// <summary>
        /// Updates the object's time-stamp so it won't get dropped from the cache.
        /// </summary>
        void Touch();

        /// <summary>
        /// Gets a value indicating whether the object has to be cached.
        /// </summary>
        bool IsCacheable { get; }

        /// <summary>
        /// Gets the current version of the object in
        /// the cache
        /// </summary>
        long CachedVersion { get; }
    }
}
