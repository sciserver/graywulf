﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Parsing.Generator;

namespace Jhu.Graywulf.Sql.Parser.Grammar
{
    [Grammar(
        Namespace = "Jhu.Graywulf.Sql.Parsing",
        ParserName = "SqlParser",
        Comparer = "StringComparer.InvariantCultureIgnoreCase",
        RootToken = "StatementBlock")]
    public class SqlGrammar : Jhu.Graywulf.Parsing.Generator.Grammar
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

        public static Expression<Symbol> PlusEquals = () => @"+=";
        public static Expression<Symbol> MinusEquals = () => @"-=";
        public static Expression<Symbol> MulEquals = () => @"*=";
        public static Expression<Symbol> DivEquals = () => @"/=";
        public static Expression<Symbol> ModEquals = () => @"%=";
        public static Expression<Symbol> AndEquals = () => @"&=";
        public static Expression<Symbol> XorEquals = () => @"^=";
        public static Expression<Symbol> OrEquals = () => @"|=";

        public static Expression<Symbol> Equals1 = () => @"=";
        public static Expression<Symbol> Equals2 = () => @"==";
        public static Expression<Symbol> LessOrGreaterThan = () => @"<>";
        public static Expression<Symbol> NotEquals = () => @"!=";
        public static Expression<Symbol> NotLessThan = () => @"!<";
        public static Expression<Symbol> NotGreaterThan = () => @"!>";
        public static Expression<Symbol> LessThanOrEqual = () => @"<=";
        public static Expression<Symbol> GreaterThanOrEqual = () => @">=";
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
        public static Expression<Terminal> NumericConstant = () => @"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)";
        public static Expression<Terminal> HexLiteral = () => @"\G0[xX][0-9a-fA-F]+";
        public static Expression<Terminal> StringConstant = () => @"\G('([^']|'')*')";
        public static Expression<Terminal> Identifier = () => @"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])";
        public static Expression<Terminal> Variable = () => @"\G(@[a-zA-Z_][0-9a-zA-Z_]*)";
        public static Expression<Terminal> Variable2 = () => @"\G(@@[a-zA-Z_][0-9a-zA-Z_]*)";

        #endregion
        #region Arithmetic operators used in expressions

        public static Expression<Rule> UnaryOperator = () =>
            Must(Plus, Minus, BitwiseNot);
        public static Expression<Rule> ArithmeticOperator = () =>
            Must(Plus, Minus, Mul, Div, Mod);
        public static Expression<Rule> BitwiseOperator = () =>
            Must(BitwiseAnd, BitwiseOr, BitwiseXor);
        public static Expression<Rule> ComparisonOperator = () =>
            Must(Equals2, Equals1, LessOrGreaterThan, NotEquals,
            NotLessThan, NotGreaterThan, LessThanOrEqual, GreaterThanOrEqual,
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
        public static Expression<Rule> ConstraintName = () => Identifier;
        public static Expression<Rule> IndexName = () => Identifier;
        public static Expression<Rule> DerivedTable = () => Identifier;
        public static Expression<Rule> TableAlias = () => Identifier;
        public static Expression<Rule> MethodName = () => Identifier;
        public static Expression<Rule> ColumnName = () => Identifier;
        public static Expression<Rule> ColumnAlias = () => Identifier;
        public static Expression<Rule> CursorName = () => Identifier;
        public static Expression<Rule> UdtColumnName = () => Identifier;
        public static Expression<Rule> PropertyName = () => Identifier;
        public static Expression<Rule> SampleNumber = () => NumericConstant;
        public static Expression<Rule> RepeatSeed = () => NumericConstant;
        public static Expression<Rule> IndexValue = () => Identifier;

        public static Expression<Rule> DatasetPrefix = () =>
            Sequence
            (
                DatasetName,
                May(CommentOrWhitespace),
                Colon
            );

        public static Expression<Rule> MultiPartIdentifier = () => NamePartList;

        public static Expression<Rule> NamePartList = () =>
            Sequence
            (
                Identifier,
                May(Sequence(May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), NamePartList))
            );

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
                May(Sequence(UnaryOperator, May(CommentOrWhitespace))),
                Must
                (
                    Constant,

                    Subquery,
                    ExpressionBrackets,

                    SystemVariable,
                    UserVariable,

                    SimpleCaseExpression,
                    SearchedCaseExpression,

                    UdtStaticMethodCall,            // ::method() syntax
                    UdtStaticPropertyAccess,        // ::property syntax

                    WindowedFunctionCall,           // OVER () syntax
                    ScalarFunctionCall,             // function()
                    CountStar,

                    ColumnIdentifier                // This parses all name.name.name ...
                ),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        UdtMemberList
                    )
                ),
                May
                (
                    // Binary operators
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        Must(ArithmeticOperator, BitwiseOperator),
                        May(CommentOrWhitespace),
                        Expression
                    )
                )
            );

        public static Expression<Rule> Constant = () =>
            Must
            (
                    Null,
                    HexLiteral,
                    NumericConstant,
                    StringConstant
            );

        public static Expression<Rule> CountStar = () =>
            Sequence
            (
                Keyword("COUNT"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                Mul,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ExpressionBrackets = () =>
            Sequence(BracketOpen, May(CommentOrWhitespace), Expression, May(CommentOrWhitespace), BracketClose);

        public static Expression<Rule> Null = () => Keyword("NULL");

        public static Expression<Rule> UserVariable = () => Variable;

        public static Expression<Rule> SystemVariable = () => Variable2;

        public static Expression<Rule> UdtMemberList = () =>
            Sequence
            (
                Must(UdtMethodCall, UdtPropertyAccess),
                May(Sequence(May(CommentOrWhitespace), UdtMemberList))
            );

        #endregion
        #region Boolean expressions

        public static Expression<Rule> BooleanExpression = () =>
            Sequence
            (
                May(Sequence(LogicalNot, May(CommentOrWhitespace))),
                Must
                (
                    Predicate, 
                    BooleanExpressionBrackets
                ),
                May(Sequence(May(CommentOrWhitespace), LogicalOperator, May(CommentOrWhitespace), BooleanExpression))
            );

        public static Expression<Rule> BooleanExpressionBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                BooleanExpression,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> Predicate = () =>
            Must
            (
                ComparisonPredicate,
                LikePredicate,
                BetweenPredicate,
                IsNullPredicate,
                InSemiJoinPredicate,
                ComparisonSemiJoinPredicate,
                ExistsSemiJoinPredicate
            // *** TODO: add string constructs (contains, freetext etc.)
            );

        public static Expression<Rule> ComparisonPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                ComparisonOperator,
                May(CommentOrWhitespace),
                Expression
            );

        public static Expression<Rule> LikePredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("LIKE"),
                May(CommentOrWhitespace),
                Expression,
                May(Sequence(May(CommentOrWhitespace), Keyword("ESCAPE"), May(CommentOrWhitespace), Expression))
            );

        public static Expression<Rule> BetweenPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("BETWEEN"),
                May(CommentOrWhitespace),
                Expression,
                May(CommentOrWhitespace),
                Keyword("AND"),
                May(CommentOrWhitespace),
                Expression
            );

        public static Expression<Rule> IsNullPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                Keyword("IS"),
                May(Sequence(CommentOrWhitespace, Keyword("NOT"))),
                CommentOrWhitespace,
                Keyword("NULL")
            );

        public static Expression<Rule> InSemiJoinPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("IN"),
                May(CommentOrWhitespace),
                Must
                (
                    Subquery,
                    Sequence
                    (
                        BracketOpen,
                        May(CommentOrWhitespace),
                        ArgumentList,
                        May(CommentOrWhitespace),
                        BracketClose
                    )
                )
            );

        public static Expression<Rule> ComparisonSemiJoinPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                ComparisonOperator,
                May(CommentOrWhitespace),
                Must(Keyword("ALL"), Keyword("SOME"), Keyword("ANY")),
                May(CommentOrWhitespace),
                Subquery
            );

        public static Expression<Rule> ExistsSemiJoinPredicate = () =>
            Sequence
            (
                Keyword("EXISTS"),
                May(CommentOrWhitespace),
                Subquery
            );

        #endregion
        #region Case-When constructs

        public static Expression<Rule> SimpleCaseExpression = () =>
            Sequence
            (
                Keyword("CASE"),
                May(CommentOrWhitespace),
                Expression,
                May(CommentOrWhitespace),
                SimpleCaseWhenList,
                May(Sequence
                (
                    May(CommentOrWhitespace),
                    Keyword("ELSE"),
                    May(CommentOrWhitespace),
                    Expression
                )),
                May(CommentOrWhitespace),
                Keyword("END")
            );

        public static Expression<Rule> SimpleCaseWhenList = () =>
            Sequence(SimpleCaseWhen, May(CommentOrWhitespace), May(SimpleCaseWhenList));

        public static Expression<Rule> SimpleCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                May(CommentOrWhitespace),
                Expression,
                May(CommentOrWhitespace),
                Keyword("THEN"),
                May(CommentOrWhitespace),
                Expression
            );

        public static Expression<Rule> SearchedCaseExpression = () =>
            Sequence
            (
                Keyword("CASE"),
                CommentOrWhitespace,
                SearchedCaseWhenList,
                May(Sequence
                (
                    May(CommentOrWhitespace),
                    Keyword("ELSE"),
                    May(CommentOrWhitespace),
                    Expression
                )),
                May(CommentOrWhitespace),
                Keyword("END")
            );

        public static Expression<Rule> SearchedCaseWhenList = () =>
            Sequence(SearchedCaseWhen, May(CommentOrWhitespace), May(SearchedCaseWhenList));

        public static Expression<Rule> SearchedCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                May(CommentOrWhitespace),
                BooleanExpression,
                May(CommentOrWhitespace),
                Keyword("THEN"),
                May(CommentOrWhitespace),
                Expression
            );

        #endregion
        #region Table and column names

        public static Expression<Rule> TableOrViewIdentifier = () =>
            Sequence
            (
                May(Sequence(DatasetPrefix, May(CommentOrWhitespace))),
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), May(Sequence(SchemaName, May(CommentOrWhitespace))), Dot, May(CommentOrWhitespace), TableName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName),
                    TableName
                )
            );

        public static Expression<Rule> TableSourceIdentifier = () =>
            Must
            (
                Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName),
                TableName
            );

        public static Expression<Rule> ColumnIdentifier = () => MultiPartIdentifier;

        public static Expression<Rule> StarColumnIdentifier = () =>
            Must
            (
                Mul,
                Sequence(TableSourceIdentifier, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Mul)
            );

        #endregion
        #region Data types

        // TODO: extend this when implementing array support

        // UDT restrictions:
        // - cannot have a schema name
        // - can only reference types from current database
        // - https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008-r2/ms178069(v=sql.105)

        public static Expression<Rule> DataTypeIdentifier = () =>
            Sequence
            (
                MultiPartIdentifier,
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        Must
                        (
                            DataTypeScaleAndPrecision,
                            DataTypeSize
                        )
                    )
                )
            );

        public static Expression<Rule> DataTypeScaleAndPrecision = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                NumericConstant,
                May(CommentOrWhitespace),
                Comma,
                May(CommentOrWhitespace),
                NumericConstant,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> DataTypeSize = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                Must(Literal("MAX"), NumericConstant),
                May(CommentOrWhitespace),
                BracketClose
            );

        #endregion
        #region Function call syntax

        public static Expression<Rule> ScalarFunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                FunctionArguments
            );

        public static Expression<Rule> UdtMethodCall = () =>
            Sequence
            (
                Dot,
                May(CommentOrWhitespace),
                MethodName,
                May(CommentOrWhitespace),
                FunctionArguments
            );

        public static Expression<Rule> UdtStaticMethodCall = () =>
            Sequence
            (
                UdtStaticMethodIdentifier,
                May(CommentOrWhitespace),
                FunctionArguments
            );

        public static Expression<Rule> FunctionArguments = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                May(ArgumentList),
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ArgumentList = () =>
            Sequence
            (
                Argument,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ArgumentList))
            );

        public static Expression<Rule> Argument = () => Expression;

        public static Expression<Rule> UdtStaticMethodIdentifier = () =>
            Sequence
            (
                DataTypeIdentifier,
                May(CommentOrWhitespace),
                DoubleColon,
                May(CommentOrWhitespace),
                MethodName
            );

        public static Expression<Rule> FunctionIdentifier = () =>
            Sequence
            (
                // Optional dataset prefix
                May(Sequence(DatasetPrefix, May(CommentOrWhitespace))),
                MultiPartIdentifier
            );

        public static Expression<Rule> TableValuedFunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                FunctionArguments
            );

        public static Expression<Rule> WindowedFunctionCall = () =>
            Sequence
            (
                FunctionIdentifier,
                May(CommentOrWhitespace),
                FunctionArguments,
                May(CommentOrWhitespace),
                OverClause
            );

        public static Expression<Rule> OverClause = () =>
            Sequence
            (
                Keyword("OVER"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                May(Sequence(PartitionByClause, May(CommentOrWhitespace))),
                May(Sequence(OrderByClause, May(CommentOrWhitespace))),
                BracketClose
            );

        public static Expression<Rule> PartitionByClause = () =>
            Sequence
            (
                Keyword("PARTITION"),
                CommentOrWhitespace,
                Keyword("BY"),
                May(CommentOrWhitespace),
                ArgumentList
            );

        #endregion
        #region Property access syntax

        public static Expression<Rule> UdtPropertyAccess = () => 
            Sequence
            (
                Dot,
                May(CommentOrWhitespace),
                PropertyName
            );

        public static Expression<Rule> UdtStaticPropertyAccess = () =>
            Sequence
            (
                DataTypeIdentifier,
                May(CommentOrWhitespace),
                DoubleColon,
                May(CommentOrWhitespace),
                PropertyName
            );

        #endregion
        #region Statements

        public static Expression<Rule> StatementBlock = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                May(Statement),
                May
                (
                    Sequence
                    (
                        StatementSeparator,
                        May(StatementBlock)
                    )
                ),
                May(CommentOrWhitespace)
            );

        public static Expression<Rule> StatementSeparator = () =>
            Must
            (
                Sequence(May(CommentOrWhitespace), Semicolon, May(CommentOrWhitespace)),
                CommentOrWhitespace
            );

        public static Expression<Rule> Statement = () =>
            Must
            (
                Label,
                GotoStatement,
                BeginEndStatement,
                WhileStatement,
                BreakStatement,
                ContinueStatement,
                PrintStatement,
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

                SelectStatement,
                InsertStatement,
                UpdateStatement,
                DeleteStatement
            // TODO: MergeStatement
            );

        #endregion

        #region Control flow statements

        public static Expression<Rule> Label = () =>
            Sequence
            (
                Identifier,
                Colon
            );

        public static Expression<Rule> GotoStatement = () =>
            Sequence
            (
                Keyword("GOTO"),
                CommentOrWhitespace,
                Identifier
            );

        public static Expression<Rule> BeginEndStatement = () =>
            Sequence
            (
                Keyword("BEGIN"),
                StatementBlock,
                Keyword("END")
            );

        public static Expression<Rule> WhileStatement = () =>
            Sequence
            (
                Keyword("WHILE"),
                May(CommentOrWhitespace),
                BooleanExpression,
                May(CommentOrWhitespace),
                Statement
            );

        public static Expression<Rule> BreakStatement = () => Keyword("BREAK");

        public static Expression<Rule> ContinueStatement = () => Keyword("CONTINUE");

        public static Expression<Rule> ReturnStatement = () => Keyword("RETURN");
        // TODO: return can take a value

        public static Expression<Rule> PrintStatement = () =>
            Sequence
            (
                Keyword("PRINT"),
                CommentOrWhitespace,
                Expression
            );

        public static Expression<Rule> IfStatement = () =>
            Sequence
            (
                Keyword("IF"),
                May(CommentOrWhitespace),
                BooleanExpression,
                May(CommentOrWhitespace),
                Statement,
                May(
                    Sequence(
                        StatementSeparator,
                        Keyword("ELSE"),
                        CommentOrWhitespace,
                        Statement
                    )
                )
            );

        public static Expression<Rule> ThrowStatement = () =>
            Sequence
            (
                Keyword("THROW"),
                May(
                    Sequence(
                        CommentOrWhitespace,
                        Must(NumericConstant, UserVariable),
                        May(CommentOrWhitespace),
                        Comma,
                        May(CommentOrWhitespace),
                        Must(StringConstant, UserVariable),
                        May(CommentOrWhitespace),
                        Comma,
                        May(CommentOrWhitespace),
                        Must(NumericConstant, UserVariable),
                        May(CommentOrWhitespace)
                    )
                )
            );

        public static Expression<Rule> TryCatchStatement = () =>
            Sequence
            (
                Keyword("BEGIN"), CommentOrWhitespace, Keyword("TRY"),
                StatementBlock,
                Keyword("END"), CommentOrWhitespace, Keyword("TRY"),
                CommentOrWhitespace,
                Keyword("BEGIN"), CommentOrWhitespace, Keyword("CATCH"),
                Must(StatementBlock, CommentOrWhitespace),
                Keyword("END"), CommentOrWhitespace, Keyword("CATCH")
            );

        #endregion
        #region Scalar variables and cursors

        public static Expression<Rule> VariableList = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                UserVariable,
                May(Sequence(May(CommentOrWhitespace), Comma, VariableList))
            );

        public static Expression<Rule> DeclareVariableStatement = () =>
            Sequence
            (
                Keyword("DECLARE"),
                May(CommentOrWhitespace),
                VariableDeclarationList
            );

        public static Expression<Rule> VariableDeclarationList = () =>
            Sequence
            (
                VariableDeclaration,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), VariableDeclarationList))
            );

        public static Expression<Rule> VariableDeclaration = () =>
            Sequence
            (
                UserVariable,
                May(Sequence(CommentOrWhitespace, Keyword("AS"))),
                May(CommentOrWhitespace),
                DataTypeIdentifier,
                May(
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        Equals1,
                        May(CommentOrWhitespace),
                        Expression
                    )
                )
            );

        // TODO: add UDT support

        public static Expression<Rule> SetVariableStatement = () =>
            Sequence
            (
                Keyword("SET"),
                CommentOrWhitespace,
                UserVariable,
                May(CommentOrWhitespace),
                ValueAssignmentOperator,
                May(CommentOrWhitespace),
                Expression
            );

        public static Expression<Rule> ValueAssignmentOperator = () =>
            Must(
                Equals1,
                PlusEquals,
                MinusEquals,
                MulEquals,
                DivEquals,
                ModEquals,
                AndEquals,
                XorEquals,
                OrEquals
            );

        public static Expression<Rule> DeclareCursorStatement = () =>
            Sequence
            (
                Keyword("DECLARE"),
                May(CommentOrWhitespace),
                Must
                (
                    Sequence(CursorName, May(Sequence(May(CommentOrWhitespace), CursorDefinition))),
                    Sequence(UserVariable, May(CommentOrWhitespace), Keyword("CURSOR"))
                )
            );

        public static Expression<Rule> SetCursorStatement = () =>
            Sequence
            (
                Keyword("SET"),
                May(CommentOrWhitespace),
                UserVariable,
                May(CommentOrWhitespace),
                Equals1,
                May(CommentOrWhitespace),
                CursorDefinition
            );

        public static Expression<Rule> CursorDefinition = () =>
            // Yes, CTE is OK as cursor definition!
            Sequence
            (
                Keyword("CURSOR"),
                CommentOrWhitespace,
                Keyword("FOR"),
                CommentOrWhitespace,
                SelectStatement
            );

        // TODO: full cursor syntax
        /*
        { CURSOR[FORWARD_ONLY | SCROLL]
[STATIC | KEYSET | DYNAMIC | FAST_FORWARD]
[READ_ONLY | SCROLL_LOCKS | OPTIMISTIC]
[TYPE_WARNING]
FOR select_statement
[FOR { READ ONLY | UPDATE[OF column_name[ ,...n]]
    } ] 
      }
      */

        public static Expression<Rule> CursorOperationStatement = () =>
            Sequence
            (
                Must(Keyword("OPEN"), Keyword("CLOSE"), Keyword("DEALLOCATE")),
                May(CommentOrWhitespace),
                Must(UserVariable, CursorName)
            );

        public static Expression<Rule> FetchStatement = () =>
            Sequence
            (
                Keyword("FETCH"),
                May(Sequence(CommentOrWhitespace, FetchOriginSpecification)),
                May(CommentOrWhitespace),
                FetchFromClause,
                May(Sequence(CommentOrWhitespace, FetchIntoClause))
            );

        public static Expression<Rule> FetchOriginSpecification = () =>
            Must
            (
                Keyword("NEXT"),
                Keyword("PRIOR"),
                Keyword("FIRST"),
                Keyword("LAST"),
                Sequence
                (
                    Must
                    (
                        Keyword("ABSOLUTE"), 
                        Keyword("RELATIVE")
                    ),
                    May(CommentOrWhitespace),
                    Must(NumericConstant, UserVariable)
                )
            );

        public static Expression<Rule> FetchFromClause = () =>
            Sequence
            (
                May(Sequence(Keyword("FROM"), May(CommentOrWhitespace))),
                Must(CursorName, UserVariable)
            );

        public static Expression<Rule> FetchIntoClause = () =>
            Sequence
            (
                Keyword("INTO"),
                May(CommentOrWhitespace),
                VariableList
            );

        public static Expression<Rule> DeclareTableStatement = () =>
            Sequence
            (
                Keyword("DECLARE"),
                May(CommentOrWhitespace),
                UserVariable,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                Keyword("TABLE"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                TableDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            );

        #endregion

        #region Common table expressions

        public static Expression<Rule> CommonTableExpression = () =>
            Sequence
            (
                Keyword("WITH"),
                May(CommentOrWhitespace),
                CommonTableSpecificationList
            );

        public static Expression<Rule> CommonTableSpecificationList = () =>
            Sequence
            (
                CommonTableSpecification,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), CommonTableSpecificationList))
            );

        public static Expression<Rule> CommonTableSpecification = () =>
            Sequence
            (
                TableAlias,
                May(Sequence(May(CommentOrWhitespace), ColumnAliasBrackets)),
                May(CommentOrWhitespace),
                Keyword("AS"),
                May(CommentOrWhitespace),
                Subquery
            );

        #endregion

        #region SELECT statement and query expressions (combinations of selects)

        public static Expression<Rule> SelectStatement = () =>
            Sequence
            (
                May(Sequence(CommonTableExpression, May(CommentOrWhitespace))),
                QueryExpression,
                May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                May(Sequence(May(CommentOrWhitespace), QueryHintClause))
            );

        public static Expression<Rule> Subquery = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                QueryExpression,
                May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> QueryExpression = () =>
            Sequence
            (
                Must
                (
                    QueryExpressionBrackets,
                    QuerySpecification
                ),
                May(Sequence(May(CommentOrWhitespace), QueryOperator, May(CommentOrWhitespace), QueryExpression))
            );

        public static Expression<Rule> QueryExpressionBrackets = () =>
            Sequence(BracketOpen, May(CommentOrWhitespace), QueryExpression, May(CommentOrWhitespace), BracketClose);

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
                May(Sequence(CommentOrWhitespace, Must(Keyword("ALL"), Keyword("DISTINCT")))),
                May(Sequence(CommentOrWhitespace, TopExpression)),
                May(CommentOrWhitespace),
                SelectList,
                May(Sequence(May(CommentOrWhitespace), IntoClause)),
                May(Sequence(May(CommentOrWhitespace), FromClause)),
                May(Sequence(May(CommentOrWhitespace), WhereClause)),
                May(Sequence(May(CommentOrWhitespace), GroupByClause)),
                May(Sequence(May(CommentOrWhitespace), HavingClause))
            );

        #endregion
        #region Top expression

        public static Expression<Rule> TopExpression = () =>
            Sequence
            (
                Keyword("TOP"),
                May(CommentOrWhitespace),
                Must(NumericConstant, ExpressionBrackets),
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
                Sequence(UserVariable, May(CommentOrWhitespace), Equals1, May(CommentOrWhitespace), Expression),
                Sequence(ColumnAlias, May(CommentOrWhitespace), Equals1, May(CommentOrWhitespace), Expression),
                StarColumnIdentifier,
                Sequence(Expression, May(CommentOrWhitespace), May(Sequence(Keyword("AS"), May(CommentOrWhitespace))), ColumnAlias),
                Expression
            );
        
        #endregion
        #region Into clause

        public static Expression<Rule> IntoClause = () =>
            Sequence
            (
                Keyword("INTO"),
                May(CommentOrWhitespace),
                TargetTableSpecification
            );

        public static Expression<Rule> TargetTableSpecification = () =>
            Sequence
            (
                Must
                (
                    UserVariable,
                    TableOrViewIdentifier
                // TODO: temp table?
                ),
                May(Sequence(May(CommentOrWhitespace), TableHintClause))
            );

        #endregion
        #region From clause, table sources and joins

        public static Expression<Rule> FromClause = () =>
            Sequence
            (
                Keyword("FROM"),
                May(CommentOrWhitespace),
                TableSourceExpression);

        public static Expression<Rule> TableSourceExpression = () =>
            Sequence(
                TableSourceSpecification,
                May(Sequence(May(CommentOrWhitespace), JoinedTable))
            );

        public static Expression<Rule> JoinedTable = () =>
            Sequence
            (
                Must
                (
                    Sequence(JoinType, May(CommentOrWhitespace), TableSourceSpecification, May(CommentOrWhitespace), Keyword("ON"), May(CommentOrWhitespace), BooleanExpression),
                    Sequence(Keyword("CROSS"), CommentOrWhitespace, Keyword("JOIN"), May(CommentOrWhitespace), TableSourceSpecification),
                    Sequence(Comma, May(CommentOrWhitespace), TableSourceSpecification),
                    Sequence(Must(Keyword("CROSS"), Keyword("OUTER")), CommentOrWhitespace, Keyword("APPLY"), May(CommentOrWhitespace), TableSourceSpecification)
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

        public static Expression<Rule> TableSourceSpecification = () =>
            Must
            (
                FunctionTableSource,
                SimpleTableSource,
                VariableTableSource,
                SubqueryTableSource

                // TODO:
                // - derived table with the VALUES clause of INSERT
                // - PIVOT
                // - UNPIVOT
            );

        public static Expression<Rule> SimpleTableSource = () =>
            Sequence
            (
                TableOrViewIdentifier,
                May(Sequence(May(CommentOrWhitespace), May(Sequence(Keyword("AS"), May(CommentOrWhitespace))), TableAlias)),   // Optional
                May(Sequence(May(CommentOrWhitespace), TableSampleClause)),
                May(Sequence(May(CommentOrWhitespace), TableHintClause)),
                May(Sequence(May(CommentOrWhitespace), TablePartitionClause))
            );

        public static Expression<Rule> FunctionTableSource = () =>
            Sequence
            (
                TableValuedFunctionCall,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), May(CommentOrWhitespace))),
                TableAlias     // Required
            );

        public static Expression<Rule> VariableTableSource = () =>
            Sequence
            (
                UserVariable,
                May(Sequence(May(CommentOrWhitespace), May(Sequence(Keyword("AS"), May(CommentOrWhitespace))), TableAlias))
            );

        public static Expression<Rule> SubqueryTableSource = () =>
            Sequence
            (
                Subquery,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                TableAlias     // Required
            );

        public static Expression<Rule> ColumnAliasBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                ColumnAliasList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ColumnAliasList = () =>
            Sequence(ColumnAlias, May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ColumnAliasList)));

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

        public static Expression<Rule> TablePartitionClause = () =>
            Sequence
            (
                Keyword("PARTITION"), CommentOrWhitespace, Keyword("BY"),
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
                BooleanExpression
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
                BooleanExpression
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
            // TODO: add OFFSET .. FETCH but do it as a separate clause to select so that order by clause can be used in ranking functions
            );

        public static Expression<Rule> OrderByList = () =>
            Sequence
            (
                OrderByArgument,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), OrderByList))
            );

        public static Expression<Rule> OrderByArgument = () =>
            Sequence
            (
                Expression,
                May(Sequence(May(CommentOrWhitespace), Must(Keyword("ASC"), Keyword("DESC"))))
            );

        #endregion
        #region Table and query hints

        public static Expression<Rule> TableHintClause = () =>
            Sequence
            (
                Keyword("WITH"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                May(TableHintList),
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TableHintList = () =>
            Sequence
            (
                TableHint,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), TableHintList))
            );

        public static Expression<Rule> TableHint = () =>
            Must(
                Sequence(Identifier, May(CommentOrWhitespace), FunctionArguments),
                Identifier
            );

        public static Expression<Rule> QueryHintClause = () =>
            Sequence
            (
                Keyword("OPTION"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                QueryHintList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> QueryHintList = () =>
            Sequence
            (
                QueryHint,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), QueryHintList))
            );

        public static Expression<Rule> QueryHint = () =>
            Must(
                Sequence(Identifier, May(CommentOrWhitespace), FunctionArguments),
                Sequence(Identifier, May(CommentOrWhitespace), Equals1, May(CommentOrWhitespace), NumericConstant),
                Sequence(Identifier, CommentOrWhitespace, NumericConstant),
                QueryHintIdentifierList,
                Identifier
            );

        public static Expression<Rule> QueryHintIdentifierList = () =>
            Sequence
            (
                Identifier,
                May(Sequence(CommentOrWhitespace, Identifier))
            );

        #endregion

        #region INSERT statement

        public static Expression<Rule> InsertStatement = () =>
            Sequence
            (
                May(Sequence(CommonTableExpression, May(CommentOrWhitespace))),
                Keyword("INSERT"),
                May(CommentOrWhitespace),
                Must
                (
                    // with or without the INTO keyword
                    IntoClause,
                    TargetTableSpecification
                ),
                // Column list
                // NOTE: SQL Server allows four-part column identifiers here but uses the very last part
                // (i.e. the column name), thus invalid and gibberish table, schema etc. is allowed
                May(
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        ColumnListBrackets
                    )
                ),
                // TODO: add OUTPUT clause
                May(CommentOrWhitespace),
                Must
                (
                    ValuesClause,
                    Sequence(Keyword("DEFAULT"), CommentOrWhitespace, Keyword("VALUES")),
                    // TODO: ExecuteStatement
                    // Select
                    Sequence
                    (
                        QueryExpression,
                        May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                        May(Sequence(May(CommentOrWhitespace), QueryHintClause))
                    )
                )
            );

        public static Expression<Rule> ColumnListBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                ColumnList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ColumnList = () =>
            Sequence
            (
                ColumnIdentifier,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ColumnList))
            );

        public static Expression<Rule> ValuesClause = () =>
            Sequence
            (
                Keyword("VALUES"),
                May(CommentOrWhitespace),
                ValueGroupList,
                May(CommentOrWhitespace)
            );

        public static Expression<Rule> ValueGroupList = () =>
            Sequence
            (
                ValueGroup,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ValueGroupList))
            );

        public static Expression<Rule> ValueGroup = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                ValueList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ValueList = () =>
            Sequence
            (
                Must
                (
                    Keyword("DEFAULT"),
                    Expression
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ValueList))
            );

        #endregion

        #region UPDATE statement

        public static Expression<Rule> UpdateStatement = () =>
            Sequence
            (
                May(Sequence(CommonTableExpression, May(CommentOrWhitespace))),
                Keyword("UPDATE"),
                May(CommentOrWhitespace),
                TargetTableSpecification,
                May(CommentOrWhitespace),
                Keyword("SET"),
                May(CommentOrWhitespace),
                UpdateSetList,
                // TODO: OUTPUT clause
                May(Sequence(May(CommentOrWhitespace), FromClause)),
                May(Sequence(May(CommentOrWhitespace), WhereClause)),
                // TODO: add support for cursor updates
                May(Sequence(May(CommentOrWhitespace), QueryHintClause))
            );

        public static Expression<Rule> UpdateSetList = () =>
            Sequence
            (
                Must
                (
                    UpdateSetColumn,
                    UpdateSetMutator    // This also covers column.WRITE(...)
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), UpdateSetList))
            );

        public static Expression<Rule> UpdateSetColumn = () =>
            Sequence
            (
                UpdateSetColumnLeftHandSide,
                May(CommentOrWhitespace),
                ValueAssignmentOperator,
                May(CommentOrWhitespace),
                UpdateSetColumnRightHandSide
            );

        public static Expression<Rule> UpdateSetColumnLeftHandSide = () =>
            Must
            (
                Sequence
                (
                    UserVariable,
                    May(CommentOrWhitespace),
                    Equals1,
                    May(CommentOrWhitespace),
                    ColumnIdentifier
                ),
                UserVariable,
                ColumnIdentifier        // This covers UDT fields and properties
            );

        public static Expression<Rule> UpdateSetColumnRightHandSide = () =>
            Must
            (
                Keyword("DEFAULT"),
                Expression
            );

        public static Expression<Rule> UpdateSetMutator = () =>
            Sequence
            (
                // TODO: A column identifier megeszi a metódus nevét is
                ColumnName,
                May(CommentOrWhitespace),
                UdtMethodCall
            );

        #endregion

        #region DELETE statement

        public static Expression<Rule> DeleteStatement = () =>
            Sequence
            (
                May(Sequence(CommonTableExpression, May(CommentOrWhitespace))),
                Keyword("DELETE"),
                May(Sequence(CommentOrWhitespace, Keyword("FROM"))),
                May(CommentOrWhitespace),
                TargetTableSpecification,
                // TODO: OUTPUT clause
                May(Sequence(May(CommentOrWhitespace), FromClause)),
                May(Sequence(May(CommentOrWhitespace), WhereClause)),
                May(Sequence(May(CommentOrWhitespace), QueryHintClause))
            );

        #endregion

        #region MERGE statement

        // TODO

        #endregion

        #region CREATE, ALTER, DROP and TRUNCATE TABLE

        public static Expression<Rule> CreateTableStatement = () =>
            Sequence
            (
                Keyword("CREATE"),
                CommentOrWhitespace,
                Keyword("TABLE"),
                May(CommentOrWhitespace),
                TableOrViewIdentifier,
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                TableDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            );

        // TODO: create table could be extended with options and file group part

        public static Expression<Rule> TableDefinitionList = () =>
            Sequence
            (
                TableDefinitionItem,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), TableDefinitionList))
            );

        public static Expression<Rule> TableDefinitionItem = () =>
            Must
            (
                ColumnDefinition,
                TableConstraint,
                TableIndex
            );

        public static Expression<Rule> ColumnDefinition = () =>
            Sequence
            (
                ColumnName,
                May(CommentOrWhitespace),
                DataTypeIdentifier,
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        ColumnNullDefinition
                    )
                ),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        Must
                        (
                            ColumnDefaultDefinition,
                            ColumnIdentityDefinition
                        )
                    )
                ),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        ColumnConstraint
                    )
                )
            );

        public static Expression<Rule> ColumnNullDefinition = () =>
            Sequence
            (
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("NULL")
            );

        // TODO: add computed columns

        public static Expression<Rule> ColumnDefaultDefinition = () =>
            Sequence
            (
                May(Sequence(ConstraintNameSpecification, May(CommentOrWhitespace))),
                Keyword("DEFAULT"),
                May(CommentOrWhitespace),
                Expression
            );

        public static Expression<Rule> ConstraintNameSpecification = () =>
            Sequence(Keyword("CONSTRAINT"), May(CommentOrWhitespace), ConstraintName);

        public static Expression<Rule> ColumnIdentityDefinition = () =>
            Sequence
            (
                Keyword("IDENTITY"),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        BracketOpen,
                        May(CommentOrWhitespace),
                        NumericConstant,
                        May(CommentOrWhitespace),
                        Comma,
                        May(CommentOrWhitespace),
                        NumericConstant,
                        May(CommentOrWhitespace),
                        BracketClose
                    )
                )
            );

        public static Expression<Rule> ColumnConstraint = () =>
            Sequence
            (
                May(Sequence(ConstraintNameSpecification, May(CommentOrWhitespace))),
                ConstraintSpecification
            );

        public static Expression<Rule> TableConstraint = () =>
            Sequence
            (
                May(Sequence(ConstraintNameSpecification, May(CommentOrWhitespace))),
                ConstraintSpecification,
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                IndexColumnDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ConstraintSpecification = () =>
            Sequence
            (
                Must
                (
                    Sequence(Keyword("PRIMARY"), CommentOrWhitespace, Keyword("KEY")),
                    Keyword("UNIQUE")
                ),
                May
                (
                    Sequence
                    (
                        CommentOrWhitespace,
                        IndexType
                    )
                )
            );

        public static Expression<Rule> TableIndex = () =>
            Sequence
            (
                Keyword("INDEX"),
                May(CommentOrWhitespace),
                IndexName,
                May(Sequence(May(CommentOrWhitespace), IndexType)),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                IndexColumnDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            // TODO: WITH, ON etc.
            );

        public static Expression<Rule> IndexType = () => Must(Keyword("CLUSTERED"), Keyword("NONCLUSTERED"));

        // TODO: any other constraints?

        public static Expression<Rule> DropTableStatement = () =>
            Sequence
            (
                Keyword("DROP"),
                CommentOrWhitespace,
                Keyword("TABLE"),
                May(CommentOrWhitespace),
                TableOrViewIdentifier
            );

        public static Expression<Rule> TruncateTableStatement = () =>
            Sequence
            (
                Keyword("TRUNCATE"),
                CommentOrWhitespace,
                Keyword("TABLE"),
                May(CommentOrWhitespace),
                TableOrViewIdentifier
            );

        #endregion

        #region Create index

        public static Expression<Rule> CreateIndexStatement = () =>
            Sequence
            (
                Keyword("CREATE"),
                May(Sequence(CommentOrWhitespace, Keyword("UNIQUE"))),
                May(Sequence(CommentOrWhitespace, Must(Keyword("CLUSTERED"), Keyword("NONCLUSTERED")))),
                CommentOrWhitespace,
                Keyword("INDEX"),
                May(CommentOrWhitespace),
                IndexName,
                May(CommentOrWhitespace),
                Keyword("ON"),
                May(CommentOrWhitespace),
                TableOrViewIdentifier,
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                IndexColumnDefinitionList,
                May(CommentOrWhitespace),
                BracketClose,
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        Keyword("INCLUDE"),
                        May(CommentOrWhitespace),
                        BracketOpen,
                        May(CommentOrWhitespace),
                        IncludedColumnList,
                        May(CommentOrWhitespace),
                        BracketClose
                    )
                )

            // TODO: WITH, ON
            );

        public static Expression<Rule> IndexColumnDefinitionList = () =>
            Sequence
            (
                IndexColumnDefinition,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), IndexColumnDefinitionList))
            );

        public static Expression<Rule> IndexColumnDefinition = () =>
            Sequence
            (
                ColumnName,
                May(Sequence(May(CommentOrWhitespace), Must(Keyword("ASC"), Keyword("DESC"))))
            );

        public static Expression<Rule> IncludedColumnList = () =>
            Sequence
            (
                IncludedColumnDefinition,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), IncludedColumnList))
            );

        public static Expression<Rule> IncludedColumnDefinition = () => ColumnName;

        public static Expression<Rule> DropIndexStatement = () =>
            Sequence
            (
                Keyword("DROP"),
                CommentOrWhitespace,
                Keyword("INDEX"),
                May(CommentOrWhitespace),
                IndexName,
                May(CommentOrWhitespace),
                Keyword("ON"),
                May(CommentOrWhitespace),
                TableOrViewIdentifier
            );

        #endregion
    }
}
