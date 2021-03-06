﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Jhu.Graywulf.Parsing.Generator;

namespace Jhu.Graywulf.Sql.Grammar
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
        public static Expression<Terminal> Label = () => @"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])";
        public static Expression<Terminal> NumericConstant = () => @"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)";
        public static Expression<Terminal> HexLiteral = () => @"\G0[xX][0-9a-fA-F]+";
        public static Expression<Terminal> StringConstant = () => @"\G('([^']|'')*')";
        public static Expression<Terminal> Identifier = () => @"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])";
        public static Expression<Terminal> Variable = () => @"\G(@[a-zA-Z_][0-9a-zA-Z_]*)";
        public static Expression<Terminal> Variable2 = () => @"\G(@@[a-zA-Z_][0-9a-zA-Z_]*)";
        public static Expression<Terminal> DatePart = () => @"\G([a-zA-Z]+)";

        #endregion
        #region Arithmetic operators used in expressions

        public static Expression<Rule> Operator = () => Abstract();

        public static Expression<Rule> MemberAccessOperator = () =>
            Inherit
            (
                Operator,
                Dot
            );

        public static Expression<Rule> StaticMemberAccessOperator = () =>
            Inherit
            (
                Operator,
                DoubleColon
            );

        public static Expression<Rule> UnaryOperator = () =>
            Inherit
            (
                Operator,
                Must
                (
                    Plus,
                    Minus,
                    BitwiseNot
                )
            );

        public static Expression<Rule> BinaryOperator = () =>
            Inherit
            (
                Operator,
                Must
                (
                    Plus,
                    Minus,
                    Mul,
                    Div,
                    Mod,
                    BitwiseAnd,
                    BitwiseOr,
                    BitwiseXor
                )
            );

        public static Expression<Rule> ComparisonOperator = () =>
            Inherit
            (
                Operator,
                Must
                (
                    Equals2,
                    Equals1,
                    LessOrGreaterThan,
                    NotEquals,
                    NotLessThan,
                    NotGreaterThan,
                    LessThanOrEqual,
                    GreaterThanOrEqual,
                    LessThan,
                    GreaterThan
                )
            );

        #endregion
        #region Logical operators used in search conditions

        public static Expression<Rule> LogicalNotOperator = () =>
            Inherit
            (
                Operator,
                Keyword("NOT")
            );

        public static Expression<Rule> LogicalOperator = () =>
            Inherit
            (
                Operator,
                Must
                (
                    Keyword("AND"),
                    Keyword("OR")
                )
            );

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
        public static Expression<Rule> FunctionName = () => Identifier;
        public static Expression<Rule> DataTypeName = () => Identifier;
        public static Expression<Rule> MethodName = () => Identifier;
        public static Expression<Rule> ColumnName = () => Identifier;
        public static Expression<Rule> ColumnAlias = () => Identifier;
        public static Expression<Rule> CursorName = () => Identifier;
        public static Expression<Rule> PropertyName = () => Identifier;

        // There are used for the generic multi-part names before name resolution,
        // then replaced with context-specific nodes
        public static Expression<Rule> ObjectName = () => Identifier;
        public static Expression<Rule> MemberName = () => Identifier;

        public static Expression<Rule> DatasetPrefix = () =>
            Sequence
            (
                DatasetName,
                May(CommentOrWhitespace),
                Colon
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
                Must
                (
                    Sequence
                    (
                        UnaryOperator,
                        May(CommentOrWhitespace),
                        Expression
                    ),
                    Operand
                ),
                May
                (
                    // Binary operators
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        BinaryOperator,
                        May(CommentOrWhitespace),
                        Expression
                    )
                )
            );

        public static Expression<Rule> Operand = () =>
            Sequence
            (
                Must
                (
                    Constant,

                    SystemVariable,
                    UserVariable,

                    ExpressionSubquery,
                    ExpressionBrackets,

                    SimpleCaseExpression,
                    SearchedCaseExpression,

                    UdtStaticMemberAccessList,

                    // Special function call syntax
                    CastAndParseFunctionCall,
                    ConvertFunctionCall,
                    DateFunctionCall,
                    IifFunctionCall,

                    StarFunctionCall,               // COUNT(*) OVER ()
                    AggregateFunctionCall,          // AVG(DISTINCT ...) OVER ()
                    WindowedFunctionCall,           // dbo.function() OVER ()

                    SystemFunctionCall,             // function() syntax

                    // This and the following member access list will handle
                    // all udfs, method calls, property access etc
                    ObjectName
                ),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        MemberAccessList
                    )
                )
            );

        public static Expression<Rule> ExpressionBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                Expression,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> UdtStaticMemberAccessList = () =>
            Sequence
            (
                DataTypeIdentifier,
                May(CommentOrWhitespace),
                StaticMemberAccessOperator,
                May(CommentOrWhitespace),
                Must
                (
                    UdtStaticMethodCall,
                    UdtStaticPropertyAccess
                )
            );

        public static Expression<Rule> MemberAccessList = () =>
            Sequence
            (
                MemberAccessOperator,
                May(CommentOrWhitespace),
                Must
                (
                    MemberCall,
                    MemberAccess
                ),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        MemberAccessList
                    )
                )
            );


        public static Expression<Rule> MemberAccess = () =>
            Sequence
            (
                MemberName
            );

        public static Expression<Rule> MemberCall = () =>
            Inherit
            (
                FunctionCall,
                Sequence
                (
                    MemberName,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    May(ArgumentList),
                    May(CommentOrWhitespace),
                    BracketClose
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

        public static Expression<Rule> ExpressionSubquery = () => Subquery;

        public static Expression<Rule> Null = () => Keyword("NULL");

        public static Expression<Rule> UserVariable = () => Variable;

        public static Expression<Rule> SystemVariable = () => Variable2;

        #endregion
        #region Special functions

        public static Expression<Rule> StarArgument = () => Mul;

        public static Expression<Rule> LogicalArgument = () => LogicalExpression;

        public static Expression<Rule> DataTypeArgument = () => DataTypeSpecification;

        public static Expression<Rule> SpecialFunctionCall = () => Abstract(FunctionCall);

        public static Expression<Rule> ConversionFunctionCall = () => Abstract(SpecialFunctionCall);

        public static Expression<Rule> CastAndParseFunctionCall = () =>
            Inherit
            (
                ConversionFunctionCall,
                Sequence
                (
                    Must(Literal("CAST"), Literal("TRY_CAST"), Literal("PARSE"), Literal("TRY_PARSE")),
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    Argument,
                    May(CommentOrWhitespace),
                    Keyword("AS"),
                    May(CommentOrWhitespace),
                    DataTypeArgument,
                    May
                    (
                        Sequence
                        (
                            May(CommentOrWhitespace),
                            Keyword("USING"),
                            May(CommentOrWhitespace),
                            StringConstant
                        )
                    ),
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> ConvertFunctionCall = () =>
            Inherit
            (
                ConversionFunctionCall,
                Sequence
                (
                    Must(Literal("CONVERT"), Literal("TRY_CONVERT")),
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    DataTypeArgument,
                    May(CommentOrWhitespace),
                    Comma,
                    May(CommentOrWhitespace),
                    Argument,
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> IifFunctionCall = () =>
            Inherit
            (
                SpecialFunctionCall,
                Sequence
                (
                    Literal("IIF"),
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    LogicalArgument,
                    May(CommentOrWhitespace),
                    Comma,
                    May(CommentOrWhitespace),
                    ArgumentList,
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> DateFunctionCall = () =>
            Inherit
            (
                SpecialFunctionCall,
                Sequence
                (
                    Must
                    (
                        Literal("DATEADD"),
                        Literal("DATEDIFF"),
                        Literal("DATEDIFF_BIG"),
                        Literal("DATENAME"),
                        Literal("DATEPART")
                    ),
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    DatePart,
                    May(CommentOrWhitespace),
                    Comma,
                    May(CommentOrWhitespace),
                    ArgumentList,
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        #endregion
        #region Logical expressions

        public static Expression<Rule> LogicalExpression = () =>
            Sequence
            (
                May(Sequence(LogicalNotOperator, May(CommentOrWhitespace))),
                Must
                (
                    Predicate,
                    LogicalExpressionBrackets
                ),
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        LogicalOperator,
                        May(CommentOrWhitespace),
                        LogicalExpression
                    )
                )
            );

        public static Expression<Rule> LogicalExpressionBrackets = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                LogicalExpression,
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
                InExpressionListPredicate,
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

        public static Expression<Rule> InExpressionListPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("IN"),
                May(CommentOrWhitespace),
                Sequence
                (
                    BracketOpen,
                    May(CommentOrWhitespace),
                    ArgumentList,
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> InSemiJoinPredicate = () =>
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("IN"),
                May(CommentOrWhitespace),
                SemiJoinSubquery
            );

        public static Expression<Rule> ComparisonSemiJoinPredicate = () =>

            // TODO: it can be used without keyword
            Sequence
            (
                Expression,
                May(CommentOrWhitespace),
                ComparisonOperator,
                May(CommentOrWhitespace),
                Must(Keyword("ALL"), Keyword("SOME"), Keyword("ANY")),
                May(CommentOrWhitespace),
                SemiJoinSubquery
            );

        public static Expression<Rule> ExistsSemiJoinPredicate = () =>
            Sequence
            (
                Keyword("EXISTS"),
                May(CommentOrWhitespace),
                SemiJoinSubquery
            );

        public static Expression<Rule> SemiJoinSubquery = () => Inherit(Subquery);

        #endregion
        #region Case-When constructs

        public static Expression<Rule> CaseExpression = () => Abstract();

        public static Expression<Rule> SimpleCaseExpression = () =>
            Inherit
            (
                CaseExpression,
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
                )
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
            Inherit
            (
                CaseExpression,
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
                )
            );

        public static Expression<Rule> SearchedCaseWhenList = () =>
            Sequence(SearchedCaseWhen, May(CommentOrWhitespace), May(SearchedCaseWhenList));

        public static Expression<Rule> SearchedCaseWhen = () =>
            Sequence
            (
                Keyword("WHEN"),
                May(CommentOrWhitespace),
                LogicalExpression,
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

        public static Expression<Rule> TargetTableSpecification = () =>
            Inherit
            (
                TableSource,
                Sequence
                (
                    Must
                    (
                        UserVariable,
                        TableOrViewIdentifier
                    // TODO: temp table?
                    ),
                    May(Sequence(May(CommentOrWhitespace), TableHintClause))
                )
            );

        public static Expression<Rule> ColumnIdentifier = () =>
            Sequence
            (
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), May(Sequence(SchemaName, May(CommentOrWhitespace))), Dot, May(CommentOrWhitespace), TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), ColumnName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), ColumnName),
                    Sequence(TableName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), ColumnName),
                    ColumnName
                )
            );

        public static Expression<Rule> StarColumnIdentifier = () =>
            Must
            (
                Mul,
                Sequence(TableOrViewIdentifier, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), Mul)
            );

        #endregion
        #region Data types

        // TODO: extend this when implementing array support

        // UDT restrictions:
        // - can only reference types from current database
        // - https://docs.microsoft.com/en-us/previous-versions/sql/sql-server-2008-r2/ms178069(v=sql.105)

        public static Expression<Rule> DataTypeIdentifier = () =>
            Sequence
            (
                Must
                (
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), DataTypeName),
                    DataTypeName
                )
            );

        public static Expression<Rule> DataTypeSpecification = () =>
            Sequence
            (
                DataTypeIdentifier,
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
        #region Function and method call syntax

        public static Expression<Rule> FunctionCall = () => Abstract();

        // This cannot be merged with ScalarFunctionCall because ScalarFunctionCall
        // is instantiated only after name resolution from multi-part identifiers.
        public static Expression<Rule> SystemFunctionCall = () =>
            Inherit
            (
                FunctionCall,
                Sequence
                (
                    FunctionName,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    May(ArgumentList),
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> ScalarFunctionCall = () => Inherit(FunctionCall);

        public static Expression<Rule> StarFunctionCall = () =>
            Inherit
            (
                WindowedFunctionCall,
                Sequence
                (
                    FunctionIdentifier,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    StarArgument,
                    May(CommentOrWhitespace),
                    BracketClose,
                    May
                    (
                        Sequence
                        (
                            May(CommentOrWhitespace),
                            OverClause
                        )
                    )
                )
            );

        public static Expression<Rule> AggregateFunctionCall = () =>
            Inherit
            (
                WindowedFunctionCall,
                Sequence
                (
                    FunctionIdentifier,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    Must(Keyword("ALL"), Keyword("DISTINCT")),
                    May(CommentOrWhitespace),
                    ArgumentList,
                    May(CommentOrWhitespace),
                    BracketClose,
                    May
                    (
                        Sequence
                        (
                            May(CommentOrWhitespace),
                            OverClause
                        )
                    )
                )
            );

        public static Expression<Rule> WindowedFunctionCall = () =>
            Inherit
            (
                ScalarFunctionCall,
                Sequence
                (
                    FunctionIdentifier,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May
                    (
                        Sequence
                        (
                            May(CommentOrWhitespace),
                            ArgumentList
                        )
                    ),
                    May(CommentOrWhitespace),
                    BracketClose,
                    May(CommentOrWhitespace),
                    OverClause
                )
            );

        public static Expression<Rule> TableValuedFunctionCall = () =>
            Inherit
            (
                FunctionCall,
                Sequence
                (
                    FunctionIdentifier,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    May(ArgumentList),
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> MethodCall = () => Abstract(FunctionCall);

        public static Expression<Rule> UdtMethodCall = () =>
            Inherit
            (
                MethodCall,
                Sequence
                (
                    MethodName,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    May(ArgumentList),
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> UdtStaticMethodCall = () =>
            Inherit
            (
                UdtMethodCall,
                Sequence
                (
                    MethodName,
                    May(CommentOrWhitespace),
                    BracketOpen,
                    May(CommentOrWhitespace),
                    May(ArgumentList),
                    May(CommentOrWhitespace),
                    BracketClose
                )
            );

        public static Expression<Rule> ArgumentList = () =>
            Sequence
            (
                Argument,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ArgumentList))
            );

        public static Expression<Rule> Argument = () => Expression;

        public static Expression<Rule> FunctionIdentifier = () =>
            Sequence
            (

                May(Sequence(DatasetPrefix, May(CommentOrWhitespace))),
                Must
                (
                    Sequence(DatabaseName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), FunctionName),
                    Sequence(SchemaName, May(CommentOrWhitespace), Dot, May(CommentOrWhitespace), FunctionName),
                    FunctionName
                )
            );

        public static Expression<Rule> OverClause = () =>
            Sequence
            (
                Keyword("OVER"),
                May(CommentOrWhitespace),
                BracketOpen,
                May(Sequence(May(CommentOrWhitespace), PartitionByClause)),
                May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                // TODO: ROW or RANGE
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> PartitionByClause = () =>
            Sequence
            (
                Keyword("PARTITION"),
                CommentOrWhitespace,
                Keyword("BY"),
                May(CommentOrWhitespace),
                Argument
            );

        #endregion
        #region Property access syntax

        public static Expression<Rule> PropertyAccess = () => Abstract();

        public static Expression<Rule> UdtPropertyAccess = () =>
            Inherit
            (
                PropertyAccess,
                Sequence
                (
                    PropertyName
                )
            );

        public static Expression<Rule> UdtStaticPropertyAccess = () =>
            Inherit
            (
                UdtPropertyAccess,
                Sequence
                (
                    PropertyName
                )
            );

        #endregion
        #region Statements

        public static Expression<Rule> StatementBlock = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                May(AnyStatement),
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

        public static Expression<Rule> AnyStatement = () =>
            Must
            (
                LabelDefinition,
                GotoStatement,
                BeginEndStatement,
                WhileStatement,
                BreakStatement,
                ContinueStatement,
                ReturnStatement,
                IfStatement,

                TryCatchStatement,
                ThrowStatement,

                PrintStatement,

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

        public static Expression<Rule> Statement = () => Abstract();

        #endregion

        #region Control flow statements

        public static Expression<Rule> LabelDefinition = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Label,
                    Colon
                )
            );

        public static Expression<Rule> GotoStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("GOTO"),
                    CommentOrWhitespace,
                    Label
                )
            );

        public static Expression<Rule> BeginEndStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("BEGIN"),
                    StatementBlock,
                    Keyword("END")
                )
            );

        public static Expression<Rule> WhileStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("WHILE"),
                    May(CommentOrWhitespace),
                    LogicalExpression,
                    May(CommentOrWhitespace),
                    AnyStatement
                )
            );

        public static Expression<Rule> BreakStatement = () =>
            Inherit
            (
                Statement,
                Keyword("BREAK")
            );

        public static Expression<Rule> ContinueStatement = () =>
            Inherit
            (
                Statement,
                Keyword("CONTINUE")
            );

        public static Expression<Rule> ReturnStatement = () =>
            Inherit
            (
                Statement,
                Keyword("RETURN")
            );
        // TODO: return can take a value

        public static Expression<Rule> PrintStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("PRINT"),
                    CommentOrWhitespace,
                    Expression
                )
            );

        public static Expression<Rule> IfStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("IF"),
                    May(CommentOrWhitespace),
                    LogicalExpression,
                    May(CommentOrWhitespace),
                    AnyStatement,
                    May(
                        Sequence(
                            StatementSeparator,
                            Keyword("ELSE"),
                            CommentOrWhitespace,
                            AnyStatement
                        )
                    )
                )
            );

        public static Expression<Rule> ThrowStatement = () =>
            Inherit
            (
                Statement,
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
                            Must(NumericConstant, UserVariable)
                        )
                    )
                )
            );

        public static Expression<Rule> TryCatchStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("BEGIN"), CommentOrWhitespace, Keyword("TRY"),
                    StatementBlock,
                    Keyword("END"), CommentOrWhitespace, Keyword("TRY"),
                    CommentOrWhitespace,
                    Keyword("BEGIN"), CommentOrWhitespace, Keyword("CATCH"),
                    Must(StatementBlock, CommentOrWhitespace),
                    Keyword("END"), CommentOrWhitespace, Keyword("CATCH")
                )
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
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("DECLARE"),
                    May(CommentOrWhitespace),
                    VariableDeclarationList
                )
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
                DataTypeSpecification,
                May(
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        ValueAssignmentOperator,
                        May(CommentOrWhitespace),
                        Expression
                    )
                )
            );

        // TODO: add UDT support

        public static Expression<Rule> SetVariableStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("SET"),
                    CommentOrWhitespace,
                    UserVariable,
                    May(CommentOrWhitespace),
                    ValueAssignmentOperator,
                    May(CommentOrWhitespace),
                    Expression
                )
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
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("DECLARE"),
                    May(CommentOrWhitespace),
                    Must
                    (
                        Sequence(CursorName, May(Sequence(May(CommentOrWhitespace), CursorDefinition))),
                        Sequence(UserVariable, May(CommentOrWhitespace), Keyword("CURSOR"))
                    )
                )
            );

        public static Expression<Rule> SetCursorStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("SET"),
                    May(CommentOrWhitespace),
                    UserVariable,
                    May(CommentOrWhitespace),
                    Equals1,
                    May(CommentOrWhitespace),
                    CursorDefinition
                )
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
            Inherit
            (
                Statement,
                Sequence
                (
                    Must(Keyword("OPEN"), Keyword("CLOSE"), Keyword("DEALLOCATE")),
                    May(CommentOrWhitespace),
                    Must(UserVariable, CursorName)
                )
            );

        public static Expression<Rule> FetchStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("FETCH"),
                    May(Sequence(CommentOrWhitespace, FetchOriginSpecification)),
                    May(CommentOrWhitespace),
                    FetchFromClause,
                    May(Sequence(CommentOrWhitespace, FetchIntoClause))
                )
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
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("DECLARE"),
                    May(CommentOrWhitespace),
                    TableDeclaration
                )
            );

        public static Expression<Rule> TableDeclaration = () =>
            Sequence
            (
                UserVariable,
                May(CommentOrWhitespace),
                May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                Keyword("TABLE"),
                May(CommentOrWhitespace),
                TableDefinition
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
            Inherit
            (
                TableSource,
                Sequence
                (
                    TableAlias,
                    May(Sequence(May(CommentOrWhitespace), ColumnAliasBrackets)),
                    May(CommentOrWhitespace),
                    Keyword("AS"),
                    May(CommentOrWhitespace),
                    CommonTableSubquery
                )
            );

        public static Expression<Rule> CommonTableSubquery = () => Inherit(Subquery);

        #endregion

        #region SELECT statement and query expressions (combinations of selects)



        public static Expression<Rule> SelectStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    May(Sequence(CommonTableExpression, May(CommentOrWhitespace))),
                    QueryExpression,
                    May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                    May(Sequence(May(CommentOrWhitespace), OptionClause))
                )
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
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                QueryExpression,
                May(CommentOrWhitespace),
                BracketClose
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
                Must(Expression),
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
                Sequence(UserVariable, May(CommentOrWhitespace), ValueAssignmentOperator, May(CommentOrWhitespace), Expression),
                Sequence(ColumnAlias, May(CommentOrWhitespace), ValueAssignmentOperator, May(CommentOrWhitespace), Expression),
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
                    Sequence
                    (
                        InnerOuterJoinOperator,
                        May(CommentOrWhitespace),
                        TableSourceSpecification,
                        May(CommentOrWhitespace),
                        JoinCondition
                    ),
                    Sequence
                    (
                        CrossJoinOperator,
                        May(CommentOrWhitespace),
                        TableSourceSpecification
                    ),
                    Sequence
                    (
                        CrossApplyOperator,
                        May(CommentOrWhitespace),
                        TableSourceSpecification
                    )
                ),
                May(Sequence(May(CommentOrWhitespace), JoinedTable))
            );

        public static Expression<Rule> JoinOperator = () => Abstract();

        public static Expression<Rule> CrossJoinOperator = () =>
            Inherit
            (
                JoinOperator,
                Must
                (
                    Comma,
                    Sequence
                    (
                        Keyword("CROSS"), CommentOrWhitespace, Keyword("JOIN")
                    )
                )
            );

        public static Expression<Rule> CrossApplyOperator = () =>
            Inherit
            (
                JoinOperator,
                Sequence
                (
                    Must(Keyword("CROSS"), Keyword("OUTER")),
                    CommentOrWhitespace,
                    Keyword("APPLY")
                )
            );

        public static Expression<Rule> InnerOuterJoinOperator = () =>
            Inherit
            (
                JoinOperator,
                Sequence
                (
                    May
                    (
                        Sequence
                        (
                            Must
                            (
                                Keyword("INNER"),
                                Sequence
                                (
                                    Must
                                    (
                                        Keyword("LEFT"), Keyword("RIGHT"), Keyword("FULL")
                                    ),
                                    May
                                    (
                                        Sequence(CommentOrWhitespace, Keyword("OUTER"))
                                    )
                                )
                            ),
                            May(CommentOrWhitespace),
                            May
                            (
                                Must
                                (
                                    Keyword("LOOP"), Keyword("HASH"), Keyword("MERGE"), Keyword("REMOTE")
                                )
                            )
                        )
                    ),
                    May(CommentOrWhitespace),
                    Keyword("JOIN")
                )
            );

        public static Expression<Rule> JoinCondition = () =>
            Sequence
            (
                Keyword("ON"), May(CommentOrWhitespace), LogicalExpression
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

        public static Expression<Rule> TableSource = () => Abstract();

        public static Expression<Rule> SimpleTableSource = () =>
            Inherit
            (
                TableSource,
                Sequence
                (
                    TableOrViewIdentifier,
                    May(Sequence(May(CommentOrWhitespace), May(Sequence(Keyword("AS"), May(CommentOrWhitespace))), TableAlias)),   // Optional
                    May(Sequence(May(CommentOrWhitespace), TableSampleClause)),
                    May(Sequence(May(CommentOrWhitespace), TableHintClause))
                )
            );

        public static Expression<Rule> FunctionTableSource = () =>
            Inherit
            (
                TableSource,
                Sequence
                (
                    TableValuedFunctionCall,
                    May(CommentOrWhitespace),
                    May(Sequence(Keyword("AS"), May(CommentOrWhitespace))),
                    TableAlias     // Required
                )
            );

        public static Expression<Rule> VariableTableSource = () =>
            Inherit
            (
                TableSource,
                Sequence
                (
                    UserVariable,
                    May(Sequence(May(CommentOrWhitespace), May(Sequence(Keyword("AS"), May(CommentOrWhitespace))), TableAlias))
                )
            );

        public static Expression<Rule> SubqueryTableSource = () =>
            Inherit
            (
                TableSource,
                Sequence
                (
                    Subquery,
                    May(CommentOrWhitespace),
                    May(Sequence(Keyword("AS"), CommentOrWhitespace)),
                    TableAlias     // Required
                )
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
                NumericConstant,
                May(Sequence(CommentOrWhitespace, May(Must(Keyword("PERCENT"), Keyword("ROWS"))))),
                May(CommentOrWhitespace), BracketClose,
                May(Sequence
                (
                    CommentOrWhitespace,
                    Keyword("REPEATABLE"),
                    May(CommentOrWhitespace), BracketOpen, May(CommentOrWhitespace),
                    NumericConstant,
                    May(CommentOrWhitespace), BracketClose)
                )
            );

        #endregion
        #region Where clause, search conditions and predicates

        public static Expression<Rule> WhereClause = () =>
            Sequence
            (
                Keyword("WHERE"),
                May(CommentOrWhitespace),
                LogicalExpression
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
                LogicalExpression
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
                OrderByArgumentList
            // TODO: add OFFSET .. FETCH but do it as a separate clause to select so that order by clause can be used in ranking functions
            );

        public static Expression<Rule> OrderByArgumentList = () =>
            Sequence
            (
                OrderByArgument,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), OrderByArgumentList))
            );

        public static Expression<Rule> OrderByArgument = () =>
            Sequence
            (
                Expression,
                May(Sequence(May(CommentOrWhitespace), Must(Keyword("ASC"), Keyword("DESC"))))
            );

        #endregion
        #region Table and query hints

        public static Expression<Rule> HintArguments = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                May(HintArgumentList),
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> HintArgumentList = () =>
            Sequence
            (
                HintArgument,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), HintArgumentList))
            );

        public static Expression<Rule> HintArgument = () => Expression;

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
                Sequence(HintName, May(CommentOrWhitespace), HintArguments),
                HintName
            );

        public static Expression<Rule> HintName = () => Identifier;

        public static Expression<Rule> OptionClause = () =>
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
                Sequence(HintName, May(CommentOrWhitespace), HintArguments),
                Sequence(HintName, May(CommentOrWhitespace), Equals1, May(CommentOrWhitespace), NumericConstant),
                Sequence(HintName, CommentOrWhitespace, NumericConstant),
                QueryHintNameList,
                HintName
            );

        public static Expression<Rule> QueryHintNameList = () =>
            Sequence
            (
                HintName,
                May(Sequence(CommentOrWhitespace, HintName))
            );

        #endregion

        #region INSERT statement

        public static Expression<Rule> InsertStatement = () =>
            Inherit
            (
                Statement,
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
                    May(Sequence(May(CommentOrWhitespace), InsertColumnList)),
                    // TODO: add OUTPUT clause
                    May(CommentOrWhitespace),
                    Must
                    (
                        DefaultValues,
                        ValuesClause,
                        // TODO: ExecuteStatement
                        // Select
                        Sequence
                        (
                            QueryExpression,
                            May(Sequence(May(CommentOrWhitespace), OrderByClause)),
                            May(Sequence(May(CommentOrWhitespace), OptionClause))
                        )
                    )
                )
            );

        public static Expression<Rule> InsertColumnList = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                ColumnIdentifierList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> ColumnIdentifierList = () =>
            Sequence
            (
                ColumnIdentifier,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ColumnIdentifierList))
            );

        public static Expression<Rule> DefaultValues = () =>
            Sequence(Keyword("DEFAULT"), CommentOrWhitespace, Keyword("VALUES"));

        public static Expression<Rule> ValuesClause = () =>
            Sequence
            (
                Keyword("VALUES"),
                May(CommentOrWhitespace),
                ValueGroupList
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
                    DefaultValue,
                    Expression
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), ValueList))
            );

        public static Expression<Rule> DefaultValue = () =>
            Keyword("DEFAULT");

        #endregion

        #region UPDATE statement

        public static Expression<Rule> UpdateStatement = () =>
            Inherit
            (
                Statement,
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
                    May(Sequence(May(CommentOrWhitespace), OptionClause))
                )
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
                    ColumnIdentifier    // TODO: UDT properties!
                ),
                UserVariable,
                ColumnIdentifier        // TODO: UDT properties!
            );

        public static Expression<Rule> UpdateSetColumnRightHandSide = () =>
            Must
            (
                DefaultValue,
                Expression
            );

        public static Expression<Rule> UpdateSetMutator = () =>
            Sequence
            (
                ColumnName,
                May(CommentOrWhitespace),
                MemberAccessOperator,
                May(CommentOrWhitespace),
                UdtMethodCall
            );

        #endregion

        #region DELETE statement

        public static Expression<Rule> DeleteStatement = () =>
            Inherit
            (
                Statement,
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
                    May(Sequence(May(CommentOrWhitespace), OptionClause))
                )
            );

        #endregion

        #region MERGE statement

        // TODO

        #endregion

        #region CREATE, ALTER, DROP and TRUNCATE TABLE

        public static Expression<Rule> CreateTableStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("CREATE"),
                    CommentOrWhitespace,
                    Keyword("TABLE"),
                    May(CommentOrWhitespace),
                    TableOrViewIdentifier,
                    May(CommentOrWhitespace),
                    TableDefinition
                )
            );

        // TODO: create table could be extended with options and file group part

        public static Expression<Rule> TableDefinition = () =>
            Sequence
            (
                BracketOpen,
                May(CommentOrWhitespace),
                TableDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            );

        public static Expression<Rule> TableDefinitionList = () =>
            Sequence
            (
                Must
                (
                    ColumnDefinition,
                    TableConstraint,
                    TableIndex
                ),
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), TableDefinitionList))
            );

        public static Expression<Rule> ColumnDefinition = () =>
            Sequence
            (
                ColumnName,
                May(CommentOrWhitespace),
                DataTypeSpecification,
                May(Sequence(May(CommentOrWhitespace), ColumnSpecificationList))
            );

        public static Expression<Rule> ColumnSpecificationList = () =>
            Sequence
            (
                Must
                (
                    ColumnNullSpecification,
                    ColumnDefaultSpecification,
                    ColumnIdentitySpecification,
                    ColumnConstraint,
                    ColumnIndex
                ),
                May(Sequence(May(CommentOrWhitespace), ColumnSpecificationList))
            );

        public static Expression<Rule> ColumnNullSpecification = () =>
            Sequence
            (
                May(Sequence(Keyword("NOT"), CommentOrWhitespace)),
                Keyword("NULL")
            );

        // TODO: add computed columns

        public static Expression<Rule> ColumnDefaultSpecification = () =>
            Sequence
            (
                Keyword("DEFAULT"),
                May(CommentOrWhitespace),
                Expression
            );

        public static Expression<Rule> ColumnIdentitySpecification = () =>
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
                May
                (
                    Sequence
                    (
                        Keyword("CONSTRAINT"),
                        May(CommentOrWhitespace),
                        ConstraintName,
                        May(CommentOrWhitespace)
                    )
                ),
                Must
                (
                    IndexTypeSpecification,
                    ColumnDefaultSpecification
                    // TODO: foreign key
                )
            );

        public static Expression<Rule> ColumnIndex = () =>
            Sequence
            (
                May
                (
                    Sequence
                    (
                        Keyword("INDEX"),
                        May(CommentOrWhitespace),
                        IndexName,
                        May(CommentOrWhitespace)
                    )
                ),
                IndexTypeSpecification
            );

        public static Expression<Rule> TableConstraint = () =>
            Sequence
            (
                May
                (
                    Sequence
                    (
                        Keyword("CONSTRAINT"),
                        May(CommentOrWhitespace),
                        ConstraintName,
                        May(CommentOrWhitespace)
                    )
                ),
                May(Sequence(IndexTypeSpecification, May(CommentOrWhitespace))),
                BracketOpen,
                May(CommentOrWhitespace),
                IndexColumnDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            );
        // TODO: FOREIGN KEY

        public static Expression<Rule> TableIndex = () =>
            Sequence
            (
                Keyword("INDEX"),
                May(CommentOrWhitespace),
                IndexName,
                May(Sequence(May(CommentOrWhitespace), IndexTypeSpecification)),
                May(CommentOrWhitespace),
                BracketOpen,
                May(CommentOrWhitespace),
                IndexColumnDefinitionList,
                May(CommentOrWhitespace),
                BracketClose
            // TODO: WITH, ON etc.
            );

        // TODO: any other constraints?

        public static Expression<Rule> DropTableStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("DROP"),
                    CommentOrWhitespace,
                    Keyword("TABLE"),
                    May(CommentOrWhitespace),
                    TableOrViewIdentifier
                )
            );

        public static Expression<Rule> TruncateTableStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("TRUNCATE"),
                    CommentOrWhitespace,
                    Keyword("TABLE"),
                    May(CommentOrWhitespace),
                    TableOrViewIdentifier
                )
            );

        #endregion

        #region Create index

        public static Expression<Rule> CreateIndexStatement = () =>
            Inherit
            (
                Statement,
                Sequence
                (
                    Keyword("CREATE"),
                    May(Sequence(CommentOrWhitespace, IndexTypeSpecification)),
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
                            IncludedColumnDefinitionList,
                            May(CommentOrWhitespace),
                            BracketClose
                        )
                    )

                // TODO: WITH, ON
                )
            );

        public static Expression<Rule> IndexTypeSpecification = () =>
            Must
            (
                Keyword("CLUSTERED"),
                Keyword("NONCLUSTERED"),
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
                            Must
                            (
                                Keyword("CLUSTERED"),
                                Keyword("NONCLUSTERED")
                            )
                        )
                    )
                )  
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

        public static Expression<Rule> IncludedColumnDefinitionList = () =>
            Sequence
            (
                IncludedColumnDefinition,
                May(Sequence(May(CommentOrWhitespace), Comma, May(CommentOrWhitespace), IncludedColumnDefinitionList))
            );

        public static Expression<Rule> IncludedColumnDefinition = () => ColumnName;

        public static Expression<Rule> DropIndexStatement = () =>
            Inherit
            (
                Statement,
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
                )
            );

        #endregion
    }
}
