using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.SqlParser.Generator
{
    [Grammar(Namespace = "Jhu.Graywulf.SqlParser", ParserName = "SqlParser",
        Comparer="StringComparer.InvariantCultureIgnoreCase", RootToken = "SelectStatement")]
    public class SqlGrammar : Grammar
    {

        #region Symbols (matched by value)

        public static Expression<Symbol> Plus = () => @"+";
        public static Expression<Symbol> Minus = () => @"-";
        public static Expression<Symbol> Mul = () => @"*";
        public static Expression<Symbol> Div = () => @"/";
        public static Expression<Symbol> Mod = () => @"%";

        public static Expression<Symbol> BitwiseNot = () => @"~";
        public static Expression<Symbol> BitwiseAnd = () => @"&";
        public static Expression<Symbol> BitwiseOr = () => @"|";
        public static Expression<Symbol> BitwiseXor = () => @"^";

        public static Expression<Symbol> Equals = () => @"=";
        public static Expression<Symbol> Equals2 = () => @"==";
        public static Expression<Symbol> LessOrGreaterThan = () => @"<>";
        public static Expression<Symbol> NotEquals = () => @"!=";
        public static Expression<Symbol> NotLessThan = () => @"!<";
        public static Expression<Symbol> NotGreaterThan = () => @"!>";
        public static Expression<Symbol> LessOrEqualThan = () => @"<=";
        public static Expression<Symbol> GreaterOrEqualThan = () => @">=";
        public static Expression<Symbol> LessThan = () => @"<";
        public static Expression<Symbol> GreaterThan = () => @">";

        public static Expression<Symbol> DoubleColon = () => @"::";
        public static Expression<Symbol> Dot = () => @".";
        public static Expression<Symbol> Comma = () => @",";
        public static Expression<Symbol> Semicolon = () => @";";
        public static Expression<Symbol> Colon = () => @":";

        public static Expression<Symbol> BracketOpen = () => @"(";
        public static Expression<Symbol> BracketClose = () => @")";
        public static Expression<Symbol> VectorOpen = () => @"{";
        public static Expression<Symbol> VectorClose = () => @"}";

        #endregion
        #region Terminals (matched by regular expressions)

        public static Expression<Whitespace> Whitespace = () => @"\G\s+";
        public static Expression<Comment> SingleLineComment = () => @"\G--.*";
        public static Expression<Comment> MultiLineComment = () => @"\G(?sm)/\*.*?\*/";
        public static Expression<Terminal> Number = () => @"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)";
        public static Expression<Terminal> StringConstant = () => @"\G('([^']|'')*')";
        public static Expression<Terminal> Identifier = () => @"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])";
        public static Expression<Terminal> Variable = () => @"\G(@[$a-zA-Z_]+)";
        public static Expression<Terminal> SystemVariable = () => @"\G(@@[$a-zA-Z_]+)";

        #endregion
        #region Arithmetic operators used in expressions

        public static Expression<Rule> UnaryOperator = () =>
            Must(Plus, Minus, BitwiseNot);
        public static Expression<Rule> ArithmeticOperator = () =>
            Must(Plus, Minus, Mul, Div, Mod);
        public static Expression<Rule> BitwiseOperator = () =>
            Must(BitwiseAnd, BitwiseOr, BitwiseXor);
        public static Expression<Rule> ComparisonOperator = () =>
            Must(Equals2, Equals, LessOrGreaterThan, NotEquals,
            NotLessThan, NotGreaterThan, LessOrEqualThan, GreaterOrEqualThan,
            LessThan, GreaterThan);

        #endregion
        #region Logical operators used in search conditions

        public static Expression<Rule> LogicalNot = () => Keyword("NOT");
        public static Expression<Rule> LogicalOperator = () => Must(Keyword("AND"), Keyword("OR"));

        #endregion
        #region Identifiers

        public static Expression<Rule> DatasetName = () => Identifier;
        public static Expression<Rule> DatabaseName = () => Identifier;
        public static Expression<Rule> SchemaName = () => Identifier;
        public static Expression<Rule> TableName = () => Identifier;
        public static Expression<Rule> DerivedTable = () => Identifier;
        public static Expression<Rule> TableAlias = () => Identifier;
        public static Expression<Rule> FunctionName = () => Identifier;
        public static Expression<Rule> ColumnName = () => Identifier;
        public static Expression<Rule> ColumnAlias = () => Identifier;
        public static Expression<Rule> ColumnPosition = () => Number;
        public static Expression<Rule> UdtColumnName = () => Identifier;
        public static Expression<Rule> PropertyName = () => Identifier;
        public static Expression<Rule> SampleNumber = () => Number;
        public static Expression<Rule> RepeatSeed = () => Number;
        public static Expression<Rule> IndexValue = () => Identifier;

        #endregion
        #region Arithemtic expressions

        public static Expression<Rule> CommentOrWhitespace = () =>
            Sequence
            (
                Must(MultiLineComment, SingleLineComment, Whitespace),
                May(CommentOrWhitespace)
            );

        public static Expression<Rule> Expression = () =>
            Sequence
            (
                Must
                (
                    ExpressionBrackets,
                    FunctionCall,
                    Sequence(UnaryOperator, May(CommentOrWhitespace), Number),
                    Sequence(UnaryOperator, May(CommentOrWhitespace), AnyVariable),
                    Number,
                    AnyVariable,
                    StringConstant,
                    SimpleCaseExpression,
                    SearchedCaseExpression
                ),
                May
                (
                    Must
                    (
                        Sequence(May(CommentOrWhitespace), ArithmeticOperator, May(CommentOrWhitespace), Expression),
                        Sequence(May(CommentOrWhitespace), BitwiseOperator, May(CommentOrWhitespace), Expression)
                    )
                )
            );

        public static Expression<Rule> ExpressionBrackets = () =>
            Sequence(BracketOpen, May(CommentOrWhitespace), Expression, May(CommentOrWhitespace), BracketClose);

        public static Expression<Rule> AnyVariable = () =>
            Must(ColumnIdentifier, SystemVariable, Variable);

        #endregion
        #region Case-When constructs *** TODO: test, especially whitespaces

        public static Expression<Rule> SimpleCaseExpression = () =>
            Sequence
            (
                Keyword("CASE"),
                May(CommentOrWhitespace),
                Expression,
                May(CommentOrWhitespace),
                SimpleCaseWhenList,
                May(CaseElse),
                Keyword("END")
            );

        public static Expression<Rule> SimpleCaseWhenList = () =>
            Sequence(SimpleCaseWhen, May(SimpleCaseWhenList));

        // *** TODO: add whitespaces
        public static Expression<Rule> SimpleCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                Expression,
                Keyword("THEN"),
                Expression
            );

        // *** TODO: add whitespaces
        public static Expression<Rule> SearchedCaseExpression = () =>
            Sequence
            (
                Keyword("CASE"),
                SearchedCaseWhenList,
                May(CaseElse),
                Keyword("END")
            );

        public static Expression<Rule> SearchedCaseWhenList = () =>
            Sequence(SearchedCaseWhen, May(SearchedCaseWhenList));

        // *** TODO: add whitespaces
        public static Expression<Rule> SearchedCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                //SearchCondition,
                Keyword("THEN"),
                Expression
            );

        // *** TODO: add whitespaces
        public static Expression<Rule> CaseElse = () =>
            Sequence
            (
                Keyword("ELSE"),
                Expression
            );

        #endregion
        #region Table and column names

        public static Expression<Rule> TableOrViewName = () =>
            Sequence
            (
                // Dataset prefix
                May(Sequence(DatasetName, May(CommentOrWhitespace), Colon, May(CommentOrWhitespace))),
                // Standard table name
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(Sequence(May(CommentOrWhitespace), SchemaName)), May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName),
                    TableName
                )
            );

        public static Expression<Rule> ColumnIdentifier = () =>
            Must
            (
                Sequence
                (
                // Optional dataset prefix
                    May(Sequence(DatasetName, May(CommentOrWhitespace), Colon, May(CommentOrWhitespace))),
                // Original column name syntax
                    Must
                    (
                        Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), May(Sequence(SchemaName, May(CommentOrWhitespace))), Dot, May(CommentOrWhitespace), TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Must(Mul, ColumnName)),
                        Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Must(Mul, ColumnName)),
                        Sequence(TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Must(Mul, ColumnName))
                    )
                ),
                Must(Mul, ColumnName)
            );

        #endregion
        #region Function call syntax

        public static Expression<Rule> FunctionIdentifier = () => Must(UdfIdentifier, FunctionName);

        // *** TODO: maybe add dataset support here to be able to call mydb functions?
        public static Expression<Rule> UdfIdentifier = () =>
            Sequence
            (
                May(Sequence(DatasetName, May(CommentOrWhitespace), Comma, May(CommentOrWhitespace))),
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(Sequence(May(CommentOrWhitespace), SchemaName)), May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), FunctionName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), FunctionName)
                )
            );

        public static Expression<Rule> UdtFunctionIdentifier = () =>
            Sequence
            (
                Variable,
                May(CommentOrWhitespace),
                Dot,        // Need to add :: ?
                May(CommentOrWhitespace),
                FunctionName
            );

        public static Expression<Rule> Argument = () => Expression;

        public static Expression<Rule> ArgumentList = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Argument,
                May(Sequence(May(CommentOrWhitespace), Comma, ArgumentList))
            );

        public static Expression<Rule> FunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                BracketOpen,
                May(ArgumentList),
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TableValuedFunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                BracketOpen,
                May(ArgumentList),
                May(CommentOrWhitespace),
                BracketClose
            );

        #endregion
        #region Select statement and query expressions (combinations of selects)

        public static Expression<Rule> SelectStatement = () =>
            Sequence
            (
                May(CommentOrWhitespace),   // remove this when wrapped in generic statement grammar
                QueryExpression,
                May(CommentOrWhitespace),
                May(OrderByClause),
                May(CommentOrWhitespace)   // remove this when wrapped in generic statement grammar
            );

        public static Expression<Rule> QueryExpression = () =>
            Sequence
            (
                Must
                (
                    Sequence(BracketOpen, May(CommentOrWhitespace), QueryExpression, May(CommentOrWhitespace), BracketClose),
                    QuerySpecification
                ),
                May(Sequence(May(CommentOrWhitespace), QueryOperator, May(CommentOrWhitespace), QueryExpression))
            );

        public static Expression<Rule> QueryOperator = () =>
            Must
            (
                Sequence(Keyword("UNION"), May(Sequence(CommentOrWhitespace, Keyword("ALL")))),
                Keyword("EXCEPT"),
                Keyword("INTERSECT")
            );

        public static Expression<Rule> QuerySpecification = () =>
            Sequence
            (
                Keyword("SELECT"),
                May(Sequence(CommentOrWhitespace, Must(Keyword("ALL"), CommentOrWhitespace, Keyword("DISTINCT")))),
                May(Sequence(CommentOrWhitespace, TopExpression)),
                CommentOrWhitespace, SelectList,
                May(Sequence(CommentOrWhitespace, IntoClause)),
                May(Sequence(CommentOrWhitespace, FromClause)),
                May(Sequence(CommentOrWhitespace, WhereClause)),
                May(Sequence(CommentOrWhitespace, GroupByClause)),
                May(Sequence(CommentOrWhitespace, HavingClause))
            );

        #endregion
        #region Top expression

        public static Expression<Rule> TopExpression = () =>
            Sequence
            (
                Keyword("TOP"),
                CommentOrWhitespace,
                Expression,
                May(Sequence(CommentOrWhitespace, Keyword("PERCENT"))),
                May(Sequence(CommentOrWhitespace, Keyword("WITH"), CommentOrWhitespace, Keyword("TIES")))
            );

        #endregion
        #region Select list and column expression

        public static Expression<Rule> SelectList = () =>
            Sequence
            (
                ColumnExpression,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), SelectList))
            );

        public static Expression<Rule> ColumnExpression = () =>
            Must
            (
                Sequence(ColumnAlias, May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Expression),
                Sequence(Expression, May(Sequence(May(Sequence(CommentOrWhitespace, Keyword("AS"))), CommentOrWhitespace, ColumnAlias)))
            );

        #endregion
        #region Into clause

        public static Expression<Rule> IntoClause = () => Sequence(Keyword("INTO"), CommentOrWhitespace, TableOrViewName);

        #endregion
        #region From clause, table sources and joins

        public static Expression<Rule> FromClause = () =>
            Sequence(Keyword("FROM"), May(CommentOrWhitespace), TableSourceExpression);

        public static Expression<Rule> TableSourceExpression = () =>
            Sequence(TableSource, May(Sequence(May(CommentOrWhitespace), JoinedTable)));

        public static Expression<Rule> JoinedTable = () =>
            Sequence
            (
                Must
                (
                    Sequence(JoinType, May(CommentOrWhitespace), TableSource, May(CommentOrWhitespace), Keyword("ON"), May(CommentOrWhitespace), SearchCondition),
                    Sequence(Keyword("CROSS"), CommentOrWhitespace, Keyword("JOIN"), May(CommentOrWhitespace), TableSource),
                    Sequence(Comma, May(CommentOrWhitespace), TableSource),
                    Sequence(Must(Keyword("CROSS"), Keyword("OUTER")), CommentOrWhitespace, Keyword("APPLY"), May(CommentOrWhitespace), TableSource)
                ),
                May(Sequence(May(CommentOrWhitespace), JoinedTable))
            );

        public static Expression<Rule> JoinType = () =>
            Sequence
            (
                May
                (
                    Sequence
                    (
                        Must
                        (
                            Keyword("INNER"),
                            Sequence(Must(Keyword("LEFT"), Keyword("RIGHT"), Keyword("FULL")), May(Sequence(CommentOrWhitespace, Keyword("OUTER"))))
                        ),
                        May(CommentOrWhitespace),
                        May(JoinHint)
                    )
                ),
                May(CommentOrWhitespace),
                Keyword("JOIN")
            );

        public static Expression<Rule> JoinHint = () =>
            Must
            (
                Keyword("LOOP"), Keyword("HASH"), Keyword("MERGE"), Keyword("REMOTE")
            );

        public static Expression<Rule> TableSource = () =>
            Must
            (
                FunctionTableSource,
                SimpleTableSource,
                VariableTableSource,
                SubqueryTableSource
            );

        public static Expression<Rule> SimpleTableSource = () =>
            Sequence
            (
                TableOrViewName,
                May(Sequence(CommentOrWhitespace, May(Sequence(Keyword("AS"), CommentOrWhitespace)), TableAlias)),   // Optional
                May(Sequence(CommentOrWhitespace, TableSampleClause)),
                May(Sequence(CommentOrWhitespace, TableHintClause)),
                May(Sequence(CommentOrWhitespace, TablePartitionClause))
            );

        public static Expression<Rule> FunctionTableSource = () =>
            Sequence
            (
                TableValuedFunctionCall,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                TableAlias,     // Required
                May(Sequence(May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), ColumnAliasList, May(CommentOrWhitespace), BracketClose))
            );

        public static Expression<Rule> VariableTableSource = () =>
            Sequence
            (
                Variable,
                May(Sequence(CommentOrWhitespace, May(Sequence(Keyword("AS"), CommentOrWhitespace)), TableAlias))   // Optional
            );

        public static Expression<Rule> SubqueryTableSource = () =>
            Sequence
            (
                Subquery,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                TableAlias     // Required
            );

        public static Expression<Rule> ColumnAliasList = () =>
            Sequence(ColumnAlias, May(Sequence(May(CommentOrWhitespace), Comma, ColumnAliasList)));

        public static Expression<Rule> TableSampleClause = () =>
            Sequence
            (
                Keyword("TABLESAMPLE"),
                May(Sequence(CommentOrWhitespace, Keyword("SYSTEM"))),
                May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                SampleNumber,
                May(Sequence(CommentOrWhitespace, May(Must(Keyword("PERCENT"), Keyword("ROWS"))))),
                May(CommentOrWhitespace), BracketClose,
                May(Sequence
                (
                    CommentOrWhitespace,
                    Keyword("REPEATABLE"),
                    May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                    RepeatSeed,
                    May(CommentOrWhitespace), BracketClose)
                )
            );

        public static Expression<Rule> TableHintClause = () =>
            Sequence
            (
                Keyword("WITH"),
                May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                TableHintList,
                May(CommentOrWhitespace), BracketClose
            );

        public static Expression<Rule> TableHintList = () =>
            Sequence
            (
                TableHint,
                Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), TableHintList)
            );

        public static Expression<Rule> TableHint = () =>
            Must(
                Sequence(May(Sequence(Keyword("NOEXPAND"), CommentOrWhitespace)), Keyword("INDEX"), May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace), IndexValueList, May(CommentOrWhitespace), BracketClose),
                Keyword("FASTFIRSTROW"),
                Keyword("HOLDLOCK"),
                Keyword("NOLOCK"),
                Keyword("NOWAIT"),
                Keyword("PAGLOCK"),
                Keyword("READCOMMITTED"),
                Keyword("READCOMMITTEDLOCK"),
                Keyword("READPAST"),
                Keyword("READUNCOMMITTED"),
                Keyword("REPEATABLEREAD"),
                Keyword("ROWLOCK"),
                Keyword("SERIALIZABLE"),
                Keyword("TABLOCK"),
                Keyword("TABLOCKX"),
                Keyword("UPDLOCK"),
                Keyword("XLOCK")
            );

        public static Expression<Rule> IndexValueList = () =>
            Sequence
            (
                IndexValue,
                Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), IndexValueList)
            );

        public static Expression<Rule> Subquery = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                SelectStatement,            // Order by is also allowed here with top!
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TablePartitionClause = () =>
            Sequence
            (
                Keyword("PARTITION"), CommentOrWhitespace, Keyword("ON"),
                CommentOrWhitespace,
                ColumnIdentifier
            );

        #endregion
        #region Where clause, search conditions and predicates

        public static Expression<Rule> WhereClause = () =>
            Sequence
            (
                Keyword("WHERE"),
                May(CommentOrWhitespace),
                SearchCondition
            );

        public static Expression<Rule> SearchCondition = () =>
            Sequence
            (
                May(LogicalNot),
                May(CommentOrWhitespace),
                Must(Predicate, SearchConditionBrackets),
                May(Sequence(May(CommentOrWhitespace), LogicalOperator, May(CommentOrWhitespace), SearchCondition))
            );

        public static Expression<Rule> SearchConditionBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                SearchCondition,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> Predicate = () =>
            Must
            (
                // Expression comparieson
                Sequence
                (
                    Expression,
                    May(CommentOrWhitespace),
                    ComparisonOperator,
                    May(CommentOrWhitespace),
                    Expression
                ),
                // Like
                Sequence
                (
                    Expression,
                    May(Keyword("NOT")),
                    May(CommentOrWhitespace),
                    Keyword("LIKE"),
                    May(CommentOrWhitespace),
                    Expression,
                    May(Sequence(May(CommentOrWhitespace), Keyword("ESCAPE"), May(CommentOrWhitespace), Expression))
                ),
                // Between
                Sequence
                (
                    Expression,
                    May(Sequence(May(CommentOrWhitespace), Keyword("NOT"))),
                    May(CommentOrWhitespace),
                    Keyword("BETWEEN"),
                    May(CommentOrWhitespace),
                    Expression,
                    May(CommentOrWhitespace),
                    Keyword("AND"),
                    May(CommentOrWhitespace),
                    Expression
                ),
                // IS NULL
                Sequence
                (
                    Expression,
                    May(CommentOrWhitespace),
                    Keyword("IS"),
                    May(Sequence(CommentOrWhitespace, Keyword("NOT"))),
                    CommentOrWhitespace,
                    Keyword("NULL")
                ),
                // IN - semi join
                Sequence
                (
                    Expression,
                    May(Keyword("NOT")),
                    May(CommentOrWhitespace),
                    Keyword("IN"),
                    May(CommentOrWhitespace),
                    Must
                    (
                        Subquery,
                        Sequence(BracketOpen, May(CommentOrWhitespace), ArgumentList, May(CommentOrWhitespace), BracketClose)
                    )
                ),
                // comparision semi join
                Sequence
                (
                    Expression,
                    May(CommentOrWhitespace),
                    ComparisonOperator,
                    May(CommentOrWhitespace),
                    Must(Keyword("ALL"), Keyword("SOME"), Keyword("ANY")),
                    May(CommentOrWhitespace),
                    Subquery
                ),
                // EXISTS
                Sequence
                (
                    Keyword("EXISTS"),
                    May(CommentOrWhitespace),
                    Subquery
                )
                // *** TODO: add string constructs (contains, freetext etc.)
            );

        #endregion
        #region Group by clause

        public static Expression<Rule> GroupByClause = () =>
            Sequence
            (
                Keyword("GROUP"),
                CommentOrWhitespace,
                Keyword("BY"),
                Must
                (
                    Sequence(CommentOrWhitespace, Keyword("ALL")),
                    Sequence(May(CommentOrWhitespace), GroupByList)
                )
            );

        public static Expression<Rule> GroupByList = () =>
            Sequence
            (
                Expression,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), GroupByList))
            );

        #endregion
        #region Having clause

        public static Expression<Rule> HavingClause = () =>
            Sequence
            (
                Keyword("HAVING"),
                May(CommentOrWhitespace),
                SearchCondition
            );

        #endregion
        #region Order by clause

        public static Expression<Rule> OrderByClause = () =>
            Sequence
            (
                Keyword("ORDER"),
                CommentOrWhitespace,
                Keyword("BY"),
                May(CommentOrWhitespace),
                OrderByList
            );

        public static Expression<Rule> OrderByList = () =>
            Sequence
            (
                Must
                (
                    Expression,
                    Sequence(ColumnPosition, May(Sequence(May(CommentOrWhitespace), Must(Keyword("ASC"), Keyword("DESC")))))
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), OrderByList))
            );

        #endregion

#if false




	



	

    

! <common_table_expression> ::=
!    Identifier [ '(' <ColumnList> ')' ]
!    "AS" '(' <QueryExpression> ')' ;
    
<ColumnList> ::= <ColumnName> [ ',' <ColumnList> ];
    

	
! <compute_clause> ::= "COMPUTE" { <function_call_list> } [ "BY" <ExpressionList> ];
	
! <function_call_list> ::= <FunctionCall> [ ',' <FunctionCall> ];

<ExpressionList> ::= <Expression> [ ',' <Expression> ];
    


! <query_hint > ::= 
!	{ { "HASH" | "ORDER" } "GROUP"
!    | { "CONCAT" | "HASH" | "MERGE" } "UNION"
!    | { "LOOP" | "MERGE" | "HASH" } "JOIN"
!    | "FAST" number_rows 
!    | "FORCE" "ORDER"
!    | "MAXDOP" number_of_processors 
!    | "OPTIMIZE" "FOR" ( @variable_name = literal_constant [ , ...n ] ) 
!    | "PARAMETERIZATION" { "SIMPLE" | "FORCED" }
!    | "RECOMPILE"
!    | "ROBUST" "PLAN"
!    | "KEEP" "PLAN"
!    | "KEEPFIXED" "PLAN"
!    | "EXPAND" "VIEWS"
!    | "MAXRECURSION" Number
!    | "USE" "PLAN" N'xml_plan'
!    } ;
    
! <query_hint_list> ::= <query_hint> [ ',' <query_hint> ]    ;
        
   

! <NewTable> ::= Identifier;








! -----------------------------------------------------------------------------------------------------





<TableExpression> ::=	{ <TableOrViewName>	| <TableAlias> };
    
<UdtColumnExpression> ::=	<UdtColumnName> [ <UdtMemberExpression> ];
                               
<UdtMemberExpression> ::=	{ '.' | '::' } { <PropertyName> '(' <ArgumentList> ')' | <PropertyName> };
	
        public static Expression<Rule> ColumnExpression = () =>
            Must
            (
                Sequence(ColumnAlias, May(CommentOrWhitespace), Equals, May(CommentOrWhitespace), Expression),
                Sequence(Expression, May(Sequence(May(Sequence(CommentOrWhitespace, Keyword("AS"))), CommentOrWhitespace, ColumnAlias)))
            );
    




<TableHintLimited> ::=
	{ "KEEPIDENTITY"
	| "KEEPDEFAULTS"
	| "FASTFIRSTROW"
	| "HOLDLOCK"
	| "IGNORE_CONSTRAINTS"
	| "IGNORE_TRIGGERS"
	| "NOWAIT"
	| "PAGLOCK"
	| "READCOMMITTED"
	| "READCOMMITTEDLOCK"
	| "READPAST"
	| "REPEATABLEREAD"
	| "ROWLOCK"
	| "SERIALIZABLE"
	| "TABLOCK"
	| "TABLOCKX"
	| "UPDLOCK"
	| "XLOCK"
	} ;
	

       
<TableSource> ::=
     { <FunctionCall> [ "AS" ] <TableAlias> [ '(' <ColumnAliasList> ')' ] 
     | <TableOrViewName> [ [ "AS" ] <TableAlias> ] [ <TablesampleClause> ] [ "WITH" '(' <TableHint> ')' ] [ <TablePartitionClause> ]
!    | "OPENXML" <openxml_clause> 
     | <DerivedTable> [ "AS" ] <TableAlias> [ '(' <ColumnAliasList> ')' ] 
!    | <pivoted_table> 
!    | <unpivoted_table>
     | <Variable> [ [ "AS" ] <TableAlias> ]
     | <Variable> '.' <FunctionCall> [ [ "AS" ] <TableAlias> ] [ '(' <ColumnAliasList> ')' ]
     | <Subquery> [ [ "AS" ] <TableAlias> ]
     };
     


<JoinedTable> ::= 
     { <JoinType> <TableSource> "ON" <SearchCondition> 
     | "CROSS" "JOIN" <TableSource>
     | { "CROSS" | "OUTER" } "APPLY" <TableSource>
	 | ',' <TableSource>
	 }
     [ <JoinedTable> ];


!<SearchCondition> ::= 
!    { <Predicate> | <LogicalNot> <SearchCondition> | <SearchConditionBrackets> } 
!    [ <LogicalOperator> <SearchCondition> ]
!    [ ',' <SearchCondition> ];

<SearchCondition> ::= 
    [ <LogicalNot> ] { <Predicate> | <SearchConditionBrackets> }
    [ <LogicalOperator> <SearchCondition> ];

<SearchConditionBrackets> ::= '(' <SearchCondition> ')';

! <SearchConditionTerms> ::= { "AND" | "OR" } [ "NOT" ] { <Predicate> | '(' <SearchCondition> ')' } [ <SearchConditionTerms> ];

<Predicate> ::= 
    { <Expression> <ComparisonOperator> <Expression>
    | <Expression> [ "NOT" ] "LIKE" <Expression> [ "ESCAPE" <Expression> ]
    | <Expression> [ "NOT" ] "BETWEEN" <Expression> "AND" <Expression>
    | <Expression> "IS" [ "NOT" ] "NULL" 
!    | "CONTAINS" '(' { <ColumnName> | '*' } ',' <contains_search_condition> ')' 
!    | "FREETEXT" '(' { <ColumnName> | '*' } ',' <freetext_string> ')'
    | <Expression> [ "NOT" ] "IN" { <Subquery> | '(' <ArgumentList> ')' }
    | <Expression> <ComparisonOperator> { "ALL" | "SOME" | "ANY"} '(' <Subquery> ')'
    | "EXISTS" <Subquery>
    };

	
! <contains_search_condition> ::= <StringExpression>;

! <freetext_string> ::= <StringExpression>;

#endif
    }
}
