using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    /// <summary>
    /// Used with statements that can define source tables which other parts of the
    /// query can reference.
    /// </summary>
    /// <remarks>
    /// A few examples:
    /// - SELECT statement FROM clause table can be referenced from select list or WHERE predicates
    /// - INSERT statement INTO clause table can be referenced from target column list
    /// </remarks>
    public interface ISourceTableCollection : ISourceTableConsumer
    {
        Dictionary<string, TableReference> ResolvedSourceTableReferences { get; }
    }
}
