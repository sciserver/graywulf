using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing.Generator;

namespace Jhu.Graywulf.Sql.Extensions.Grammar
{
    [Grammar(
        Namespace = "Jhu.Graywulf.Sql.Extensions.Parsing",
        ParserName = "GraywulfSqlParser",
        Comparer = "StringComparer.InvariantCultureIgnoreCase",
        RootToken = "Jhu.Graywulf.Sql.Extensions.Parsing.StatementBlock")]
    public class GraywulfSqlGrammar : Jhu.Graywulf.Sql.Grammar.SqlGrammar
    {
        public static new Expression<Rule> QueryExpression = () =>
            Override
            (
                Sequence
                (
                    Must
                    (
                        QueryExpressionBrackets,
                        PartitionedQuerySpecification,
                        QuerySpecification
                    ),
                    May(Sequence(May(CommentOrWhitespace), QueryOperator, May(CommentOrWhitespace), QueryExpression))
                )
            );

        public static Expression<Rule> PartitionedQuerySpecification = () =>
            Inherit
            (
                QuerySpecification,
                Sequence
                (
                    Keyword("SELECT"),
                    May(Sequence(CommentOrWhitespace, Must(Keyword("ALL"), Keyword("DISTINCT")))),
                    May(Sequence(CommentOrWhitespace, TopExpression)),
                    May(CommentOrWhitespace),
                    SelectList,
                    May(Sequence(May(CommentOrWhitespace), IntoClause)),
                    May(CommentOrWhitespace), PartitionedFromClause,        // Partitioned queries must have a FROM clause
                    May(Sequence(May(CommentOrWhitespace), WhereClause)),
                    May(Sequence(May(CommentOrWhitespace), GroupByClause)),
                    May(Sequence(May(CommentOrWhitespace), HavingClause))
                )
            );

        public static Expression<Rule> PartitionedFromClause = () =>
            Inherit
            (
                FromClause,
                Sequence
                (
                    Keyword("FROM"),
                    May(CommentOrWhitespace),
                    PartitionedTableSourceExpression
                )
            );

        public static Expression<Rule> PartitionedTableSourceExpression = () =>
            Inherit
            (
                TableSourceExpression,
                Sequence
                (
                    PartitionedTableSourceSpecification,                    // The first table must be a partitioned one
                    May(Sequence(May(CommentOrWhitespace), JoinedTable))
                )
            );

        public static Expression<Rule> PartitionedTableSourceSpecification = () =>
            Inherit
            (
                TableSourceSpecification,
                Sequence
                (
                    SimpleTableSource,
                    May(CommentOrWhitespace),
                    TablePartitionClause
                )
            );
        
        public static Expression<Rule> TablePartitionClause = () =>
            Sequence
            (
                Keyword("PARTITION"), CommentOrWhitespace, Keyword("BY"),
                CommentOrWhitespace,
                ColumnIdentifier
            );
    }
}
