using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.LogicalExpressions
{
    public class LogicalExpressionsTestBase : SqlNameResolverTestBase
    {
        protected List<LogicalExpression> GenerateWhereClauseByTable(string sql)
        {
            var details = ParseAndResolveNames(sql);
            var res = new List<LogicalExpression>();

            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(details.ParsingTree);

            foreach (var key in details.SourceTableReferences.Keys)
            {
                var trs = details.SourceTableReferences[key];
                var where = scn.GenerateWherePredicatesSpecificToTable(trs);
                res.Add(where);
            }

            return res;
        }

        protected List<LogicalExpression> GenerateWhereClauseByTableReference(string sql)
        {
            var details = ParseAndResolveNames(sql);
            var res = new List<LogicalExpression>();

            var scn = new SearchConditionNormalizer();
            scn.CollectConditions(details.ParsingTree);

            foreach (var key in details.SourceTableReferences.Keys)
            {
                foreach (var tr in details.SourceTableReferences[key])
                {
                    res.Add(scn.GenerateWherePredicatesSpecificToTable(tr));
                }
            }

            return res;
        }

        protected string[] GetWhereClauses(string query)
        {
            var cn = new LogicalExpressions.SearchConditionNormalizer();

            var select = CreateSelect(query);
            var res = new List<string>();

            foreach (var qs in select.QueryExpression.EnumerateQuerySpecifications())
            {
                cn.CollectConditions(qs);

                // TODO use qs.SourceTableReferences ???
                foreach (var tr in qs.SourceTableReferences.Values)
                {
                    var where = cn.GenerateWherePredicatesSpecificToTable(tr);

                    if (where != null)
                    {
                        var cg = CreateCodeGenerator();
                        var sw = new StringWriter();
                        cg.Execute(sw, where);

                        res.Add(sw.ToString());
                    }
                    else
                    {
                        res.Add("");
                    }
                }
            }

            return res.ToArray();
        }

        protected void GetSearchCondition(string query, out QuerySpecification qs, out LogicalExpression where)
        {
            var select = ParseAndResolveNames<SelectStatement>(query);
            qs = select.FindDescendant<QueryExpression>().FirstQuerySpecification;
            where = qs.FindDescendant<WhereClause>().FindDescendant<LogicalExpression>();
        }
    }
}
