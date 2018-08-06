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
        public static new Expression<Rule> AnyStatement = () =>
            Override
            (
                Must
                (
                    Label,
                    GotoStatement,
                    BeginEndStatement,
                    WhileStatement,
                    BreakStatement,
                    ContinueStatement,
                    ReturnStatement,
                    IfStatement,
                    TryCatchStatement,
                    ThrowStatement,

                    DeclareCursorStatement,
                    SetCursorStatement,
                    CursorOperationStatement,
                    FetchStatement,

                    DeclareVariableStatement,
                    SetVariableStatement,

                    DeclareTableStatement,

                    CreateTableStatement,
                    DropTableStatement,
                    TruncateTableStatement,

                    CreateIndexStatement,
                    DropIndexStatement,

                    PartitionedSelectStatement,      //

                    SelectStatement,
                    InsertStatement,
                    UpdateStatement,
                    DeleteStatement
                )
            );

        public static Expression<Rule> PartitionedSelectStatement = () =>
            Inherit
            (
                SelectStatement,
                Sequence
                (
                    May(Sequence(CommonTableExpression, May(CommentOrWhitespace))),
                    PartitionedQueryExpression,
                    May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                    May(Sequence(May(CommentOrWhitespace), OptionClause))
                )
            );

        // No UNION, EXCEPT etc. are allowed with partitioned queries
        public static Expression<Rule> PartitionedQueryExpression = () =>
            Inherit
            (
                QueryExpression,
                PartitionedQuerySpecification
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
                PartitionedTableSource
            );

        public static Expression<Rule> PartitionedTableSource = () =>
            Inherit
            (
                SimpleTableSource,
                Sequence
                (
                    TableOrViewIdentifier,
                    May(Sequence(May(CommentOrWhitespace), May(Sequence(Keyword("AS"), May(CommentOrWhitespace))), TableAlias)),   // Optional
                    May(Sequence(May(CommentOrWhitespace), TableSampleClause)),
                    May(Sequence(May(CommentOrWhitespace), TableHintClause)),
                    May(CommentOrWhitespace), TablePartitionClause
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
