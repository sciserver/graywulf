using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    /// <summary>
    /// When implemented, this interface provides functions to enumerate all source table of
    /// a given query.
    /// </summary>
    public interface ISourceTableConsumer
    {
        IEnumerable<ITableSource> EnumerateSourceTables(bool recursive);
    }
}
