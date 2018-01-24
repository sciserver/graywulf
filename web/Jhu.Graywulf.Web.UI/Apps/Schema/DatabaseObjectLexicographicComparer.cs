using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public class DatabaseObjectLexicographicComparer : IComparer<DatabaseObject>
    {
        public int Compare(DatabaseObject x, DatabaseObject y)
        {
            var c = SchemaManager.Comparer.Compare(x.SchemaName, y.SchemaName);

            if (c != 0)
            {
                return c;
            }

            c = SchemaManager.Comparer.Compare(x.ObjectName, y.ObjectName);

            return c;
        }
    }
}