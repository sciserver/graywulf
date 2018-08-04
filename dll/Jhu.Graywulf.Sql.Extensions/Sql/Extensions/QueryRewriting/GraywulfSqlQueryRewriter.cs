using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Extensions.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryTraversal;

namespace Jhu.Graywulf.Sql.Extensions.QueryRewriting
{
    public class GraywulfSqlQueryRewriter : Jhu.Graywulf.Sql.QueryRewriting.SqlServer.SqlServerQueryRewriter
    {
        #region Private member variables

        private GraywulfSqlQueryRewriterOptions options;

        #endregion
        #region Properties

        public GraywulfSqlQueryRewriterOptions Options
        {
            get { return options; }
            set { options = value; }
        }

        #endregion
        #region Constructors and initializers

        public GraywulfSqlQueryRewriter()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.options = CreateOptions();
        }

        protected virtual GraywulfSqlQueryRewriterOptions CreateOptions()
        {
            return new GraywulfSqlQueryRewriterOptions();
        }

        #endregion
        #region Visitor sink implementation

        protected override void AcceptVisitor(SqlQueryVisitor visitor, IDatabaseObjectReference node)
        {
            // Nothing to do here during query rewriting
        }

        protected override void AcceptVisitor(SqlQueryVisitor visitor, Token node)
        {
            Accept((dynamic)node);
        }

        protected virtual void Accept(Token node)
        {
            // Fall-back case
        }

        #endregion
        #region Visitor entry points

        protected virtual void Accept(QuerySpecification qs)
        {
            if (Visitor.Pass == 1)
            {
                if (Options.AssignColumnAliases)
                {
                    AssignDefaultColumnAliases(qs);
                }
            }
        }

        protected virtual void Accept(SelectList selectList)
        {
            if (options.SubstituteStars)
            {
                var nsl = SubstituteStars(Visitor.CurrentQuerySpecification, selectList);
                selectList.ReplaceWith(nsl);
                selectList = nsl;
            }
        }

        protected virtual void Accept(IntoClause into)
        {
            // Create a magic statement and insert before the SELECT

            var selectStatement = Visitor.CurrentStatement;
            var parent = selectStatement.FindAscendant<Jhu.Graywulf.Sql.Parsing.StatementBlock>();
            var magic = new ServerMessageMagicToken()
            {
                DestinationTable = into.TargetTable.TableOrViewIdentifier.TableReference
            };

            var sb = Jhu.Graywulf.Sql.Parsing.StatementBlock.Create(magic, selectStatement);
            var be = BeginEndStatement.Create(sb);
            selectStatement.ReplaceWith(be);

            into.Remove();
        }

        protected virtual void Accept(OrderByClause orderby)
        {
            // TODO: Right now keep order by intact, it might be necessary for the TOP expression
            // to work correctly. Later the order by clause could be rewritten to match primary key
            // ordering.

            var selectStatement = Visitor.CurrentStatement as SelectStatement;

            if (selectStatement != null &&
                Visitor.TableContext.HasFlag(TableContext.OrderBy) &&
                !Visitor.TableContext.HasFlag(TableContext.Subquery))
            {
                orderby.Remove();
            }
        }

        #endregion
        #region Select list rewrite

        public SelectList SubstituteStars(QuerySpecification qs, SelectList selectList)
        {
            var ce = selectList.FindDescendant<ColumnExpression>();
            var subsl = selectList.FindDescendant<SelectList>();

            SelectList sl = null;

            if (ce.ColumnReference.IsStar)
            {
                // Build select list from the column list of
                // the referenced table, then replace current node
                if (ce.TableReference.IsUndefined)
                {
                    sl = SelectList.Create(qs);
                }
                else
                {
                    sl = SelectList.Create(ce.TableReference);
                }

                if (subsl != null)
                {
                    sl.Append(SubstituteStars(qs, subsl));
                }

                return sl;
            }
            else
            {
                if (subsl != null)
                {
                    selectList.Stack.Replace<SelectList>(SubstituteStars(qs, subsl));
                }

                return selectList;
            }
        }

        /// <summary>
        /// Adds default aliases to columns with no aliases specified in the query
        /// </summary>
        /// <param name="qs"></param>
        private void AssignDefaultColumnAliases(QuerySpecification qs)
        {
            bool subquery = Visitor.QueryContext.HasFlag(QueryContext.Subquery);
            bool singleColumnSubquery = Visitor.QueryContext.HasFlag(QueryContext.SemiJoin);
            var selectList = qs.SelectList;
            var aliases = new HashSet<string>(Jhu.Graywulf.Sql.Schema.SchemaManager.Comparer);
            var cnt = selectList.EnumerateColumnExpressions().Count();

            int q = 0;
            foreach (var ce in selectList.EnumerateColumnExpressions())
            {
                var cr = ce.ColumnReference;
                string alias;

                if (singleColumnSubquery && q > 0)
                {
                    throw NameResolutionError.SingleColumnSubqueryRequired(ce);
                }

                if (cr.ColumnAlias == null)
                {
                    if (cr.ColumnName == null)
                    {
                        if (subquery && !singleColumnSubquery && cnt > 1)
                        {
                            throw NameResolutionError.MissingColumnAlias(ce);
                        }
                        else
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("Col_{0}", cr.SelectListIndex));
                        }
                    }
                    else if (!subquery)
                    {
                        if (cr.TableReference != null && cr.TableReference.Alias != null)
                        {
                            alias = GetUniqueColumnAlias(aliases, String.Format("{0}_{1}", cr.TableReference.Alias, cr.ColumnName));
                        }
                        else
                        {
                            alias = GetUniqueColumnAlias(aliases, cr.ColumnName);
                        }
                    }
                    else
                    {
                        alias = null;
                    }
                }
                else
                {
                    // Alias is set explicitly, so do not make it unique forcibly
                    alias = cr.ColumnAlias;
                }

                if (alias != null)
                {
                    if (aliases.Contains(alias))
                    {
                        throw NameResolutionError.DuplicateColumnAlias(alias, ce);
                    }

                    aliases.Add(alias);
                    cr.ColumnAlias = alias;
                }

                q++;
            }
        }

        private string GetUniqueColumnAlias(HashSet<string> aliases, string alias)
        {
            int q = 0;
            var alias2 = alias;
            while (aliases.Contains(alias2))
            {
                alias2 = String.Format("{0}_{1}", alias, q);
                q++;
            }

            return alias2;
        }

        #endregion
    }
}
