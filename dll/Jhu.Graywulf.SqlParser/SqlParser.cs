using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.SqlParser
{
    public partial class SqlParser : Jhu.Graywulf.ParserLib.Parser
    {
        private static HashSet<string> keywords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "ALL", "AND", "ANY", "APPLY", "AS", "ASC", "BETWEEN", "BY", "CASE", 
            "CROSS", "DESC", "DISTINCT", "ELSE", "END", "ESCAPE", "EXCEPT", "EXISTS", "FASTFIRSTROW", "FROM", 
            "FULL", "GROUP", "HASH", "HAVING", "HOLDLOCK", "IN", "INDEX", "INNER", "INTERSECT", "INTO", 
            "IS", "JOIN", "LEFT", "LIKE", "LOOP", "MERGE", "NOEXPAND", "NOLOCK", "NOT", "NOWAIT", 
            "NULL", "ON", "OR", "ORDER", "OUTER", "PAGLOCK", "PARTITION", "PERCENT", "READCOMMITTED", "READCOMMITTEDLOCK", 
            "READPAST", "READUNCOMMITTED", "REMOTE", "REPEATABLE", "REPEATABLEREAD", "RIGHT", "ROWLOCK", "ROWS", "SELECT", "SERIALIZABLE", 
            "SOME", "SYSTEM", "TABLESAMPLE", "TABLOCK", "TABLOCKX", "THEN", "TIES", "TOP", "UNION", "UPDLOCK", 
            "WHEN", "WHERE", "WITH", "XLOCK", 

        };

        public override HashSet<string> Keywords
        {
            get { return keywords; }
        }   

        public static StringComparer ComparerInstance
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }

        public override StringComparer Comparer
        {
            get { return StringComparer.InvariantCultureIgnoreCase; }
        }

        public override Jhu.Graywulf.ParserLib.Token Execute(string code)
        {
            return Execute(new SelectStatement(), code);
        }
    }

    public partial class Plus : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"+"; }
        }

        public Plus()
        {
            Value = @"+";
        }
    }

    public partial class Minus : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"-"; }
        }

        public Minus()
        {
            Value = @"-";
        }
    }

    public partial class Mul : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"*"; }
        }

        public Mul()
        {
            Value = @"*";
        }
    }

    public partial class Div : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"/"; }
        }

        public Div()
        {
            Value = @"/";
        }
    }

    public partial class Mod : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"%"; }
        }

        public Mod()
        {
            Value = @"%";
        }
    }

    public partial class BitwiseNot : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"~"; }
        }

        public BitwiseNot()
        {
            Value = @"~";
        }
    }

    public partial class BitwiseAnd : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"&"; }
        }

        public BitwiseAnd()
        {
            Value = @"&";
        }
    }

    public partial class BitwiseOr : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"|"; }
        }

        public BitwiseOr()
        {
            Value = @"|";
        }
    }

    public partial class BitwiseXor : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"^"; }
        }

        public BitwiseXor()
        {
            Value = @"^";
        }
    }

    public partial class Equals : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"="; }
        }

        public Equals()
        {
            Value = @"=";
        }
    }

    public partial class Equals2 : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"=="; }
        }

        public Equals2()
        {
            Value = @"==";
        }
    }

    public partial class LessOrGreaterThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"<>"; }
        }

        public LessOrGreaterThan()
        {
            Value = @"<>";
        }
    }

    public partial class NotEquals : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"!="; }
        }

        public NotEquals()
        {
            Value = @"!=";
        }
    }

    public partial class NotLessThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"!<"; }
        }

        public NotLessThan()
        {
            Value = @"!<";
        }
    }

    public partial class NotGreaterThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"!>"; }
        }

        public NotGreaterThan()
        {
            Value = @"!>";
        }
    }

    public partial class LessOrEqualThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"<="; }
        }

        public LessOrEqualThan()
        {
            Value = @"<=";
        }
    }

    public partial class GreaterOrEqualThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @">="; }
        }

        public GreaterOrEqualThan()
        {
            Value = @">=";
        }
    }

    public partial class LessThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"<"; }
        }

        public LessThan()
        {
            Value = @"<";
        }
    }

    public partial class GreaterThan : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @">"; }
        }

        public GreaterThan()
        {
            Value = @">";
        }
    }

    public partial class DoubleColon : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"::"; }
        }

        public DoubleColon()
        {
            Value = @"::";
        }
    }

    public partial class Dot : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"."; }
        }

        public Dot()
        {
            Value = @".";
        }
    }

    public partial class Comma : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @","; }
        }

        public Comma()
        {
            Value = @",";
        }
    }

    public partial class Semicolon : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @";"; }
        }

        public Semicolon()
        {
            Value = @";";
        }
    }

    public partial class Colon : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @":"; }
        }

        public Colon()
        {
            Value = @":";
        }
    }

    public partial class BracketOpen : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"("; }
        }

        public BracketOpen()
        {
            Value = @"(";
        }
    }

    public partial class BracketClose : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @")"; }
        }

        public BracketClose()
        {
            Value = @")";
        }
    }

    public partial class VectorOpen : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"{"; }
        }

        public VectorOpen()
        {
            Value = @"{";
        }
    }

    public partial class VectorClose : Jhu.Graywulf.ParserLib.Symbol
    {
        protected override string Pattern
        {
            get { return @"}"; }
        }

        public VectorClose()
        {
            Value = @"}";
        }
    }


    public partial class Number : Jhu.Graywulf.ParserLib.Terminal
    {
        private static Regex pattern = new Regex(@"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }

    public partial class StringConstant : Jhu.Graywulf.ParserLib.Terminal
    {
        private static Regex pattern = new Regex(@"\G('([^']|'')*')", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }

    public partial class Identifier : Jhu.Graywulf.ParserLib.Terminal
    {
        private static Regex pattern = new Regex(@"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }

    public partial class Variable : Jhu.Graywulf.ParserLib.Terminal
    {
        private static Regex pattern = new Regex(@"\G(@[$a-zA-Z_]+)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }

    public partial class SystemVariable : Jhu.Graywulf.ParserLib.Terminal
    {
        private static Regex pattern = new Regex(@"\G(@@[$a-zA-Z_]+)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }


    public partial class Whitespace : Jhu.Graywulf.ParserLib.Whitespace
    {
        private static Regex pattern = new Regex(@"\G\s+", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }


    public partial class SingleLineComment : Jhu.Graywulf.ParserLib.Comment
    {
        private static Regex pattern = new Regex(@"\G--.*", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }

    public partial class MultiLineComment : Jhu.Graywulf.ParserLib.Comment
    {
        private static Regex pattern = new Regex(@"\G(?sm)/\*.*?\*/", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }


    public partial class UnaryOperator : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1

                bool r1 = true;
                if (r1)
                { // alternatives a2 must
                    bool a2 = false;
                    if (!a2)
                    {
                        Checkpoint(parser); // r3

                        bool r3 = true;
                        r3 = r3 && Match(parser, new Jhu.Graywulf.SqlParser.Plus());
                        CommitOrRollback(r3, parser);
                        a2 = r3;
                    }

                    if (!a2)
                    {
                        Checkpoint(parser); // r4

                        bool r4 = true;
                        r4 = r4 && Match(parser, new Jhu.Graywulf.SqlParser.Minus());
                        CommitOrRollback(r4, parser);
                        a2 = r4;
                    }

                    if (!a2)
                    {
                        Checkpoint(parser); // r5

                        bool r5 = true;
                        r5 = r5 && Match(parser, new Jhu.Graywulf.SqlParser.BitwiseNot());
                        CommitOrRollback(r5, parser);
                        a2 = r5;
                    }

                    r1 &= a2;

                } // end alternatives a2

                CommitOrRollback(r1, parser);
                res = r1;
            }

            return res;

        }
    }

    public partial class ArithmeticOperator : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r6

                bool r6 = true;
                if (r6)
                { // alternatives a7 must
                    bool a7 = false;
                    if (!a7)
                    {
                        Checkpoint(parser); // r8

                        bool r8 = true;
                        r8 = r8 && Match(parser, new Jhu.Graywulf.SqlParser.Plus());
                        CommitOrRollback(r8, parser);
                        a7 = r8;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r9

                        bool r9 = true;
                        r9 = r9 && Match(parser, new Jhu.Graywulf.SqlParser.Minus());
                        CommitOrRollback(r9, parser);
                        a7 = r9;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r10

                        bool r10 = true;
                        r10 = r10 && Match(parser, new Jhu.Graywulf.SqlParser.Mul());
                        CommitOrRollback(r10, parser);
                        a7 = r10;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r11

                        bool r11 = true;
                        r11 = r11 && Match(parser, new Jhu.Graywulf.SqlParser.Div());
                        CommitOrRollback(r11, parser);
                        a7 = r11;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r12

                        bool r12 = true;
                        r12 = r12 && Match(parser, new Jhu.Graywulf.SqlParser.Mod());
                        CommitOrRollback(r12, parser);
                        a7 = r12;
                    }

                    r6 &= a7;

                } // end alternatives a7

                CommitOrRollback(r6, parser);
                res = r6;
            }

            return res;

        }
    }

    public partial class BitwiseOperator : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r13

                bool r13 = true;
                if (r13)
                { // alternatives a14 must
                    bool a14 = false;
                    if (!a14)
                    {
                        Checkpoint(parser); // r15

                        bool r15 = true;
                        r15 = r15 && Match(parser, new Jhu.Graywulf.SqlParser.BitwiseAnd());
                        CommitOrRollback(r15, parser);
                        a14 = r15;
                    }

                    if (!a14)
                    {
                        Checkpoint(parser); // r16

                        bool r16 = true;
                        r16 = r16 && Match(parser, new Jhu.Graywulf.SqlParser.BitwiseOr());
                        CommitOrRollback(r16, parser);
                        a14 = r16;
                    }

                    if (!a14)
                    {
                        Checkpoint(parser); // r17

                        bool r17 = true;
                        r17 = r17 && Match(parser, new Jhu.Graywulf.SqlParser.BitwiseXor());
                        CommitOrRollback(r17, parser);
                        a14 = r17;
                    }

                    r13 &= a14;

                } // end alternatives a14

                CommitOrRollback(r13, parser);
                res = r13;
            }

            return res;

        }
    }

    public partial class ComparisonOperator : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r18

                bool r18 = true;
                if (r18)
                { // alternatives a19 must
                    bool a19 = false;
                    if (!a19)
                    {
                        Checkpoint(parser); // r20

                        bool r20 = true;
                        r20 = r20 && Match(parser, new Jhu.Graywulf.SqlParser.Equals2());
                        CommitOrRollback(r20, parser);
                        a19 = r20;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r21

                        bool r21 = true;
                        r21 = r21 && Match(parser, new Jhu.Graywulf.SqlParser.Equals());
                        CommitOrRollback(r21, parser);
                        a19 = r21;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r22

                        bool r22 = true;
                        r22 = r22 && Match(parser, new Jhu.Graywulf.SqlParser.LessOrGreaterThan());
                        CommitOrRollback(r22, parser);
                        a19 = r22;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r23

                        bool r23 = true;
                        r23 = r23 && Match(parser, new Jhu.Graywulf.SqlParser.NotEquals());
                        CommitOrRollback(r23, parser);
                        a19 = r23;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r24

                        bool r24 = true;
                        r24 = r24 && Match(parser, new Jhu.Graywulf.SqlParser.NotLessThan());
                        CommitOrRollback(r24, parser);
                        a19 = r24;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r25

                        bool r25 = true;
                        r25 = r25 && Match(parser, new Jhu.Graywulf.SqlParser.NotGreaterThan());
                        CommitOrRollback(r25, parser);
                        a19 = r25;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r26

                        bool r26 = true;
                        r26 = r26 && Match(parser, new Jhu.Graywulf.SqlParser.LessOrEqualThan());
                        CommitOrRollback(r26, parser);
                        a19 = r26;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r27

                        bool r27 = true;
                        r27 = r27 && Match(parser, new Jhu.Graywulf.SqlParser.GreaterOrEqualThan());
                        CommitOrRollback(r27, parser);
                        a19 = r27;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r28

                        bool r28 = true;
                        r28 = r28 && Match(parser, new Jhu.Graywulf.SqlParser.LessThan());
                        CommitOrRollback(r28, parser);
                        a19 = r28;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r29

                        bool r29 = true;
                        r29 = r29 && Match(parser, new Jhu.Graywulf.SqlParser.GreaterThan());
                        CommitOrRollback(r29, parser);
                        a19 = r29;
                    }

                    r18 &= a19;

                } // end alternatives a19

                CommitOrRollback(r18, parser);
                res = r18;
            }

            return res;

        }
    }

    public partial class LogicalNot : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r30

                bool r30 = true;
                r30 = r30 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOT"));
                CommitOrRollback(r30, parser);
                res = r30;
            }

            return res;

        }
    }

    public partial class LogicalOperator : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r31

                bool r31 = true;
                if (r31)
                { // alternatives a32 must
                    bool a32 = false;
                    if (!a32)
                    {
                        Checkpoint(parser); // r33

                        bool r33 = true;
                        r33 = r33 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AND"));
                        CommitOrRollback(r33, parser);
                        a32 = r33;
                    }

                    if (!a32)
                    {
                        Checkpoint(parser); // r34

                        bool r34 = true;
                        r34 = r34 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"OR"));
                        CommitOrRollback(r34, parser);
                        a32 = r34;
                    }

                    r31 &= a32;

                } // end alternatives a32

                CommitOrRollback(r31, parser);
                res = r31;
            }

            return res;

        }
    }

    public partial class DatasetName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r35

                bool r35 = true;
                r35 = r35 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r35, parser);
                res = r35;
            }

            return res;

        }
    }

    public partial class DatabaseName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r36

                bool r36 = true;
                r36 = r36 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r36, parser);
                res = r36;
            }

            return res;

        }
    }

    public partial class SchemaName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r37

                bool r37 = true;
                r37 = r37 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r37, parser);
                res = r37;
            }

            return res;

        }
    }

    public partial class TableName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r38

                bool r38 = true;
                r38 = r38 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r38, parser);
                res = r38;
            }

            return res;

        }
    }

    public partial class DerivedTable : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r39

                bool r39 = true;
                r39 = r39 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r39, parser);
                res = r39;
            }

            return res;

        }
    }

    public partial class TableAlias : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r40

                bool r40 = true;
                r40 = r40 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r40, parser);
                res = r40;
            }

            return res;

        }
    }

    public partial class FunctionName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r41

                bool r41 = true;
                r41 = r41 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r41, parser);
                res = r41;
            }

            return res;

        }
    }

    public partial class ColumnName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r42

                bool r42 = true;
                r42 = r42 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r42, parser);
                res = r42;
            }

            return res;

        }
    }

    public partial class ColumnAlias : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r43

                bool r43 = true;
                r43 = r43 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r43, parser);
                res = r43;
            }

            return res;

        }
    }

    public partial class ColumnPosition : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r44

                bool r44 = true;
                r44 = r44 && Match(parser, new Jhu.Graywulf.SqlParser.Number());
                CommitOrRollback(r44, parser);
                res = r44;
            }

            return res;

        }
    }

    public partial class UdtColumnName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r45

                bool r45 = true;
                r45 = r45 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r45, parser);
                res = r45;
            }

            return res;

        }
    }

    public partial class PropertyName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r46

                bool r46 = true;
                r46 = r46 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r46, parser);
                res = r46;
            }

            return res;

        }
    }

    public partial class SampleNumber : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r47

                bool r47 = true;
                r47 = r47 && Match(parser, new Jhu.Graywulf.SqlParser.Number());
                CommitOrRollback(r47, parser);
                res = r47;
            }

            return res;

        }
    }

    public partial class RepeatSeed : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r48

                bool r48 = true;
                r48 = r48 && Match(parser, new Jhu.Graywulf.SqlParser.Number());
                CommitOrRollback(r48, parser);
                res = r48;
            }

            return res;

        }
    }

    public partial class IndexValue : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r49

                bool r49 = true;
                r49 = r49 && Match(parser, new Jhu.Graywulf.SqlParser.Identifier());
                CommitOrRollback(r49, parser);
                res = r49;
            }

            return res;

        }
    }

    public partial class CommentOrWhitespace : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r50

                bool r50 = true;
                if (r50)
                { // alternatives a51 must
                    bool a51 = false;
                    if (!a51)
                    {
                        Checkpoint(parser); // r52

                        bool r52 = true;
                        r52 = r52 && Match(parser, new Jhu.Graywulf.SqlParser.MultiLineComment());
                        CommitOrRollback(r52, parser);
                        a51 = r52;
                    }

                    if (!a51)
                    {
                        Checkpoint(parser); // r53

                        bool r53 = true;
                        r53 = r53 && Match(parser, new Jhu.Graywulf.SqlParser.SingleLineComment());
                        CommitOrRollback(r53, parser);
                        a51 = r53;
                    }

                    if (!a51)
                    {
                        Checkpoint(parser); // r54

                        bool r54 = true;
                        r54 = r54 && Match(parser, new Jhu.Graywulf.SqlParser.Whitespace());
                        CommitOrRollback(r54, parser);
                        a51 = r54;
                    }

                    r50 &= a51;

                } // end alternatives a51

                if (r50)
                { // may a55
                    bool a55 = false;
                    {
                        Checkpoint(parser); // r56

                        bool r56 = true;
                        r56 = r56 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r56, parser);
                        a55 = r56;
                    }

                    r50 |= a55;
                } // end may a55

                CommitOrRollback(r50, parser);
                res = r50;
            }

            return res;

        }
    }

    public partial class Expression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r57

                bool r57 = true;
                if (r57)
                { // alternatives a58 must
                    bool a58 = false;
                    if (!a58)
                    {
                        Checkpoint(parser); // r59

                        bool r59 = true;
                        r59 = r59 && Match(parser, new Jhu.Graywulf.SqlParser.ExpressionBrackets());
                        CommitOrRollback(r59, parser);
                        a58 = r59;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r60

                        bool r60 = true;
                        r60 = r60 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionCall());
                        CommitOrRollback(r60, parser);
                        a58 = r60;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r61

                        bool r61 = true;
                        r61 = r61 && Match(parser, new Jhu.Graywulf.SqlParser.UnaryOperator());
                        if (r61)
                        { // may a62
                            bool a62 = false;
                            {
                                Checkpoint(parser); // r63

                                bool r63 = true;
                                r63 = r63 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r63, parser);
                                a62 = r63;
                            }

                            r61 |= a62;
                        } // end may a62

                        r61 = r61 && Match(parser, new Jhu.Graywulf.SqlParser.Number());
                        CommitOrRollback(r61, parser);
                        a58 = r61;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r64

                        bool r64 = true;
                        r64 = r64 && Match(parser, new Jhu.Graywulf.SqlParser.UnaryOperator());
                        if (r64)
                        { // may a65
                            bool a65 = false;
                            {
                                Checkpoint(parser); // r66

                                bool r66 = true;
                                r66 = r66 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r66, parser);
                                a65 = r66;
                            }

                            r64 |= a65;
                        } // end may a65

                        r64 = r64 && Match(parser, new Jhu.Graywulf.SqlParser.AnyVariable());
                        CommitOrRollback(r64, parser);
                        a58 = r64;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r67

                        bool r67 = true;
                        r67 = r67 && Match(parser, new Jhu.Graywulf.SqlParser.Number());
                        CommitOrRollback(r67, parser);
                        a58 = r67;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r68

                        bool r68 = true;
                        r68 = r68 && Match(parser, new Jhu.Graywulf.SqlParser.AnyVariable());
                        CommitOrRollback(r68, parser);
                        a58 = r68;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r69

                        bool r69 = true;
                        r69 = r69 && Match(parser, new Jhu.Graywulf.SqlParser.StringConstant());
                        CommitOrRollback(r69, parser);
                        a58 = r69;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r70

                        bool r70 = true;
                        r70 = r70 && Match(parser, new Jhu.Graywulf.SqlParser.SimpleCaseExpression());
                        CommitOrRollback(r70, parser);
                        a58 = r70;
                    }

                    if (!a58)
                    {
                        Checkpoint(parser); // r71

                        bool r71 = true;
                        r71 = r71 && Match(parser, new Jhu.Graywulf.SqlParser.SearchedCaseExpression());
                        CommitOrRollback(r71, parser);
                        a58 = r71;
                    }

                    r57 &= a58;

                } // end alternatives a58

                if (r57)
                { // may a72
                    bool a72 = false;
                    {
                        Checkpoint(parser); // r73

                        bool r73 = true;
                        if (r73)
                        { // alternatives a74 must
                            bool a74 = false;
                            if (!a74)
                            {
                                Checkpoint(parser); // r75

                                bool r75 = true;
                                if (r75)
                                { // may a76
                                    bool a76 = false;
                                    {
                                        Checkpoint(parser); // r77

                                        bool r77 = true;
                                        r77 = r77 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r77, parser);
                                        a76 = r77;
                                    }

                                    r75 |= a76;
                                } // end may a76

                                r75 = r75 && Match(parser, new Jhu.Graywulf.SqlParser.ArithmeticOperator());
                                if (r75)
                                { // may a78
                                    bool a78 = false;
                                    {
                                        Checkpoint(parser); // r79

                                        bool r79 = true;
                                        r79 = r79 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r79, parser);
                                        a78 = r79;
                                    }

                                    r75 |= a78;
                                } // end may a78

                                r75 = r75 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                                CommitOrRollback(r75, parser);
                                a74 = r75;
                            }

                            if (!a74)
                            {
                                Checkpoint(parser); // r80

                                bool r80 = true;
                                if (r80)
                                { // may a81
                                    bool a81 = false;
                                    {
                                        Checkpoint(parser); // r82

                                        bool r82 = true;
                                        r82 = r82 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r82, parser);
                                        a81 = r82;
                                    }

                                    r80 |= a81;
                                } // end may a81

                                r80 = r80 && Match(parser, new Jhu.Graywulf.SqlParser.BitwiseOperator());
                                if (r80)
                                { // may a83
                                    bool a83 = false;
                                    {
                                        Checkpoint(parser); // r84

                                        bool r84 = true;
                                        r84 = r84 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r84, parser);
                                        a83 = r84;
                                    }

                                    r80 |= a83;
                                } // end may a83

                                r80 = r80 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                                CommitOrRollback(r80, parser);
                                a74 = r80;
                            }

                            r73 &= a74;

                        } // end alternatives a74

                        CommitOrRollback(r73, parser);
                        a72 = r73;
                    }

                    r57 |= a72;
                } // end may a72

                CommitOrRollback(r57, parser);
                res = r57;
            }

            return res;

        }
    }

    public partial class ExpressionBrackets : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r85

                bool r85 = true;
                r85 = r85 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r85)
                { // may a86
                    bool a86 = false;
                    {
                        Checkpoint(parser); // r87

                        bool r87 = true;
                        r87 = r87 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r87, parser);
                        a86 = r87;
                    }

                    r85 |= a86;
                } // end may a86

                r85 = r85 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                if (r85)
                { // may a88
                    bool a88 = false;
                    {
                        Checkpoint(parser); // r89

                        bool r89 = true;
                        r89 = r89 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r89, parser);
                        a88 = r89;
                    }

                    r85 |= a88;
                } // end may a88

                r85 = r85 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                CommitOrRollback(r85, parser);
                res = r85;
            }

            return res;

        }
    }

    public partial class AnyVariable : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r90

                bool r90 = true;
                if (r90)
                { // alternatives a91 must
                    bool a91 = false;
                    if (!a91)
                    {
                        Checkpoint(parser); // r92

                        bool r92 = true;
                        r92 = r92 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnIdentifier());
                        CommitOrRollback(r92, parser);
                        a91 = r92;
                    }

                    if (!a91)
                    {
                        Checkpoint(parser); // r93

                        bool r93 = true;
                        r93 = r93 && Match(parser, new Jhu.Graywulf.SqlParser.SystemVariable());
                        CommitOrRollback(r93, parser);
                        a91 = r93;
                    }

                    if (!a91)
                    {
                        Checkpoint(parser); // r94

                        bool r94 = true;
                        r94 = r94 && Match(parser, new Jhu.Graywulf.SqlParser.Variable());
                        CommitOrRollback(r94, parser);
                        a91 = r94;
                    }

                    r90 &= a91;

                } // end alternatives a91

                CommitOrRollback(r90, parser);
                res = r90;
            }

            return res;

        }
    }

    public partial class SimpleCaseExpression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r95

                bool r95 = true;
                r95 = r95 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"CASE"));
                if (r95)
                { // may a96
                    bool a96 = false;
                    {
                        Checkpoint(parser); // r97

                        bool r97 = true;
                        r97 = r97 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r97, parser);
                        a96 = r97;
                    }

                    r95 |= a96;
                } // end may a96

                r95 = r95 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                if (r95)
                { // may a98
                    bool a98 = false;
                    {
                        Checkpoint(parser); // r99

                        bool r99 = true;
                        r99 = r99 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r99, parser);
                        a98 = r99;
                    }

                    r95 |= a98;
                } // end may a98

                r95 = r95 && Match(parser, new Jhu.Graywulf.SqlParser.SimpleCaseWhenList());
                if (r95)
                { // may a100
                    bool a100 = false;
                    {
                        Checkpoint(parser); // r101

                        bool r101 = true;
                        r101 = r101 && Match(parser, new Jhu.Graywulf.SqlParser.CaseElse());
                        CommitOrRollback(r101, parser);
                        a100 = r101;
                    }

                    r95 |= a100;
                } // end may a100

                r95 = r95 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"END"));
                CommitOrRollback(r95, parser);
                res = r95;
            }

            return res;

        }
    }

    public partial class SimpleCaseWhenList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r102

                bool r102 = true;
                r102 = r102 && Match(parser, new Jhu.Graywulf.SqlParser.SimpleCaseWhen());
                if (r102)
                { // may a103
                    bool a103 = false;
                    {
                        Checkpoint(parser); // r104

                        bool r104 = true;
                        r104 = r104 && Match(parser, new Jhu.Graywulf.SqlParser.SimpleCaseWhenList());
                        CommitOrRollback(r104, parser);
                        a103 = r104;
                    }

                    r102 |= a103;
                } // end may a103

                CommitOrRollback(r102, parser);
                res = r102;
            }

            return res;

        }
    }

    public partial class SimpleCaseWhen : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r105

                bool r105 = true;
                r105 = r105 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"WHEN"));
                r105 = r105 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                r105 = r105 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"THEN"));
                r105 = r105 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                CommitOrRollback(r105, parser);
                res = r105;
            }

            return res;

        }
    }

    public partial class SearchedCaseExpression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r106

                bool r106 = true;
                r106 = r106 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"CASE"));
                r106 = r106 && Match(parser, new Jhu.Graywulf.SqlParser.SearchedCaseWhenList());
                if (r106)
                { // may a107
                    bool a107 = false;
                    {
                        Checkpoint(parser); // r108

                        bool r108 = true;
                        r108 = r108 && Match(parser, new Jhu.Graywulf.SqlParser.CaseElse());
                        CommitOrRollback(r108, parser);
                        a107 = r108;
                    }

                    r106 |= a107;
                } // end may a107

                r106 = r106 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"END"));
                CommitOrRollback(r106, parser);
                res = r106;
            }

            return res;

        }
    }

    public partial class SearchedCaseWhenList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r109

                bool r109 = true;
                r109 = r109 && Match(parser, new Jhu.Graywulf.SqlParser.SearchedCaseWhen());
                if (r109)
                { // may a110
                    bool a110 = false;
                    {
                        Checkpoint(parser); // r111

                        bool r111 = true;
                        r111 = r111 && Match(parser, new Jhu.Graywulf.SqlParser.SearchedCaseWhenList());
                        CommitOrRollback(r111, parser);
                        a110 = r111;
                    }

                    r109 |= a110;
                } // end may a110

                CommitOrRollback(r109, parser);
                res = r109;
            }

            return res;

        }
    }

    public partial class SearchedCaseWhen : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r112

                bool r112 = true;
                r112 = r112 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"WHEN"));
                r112 = r112 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"THEN"));
                r112 = r112 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                CommitOrRollback(r112, parser);
                res = r112;
            }

            return res;

        }
    }

    public partial class CaseElse : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r113

                bool r113 = true;
                r113 = r113 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ELSE"));
                r113 = r113 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                CommitOrRollback(r113, parser);
                res = r113;
            }

            return res;

        }
    }

    public partial class TableOrViewName : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r114

                bool r114 = true;
                if (r114)
                { // may a115
                    bool a115 = false;
                    {
                        Checkpoint(parser); // r116

                        bool r116 = true;
                        r116 = r116 && Match(parser, new Jhu.Graywulf.SqlParser.DatasetName());
                        if (r116)
                        { // may a117
                            bool a117 = false;
                            {
                                Checkpoint(parser); // r118

                                bool r118 = true;
                                r118 = r118 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r118, parser);
                                a117 = r118;
                            }

                            r116 |= a117;
                        } // end may a117

                        r116 = r116 && Match(parser, new Jhu.Graywulf.SqlParser.Colon());
                        if (r116)
                        { // may a119
                            bool a119 = false;
                            {
                                Checkpoint(parser); // r120

                                bool r120 = true;
                                r120 = r120 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r120, parser);
                                a119 = r120;
                            }

                            r116 |= a119;
                        } // end may a119

                        CommitOrRollback(r116, parser);
                        a115 = r116;
                    }

                    r114 |= a115;
                } // end may a115

                if (r114)
                { // alternatives a121 must
                    bool a121 = false;
                    if (!a121)
                    {
                        Checkpoint(parser); // r122

                        bool r122 = true;
                        r122 = r122 && Match(parser, new Jhu.Graywulf.SqlParser.DatabaseName());
                        if (r122)
                        { // may a123
                            bool a123 = false;
                            {
                                Checkpoint(parser); // r124

                                bool r124 = true;
                                r124 = r124 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r124, parser);
                                a123 = r124;
                            }

                            r122 |= a123;
                        } // end may a123

                        r122 = r122 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                        if (r122)
                        { // may a125
                            bool a125 = false;
                            {
                                Checkpoint(parser); // r126

                                bool r126 = true;
                                if (r126)
                                { // may a127
                                    bool a127 = false;
                                    {
                                        Checkpoint(parser); // r128

                                        bool r128 = true;
                                        r128 = r128 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r128, parser);
                                        a127 = r128;
                                    }

                                    r126 |= a127;
                                } // end may a127

                                r126 = r126 && Match(parser, new Jhu.Graywulf.SqlParser.SchemaName());
                                CommitOrRollback(r126, parser);
                                a125 = r126;
                            }

                            r122 |= a125;
                        } // end may a125

                        if (r122)
                        { // may a129
                            bool a129 = false;
                            {
                                Checkpoint(parser); // r130

                                bool r130 = true;
                                r130 = r130 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r130, parser);
                                a129 = r130;
                            }

                            r122 |= a129;
                        } // end may a129

                        r122 = r122 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                        if (r122)
                        { // may a131
                            bool a131 = false;
                            {
                                Checkpoint(parser); // r132

                                bool r132 = true;
                                r132 = r132 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r132, parser);
                                a131 = r132;
                            }

                            r122 |= a131;
                        } // end may a131

                        r122 = r122 && Match(parser, new Jhu.Graywulf.SqlParser.TableName());
                        CommitOrRollback(r122, parser);
                        a121 = r122;
                    }

                    if (!a121)
                    {
                        Checkpoint(parser); // r133

                        bool r133 = true;
                        r133 = r133 && Match(parser, new Jhu.Graywulf.SqlParser.SchemaName());
                        if (r133)
                        { // may a134
                            bool a134 = false;
                            {
                                Checkpoint(parser); // r135

                                bool r135 = true;
                                r135 = r135 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r135, parser);
                                a134 = r135;
                            }

                            r133 |= a134;
                        } // end may a134

                        r133 = r133 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                        if (r133)
                        { // may a136
                            bool a136 = false;
                            {
                                Checkpoint(parser); // r137

                                bool r137 = true;
                                r137 = r137 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r137, parser);
                                a136 = r137;
                            }

                            r133 |= a136;
                        } // end may a136

                        r133 = r133 && Match(parser, new Jhu.Graywulf.SqlParser.TableName());
                        CommitOrRollback(r133, parser);
                        a121 = r133;
                    }

                    if (!a121)
                    {
                        Checkpoint(parser); // r138

                        bool r138 = true;
                        r138 = r138 && Match(parser, new Jhu.Graywulf.SqlParser.TableName());
                        CommitOrRollback(r138, parser);
                        a121 = r138;
                    }

                    r114 &= a121;

                } // end alternatives a121

                CommitOrRollback(r114, parser);
                res = r114;
            }

            return res;

        }
    }

    public partial class ColumnIdentifier : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r139

                bool r139 = true;
                if (r139)
                { // alternatives a140 must
                    bool a140 = false;
                    if (!a140)
                    {
                        Checkpoint(parser); // r141

                        bool r141 = true;
                        if (r141)
                        { // may a142
                            bool a142 = false;
                            {
                                Checkpoint(parser); // r143

                                bool r143 = true;
                                r143 = r143 && Match(parser, new Jhu.Graywulf.SqlParser.DatasetName());
                                if (r143)
                                { // may a144
                                    bool a144 = false;
                                    {
                                        Checkpoint(parser); // r145

                                        bool r145 = true;
                                        r145 = r145 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r145, parser);
                                        a144 = r145;
                                    }

                                    r143 |= a144;
                                } // end may a144

                                r143 = r143 && Match(parser, new Jhu.Graywulf.SqlParser.Colon());
                                if (r143)
                                { // may a146
                                    bool a146 = false;
                                    {
                                        Checkpoint(parser); // r147

                                        bool r147 = true;
                                        r147 = r147 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r147, parser);
                                        a146 = r147;
                                    }

                                    r143 |= a146;
                                } // end may a146

                                CommitOrRollback(r143, parser);
                                a142 = r143;
                            }

                            r141 |= a142;
                        } // end may a142

                        if (r141)
                        { // alternatives a148 must
                            bool a148 = false;
                            if (!a148)
                            {
                                Checkpoint(parser); // r149

                                bool r149 = true;
                                r149 = r149 && Match(parser, new Jhu.Graywulf.SqlParser.DatabaseName());
                                if (r149)
                                { // may a150
                                    bool a150 = false;
                                    {
                                        Checkpoint(parser); // r151

                                        bool r151 = true;
                                        r151 = r151 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r151, parser);
                                        a150 = r151;
                                    }

                                    r149 |= a150;
                                } // end may a150

                                r149 = r149 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                                if (r149)
                                { // may a152
                                    bool a152 = false;
                                    {
                                        Checkpoint(parser); // r153

                                        bool r153 = true;
                                        r153 = r153 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r153, parser);
                                        a152 = r153;
                                    }

                                    r149 |= a152;
                                } // end may a152

                                if (r149)
                                { // may a154
                                    bool a154 = false;
                                    {
                                        Checkpoint(parser); // r155

                                        bool r155 = true;
                                        r155 = r155 && Match(parser, new Jhu.Graywulf.SqlParser.SchemaName());
                                        if (r155)
                                        { // may a156
                                            bool a156 = false;
                                            {
                                                Checkpoint(parser); // r157

                                                bool r157 = true;
                                                r157 = r157 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                                CommitOrRollback(r157, parser);
                                                a156 = r157;
                                            }

                                            r155 |= a156;
                                        } // end may a156

                                        CommitOrRollback(r155, parser);
                                        a154 = r155;
                                    }

                                    r149 |= a154;
                                } // end may a154

                                r149 = r149 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                                if (r149)
                                { // may a158
                                    bool a158 = false;
                                    {
                                        Checkpoint(parser); // r159

                                        bool r159 = true;
                                        r159 = r159 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r159, parser);
                                        a158 = r159;
                                    }

                                    r149 |= a158;
                                } // end may a158

                                r149 = r149 && Match(parser, new Jhu.Graywulf.SqlParser.TableName());
                                if (r149)
                                { // may a160
                                    bool a160 = false;
                                    {
                                        Checkpoint(parser); // r161

                                        bool r161 = true;
                                        r161 = r161 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r161, parser);
                                        a160 = r161;
                                    }

                                    r149 |= a160;
                                } // end may a160

                                r149 = r149 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                                if (r149)
                                { // may a162
                                    bool a162 = false;
                                    {
                                        Checkpoint(parser); // r163

                                        bool r163 = true;
                                        r163 = r163 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r163, parser);
                                        a162 = r163;
                                    }

                                    r149 |= a162;
                                } // end may a162

                                if (r149)
                                { // alternatives a164 must
                                    bool a164 = false;
                                    if (!a164)
                                    {
                                        Checkpoint(parser); // r165

                                        bool r165 = true;
                                        r165 = r165 && Match(parser, new Jhu.Graywulf.SqlParser.Mul());
                                        CommitOrRollback(r165, parser);
                                        a164 = r165;
                                    }

                                    if (!a164)
                                    {
                                        Checkpoint(parser); // r166

                                        bool r166 = true;
                                        r166 = r166 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnName());
                                        CommitOrRollback(r166, parser);
                                        a164 = r166;
                                    }

                                    r149 &= a164;

                                } // end alternatives a164

                                CommitOrRollback(r149, parser);
                                a148 = r149;
                            }

                            if (!a148)
                            {
                                Checkpoint(parser); // r167

                                bool r167 = true;
                                r167 = r167 && Match(parser, new Jhu.Graywulf.SqlParser.SchemaName());
                                if (r167)
                                { // may a168
                                    bool a168 = false;
                                    {
                                        Checkpoint(parser); // r169

                                        bool r169 = true;
                                        r169 = r169 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r169, parser);
                                        a168 = r169;
                                    }

                                    r167 |= a168;
                                } // end may a168

                                r167 = r167 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                                if (r167)
                                { // may a170
                                    bool a170 = false;
                                    {
                                        Checkpoint(parser); // r171

                                        bool r171 = true;
                                        r171 = r171 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r171, parser);
                                        a170 = r171;
                                    }

                                    r167 |= a170;
                                } // end may a170

                                r167 = r167 && Match(parser, new Jhu.Graywulf.SqlParser.TableName());
                                if (r167)
                                { // may a172
                                    bool a172 = false;
                                    {
                                        Checkpoint(parser); // r173

                                        bool r173 = true;
                                        r173 = r173 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r173, parser);
                                        a172 = r173;
                                    }

                                    r167 |= a172;
                                } // end may a172

                                r167 = r167 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                                if (r167)
                                { // may a174
                                    bool a174 = false;
                                    {
                                        Checkpoint(parser); // r175

                                        bool r175 = true;
                                        r175 = r175 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r175, parser);
                                        a174 = r175;
                                    }

                                    r167 |= a174;
                                } // end may a174

                                if (r167)
                                { // alternatives a176 must
                                    bool a176 = false;
                                    if (!a176)
                                    {
                                        Checkpoint(parser); // r177

                                        bool r177 = true;
                                        r177 = r177 && Match(parser, new Jhu.Graywulf.SqlParser.Mul());
                                        CommitOrRollback(r177, parser);
                                        a176 = r177;
                                    }

                                    if (!a176)
                                    {
                                        Checkpoint(parser); // r178

                                        bool r178 = true;
                                        r178 = r178 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnName());
                                        CommitOrRollback(r178, parser);
                                        a176 = r178;
                                    }

                                    r167 &= a176;

                                } // end alternatives a176

                                CommitOrRollback(r167, parser);
                                a148 = r167;
                            }

                            if (!a148)
                            {
                                Checkpoint(parser); // r179

                                bool r179 = true;
                                r179 = r179 && Match(parser, new Jhu.Graywulf.SqlParser.TableName());
                                if (r179)
                                { // may a180
                                    bool a180 = false;
                                    {
                                        Checkpoint(parser); // r181

                                        bool r181 = true;
                                        r181 = r181 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r181, parser);
                                        a180 = r181;
                                    }

                                    r179 |= a180;
                                } // end may a180

                                r179 = r179 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                                if (r179)
                                { // may a182
                                    bool a182 = false;
                                    {
                                        Checkpoint(parser); // r183

                                        bool r183 = true;
                                        r183 = r183 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r183, parser);
                                        a182 = r183;
                                    }

                                    r179 |= a182;
                                } // end may a182

                                if (r179)
                                { // alternatives a184 must
                                    bool a184 = false;
                                    if (!a184)
                                    {
                                        Checkpoint(parser); // r185

                                        bool r185 = true;
                                        r185 = r185 && Match(parser, new Jhu.Graywulf.SqlParser.Mul());
                                        CommitOrRollback(r185, parser);
                                        a184 = r185;
                                    }

                                    if (!a184)
                                    {
                                        Checkpoint(parser); // r186

                                        bool r186 = true;
                                        r186 = r186 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnName());
                                        CommitOrRollback(r186, parser);
                                        a184 = r186;
                                    }

                                    r179 &= a184;

                                } // end alternatives a184

                                CommitOrRollback(r179, parser);
                                a148 = r179;
                            }

                            r141 &= a148;

                        } // end alternatives a148

                        CommitOrRollback(r141, parser);
                        a140 = r141;
                    }

                    if (!a140)
                    {
                        Checkpoint(parser); // r187

                        bool r187 = true;
                        if (r187)
                        { // alternatives a188 must
                            bool a188 = false;
                            if (!a188)
                            {
                                Checkpoint(parser); // r189

                                bool r189 = true;
                                r189 = r189 && Match(parser, new Jhu.Graywulf.SqlParser.Mul());
                                CommitOrRollback(r189, parser);
                                a188 = r189;
                            }

                            if (!a188)
                            {
                                Checkpoint(parser); // r190

                                bool r190 = true;
                                r190 = r190 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnName());
                                CommitOrRollback(r190, parser);
                                a188 = r190;
                            }

                            r187 &= a188;

                        } // end alternatives a188

                        CommitOrRollback(r187, parser);
                        a140 = r187;
                    }

                    r139 &= a140;

                } // end alternatives a140

                CommitOrRollback(r139, parser);
                res = r139;
            }

            return res;

        }
    }

    public partial class FunctionIdentifier : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r191

                bool r191 = true;
                if (r191)
                { // alternatives a192 must
                    bool a192 = false;
                    if (!a192)
                    {
                        Checkpoint(parser); // r193

                        bool r193 = true;
                        r193 = r193 && Match(parser, new Jhu.Graywulf.SqlParser.UdfIdentifier());
                        CommitOrRollback(r193, parser);
                        a192 = r193;
                    }

                    if (!a192)
                    {
                        Checkpoint(parser); // r194

                        bool r194 = true;
                        r194 = r194 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionName());
                        CommitOrRollback(r194, parser);
                        a192 = r194;
                    }

                    r191 &= a192;

                } // end alternatives a192

                CommitOrRollback(r191, parser);
                res = r191;
            }

            return res;

        }
    }

    public partial class UdfIdentifier : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r195

                bool r195 = true;
                if (r195)
                { // may a196
                    bool a196 = false;
                    {
                        Checkpoint(parser); // r197

                        bool r197 = true;
                        r197 = r197 && Match(parser, new Jhu.Graywulf.SqlParser.DatasetName());
                        if (r197)
                        { // may a198
                            bool a198 = false;
                            {
                                Checkpoint(parser); // r199

                                bool r199 = true;
                                r199 = r199 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r199, parser);
                                a198 = r199;
                            }

                            r197 |= a198;
                        } // end may a198

                        r197 = r197 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        if (r197)
                        { // may a200
                            bool a200 = false;
                            {
                                Checkpoint(parser); // r201

                                bool r201 = true;
                                r201 = r201 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r201, parser);
                                a200 = r201;
                            }

                            r197 |= a200;
                        } // end may a200

                        CommitOrRollback(r197, parser);
                        a196 = r197;
                    }

                    r195 |= a196;
                } // end may a196

                if (r195)
                { // alternatives a202 must
                    bool a202 = false;
                    if (!a202)
                    {
                        Checkpoint(parser); // r203

                        bool r203 = true;
                        r203 = r203 && Match(parser, new Jhu.Graywulf.SqlParser.DatabaseName());
                        if (r203)
                        { // may a204
                            bool a204 = false;
                            {
                                Checkpoint(parser); // r205

                                bool r205 = true;
                                r205 = r205 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r205, parser);
                                a204 = r205;
                            }

                            r203 |= a204;
                        } // end may a204

                        r203 = r203 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                        if (r203)
                        { // may a206
                            bool a206 = false;
                            {
                                Checkpoint(parser); // r207

                                bool r207 = true;
                                if (r207)
                                { // may a208
                                    bool a208 = false;
                                    {
                                        Checkpoint(parser); // r209

                                        bool r209 = true;
                                        r209 = r209 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r209, parser);
                                        a208 = r209;
                                    }

                                    r207 |= a208;
                                } // end may a208

                                r207 = r207 && Match(parser, new Jhu.Graywulf.SqlParser.SchemaName());
                                CommitOrRollback(r207, parser);
                                a206 = r207;
                            }

                            r203 |= a206;
                        } // end may a206

                        if (r203)
                        { // may a210
                            bool a210 = false;
                            {
                                Checkpoint(parser); // r211

                                bool r211 = true;
                                r211 = r211 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r211, parser);
                                a210 = r211;
                            }

                            r203 |= a210;
                        } // end may a210

                        r203 = r203 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                        if (r203)
                        { // may a212
                            bool a212 = false;
                            {
                                Checkpoint(parser); // r213

                                bool r213 = true;
                                r213 = r213 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r213, parser);
                                a212 = r213;
                            }

                            r203 |= a212;
                        } // end may a212

                        r203 = r203 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionName());
                        CommitOrRollback(r203, parser);
                        a202 = r203;
                    }

                    if (!a202)
                    {
                        Checkpoint(parser); // r214

                        bool r214 = true;
                        r214 = r214 && Match(parser, new Jhu.Graywulf.SqlParser.SchemaName());
                        if (r214)
                        { // may a215
                            bool a215 = false;
                            {
                                Checkpoint(parser); // r216

                                bool r216 = true;
                                r216 = r216 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r216, parser);
                                a215 = r216;
                            }

                            r214 |= a215;
                        } // end may a215

                        r214 = r214 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                        if (r214)
                        { // may a217
                            bool a217 = false;
                            {
                                Checkpoint(parser); // r218

                                bool r218 = true;
                                r218 = r218 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r218, parser);
                                a217 = r218;
                            }

                            r214 |= a217;
                        } // end may a217

                        r214 = r214 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionName());
                        CommitOrRollback(r214, parser);
                        a202 = r214;
                    }

                    r195 &= a202;

                } // end alternatives a202

                CommitOrRollback(r195, parser);
                res = r195;
            }

            return res;

        }
    }

    public partial class UdtFunctionIdentifier : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r219

                bool r219 = true;
                r219 = r219 && Match(parser, new Jhu.Graywulf.SqlParser.Variable());
                if (r219)
                { // may a220
                    bool a220 = false;
                    {
                        Checkpoint(parser); // r221

                        bool r221 = true;
                        r221 = r221 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r221, parser);
                        a220 = r221;
                    }

                    r219 |= a220;
                } // end may a220

                r219 = r219 && Match(parser, new Jhu.Graywulf.SqlParser.Dot());
                if (r219)
                { // may a222
                    bool a222 = false;
                    {
                        Checkpoint(parser); // r223

                        bool r223 = true;
                        r223 = r223 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r223, parser);
                        a222 = r223;
                    }

                    r219 |= a222;
                } // end may a222

                r219 = r219 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionName());
                CommitOrRollback(r219, parser);
                res = r219;
            }

            return res;

        }
    }

    public partial class Argument : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r224

                bool r224 = true;
                r224 = r224 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                CommitOrRollback(r224, parser);
                res = r224;
            }

            return res;

        }
    }

    public partial class ArgumentList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r225

                bool r225 = true;
                if (r225)
                { // may a226
                    bool a226 = false;
                    {
                        Checkpoint(parser); // r227

                        bool r227 = true;
                        r227 = r227 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r227, parser);
                        a226 = r227;
                    }

                    r225 |= a226;
                } // end may a226

                r225 = r225 && Match(parser, new Jhu.Graywulf.SqlParser.Argument());
                if (r225)
                { // may a228
                    bool a228 = false;
                    {
                        Checkpoint(parser); // r229

                        bool r229 = true;
                        if (r229)
                        { // may a230
                            bool a230 = false;
                            {
                                Checkpoint(parser); // r231

                                bool r231 = true;
                                r231 = r231 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r231, parser);
                                a230 = r231;
                            }

                            r229 |= a230;
                        } // end may a230

                        r229 = r229 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        r229 = r229 && Match(parser, new Jhu.Graywulf.SqlParser.ArgumentList());
                        CommitOrRollback(r229, parser);
                        a228 = r229;
                    }

                    r225 |= a228;
                } // end may a228

                CommitOrRollback(r225, parser);
                res = r225;
            }

            return res;

        }
    }

    public partial class FunctionCall : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r232

                bool r232 = true;
                r232 = r232 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionIdentifier());
                if (r232)
                { // may a233
                    bool a233 = false;
                    {
                        Checkpoint(parser); // r234

                        bool r234 = true;
                        r234 = r234 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r234, parser);
                        a233 = r234;
                    }

                    r232 |= a233;
                } // end may a233

                r232 = r232 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r232)
                { // may a235
                    bool a235 = false;
                    {
                        Checkpoint(parser); // r236

                        bool r236 = true;
                        r236 = r236 && Match(parser, new Jhu.Graywulf.SqlParser.ArgumentList());
                        CommitOrRollback(r236, parser);
                        a235 = r236;
                    }

                    r232 |= a235;
                } // end may a235

                if (r232)
                { // may a237
                    bool a237 = false;
                    {
                        Checkpoint(parser); // r238

                        bool r238 = true;
                        r238 = r238 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r238, parser);
                        a237 = r238;
                    }

                    r232 |= a237;
                } // end may a237

                r232 = r232 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                CommitOrRollback(r232, parser);
                res = r232;
            }

            return res;

        }
    }

    public partial class TableValuedFunctionCall : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r239

                bool r239 = true;
                r239 = r239 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionIdentifier());
                if (r239)
                { // may a240
                    bool a240 = false;
                    {
                        Checkpoint(parser); // r241

                        bool r241 = true;
                        r241 = r241 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r241, parser);
                        a240 = r241;
                    }

                    r239 |= a240;
                } // end may a240

                r239 = r239 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r239)
                { // may a242
                    bool a242 = false;
                    {
                        Checkpoint(parser); // r243

                        bool r243 = true;
                        r243 = r243 && Match(parser, new Jhu.Graywulf.SqlParser.ArgumentList());
                        CommitOrRollback(r243, parser);
                        a242 = r243;
                    }

                    r239 |= a242;
                } // end may a242

                if (r239)
                { // may a244
                    bool a244 = false;
                    {
                        Checkpoint(parser); // r245

                        bool r245 = true;
                        r245 = r245 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r245, parser);
                        a244 = r245;
                    }

                    r239 |= a244;
                } // end may a244

                r239 = r239 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                CommitOrRollback(r239, parser);
                res = r239;
            }

            return res;

        }
    }

    public partial class SelectStatement : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r246

                bool r246 = true;
                if (r246)
                { // may a247
                    bool a247 = false;
                    {
                        Checkpoint(parser); // r248

                        bool r248 = true;
                        r248 = r248 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r248, parser);
                        a247 = r248;
                    }

                    r246 |= a247;
                } // end may a247

                r246 = r246 && Match(parser, new Jhu.Graywulf.SqlParser.QueryExpression());
                if (r246)
                { // may a249
                    bool a249 = false;
                    {
                        Checkpoint(parser); // r250

                        bool r250 = true;
                        r250 = r250 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r250, parser);
                        a249 = r250;
                    }

                    r246 |= a249;
                } // end may a249

                if (r246)
                { // may a251
                    bool a251 = false;
                    {
                        Checkpoint(parser); // r252

                        bool r252 = true;
                        r252 = r252 && Match(parser, new Jhu.Graywulf.SqlParser.OrderByClause());
                        CommitOrRollback(r252, parser);
                        a251 = r252;
                    }

                    r246 |= a251;
                } // end may a251

                if (r246)
                { // may a253
                    bool a253 = false;
                    {
                        Checkpoint(parser); // r254

                        bool r254 = true;
                        r254 = r254 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r254, parser);
                        a253 = r254;
                    }

                    r246 |= a253;
                } // end may a253

                CommitOrRollback(r246, parser);
                res = r246;
            }

            return res;

        }
    }

    public partial class QueryExpression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r255

                bool r255 = true;
                if (r255)
                { // alternatives a256 must
                    bool a256 = false;
                    if (!a256)
                    {
                        Checkpoint(parser); // r257

                        bool r257 = true;
                        r257 = r257 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                        if (r257)
                        { // may a258
                            bool a258 = false;
                            {
                                Checkpoint(parser); // r259

                                bool r259 = true;
                                r259 = r259 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r259, parser);
                                a258 = r259;
                            }

                            r257 |= a258;
                        } // end may a258

                        r257 = r257 && Match(parser, new Jhu.Graywulf.SqlParser.QueryExpression());
                        if (r257)
                        { // may a260
                            bool a260 = false;
                            {
                                Checkpoint(parser); // r261

                                bool r261 = true;
                                r261 = r261 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r261, parser);
                                a260 = r261;
                            }

                            r257 |= a260;
                        } // end may a260

                        r257 = r257 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                        CommitOrRollback(r257, parser);
                        a256 = r257;
                    }

                    if (!a256)
                    {
                        Checkpoint(parser); // r262

                        bool r262 = true;
                        r262 = r262 && Match(parser, new Jhu.Graywulf.SqlParser.QuerySpecification());
                        CommitOrRollback(r262, parser);
                        a256 = r262;
                    }

                    r255 &= a256;

                } // end alternatives a256

                if (r255)
                { // may a263
                    bool a263 = false;
                    {
                        Checkpoint(parser); // r264

                        bool r264 = true;
                        if (r264)
                        { // may a265
                            bool a265 = false;
                            {
                                Checkpoint(parser); // r266

                                bool r266 = true;
                                r266 = r266 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r266, parser);
                                a265 = r266;
                            }

                            r264 |= a265;
                        } // end may a265

                        r264 = r264 && Match(parser, new Jhu.Graywulf.SqlParser.QueryOperator());
                        if (r264)
                        { // may a267
                            bool a267 = false;
                            {
                                Checkpoint(parser); // r268

                                bool r268 = true;
                                r268 = r268 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r268, parser);
                                a267 = r268;
                            }

                            r264 |= a267;
                        } // end may a267

                        r264 = r264 && Match(parser, new Jhu.Graywulf.SqlParser.QueryExpression());
                        CommitOrRollback(r264, parser);
                        a263 = r264;
                    }

                    r255 |= a263;
                } // end may a263

                CommitOrRollback(r255, parser);
                res = r255;
            }

            return res;

        }
    }

    public partial class QueryOperator : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r269

                bool r269 = true;
                if (r269)
                { // alternatives a270 must
                    bool a270 = false;
                    if (!a270)
                    {
                        Checkpoint(parser); // r271

                        bool r271 = true;
                        r271 = r271 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"UNION"));
                        if (r271)
                        { // may a272
                            bool a272 = false;
                            {
                                Checkpoint(parser); // r273

                                bool r273 = true;
                                r273 = r273 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                r273 = r273 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ALL"));
                                CommitOrRollback(r273, parser);
                                a272 = r273;
                            }

                            r271 |= a272;
                        } // end may a272

                        CommitOrRollback(r271, parser);
                        a270 = r271;
                    }

                    if (!a270)
                    {
                        Checkpoint(parser); // r274

                        bool r274 = true;
                        r274 = r274 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"EXCEPT"));
                        CommitOrRollback(r274, parser);
                        a270 = r274;
                    }

                    if (!a270)
                    {
                        Checkpoint(parser); // r275

                        bool r275 = true;
                        r275 = r275 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"INTERSECT"));
                        CommitOrRollback(r275, parser);
                        a270 = r275;
                    }

                    r269 &= a270;

                } // end alternatives a270

                CommitOrRollback(r269, parser);
                res = r269;
            }

            return res;

        }
    }

    public partial class QuerySpecification : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r276

                bool r276 = true;
                r276 = r276 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"SELECT"));
                if (r276)
                { // may a277
                    bool a277 = false;
                    {
                        Checkpoint(parser); // r278

                        bool r278 = true;
                        r278 = r278 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        if (r278)
                        { // alternatives a279 must
                            bool a279 = false;
                            if (!a279)
                            {
                                Checkpoint(parser); // r280

                                bool r280 = true;
                                r280 = r280 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ALL"));
                                CommitOrRollback(r280, parser);
                                a279 = r280;
                            }

                            if (!a279)
                            {
                                Checkpoint(parser); // r281

                                bool r281 = true;
                                r281 = r281 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r281, parser);
                                a279 = r281;
                            }

                            if (!a279)
                            {
                                Checkpoint(parser); // r282

                                bool r282 = true;
                                r282 = r282 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"DISTINCT"));
                                CommitOrRollback(r282, parser);
                                a279 = r282;
                            }

                            r278 &= a279;

                        } // end alternatives a279

                        CommitOrRollback(r278, parser);
                        a277 = r278;
                    }

                    r276 |= a277;
                } // end may a277

                if (r276)
                { // may a283
                    bool a283 = false;
                    {
                        Checkpoint(parser); // r284

                        bool r284 = true;
                        r284 = r284 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r284 = r284 && Match(parser, new Jhu.Graywulf.SqlParser.TopExpression());
                        CommitOrRollback(r284, parser);
                        a283 = r284;
                    }

                    r276 |= a283;
                } // end may a283

                r276 = r276 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r276 = r276 && Match(parser, new Jhu.Graywulf.SqlParser.SelectList());
                if (r276)
                { // may a285
                    bool a285 = false;
                    {
                        Checkpoint(parser); // r286

                        bool r286 = true;
                        r286 = r286 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r286 = r286 && Match(parser, new Jhu.Graywulf.SqlParser.IntoClause());
                        CommitOrRollback(r286, parser);
                        a285 = r286;
                    }

                    r276 |= a285;
                } // end may a285

                if (r276)
                { // may a287
                    bool a287 = false;
                    {
                        Checkpoint(parser); // r288

                        bool r288 = true;
                        r288 = r288 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r288 = r288 && Match(parser, new Jhu.Graywulf.SqlParser.FromClause());
                        CommitOrRollback(r288, parser);
                        a287 = r288;
                    }

                    r276 |= a287;
                } // end may a287

                if (r276)
                { // may a289
                    bool a289 = false;
                    {
                        Checkpoint(parser); // r290

                        bool r290 = true;
                        r290 = r290 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r290 = r290 && Match(parser, new Jhu.Graywulf.SqlParser.WhereClause());
                        CommitOrRollback(r290, parser);
                        a289 = r290;
                    }

                    r276 |= a289;
                } // end may a289

                if (r276)
                { // may a291
                    bool a291 = false;
                    {
                        Checkpoint(parser); // r292

                        bool r292 = true;
                        r292 = r292 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r292 = r292 && Match(parser, new Jhu.Graywulf.SqlParser.GroupByClause());
                        CommitOrRollback(r292, parser);
                        a291 = r292;
                    }

                    r276 |= a291;
                } // end may a291

                if (r276)
                { // may a293
                    bool a293 = false;
                    {
                        Checkpoint(parser); // r294

                        bool r294 = true;
                        r294 = r294 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r294 = r294 && Match(parser, new Jhu.Graywulf.SqlParser.HavingClause());
                        CommitOrRollback(r294, parser);
                        a293 = r294;
                    }

                    r276 |= a293;
                } // end may a293

                CommitOrRollback(r276, parser);
                res = r276;
            }

            return res;

        }
    }

    public partial class TopExpression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r295

                bool r295 = true;
                r295 = r295 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"TOP"));
                r295 = r295 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r295 = r295 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                if (r295)
                { // may a296
                    bool a296 = false;
                    {
                        Checkpoint(parser); // r297

                        bool r297 = true;
                        r297 = r297 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r297 = r297 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"PERCENT"));
                        CommitOrRollback(r297, parser);
                        a296 = r297;
                    }

                    r295 |= a296;
                } // end may a296

                if (r295)
                { // may a298
                    bool a298 = false;
                    {
                        Checkpoint(parser); // r299

                        bool r299 = true;
                        r299 = r299 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r299 = r299 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"WITH"));
                        r299 = r299 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r299 = r299 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"TIES"));
                        CommitOrRollback(r299, parser);
                        a298 = r299;
                    }

                    r295 |= a298;
                } // end may a298

                CommitOrRollback(r295, parser);
                res = r295;
            }

            return res;

        }
    }

    public partial class SelectList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r300

                bool r300 = true;
                r300 = r300 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnExpression());
                if (r300)
                { // may a301
                    bool a301 = false;
                    {
                        Checkpoint(parser); // r302

                        bool r302 = true;
                        if (r302)
                        { // may a303
                            bool a303 = false;
                            {
                                Checkpoint(parser); // r304

                                bool r304 = true;
                                r304 = r304 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r304, parser);
                                a303 = r304;
                            }

                            r302 |= a303;
                        } // end may a303

                        r302 = r302 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        if (r302)
                        { // may a305
                            bool a305 = false;
                            {
                                Checkpoint(parser); // r306

                                bool r306 = true;
                                r306 = r306 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r306, parser);
                                a305 = r306;
                            }

                            r302 |= a305;
                        } // end may a305

                        r302 = r302 && Match(parser, new Jhu.Graywulf.SqlParser.SelectList());
                        CommitOrRollback(r302, parser);
                        a301 = r302;
                    }

                    r300 |= a301;
                } // end may a301

                CommitOrRollback(r300, parser);
                res = r300;
            }

            return res;

        }
    }

    public partial class ColumnExpression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r307

                bool r307 = true;
                if (r307)
                { // alternatives a308 must
                    bool a308 = false;
                    if (!a308)
                    {
                        Checkpoint(parser); // r309

                        bool r309 = true;
                        r309 = r309 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnAlias());
                        if (r309)
                        { // may a310
                            bool a310 = false;
                            {
                                Checkpoint(parser); // r311

                                bool r311 = true;
                                r311 = r311 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r311, parser);
                                a310 = r311;
                            }

                            r309 |= a310;
                        } // end may a310

                        r309 = r309 && Match(parser, new Jhu.Graywulf.SqlParser.Equals());
                        if (r309)
                        { // may a312
                            bool a312 = false;
                            {
                                Checkpoint(parser); // r313

                                bool r313 = true;
                                r313 = r313 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r313, parser);
                                a312 = r313;
                            }

                            r309 |= a312;
                        } // end may a312

                        r309 = r309 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        CommitOrRollback(r309, parser);
                        a308 = r309;
                    }

                    if (!a308)
                    {
                        Checkpoint(parser); // r314

                        bool r314 = true;
                        r314 = r314 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r314)
                        { // may a315
                            bool a315 = false;
                            {
                                Checkpoint(parser); // r316

                                bool r316 = true;
                                if (r316)
                                { // may a317
                                    bool a317 = false;
                                    {
                                        Checkpoint(parser); // r318

                                        bool r318 = true;
                                        r318 = r318 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        r318 = r318 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AS"));
                                        CommitOrRollback(r318, parser);
                                        a317 = r318;
                                    }

                                    r316 |= a317;
                                } // end may a317

                                r316 = r316 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                r316 = r316 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnAlias());
                                CommitOrRollback(r316, parser);
                                a315 = r316;
                            }

                            r314 |= a315;
                        } // end may a315

                        CommitOrRollback(r314, parser);
                        a308 = r314;
                    }

                    r307 &= a308;

                } // end alternatives a308

                CommitOrRollback(r307, parser);
                res = r307;
            }

            return res;

        }
    }

    public partial class IntoClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r319

                bool r319 = true;
                r319 = r319 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"INTO"));
                r319 = r319 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r319 = r319 && Match(parser, new Jhu.Graywulf.SqlParser.TableOrViewName());
                CommitOrRollback(r319, parser);
                res = r319;
            }

            return res;

        }
    }

    public partial class FromClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r320

                bool r320 = true;
                r320 = r320 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"FROM"));
                if (r320)
                { // may a321
                    bool a321 = false;
                    {
                        Checkpoint(parser); // r322

                        bool r322 = true;
                        r322 = r322 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r322, parser);
                        a321 = r322;
                    }

                    r320 |= a321;
                } // end may a321

                r320 = r320 && Match(parser, new Jhu.Graywulf.SqlParser.TableSourceExpression());
                CommitOrRollback(r320, parser);
                res = r320;
            }

            return res;

        }
    }

    public partial class TableSourceExpression : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r323

                bool r323 = true;
                r323 = r323 && Match(parser, new Jhu.Graywulf.SqlParser.TableSource());
                if (r323)
                { // may a324
                    bool a324 = false;
                    {
                        Checkpoint(parser); // r325

                        bool r325 = true;
                        if (r325)
                        { // may a326
                            bool a326 = false;
                            {
                                Checkpoint(parser); // r327

                                bool r327 = true;
                                r327 = r327 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r327, parser);
                                a326 = r327;
                            }

                            r325 |= a326;
                        } // end may a326

                        r325 = r325 && Match(parser, new Jhu.Graywulf.SqlParser.JoinedTable());
                        CommitOrRollback(r325, parser);
                        a324 = r325;
                    }

                    r323 |= a324;
                } // end may a324

                CommitOrRollback(r323, parser);
                res = r323;
            }

            return res;

        }
    }

    public partial class JoinedTable : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r328

                bool r328 = true;
                if (r328)
                { // alternatives a329 must
                    bool a329 = false;
                    if (!a329)
                    {
                        Checkpoint(parser); // r330

                        bool r330 = true;
                        r330 = r330 && Match(parser, new Jhu.Graywulf.SqlParser.JoinType());
                        if (r330)
                        { // may a331
                            bool a331 = false;
                            {
                                Checkpoint(parser); // r332

                                bool r332 = true;
                                r332 = r332 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r332, parser);
                                a331 = r332;
                            }

                            r330 |= a331;
                        } // end may a331

                        r330 = r330 && Match(parser, new Jhu.Graywulf.SqlParser.TableSource());
                        if (r330)
                        { // may a333
                            bool a333 = false;
                            {
                                Checkpoint(parser); // r334

                                bool r334 = true;
                                r334 = r334 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r334, parser);
                                a333 = r334;
                            }

                            r330 |= a333;
                        } // end may a333

                        r330 = r330 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ON"));
                        if (r330)
                        { // may a335
                            bool a335 = false;
                            {
                                Checkpoint(parser); // r336

                                bool r336 = true;
                                r336 = r336 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r336, parser);
                                a335 = r336;
                            }

                            r330 |= a335;
                        } // end may a335

                        r330 = r330 && Match(parser, new Jhu.Graywulf.SqlParser.SearchCondition());
                        CommitOrRollback(r330, parser);
                        a329 = r330;
                    }

                    if (!a329)
                    {
                        Checkpoint(parser); // r337

                        bool r337 = true;
                        r337 = r337 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"CROSS"));
                        r337 = r337 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r337 = r337 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"JOIN"));
                        if (r337)
                        { // may a338
                            bool a338 = false;
                            {
                                Checkpoint(parser); // r339

                                bool r339 = true;
                                r339 = r339 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r339, parser);
                                a338 = r339;
                            }

                            r337 |= a338;
                        } // end may a338

                        r337 = r337 && Match(parser, new Jhu.Graywulf.SqlParser.TableSource());
                        CommitOrRollback(r337, parser);
                        a329 = r337;
                    }

                    if (!a329)
                    {
                        Checkpoint(parser); // r340

                        bool r340 = true;
                        r340 = r340 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        if (r340)
                        { // may a341
                            bool a341 = false;
                            {
                                Checkpoint(parser); // r342

                                bool r342 = true;
                                r342 = r342 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r342, parser);
                                a341 = r342;
                            }

                            r340 |= a341;
                        } // end may a341

                        r340 = r340 && Match(parser, new Jhu.Graywulf.SqlParser.TableSource());
                        CommitOrRollback(r340, parser);
                        a329 = r340;
                    }

                    if (!a329)
                    {
                        Checkpoint(parser); // r343

                        bool r343 = true;
                        if (r343)
                        { // alternatives a344 must
                            bool a344 = false;
                            if (!a344)
                            {
                                Checkpoint(parser); // r345

                                bool r345 = true;
                                r345 = r345 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"CROSS"));
                                CommitOrRollback(r345, parser);
                                a344 = r345;
                            }

                            if (!a344)
                            {
                                Checkpoint(parser); // r346

                                bool r346 = true;
                                r346 = r346 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"OUTER"));
                                CommitOrRollback(r346, parser);
                                a344 = r346;
                            }

                            r343 &= a344;

                        } // end alternatives a344

                        r343 = r343 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r343 = r343 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"APPLY"));
                        if (r343)
                        { // may a347
                            bool a347 = false;
                            {
                                Checkpoint(parser); // r348

                                bool r348 = true;
                                r348 = r348 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r348, parser);
                                a347 = r348;
                            }

                            r343 |= a347;
                        } // end may a347

                        r343 = r343 && Match(parser, new Jhu.Graywulf.SqlParser.TableSource());
                        CommitOrRollback(r343, parser);
                        a329 = r343;
                    }

                    r328 &= a329;

                } // end alternatives a329

                if (r328)
                { // may a349
                    bool a349 = false;
                    {
                        Checkpoint(parser); // r350

                        bool r350 = true;
                        if (r350)
                        { // may a351
                            bool a351 = false;
                            {
                                Checkpoint(parser); // r352

                                bool r352 = true;
                                r352 = r352 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r352, parser);
                                a351 = r352;
                            }

                            r350 |= a351;
                        } // end may a351

                        r350 = r350 && Match(parser, new Jhu.Graywulf.SqlParser.JoinedTable());
                        CommitOrRollback(r350, parser);
                        a349 = r350;
                    }

                    r328 |= a349;
                } // end may a349

                CommitOrRollback(r328, parser);
                res = r328;
            }

            return res;

        }
    }

    public partial class JoinType : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r353

                bool r353 = true;
                if (r353)
                { // may a354
                    bool a354 = false;
                    {
                        Checkpoint(parser); // r355

                        bool r355 = true;
                        if (r355)
                        { // alternatives a356 must
                            bool a356 = false;
                            if (!a356)
                            {
                                Checkpoint(parser); // r357

                                bool r357 = true;
                                r357 = r357 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"INNER"));
                                CommitOrRollback(r357, parser);
                                a356 = r357;
                            }

                            if (!a356)
                            {
                                Checkpoint(parser); // r358

                                bool r358 = true;
                                if (r358)
                                { // alternatives a359 must
                                    bool a359 = false;
                                    if (!a359)
                                    {
                                        Checkpoint(parser); // r360

                                        bool r360 = true;
                                        r360 = r360 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"LEFT"));
                                        CommitOrRollback(r360, parser);
                                        a359 = r360;
                                    }

                                    if (!a359)
                                    {
                                        Checkpoint(parser); // r361

                                        bool r361 = true;
                                        r361 = r361 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"RIGHT"));
                                        CommitOrRollback(r361, parser);
                                        a359 = r361;
                                    }

                                    if (!a359)
                                    {
                                        Checkpoint(parser); // r362

                                        bool r362 = true;
                                        r362 = r362 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"FULL"));
                                        CommitOrRollback(r362, parser);
                                        a359 = r362;
                                    }

                                    r358 &= a359;

                                } // end alternatives a359

                                if (r358)
                                { // may a363
                                    bool a363 = false;
                                    {
                                        Checkpoint(parser); // r364

                                        bool r364 = true;
                                        r364 = r364 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        r364 = r364 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"OUTER"));
                                        CommitOrRollback(r364, parser);
                                        a363 = r364;
                                    }

                                    r358 |= a363;
                                } // end may a363

                                CommitOrRollback(r358, parser);
                                a356 = r358;
                            }

                            r355 &= a356;

                        } // end alternatives a356

                        if (r355)
                        { // may a365
                            bool a365 = false;
                            {
                                Checkpoint(parser); // r366

                                bool r366 = true;
                                r366 = r366 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r366, parser);
                                a365 = r366;
                            }

                            r355 |= a365;
                        } // end may a365

                        if (r355)
                        { // may a367
                            bool a367 = false;
                            {
                                Checkpoint(parser); // r368

                                bool r368 = true;
                                r368 = r368 && Match(parser, new Jhu.Graywulf.SqlParser.JoinHint());
                                CommitOrRollback(r368, parser);
                                a367 = r368;
                            }

                            r355 |= a367;
                        } // end may a367

                        CommitOrRollback(r355, parser);
                        a354 = r355;
                    }

                    r353 |= a354;
                } // end may a354

                if (r353)
                { // may a369
                    bool a369 = false;
                    {
                        Checkpoint(parser); // r370

                        bool r370 = true;
                        r370 = r370 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r370, parser);
                        a369 = r370;
                    }

                    r353 |= a369;
                } // end may a369

                r353 = r353 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"JOIN"));
                CommitOrRollback(r353, parser);
                res = r353;
            }

            return res;

        }
    }

    public partial class JoinHint : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r371

                bool r371 = true;
                if (r371)
                { // alternatives a372 must
                    bool a372 = false;
                    if (!a372)
                    {
                        Checkpoint(parser); // r373

                        bool r373 = true;
                        r373 = r373 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"LOOP"));
                        CommitOrRollback(r373, parser);
                        a372 = r373;
                    }

                    if (!a372)
                    {
                        Checkpoint(parser); // r374

                        bool r374 = true;
                        r374 = r374 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"HASH"));
                        CommitOrRollback(r374, parser);
                        a372 = r374;
                    }

                    if (!a372)
                    {
                        Checkpoint(parser); // r375

                        bool r375 = true;
                        r375 = r375 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"MERGE"));
                        CommitOrRollback(r375, parser);
                        a372 = r375;
                    }

                    if (!a372)
                    {
                        Checkpoint(parser); // r376

                        bool r376 = true;
                        r376 = r376 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"REMOTE"));
                        CommitOrRollback(r376, parser);
                        a372 = r376;
                    }

                    r371 &= a372;

                } // end alternatives a372

                CommitOrRollback(r371, parser);
                res = r371;
            }

            return res;

        }
    }

    public partial class TableSource : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r377

                bool r377 = true;
                if (r377)
                { // alternatives a378 must
                    bool a378 = false;
                    if (!a378)
                    {
                        Checkpoint(parser); // r379

                        bool r379 = true;
                        r379 = r379 && Match(parser, new Jhu.Graywulf.SqlParser.FunctionTableSource());
                        CommitOrRollback(r379, parser);
                        a378 = r379;
                    }

                    if (!a378)
                    {
                        Checkpoint(parser); // r380

                        bool r380 = true;
                        r380 = r380 && Match(parser, new Jhu.Graywulf.SqlParser.SimpleTableSource());
                        CommitOrRollback(r380, parser);
                        a378 = r380;
                    }

                    if (!a378)
                    {
                        Checkpoint(parser); // r381

                        bool r381 = true;
                        r381 = r381 && Match(parser, new Jhu.Graywulf.SqlParser.VariableTableSource());
                        CommitOrRollback(r381, parser);
                        a378 = r381;
                    }

                    if (!a378)
                    {
                        Checkpoint(parser); // r382

                        bool r382 = true;
                        r382 = r382 && Match(parser, new Jhu.Graywulf.SqlParser.SubqueryTableSource());
                        CommitOrRollback(r382, parser);
                        a378 = r382;
                    }

                    r377 &= a378;

                } // end alternatives a378

                CommitOrRollback(r377, parser);
                res = r377;
            }

            return res;

        }
    }

    public partial class SimpleTableSource : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r383

                bool r383 = true;
                r383 = r383 && Match(parser, new Jhu.Graywulf.SqlParser.TableOrViewName());
                if (r383)
                { // may a384
                    bool a384 = false;
                    {
                        Checkpoint(parser); // r385

                        bool r385 = true;
                        r385 = r385 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        if (r385)
                        { // may a386
                            bool a386 = false;
                            {
                                Checkpoint(parser); // r387

                                bool r387 = true;
                                r387 = r387 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AS"));
                                r387 = r387 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r387, parser);
                                a386 = r387;
                            }

                            r385 |= a386;
                        } // end may a386

                        r385 = r385 && Match(parser, new Jhu.Graywulf.SqlParser.TableAlias());
                        CommitOrRollback(r385, parser);
                        a384 = r385;
                    }

                    r383 |= a384;
                } // end may a384

                if (r383)
                { // may a388
                    bool a388 = false;
                    {
                        Checkpoint(parser); // r389

                        bool r389 = true;
                        r389 = r389 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r389 = r389 && Match(parser, new Jhu.Graywulf.SqlParser.TableSampleClause());
                        CommitOrRollback(r389, parser);
                        a388 = r389;
                    }

                    r383 |= a388;
                } // end may a388

                if (r383)
                { // may a390
                    bool a390 = false;
                    {
                        Checkpoint(parser); // r391

                        bool r391 = true;
                        r391 = r391 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r391 = r391 && Match(parser, new Jhu.Graywulf.SqlParser.TableHintClause());
                        CommitOrRollback(r391, parser);
                        a390 = r391;
                    }

                    r383 |= a390;
                } // end may a390

                if (r383)
                { // may a392
                    bool a392 = false;
                    {
                        Checkpoint(parser); // r393

                        bool r393 = true;
                        r393 = r393 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r393 = r393 && Match(parser, new Jhu.Graywulf.SqlParser.TablePartitionClause());
                        CommitOrRollback(r393, parser);
                        a392 = r393;
                    }

                    r383 |= a392;
                } // end may a392

                CommitOrRollback(r383, parser);
                res = r383;
            }

            return res;

        }
    }

    public partial class FunctionTableSource : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r394

                bool r394 = true;
                r394 = r394 && Match(parser, new Jhu.Graywulf.SqlParser.TableValuedFunctionCall());
                if (r394)
                { // may a395
                    bool a395 = false;
                    {
                        Checkpoint(parser); // r396

                        bool r396 = true;
                        r396 = r396 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r396, parser);
                        a395 = r396;
                    }

                    r394 |= a395;
                } // end may a395

                if (r394)
                { // may a397
                    bool a397 = false;
                    {
                        Checkpoint(parser); // r398

                        bool r398 = true;
                        r398 = r398 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AS"));
                        r398 = r398 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r398, parser);
                        a397 = r398;
                    }

                    r394 |= a397;
                } // end may a397

                r394 = r394 && Match(parser, new Jhu.Graywulf.SqlParser.TableAlias());
                if (r394)
                { // may a399
                    bool a399 = false;
                    {
                        Checkpoint(parser); // r400

                        bool r400 = true;
                        if (r400)
                        { // may a401
                            bool a401 = false;
                            {
                                Checkpoint(parser); // r402

                                bool r402 = true;
                                r402 = r402 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r402, parser);
                                a401 = r402;
                            }

                            r400 |= a401;
                        } // end may a401

                        r400 = r400 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                        if (r400)
                        { // may a403
                            bool a403 = false;
                            {
                                Checkpoint(parser); // r404

                                bool r404 = true;
                                r404 = r404 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r404, parser);
                                a403 = r404;
                            }

                            r400 |= a403;
                        } // end may a403

                        r400 = r400 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnAliasList());
                        if (r400)
                        { // may a405
                            bool a405 = false;
                            {
                                Checkpoint(parser); // r406

                                bool r406 = true;
                                r406 = r406 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r406, parser);
                                a405 = r406;
                            }

                            r400 |= a405;
                        } // end may a405

                        r400 = r400 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                        CommitOrRollback(r400, parser);
                        a399 = r400;
                    }

                    r394 |= a399;
                } // end may a399

                CommitOrRollback(r394, parser);
                res = r394;
            }

            return res;

        }
    }

    public partial class VariableTableSource : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r407

                bool r407 = true;
                r407 = r407 && Match(parser, new Jhu.Graywulf.SqlParser.Variable());
                if (r407)
                { // may a408
                    bool a408 = false;
                    {
                        Checkpoint(parser); // r409

                        bool r409 = true;
                        r409 = r409 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        if (r409)
                        { // may a410
                            bool a410 = false;
                            {
                                Checkpoint(parser); // r411

                                bool r411 = true;
                                r411 = r411 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AS"));
                                r411 = r411 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r411, parser);
                                a410 = r411;
                            }

                            r409 |= a410;
                        } // end may a410

                        r409 = r409 && Match(parser, new Jhu.Graywulf.SqlParser.TableAlias());
                        CommitOrRollback(r409, parser);
                        a408 = r409;
                    }

                    r407 |= a408;
                } // end may a408

                CommitOrRollback(r407, parser);
                res = r407;
            }

            return res;

        }
    }

    public partial class SubqueryTableSource : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r412

                bool r412 = true;
                r412 = r412 && Match(parser, new Jhu.Graywulf.SqlParser.Subquery());
                if (r412)
                { // may a413
                    bool a413 = false;
                    {
                        Checkpoint(parser); // r414

                        bool r414 = true;
                        r414 = r414 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r414, parser);
                        a413 = r414;
                    }

                    r412 |= a413;
                } // end may a413

                if (r412)
                { // may a415
                    bool a415 = false;
                    {
                        Checkpoint(parser); // r416

                        bool r416 = true;
                        r416 = r416 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AS"));
                        r416 = r416 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r416, parser);
                        a415 = r416;
                    }

                    r412 |= a415;
                } // end may a415

                r412 = r412 && Match(parser, new Jhu.Graywulf.SqlParser.TableAlias());
                CommitOrRollback(r412, parser);
                res = r412;
            }

            return res;

        }
    }

    public partial class ColumnAliasList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r417

                bool r417 = true;
                r417 = r417 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnAlias());
                if (r417)
                { // may a418
                    bool a418 = false;
                    {
                        Checkpoint(parser); // r419

                        bool r419 = true;
                        if (r419)
                        { // may a420
                            bool a420 = false;
                            {
                                Checkpoint(parser); // r421

                                bool r421 = true;
                                r421 = r421 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r421, parser);
                                a420 = r421;
                            }

                            r419 |= a420;
                        } // end may a420

                        r419 = r419 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        r419 = r419 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnAliasList());
                        CommitOrRollback(r419, parser);
                        a418 = r419;
                    }

                    r417 |= a418;
                } // end may a418

                CommitOrRollback(r417, parser);
                res = r417;
            }

            return res;

        }
    }

    public partial class TableSampleClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r422

                bool r422 = true;
                r422 = r422 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"TABLESAMPLE"));
                if (r422)
                { // may a423
                    bool a423 = false;
                    {
                        Checkpoint(parser); // r424

                        bool r424 = true;
                        r424 = r424 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r424 = r424 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"SYSTEM"));
                        CommitOrRollback(r424, parser);
                        a423 = r424;
                    }

                    r422 |= a423;
                } // end may a423

                if (r422)
                { // may a425
                    bool a425 = false;
                    {
                        Checkpoint(parser); // r426

                        bool r426 = true;
                        r426 = r426 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r426, parser);
                        a425 = r426;
                    }

                    r422 |= a425;
                } // end may a425

                r422 = r422 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r422)
                { // may a427
                    bool a427 = false;
                    {
                        Checkpoint(parser); // r428

                        bool r428 = true;
                        r428 = r428 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r428, parser);
                        a427 = r428;
                    }

                    r422 |= a427;
                } // end may a427

                r422 = r422 && Match(parser, new Jhu.Graywulf.SqlParser.SampleNumber());
                if (r422)
                { // may a429
                    bool a429 = false;
                    {
                        Checkpoint(parser); // r430

                        bool r430 = true;
                        r430 = r430 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        if (r430)
                        { // may a431
                            bool a431 = false;
                            {
                                Checkpoint(parser); // r432

                                bool r432 = true;
                                if (r432)
                                { // alternatives a433 must
                                    bool a433 = false;
                                    if (!a433)
                                    {
                                        Checkpoint(parser); // r434

                                        bool r434 = true;
                                        r434 = r434 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"PERCENT"));
                                        CommitOrRollback(r434, parser);
                                        a433 = r434;
                                    }

                                    if (!a433)
                                    {
                                        Checkpoint(parser); // r435

                                        bool r435 = true;
                                        r435 = r435 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ROWS"));
                                        CommitOrRollback(r435, parser);
                                        a433 = r435;
                                    }

                                    r432 &= a433;

                                } // end alternatives a433

                                CommitOrRollback(r432, parser);
                                a431 = r432;
                            }

                            r430 |= a431;
                        } // end may a431

                        CommitOrRollback(r430, parser);
                        a429 = r430;
                    }

                    r422 |= a429;
                } // end may a429

                if (r422)
                { // may a436
                    bool a436 = false;
                    {
                        Checkpoint(parser); // r437

                        bool r437 = true;
                        r437 = r437 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r437, parser);
                        a436 = r437;
                    }

                    r422 |= a436;
                } // end may a436

                r422 = r422 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                if (r422)
                { // may a438
                    bool a438 = false;
                    {
                        Checkpoint(parser); // r439

                        bool r439 = true;
                        r439 = r439 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r439 = r439 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"REPEATABLE"));
                        if (r439)
                        { // may a440
                            bool a440 = false;
                            {
                                Checkpoint(parser); // r441

                                bool r441 = true;
                                r441 = r441 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r441, parser);
                                a440 = r441;
                            }

                            r439 |= a440;
                        } // end may a440

                        r439 = r439 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                        if (r439)
                        { // may a442
                            bool a442 = false;
                            {
                                Checkpoint(parser); // r443

                                bool r443 = true;
                                r443 = r443 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r443, parser);
                                a442 = r443;
                            }

                            r439 |= a442;
                        } // end may a442

                        r439 = r439 && Match(parser, new Jhu.Graywulf.SqlParser.RepeatSeed());
                        if (r439)
                        { // may a444
                            bool a444 = false;
                            {
                                Checkpoint(parser); // r445

                                bool r445 = true;
                                r445 = r445 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r445, parser);
                                a444 = r445;
                            }

                            r439 |= a444;
                        } // end may a444

                        r439 = r439 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                        CommitOrRollback(r439, parser);
                        a438 = r439;
                    }

                    r422 |= a438;
                } // end may a438

                CommitOrRollback(r422, parser);
                res = r422;
            }

            return res;

        }
    }

    public partial class TableHintClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r446

                bool r446 = true;
                r446 = r446 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"WITH"));
                if (r446)
                { // may a447
                    bool a447 = false;
                    {
                        Checkpoint(parser); // r448

                        bool r448 = true;
                        r448 = r448 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r448, parser);
                        a447 = r448;
                    }

                    r446 |= a447;
                } // end may a447

                r446 = r446 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r446)
                { // may a449
                    bool a449 = false;
                    {
                        Checkpoint(parser); // r450

                        bool r450 = true;
                        r450 = r450 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r450, parser);
                        a449 = r450;
                    }

                    r446 |= a449;
                } // end may a449

                if (r446)
                { // may a451
                    bool a451 = false;
                    {
                        Checkpoint(parser); // r452

                        bool r452 = true;
                        r452 = r452 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOEXPAND"));
                        r452 = r452 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r452, parser);
                        a451 = r452;
                    }

                    r446 |= a451;
                } // end may a451

                r446 = r446 && Match(parser, new Jhu.Graywulf.SqlParser.TableHintList());
                if (r446)
                { // may a453
                    bool a453 = false;
                    {
                        Checkpoint(parser); // r454

                        bool r454 = true;
                        r454 = r454 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r454, parser);
                        a453 = r454;
                    }

                    r446 |= a453;
                } // end may a453

                r446 = r446 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                CommitOrRollback(r446, parser);
                res = r446;
            }

            return res;

        }
    }

    public partial class TableHintList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r455

                bool r455 = true;
                r455 = r455 && Match(parser, new Jhu.Graywulf.SqlParser.TableHint());
                if (r455)
                {
                    Checkpoint(parser); // r456

                    bool r456 = true;
                    if (r456)
                    { // may a457
                        bool a457 = false;
                        {
                            Checkpoint(parser); // r458

                            bool r458 = true;
                            r458 = r458 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                            CommitOrRollback(r458, parser);
                            a457 = r458;
                        }

                        r456 |= a457;
                    } // end may a457

                    r456 = r456 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                    if (r456)
                    { // may a459
                        bool a459 = false;
                        {
                            Checkpoint(parser); // r460

                            bool r460 = true;
                            r460 = r460 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                            CommitOrRollback(r460, parser);
                            a459 = r460;
                        }

                        r456 |= a459;
                    } // end may a459

                    r456 = r456 && Match(parser, new Jhu.Graywulf.SqlParser.TableHintList());
                    CommitOrRollback(r456, parser);
                    r455 = r456;
                }

                CommitOrRollback(r455, parser);
                res = r455;
            }

            return res;

        }
    }

    public partial class TableHint : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r461

                bool r461 = true;
                if (r461)
                { // alternatives a462 must
                    bool a462 = false;
                    if (!a462)
                    {
                        Checkpoint(parser); // r463

                        bool r463 = true;
                        r463 = r463 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"INDEX"));
                        if (r463)
                        { // may a464
                            bool a464 = false;
                            {
                                Checkpoint(parser); // r465

                                bool r465 = true;
                                r465 = r465 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r465, parser);
                                a464 = r465;
                            }

                            r463 |= a464;
                        } // end may a464

                        r463 = r463 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                        if (r463)
                        { // may a466
                            bool a466 = false;
                            {
                                Checkpoint(parser); // r467

                                bool r467 = true;
                                r467 = r467 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r467, parser);
                                a466 = r467;
                            }

                            r463 |= a466;
                        } // end may a466

                        r463 = r463 && Match(parser, new Jhu.Graywulf.SqlParser.IndexValueList());
                        if (r463)
                        { // may a468
                            bool a468 = false;
                            {
                                Checkpoint(parser); // r469

                                bool r469 = true;
                                r469 = r469 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r469, parser);
                                a468 = r469;
                            }

                            r463 |= a468;
                        } // end may a468

                        r463 = r463 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                        CommitOrRollback(r463, parser);
                        a462 = r463;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r470

                        bool r470 = true;
                        r470 = r470 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"FASTFIRSTROW"));
                        CommitOrRollback(r470, parser);
                        a462 = r470;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r471

                        bool r471 = true;
                        r471 = r471 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"HOLDLOCK"));
                        CommitOrRollback(r471, parser);
                        a462 = r471;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r472

                        bool r472 = true;
                        r472 = r472 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOLOCK"));
                        CommitOrRollback(r472, parser);
                        a462 = r472;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r473

                        bool r473 = true;
                        r473 = r473 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOWAIT"));
                        CommitOrRollback(r473, parser);
                        a462 = r473;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r474

                        bool r474 = true;
                        r474 = r474 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"PAGLOCK"));
                        CommitOrRollback(r474, parser);
                        a462 = r474;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r475

                        bool r475 = true;
                        r475 = r475 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"READCOMMITTED"));
                        CommitOrRollback(r475, parser);
                        a462 = r475;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r476

                        bool r476 = true;
                        r476 = r476 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"READCOMMITTEDLOCK"));
                        CommitOrRollback(r476, parser);
                        a462 = r476;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r477

                        bool r477 = true;
                        r477 = r477 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"READPAST"));
                        CommitOrRollback(r477, parser);
                        a462 = r477;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r478

                        bool r478 = true;
                        r478 = r478 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"READUNCOMMITTED"));
                        CommitOrRollback(r478, parser);
                        a462 = r478;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r479

                        bool r479 = true;
                        r479 = r479 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"REPEATABLEREAD"));
                        CommitOrRollback(r479, parser);
                        a462 = r479;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r480

                        bool r480 = true;
                        r480 = r480 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ROWLOCK"));
                        CommitOrRollback(r480, parser);
                        a462 = r480;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r481

                        bool r481 = true;
                        r481 = r481 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"SERIALIZABLE"));
                        CommitOrRollback(r481, parser);
                        a462 = r481;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r482

                        bool r482 = true;
                        r482 = r482 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"TABLOCK"));
                        CommitOrRollback(r482, parser);
                        a462 = r482;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r483

                        bool r483 = true;
                        r483 = r483 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"TABLOCKX"));
                        CommitOrRollback(r483, parser);
                        a462 = r483;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r484

                        bool r484 = true;
                        r484 = r484 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"UPDLOCK"));
                        CommitOrRollback(r484, parser);
                        a462 = r484;
                    }

                    if (!a462)
                    {
                        Checkpoint(parser); // r485

                        bool r485 = true;
                        r485 = r485 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"XLOCK"));
                        CommitOrRollback(r485, parser);
                        a462 = r485;
                    }

                    r461 &= a462;

                } // end alternatives a462

                CommitOrRollback(r461, parser);
                res = r461;
            }

            return res;

        }
    }

    public partial class IndexValueList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r486

                bool r486 = true;
                r486 = r486 && Match(parser, new Jhu.Graywulf.SqlParser.IndexValue());
                if (r486)
                {
                    Checkpoint(parser); // r487

                    bool r487 = true;
                    if (r487)
                    { // may a488
                        bool a488 = false;
                        {
                            Checkpoint(parser); // r489

                            bool r489 = true;
                            r489 = r489 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                            CommitOrRollback(r489, parser);
                            a488 = r489;
                        }

                        r487 |= a488;
                    } // end may a488

                    r487 = r487 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                    if (r487)
                    { // may a490
                        bool a490 = false;
                        {
                            Checkpoint(parser); // r491

                            bool r491 = true;
                            r491 = r491 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                            CommitOrRollback(r491, parser);
                            a490 = r491;
                        }

                        r487 |= a490;
                    } // end may a490

                    r487 = r487 && Match(parser, new Jhu.Graywulf.SqlParser.IndexValueList());
                    CommitOrRollback(r487, parser);
                    r486 = r487;
                }

                CommitOrRollback(r486, parser);
                res = r486;
            }

            return res;

        }
    }

    public partial class Subquery : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r492

                bool r492 = true;
                r492 = r492 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r492)
                { // may a493
                    bool a493 = false;
                    {
                        Checkpoint(parser); // r494

                        bool r494 = true;
                        r494 = r494 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r494, parser);
                        a493 = r494;
                    }

                    r492 |= a493;
                } // end may a493

                r492 = r492 && Match(parser, new Jhu.Graywulf.SqlParser.SelectStatement());
                if (r492)
                { // may a495
                    bool a495 = false;
                    {
                        Checkpoint(parser); // r496

                        bool r496 = true;
                        r496 = r496 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r496, parser);
                        a495 = r496;
                    }

                    r492 |= a495;
                } // end may a495

                r492 = r492 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                CommitOrRollback(r492, parser);
                res = r492;
            }

            return res;

        }
    }

    public partial class TablePartitionClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r497

                bool r497 = true;
                r497 = r497 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"PARTITION"));
                r497 = r497 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r497 = r497 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ON"));
                r497 = r497 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r497 = r497 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnIdentifier());
                CommitOrRollback(r497, parser);
                res = r497;
            }

            return res;

        }
    }

    public partial class WhereClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r498

                bool r498 = true;
                r498 = r498 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"WHERE"));
                if (r498)
                { // may a499
                    bool a499 = false;
                    {
                        Checkpoint(parser); // r500

                        bool r500 = true;
                        r500 = r500 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r500, parser);
                        a499 = r500;
                    }

                    r498 |= a499;
                } // end may a499

                r498 = r498 && Match(parser, new Jhu.Graywulf.SqlParser.SearchCondition());
                CommitOrRollback(r498, parser);
                res = r498;
            }

            return res;

        }
    }

    public partial class SearchCondition : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r501

                bool r501 = true;
                if (r501)
                { // may a502
                    bool a502 = false;
                    {
                        Checkpoint(parser); // r503

                        bool r503 = true;
                        r503 = r503 && Match(parser, new Jhu.Graywulf.SqlParser.LogicalNot());
                        CommitOrRollback(r503, parser);
                        a502 = r503;
                    }

                    r501 |= a502;
                } // end may a502

                if (r501)
                { // may a504
                    bool a504 = false;
                    {
                        Checkpoint(parser); // r505

                        bool r505 = true;
                        r505 = r505 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r505, parser);
                        a504 = r505;
                    }

                    r501 |= a504;
                } // end may a504

                if (r501)
                { // alternatives a506 must
                    bool a506 = false;
                    if (!a506)
                    {
                        Checkpoint(parser); // r507

                        bool r507 = true;
                        r507 = r507 && Match(parser, new Jhu.Graywulf.SqlParser.Predicate());
                        CommitOrRollback(r507, parser);
                        a506 = r507;
                    }

                    if (!a506)
                    {
                        Checkpoint(parser); // r508

                        bool r508 = true;
                        r508 = r508 && Match(parser, new Jhu.Graywulf.SqlParser.SearchConditionBrackets());
                        CommitOrRollback(r508, parser);
                        a506 = r508;
                    }

                    r501 &= a506;

                } // end alternatives a506

                if (r501)
                { // may a509
                    bool a509 = false;
                    {
                        Checkpoint(parser); // r510

                        bool r510 = true;
                        if (r510)
                        { // may a511
                            bool a511 = false;
                            {
                                Checkpoint(parser); // r512

                                bool r512 = true;
                                r512 = r512 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r512, parser);
                                a511 = r512;
                            }

                            r510 |= a511;
                        } // end may a511

                        r510 = r510 && Match(parser, new Jhu.Graywulf.SqlParser.LogicalOperator());
                        if (r510)
                        { // may a513
                            bool a513 = false;
                            {
                                Checkpoint(parser); // r514

                                bool r514 = true;
                                r514 = r514 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r514, parser);
                                a513 = r514;
                            }

                            r510 |= a513;
                        } // end may a513

                        r510 = r510 && Match(parser, new Jhu.Graywulf.SqlParser.SearchCondition());
                        CommitOrRollback(r510, parser);
                        a509 = r510;
                    }

                    r501 |= a509;
                } // end may a509

                CommitOrRollback(r501, parser);
                res = r501;
            }

            return res;

        }
    }

    public partial class SearchConditionBrackets : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r515

                bool r515 = true;
                r515 = r515 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                if (r515)
                { // may a516
                    bool a516 = false;
                    {
                        Checkpoint(parser); // r517

                        bool r517 = true;
                        r517 = r517 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r517, parser);
                        a516 = r517;
                    }

                    r515 |= a516;
                } // end may a516

                r515 = r515 && Match(parser, new Jhu.Graywulf.SqlParser.SearchCondition());
                if (r515)
                { // may a518
                    bool a518 = false;
                    {
                        Checkpoint(parser); // r519

                        bool r519 = true;
                        r519 = r519 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r519, parser);
                        a518 = r519;
                    }

                    r515 |= a518;
                } // end may a518

                r515 = r515 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                CommitOrRollback(r515, parser);
                res = r515;
            }

            return res;

        }
    }

    public partial class Predicate : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r520

                bool r520 = true;
                if (r520)
                { // alternatives a521 must
                    bool a521 = false;
                    if (!a521)
                    {
                        Checkpoint(parser); // r522

                        bool r522 = true;
                        r522 = r522 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r522)
                        { // may a523
                            bool a523 = false;
                            {
                                Checkpoint(parser); // r524

                                bool r524 = true;
                                r524 = r524 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r524, parser);
                                a523 = r524;
                            }

                            r522 |= a523;
                        } // end may a523

                        r522 = r522 && Match(parser, new Jhu.Graywulf.SqlParser.ComparisonOperator());
                        if (r522)
                        { // may a525
                            bool a525 = false;
                            {
                                Checkpoint(parser); // r526

                                bool r526 = true;
                                r526 = r526 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r526, parser);
                                a525 = r526;
                            }

                            r522 |= a525;
                        } // end may a525

                        r522 = r522 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        CommitOrRollback(r522, parser);
                        a521 = r522;
                    }

                    if (!a521)
                    {
                        Checkpoint(parser); // r527

                        bool r527 = true;
                        r527 = r527 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r527)
                        { // may a528
                            bool a528 = false;
                            {
                                Checkpoint(parser); // r529

                                bool r529 = true;
                                r529 = r529 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOT"));
                                CommitOrRollback(r529, parser);
                                a528 = r529;
                            }

                            r527 |= a528;
                        } // end may a528

                        if (r527)
                        { // may a530
                            bool a530 = false;
                            {
                                Checkpoint(parser); // r531

                                bool r531 = true;
                                r531 = r531 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r531, parser);
                                a530 = r531;
                            }

                            r527 |= a530;
                        } // end may a530

                        r527 = r527 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"LIKE"));
                        if (r527)
                        { // may a532
                            bool a532 = false;
                            {
                                Checkpoint(parser); // r533

                                bool r533 = true;
                                r533 = r533 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r533, parser);
                                a532 = r533;
                            }

                            r527 |= a532;
                        } // end may a532

                        r527 = r527 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r527)
                        { // may a534
                            bool a534 = false;
                            {
                                Checkpoint(parser); // r535

                                bool r535 = true;
                                if (r535)
                                { // may a536
                                    bool a536 = false;
                                    {
                                        Checkpoint(parser); // r537

                                        bool r537 = true;
                                        r537 = r537 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r537, parser);
                                        a536 = r537;
                                    }

                                    r535 |= a536;
                                } // end may a536

                                r535 = r535 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ESCAPE"));
                                if (r535)
                                { // may a538
                                    bool a538 = false;
                                    {
                                        Checkpoint(parser); // r539

                                        bool r539 = true;
                                        r539 = r539 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r539, parser);
                                        a538 = r539;
                                    }

                                    r535 |= a538;
                                } // end may a538

                                r535 = r535 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                                CommitOrRollback(r535, parser);
                                a534 = r535;
                            }

                            r527 |= a534;
                        } // end may a534

                        CommitOrRollback(r527, parser);
                        a521 = r527;
                    }

                    if (!a521)
                    {
                        Checkpoint(parser); // r540

                        bool r540 = true;
                        r540 = r540 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r540)
                        { // may a541
                            bool a541 = false;
                            {
                                Checkpoint(parser); // r542

                                bool r542 = true;
                                if (r542)
                                { // may a543
                                    bool a543 = false;
                                    {
                                        Checkpoint(parser); // r544

                                        bool r544 = true;
                                        r544 = r544 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r544, parser);
                                        a543 = r544;
                                    }

                                    r542 |= a543;
                                } // end may a543

                                r542 = r542 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOT"));
                                CommitOrRollback(r542, parser);
                                a541 = r542;
                            }

                            r540 |= a541;
                        } // end may a541

                        if (r540)
                        { // may a545
                            bool a545 = false;
                            {
                                Checkpoint(parser); // r546

                                bool r546 = true;
                                r546 = r546 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r546, parser);
                                a545 = r546;
                            }

                            r540 |= a545;
                        } // end may a545

                        r540 = r540 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"BETWEEN"));
                        if (r540)
                        { // may a547
                            bool a547 = false;
                            {
                                Checkpoint(parser); // r548

                                bool r548 = true;
                                r548 = r548 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r548, parser);
                                a547 = r548;
                            }

                            r540 |= a547;
                        } // end may a547

                        r540 = r540 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r540)
                        { // may a549
                            bool a549 = false;
                            {
                                Checkpoint(parser); // r550

                                bool r550 = true;
                                r550 = r550 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r550, parser);
                                a549 = r550;
                            }

                            r540 |= a549;
                        } // end may a549

                        r540 = r540 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"AND"));
                        if (r540)
                        { // may a551
                            bool a551 = false;
                            {
                                Checkpoint(parser); // r552

                                bool r552 = true;
                                r552 = r552 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r552, parser);
                                a551 = r552;
                            }

                            r540 |= a551;
                        } // end may a551

                        r540 = r540 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        CommitOrRollback(r540, parser);
                        a521 = r540;
                    }

                    if (!a521)
                    {
                        Checkpoint(parser); // r553

                        bool r553 = true;
                        r553 = r553 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r553)
                        { // may a554
                            bool a554 = false;
                            {
                                Checkpoint(parser); // r555

                                bool r555 = true;
                                r555 = r555 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r555, parser);
                                a554 = r555;
                            }

                            r553 |= a554;
                        } // end may a554

                        r553 = r553 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"IS"));
                        if (r553)
                        { // may a556
                            bool a556 = false;
                            {
                                Checkpoint(parser); // r557

                                bool r557 = true;
                                r557 = r557 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                r557 = r557 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOT"));
                                CommitOrRollback(r557, parser);
                                a556 = r557;
                            }

                            r553 |= a556;
                        } // end may a556

                        r553 = r553 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r553 = r553 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NULL"));
                        CommitOrRollback(r553, parser);
                        a521 = r553;
                    }

                    if (!a521)
                    {
                        Checkpoint(parser); // r558

                        bool r558 = true;
                        r558 = r558 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r558)
                        { // may a559
                            bool a559 = false;
                            {
                                Checkpoint(parser); // r560

                                bool r560 = true;
                                r560 = r560 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"NOT"));
                                CommitOrRollback(r560, parser);
                                a559 = r560;
                            }

                            r558 |= a559;
                        } // end may a559

                        if (r558)
                        { // may a561
                            bool a561 = false;
                            {
                                Checkpoint(parser); // r562

                                bool r562 = true;
                                r562 = r562 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r562, parser);
                                a561 = r562;
                            }

                            r558 |= a561;
                        } // end may a561

                        r558 = r558 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"IN"));
                        if (r558)
                        { // may a563
                            bool a563 = false;
                            {
                                Checkpoint(parser); // r564

                                bool r564 = true;
                                r564 = r564 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r564, parser);
                                a563 = r564;
                            }

                            r558 |= a563;
                        } // end may a563

                        if (r558)
                        { // alternatives a565 must
                            bool a565 = false;
                            if (!a565)
                            {
                                Checkpoint(parser); // r566

                                bool r566 = true;
                                r566 = r566 && Match(parser, new Jhu.Graywulf.SqlParser.Subquery());
                                CommitOrRollback(r566, parser);
                                a565 = r566;
                            }

                            if (!a565)
                            {
                                Checkpoint(parser); // r567

                                bool r567 = true;
                                r567 = r567 && Match(parser, new Jhu.Graywulf.SqlParser.BracketOpen());
                                if (r567)
                                { // may a568
                                    bool a568 = false;
                                    {
                                        Checkpoint(parser); // r569

                                        bool r569 = true;
                                        r569 = r569 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r569, parser);
                                        a568 = r569;
                                    }

                                    r567 |= a568;
                                } // end may a568

                                r567 = r567 && Match(parser, new Jhu.Graywulf.SqlParser.ArgumentList());
                                if (r567)
                                { // may a570
                                    bool a570 = false;
                                    {
                                        Checkpoint(parser); // r571

                                        bool r571 = true;
                                        r571 = r571 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r571, parser);
                                        a570 = r571;
                                    }

                                    r567 |= a570;
                                } // end may a570

                                r567 = r567 && Match(parser, new Jhu.Graywulf.SqlParser.BracketClose());
                                CommitOrRollback(r567, parser);
                                a565 = r567;
                            }

                            r558 &= a565;

                        } // end alternatives a565

                        CommitOrRollback(r558, parser);
                        a521 = r558;
                    }

                    if (!a521)
                    {
                        Checkpoint(parser); // r572

                        bool r572 = true;
                        r572 = r572 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        if (r572)
                        { // may a573
                            bool a573 = false;
                            {
                                Checkpoint(parser); // r574

                                bool r574 = true;
                                r574 = r574 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r574, parser);
                                a573 = r574;
                            }

                            r572 |= a573;
                        } // end may a573

                        r572 = r572 && Match(parser, new Jhu.Graywulf.SqlParser.ComparisonOperator());
                        if (r572)
                        { // may a575
                            bool a575 = false;
                            {
                                Checkpoint(parser); // r576

                                bool r576 = true;
                                r576 = r576 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r576, parser);
                                a575 = r576;
                            }

                            r572 |= a575;
                        } // end may a575

                        if (r572)
                        { // alternatives a577 must
                            bool a577 = false;
                            if (!a577)
                            {
                                Checkpoint(parser); // r578

                                bool r578 = true;
                                r578 = r578 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ALL"));
                                CommitOrRollback(r578, parser);
                                a577 = r578;
                            }

                            if (!a577)
                            {
                                Checkpoint(parser); // r579

                                bool r579 = true;
                                r579 = r579 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"SOME"));
                                CommitOrRollback(r579, parser);
                                a577 = r579;
                            }

                            if (!a577)
                            {
                                Checkpoint(parser); // r580

                                bool r580 = true;
                                r580 = r580 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ANY"));
                                CommitOrRollback(r580, parser);
                                a577 = r580;
                            }

                            r572 &= a577;

                        } // end alternatives a577

                        if (r572)
                        { // may a581
                            bool a581 = false;
                            {
                                Checkpoint(parser); // r582

                                bool r582 = true;
                                r582 = r582 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r582, parser);
                                a581 = r582;
                            }

                            r572 |= a581;
                        } // end may a581

                        r572 = r572 && Match(parser, new Jhu.Graywulf.SqlParser.Subquery());
                        CommitOrRollback(r572, parser);
                        a521 = r572;
                    }

                    if (!a521)
                    {
                        Checkpoint(parser); // r583

                        bool r583 = true;
                        r583 = r583 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"EXISTS"));
                        if (r583)
                        { // may a584
                            bool a584 = false;
                            {
                                Checkpoint(parser); // r585

                                bool r585 = true;
                                r585 = r585 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r585, parser);
                                a584 = r585;
                            }

                            r583 |= a584;
                        } // end may a584

                        r583 = r583 && Match(parser, new Jhu.Graywulf.SqlParser.Subquery());
                        CommitOrRollback(r583, parser);
                        a521 = r583;
                    }

                    r520 &= a521;

                } // end alternatives a521

                CommitOrRollback(r520, parser);
                res = r520;
            }

            return res;

        }
    }

    public partial class GroupByClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r586

                bool r586 = true;
                r586 = r586 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"GROUP"));
                r586 = r586 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r586 = r586 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"BY"));
                if (r586)
                { // alternatives a587 must
                    bool a587 = false;
                    if (!a587)
                    {
                        Checkpoint(parser); // r588

                        bool r588 = true;
                        r588 = r588 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        r588 = r588 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ALL"));
                        CommitOrRollback(r588, parser);
                        a587 = r588;
                    }

                    if (!a587)
                    {
                        Checkpoint(parser); // r589

                        bool r589 = true;
                        if (r589)
                        { // may a590
                            bool a590 = false;
                            {
                                Checkpoint(parser); // r591

                                bool r591 = true;
                                r591 = r591 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r591, parser);
                                a590 = r591;
                            }

                            r589 |= a590;
                        } // end may a590

                        r589 = r589 && Match(parser, new Jhu.Graywulf.SqlParser.GroupByList());
                        CommitOrRollback(r589, parser);
                        a587 = r589;
                    }

                    r586 &= a587;

                } // end alternatives a587

                CommitOrRollback(r586, parser);
                res = r586;
            }

            return res;

        }
    }

    public partial class GroupByList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r592

                bool r592 = true;
                r592 = r592 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                if (r592)
                { // may a593
                    bool a593 = false;
                    {
                        Checkpoint(parser); // r594

                        bool r594 = true;
                        if (r594)
                        { // may a595
                            bool a595 = false;
                            {
                                Checkpoint(parser); // r596

                                bool r596 = true;
                                r596 = r596 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r596, parser);
                                a595 = r596;
                            }

                            r594 |= a595;
                        } // end may a595

                        r594 = r594 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        if (r594)
                        { // may a597
                            bool a597 = false;
                            {
                                Checkpoint(parser); // r598

                                bool r598 = true;
                                r598 = r598 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r598, parser);
                                a597 = r598;
                            }

                            r594 |= a597;
                        } // end may a597

                        r594 = r594 && Match(parser, new Jhu.Graywulf.SqlParser.GroupByList());
                        CommitOrRollback(r594, parser);
                        a593 = r594;
                    }

                    r592 |= a593;
                } // end may a593

                CommitOrRollback(r592, parser);
                res = r592;
            }

            return res;

        }
    }

    public partial class HavingClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r599

                bool r599 = true;
                r599 = r599 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"HAVING"));
                if (r599)
                { // may a600
                    bool a600 = false;
                    {
                        Checkpoint(parser); // r601

                        bool r601 = true;
                        r601 = r601 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r601, parser);
                        a600 = r601;
                    }

                    r599 |= a600;
                } // end may a600

                r599 = r599 && Match(parser, new Jhu.Graywulf.SqlParser.SearchCondition());
                CommitOrRollback(r599, parser);
                res = r599;
            }

            return res;

        }
    }

    public partial class OrderByClause : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r602

                bool r602 = true;
                r602 = r602 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ORDER"));
                r602 = r602 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                r602 = r602 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"BY"));
                if (r602)
                { // may a603
                    bool a603 = false;
                    {
                        Checkpoint(parser); // r604

                        bool r604 = true;
                        r604 = r604 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                        CommitOrRollback(r604, parser);
                        a603 = r604;
                    }

                    r602 |= a603;
                } // end may a603

                r602 = r602 && Match(parser, new Jhu.Graywulf.SqlParser.OrderByList());
                CommitOrRollback(r602, parser);
                res = r602;
            }

            return res;

        }
    }

    public partial class OrderByList : Jhu.Graywulf.ParserLib.Node
    {
        public override bool Match(Jhu.Graywulf.ParserLib.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r605

                bool r605 = true;
                if (r605)
                { // alternatives a606 must
                    bool a606 = false;
                    if (!a606)
                    {
                        Checkpoint(parser); // r607

                        bool r607 = true;
                        r607 = r607 && Match(parser, new Jhu.Graywulf.SqlParser.Expression());
                        CommitOrRollback(r607, parser);
                        a606 = r607;
                    }

                    if (!a606)
                    {
                        Checkpoint(parser); // r608

                        bool r608 = true;
                        r608 = r608 && Match(parser, new Jhu.Graywulf.SqlParser.ColumnPosition());
                        if (r608)
                        { // may a609
                            bool a609 = false;
                            {
                                Checkpoint(parser); // r610

                                bool r610 = true;
                                if (r610)
                                { // may a611
                                    bool a611 = false;
                                    {
                                        Checkpoint(parser); // r612

                                        bool r612 = true;
                                        r612 = r612 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                        CommitOrRollback(r612, parser);
                                        a611 = r612;
                                    }

                                    r610 |= a611;
                                } // end may a611

                                if (r610)
                                { // alternatives a613 must
                                    bool a613 = false;
                                    if (!a613)
                                    {
                                        Checkpoint(parser); // r614

                                        bool r614 = true;
                                        r614 = r614 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"ASC"));
                                        CommitOrRollback(r614, parser);
                                        a613 = r614;
                                    }

                                    if (!a613)
                                    {
                                        Checkpoint(parser); // r615

                                        bool r615 = true;
                                        r615 = r615 && Match(parser, new Jhu.Graywulf.ParserLib.Keyword(@"DESC"));
                                        CommitOrRollback(r615, parser);
                                        a613 = r615;
                                    }

                                    r610 &= a613;

                                } // end alternatives a613

                                CommitOrRollback(r610, parser);
                                a609 = r610;
                            }

                            r608 |= a609;
                        } // end may a609

                        CommitOrRollback(r608, parser);
                        a606 = r608;
                    }

                    r605 &= a606;

                } // end alternatives a606

                if (r605)
                { // may a616
                    bool a616 = false;
                    {
                        Checkpoint(parser); // r617

                        bool r617 = true;
                        if (r617)
                        { // may a618
                            bool a618 = false;
                            {
                                Checkpoint(parser); // r619

                                bool r619 = true;
                                r619 = r619 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r619, parser);
                                a618 = r619;
                            }

                            r617 |= a618;
                        } // end may a618

                        r617 = r617 && Match(parser, new Jhu.Graywulf.SqlParser.Comma());
                        if (r617)
                        { // may a620
                            bool a620 = false;
                            {
                                Checkpoint(parser); // r621

                                bool r621 = true;
                                r621 = r621 && Match(parser, new Jhu.Graywulf.SqlParser.CommentOrWhitespace());
                                CommitOrRollback(r621, parser);
                                a620 = r621;
                            }

                            r617 |= a620;
                        } // end may a620

                        r617 = r617 && Match(parser, new Jhu.Graywulf.SqlParser.OrderByList());
                        CommitOrRollback(r617, parser);
                        a616 = r617;
                    }

                    r605 |= a616;
                } // end may a616

                CommitOrRollback(r605, parser);
                res = r605;
            }

            return res;

        }
    }


}