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

            var cg = new MySqlQueryRenderer()
            {
                Options = new QueryRendererOptions()
                {
                    TableNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                    TableAliasRendering = resolveAliases ? AliasRendering.Always : AliasRendering.Never,
                    ColumnNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                    UdtMemberNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                    ColumnAliasRendering = resolveAliases ? AliasRendering.Always : AliasRendering.Never,
                    DataTypeNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                    FunctionNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                    IndexNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                    ConstraintNameRendering = resolveNames ? NameRendering.FullyQualified : NameRendering.Original,
                }
            };

            cg.Execute(w, ss);

            return w.ToString();
        }
    }
}
