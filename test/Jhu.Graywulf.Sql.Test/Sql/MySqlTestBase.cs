using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryRendering;
using Jhu.Graywulf.Sql.QueryRendering.MySql;

namespace Jhu.Graywulf.Sql
{
    public abstract class MySqlTestBase : Jhu.Graywulf.Test.TestClassBase
    {
        protected virtual string RenderQuery(string query, bool resolveAliases, bool resolveNames, bool substituteStars)
        {
            var ss = ParseAndResolveNames<SelectStatement>(query);
            var w = new StringWriter();

            var cg = new MySqlQueryRenderer();
            cg.TableNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.TableAliasRendering = resolveAliases ? AliasRendering.Always : AliasRendering.Never;
            cg.ColumnNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.ColumnAliasRendering = resolveAliases ? AliasRendering.Always : AliasRendering.Never;
            cg.DataTypeNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.FunctionNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original;
            cg.Execute(w, ss);

            return w.ToString();
        }
    }
}
