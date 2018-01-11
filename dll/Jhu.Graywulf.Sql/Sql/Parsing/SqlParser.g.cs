using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class SqlParser : Jhu.Graywulf.Parsing.Parser
    {
        private static HashSet<string> keywords = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "ABSOLUTE", "ALL", "AND", "ANY", "APPLY", "AS", "ASC", "BEGIN", "BETWEEN", 
            "BREAK", "BY", "CASE", "CATCH", "CLOSE", "CLUSTERED", "CONSTRAINT", "CONTINUE", "CREATE", "CROSS", 
            "CURSOR", "DEALLOCATE", "DECLARE", "DEFAULT", "DELETE", "DESC", "DISTINCT", "DROP", "ELSE", "END", 
            "ESCAPE", "EXCEPT", "EXISTS", "FETCH", "FIRST", "FOR", "FROM", "FULL", "GOTO", "GROUP", 
            "HASH", "HAVING", "IDENTITY", "IF", "IN", "INCLUDE", "INDEX", "INNER", "INSERT", "INTERSECT", 
            "INTO", "IS", "JOIN", "KEY", "LAST", "LEFT", "LIKE", "LOOP", "MERGE", "NEXT", 
            "NONCLUSTERED", "NOT", "NULL", "ON", "OPEN", "OPTION", "OR", "ORDER", "OUTER", "OVER", 
            "PARTITION", "PERCENT", "PRIMARY", "PRIOR", "RELATIVE", "REMOTE", "REPEATABLE", "RETURN", "RIGHT", "ROWS", 
            "SELECT", "SET", "SOME", "SYSTEM", "TABLE", "TABLESAMPLE", "THEN", "THROW", "TIES", "TOP", 
            "TRUNCATE", "TRY", "UNION", "UNIQUE", "UPDATE", "VALUES", "WHEN", "WHERE", "WHILE", "WITH", 

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

        public override Jhu.Graywulf.Parsing.Token Execute(string code)
        {
            return Execute(new StatementBlock(), code);
        }
    }

    public partial class Plus : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"+"; }
        }

        public Plus()
            :base()
        {
            Value = @"+";
        }

        public Plus(Plus old)
            :base(old)
        {
        }

        public static Plus Create()
        {
            var s = new Plus();
            s.Value = @"+";
            return s;
        }

        public override object Clone()
        {
            return new Plus(this);
        }
    }

    public partial class Minus : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"-"; }
        }

        public Minus()
            :base()
        {
            Value = @"-";
        }

        public Minus(Minus old)
            :base(old)
        {
        }

        public static Minus Create()
        {
            var s = new Minus();
            s.Value = @"-";
            return s;
        }

        public override object Clone()
        {
            return new Minus(this);
        }
    }

    public partial class Mul : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"*"; }
        }

        public Mul()
            :base()
        {
            Value = @"*";
        }

        public Mul(Mul old)
            :base(old)
        {
        }

        public static Mul Create()
        {
            var s = new Mul();
            s.Value = @"*";
            return s;
        }

        public override object Clone()
        {
            return new Mul(this);
        }
    }

    public partial class Div : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"/"; }
        }

        public Div()
            :base()
        {
            Value = @"/";
        }

        public Div(Div old)
            :base(old)
        {
        }

        public static Div Create()
        {
            var s = new Div();
            s.Value = @"/";
            return s;
        }

        public override object Clone()
        {
            return new Div(this);
        }
    }

    public partial class Mod : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"%"; }
        }

        public Mod()
            :base()
        {
            Value = @"%";
        }

        public Mod(Mod old)
            :base(old)
        {
        }

        public static Mod Create()
        {
            var s = new Mod();
            s.Value = @"%";
            return s;
        }

        public override object Clone()
        {
            return new Mod(this);
        }
    }

    public partial class BitwiseNot : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"~"; }
        }

        public BitwiseNot()
            :base()
        {
            Value = @"~";
        }

        public BitwiseNot(BitwiseNot old)
            :base(old)
        {
        }

        public static BitwiseNot Create()
        {
            var s = new BitwiseNot();
            s.Value = @"~";
            return s;
        }

        public override object Clone()
        {
            return new BitwiseNot(this);
        }
    }

    public partial class BitwiseAnd : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"&"; }
        }

        public BitwiseAnd()
            :base()
        {
            Value = @"&";
        }

        public BitwiseAnd(BitwiseAnd old)
            :base(old)
        {
        }

        public static BitwiseAnd Create()
        {
            var s = new BitwiseAnd();
            s.Value = @"&";
            return s;
        }

        public override object Clone()
        {
            return new BitwiseAnd(this);
        }
    }

    public partial class BitwiseOr : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"|"; }
        }

        public BitwiseOr()
            :base()
        {
            Value = @"|";
        }

        public BitwiseOr(BitwiseOr old)
            :base(old)
        {
        }

        public static BitwiseOr Create()
        {
            var s = new BitwiseOr();
            s.Value = @"|";
            return s;
        }

        public override object Clone()
        {
            return new BitwiseOr(this);
        }
    }

    public partial class BitwiseXor : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"^"; }
        }

        public BitwiseXor()
            :base()
        {
            Value = @"^";
        }

        public BitwiseXor(BitwiseXor old)
            :base(old)
        {
        }

        public static BitwiseXor Create()
        {
            var s = new BitwiseXor();
            s.Value = @"^";
            return s;
        }

        public override object Clone()
        {
            return new BitwiseXor(this);
        }
    }

    public partial class PlusEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"+="; }
        }

        public PlusEquals()
            :base()
        {
            Value = @"+=";
        }

        public PlusEquals(PlusEquals old)
            :base(old)
        {
        }

        public static PlusEquals Create()
        {
            var s = new PlusEquals();
            s.Value = @"+=";
            return s;
        }

        public override object Clone()
        {
            return new PlusEquals(this);
        }
    }

    public partial class MinusEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"-="; }
        }

        public MinusEquals()
            :base()
        {
            Value = @"-=";
        }

        public MinusEquals(MinusEquals old)
            :base(old)
        {
        }

        public static MinusEquals Create()
        {
            var s = new MinusEquals();
            s.Value = @"-=";
            return s;
        }

        public override object Clone()
        {
            return new MinusEquals(this);
        }
    }

    public partial class MulEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"*="; }
        }

        public MulEquals()
            :base()
        {
            Value = @"*=";
        }

        public MulEquals(MulEquals old)
            :base(old)
        {
        }

        public static MulEquals Create()
        {
            var s = new MulEquals();
            s.Value = @"*=";
            return s;
        }

        public override object Clone()
        {
            return new MulEquals(this);
        }
    }

    public partial class DivEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"/="; }
        }

        public DivEquals()
            :base()
        {
            Value = @"/=";
        }

        public DivEquals(DivEquals old)
            :base(old)
        {
        }

        public static DivEquals Create()
        {
            var s = new DivEquals();
            s.Value = @"/=";
            return s;
        }

        public override object Clone()
        {
            return new DivEquals(this);
        }
    }

    public partial class ModEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"%="; }
        }

        public ModEquals()
            :base()
        {
            Value = @"%=";
        }

        public ModEquals(ModEquals old)
            :base(old)
        {
        }

        public static ModEquals Create()
        {
            var s = new ModEquals();
            s.Value = @"%=";
            return s;
        }

        public override object Clone()
        {
            return new ModEquals(this);
        }
    }

    public partial class AndEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"&="; }
        }

        public AndEquals()
            :base()
        {
            Value = @"&=";
        }

        public AndEquals(AndEquals old)
            :base(old)
        {
        }

        public static AndEquals Create()
        {
            var s = new AndEquals();
            s.Value = @"&=";
            return s;
        }

        public override object Clone()
        {
            return new AndEquals(this);
        }
    }

    public partial class XorEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"^="; }
        }

        public XorEquals()
            :base()
        {
            Value = @"^=";
        }

        public XorEquals(XorEquals old)
            :base(old)
        {
        }

        public static XorEquals Create()
        {
            var s = new XorEquals();
            s.Value = @"^=";
            return s;
        }

        public override object Clone()
        {
            return new XorEquals(this);
        }
    }

    public partial class OrEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"|="; }
        }

        public OrEquals()
            :base()
        {
            Value = @"|=";
        }

        public OrEquals(OrEquals old)
            :base(old)
        {
        }

        public static OrEquals Create()
        {
            var s = new OrEquals();
            s.Value = @"|=";
            return s;
        }

        public override object Clone()
        {
            return new OrEquals(this);
        }
    }

    public partial class Equals1 : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"="; }
        }

        public Equals1()
            :base()
        {
            Value = @"=";
        }

        public Equals1(Equals1 old)
            :base(old)
        {
        }

        public static Equals1 Create()
        {
            var s = new Equals1();
            s.Value = @"=";
            return s;
        }

        public override object Clone()
        {
            return new Equals1(this);
        }
    }

    public partial class Equals2 : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"=="; }
        }

        public Equals2()
            :base()
        {
            Value = @"==";
        }

        public Equals2(Equals2 old)
            :base(old)
        {
        }

        public static Equals2 Create()
        {
            var s = new Equals2();
            s.Value = @"==";
            return s;
        }

        public override object Clone()
        {
            return new Equals2(this);
        }
    }

    public partial class LessOrGreaterThan : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"<>"; }
        }

        public LessOrGreaterThan()
            :base()
        {
            Value = @"<>";
        }

        public LessOrGreaterThan(LessOrGreaterThan old)
            :base(old)
        {
        }

        public static LessOrGreaterThan Create()
        {
            var s = new LessOrGreaterThan();
            s.Value = @"<>";
            return s;
        }

        public override object Clone()
        {
            return new LessOrGreaterThan(this);
        }
    }

    public partial class NotEquals : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"!="; }
        }

        public NotEquals()
            :base()
        {
            Value = @"!=";
        }

        public NotEquals(NotEquals old)
            :base(old)
        {
        }

        public static NotEquals Create()
        {
            var s = new NotEquals();
            s.Value = @"!=";
            return s;
        }

        public override object Clone()
        {
            return new NotEquals(this);
        }
    }

    public partial class NotLessThan : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"!<"; }
        }

        public NotLessThan()
            :base()
        {
            Value = @"!<";
        }

        public NotLessThan(NotLessThan old)
            :base(old)
        {
        }

        public static NotLessThan Create()
        {
            var s = new NotLessThan();
            s.Value = @"!<";
            return s;
        }

        public override object Clone()
        {
            return new NotLessThan(this);
        }
    }

    public partial class NotGreaterThan : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"!>"; }
        }

        public NotGreaterThan()
            :base()
        {
            Value = @"!>";
        }

        public NotGreaterThan(NotGreaterThan old)
            :base(old)
        {
        }

        public static NotGreaterThan Create()
        {
            var s = new NotGreaterThan();
            s.Value = @"!>";
            return s;
        }

        public override object Clone()
        {
            return new NotGreaterThan(this);
        }
    }

    public partial class LessThanOrEqual : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"<="; }
        }

        public LessThanOrEqual()
            :base()
        {
            Value = @"<=";
        }

        public LessThanOrEqual(LessThanOrEqual old)
            :base(old)
        {
        }

        public static LessThanOrEqual Create()
        {
            var s = new LessThanOrEqual();
            s.Value = @"<=";
            return s;
        }

        public override object Clone()
        {
            return new LessThanOrEqual(this);
        }
    }

    public partial class GreaterThanOrEqual : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @">="; }
        }

        public GreaterThanOrEqual()
            :base()
        {
            Value = @">=";
        }

        public GreaterThanOrEqual(GreaterThanOrEqual old)
            :base(old)
        {
        }

        public static GreaterThanOrEqual Create()
        {
            var s = new GreaterThanOrEqual();
            s.Value = @">=";
            return s;
        }

        public override object Clone()
        {
            return new GreaterThanOrEqual(this);
        }
    }

    public partial class LessThan : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"<"; }
        }

        public LessThan()
            :base()
        {
            Value = @"<";
        }

        public LessThan(LessThan old)
            :base(old)
        {
        }

        public static LessThan Create()
        {
            var s = new LessThan();
            s.Value = @"<";
            return s;
        }

        public override object Clone()
        {
            return new LessThan(this);
        }
    }

    public partial class GreaterThan : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @">"; }
        }

        public GreaterThan()
            :base()
        {
            Value = @">";
        }

        public GreaterThan(GreaterThan old)
            :base(old)
        {
        }

        public static GreaterThan Create()
        {
            var s = new GreaterThan();
            s.Value = @">";
            return s;
        }

        public override object Clone()
        {
            return new GreaterThan(this);
        }
    }

    public partial class DoubleColon : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"::"; }
        }

        public DoubleColon()
            :base()
        {
            Value = @"::";
        }

        public DoubleColon(DoubleColon old)
            :base(old)
        {
        }

        public static DoubleColon Create()
        {
            var s = new DoubleColon();
            s.Value = @"::";
            return s;
        }

        public override object Clone()
        {
            return new DoubleColon(this);
        }
    }

    public partial class Dot : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"."; }
        }

        public Dot()
            :base()
        {
            Value = @".";
        }

        public Dot(Dot old)
            :base(old)
        {
        }

        public static Dot Create()
        {
            var s = new Dot();
            s.Value = @".";
            return s;
        }

        public override object Clone()
        {
            return new Dot(this);
        }
    }

    public partial class Comma : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @","; }
        }

        public Comma()
            :base()
        {
            Value = @",";
        }

        public Comma(Comma old)
            :base(old)
        {
        }

        public static Comma Create()
        {
            var s = new Comma();
            s.Value = @",";
            return s;
        }

        public override object Clone()
        {
            return new Comma(this);
        }
    }

    public partial class Semicolon : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @";"; }
        }

        public Semicolon()
            :base()
        {
            Value = @";";
        }

        public Semicolon(Semicolon old)
            :base(old)
        {
        }

        public static Semicolon Create()
        {
            var s = new Semicolon();
            s.Value = @";";
            return s;
        }

        public override object Clone()
        {
            return new Semicolon(this);
        }
    }

    public partial class Colon : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @":"; }
        }

        public Colon()
            :base()
        {
            Value = @":";
        }

        public Colon(Colon old)
            :base(old)
        {
        }

        public static Colon Create()
        {
            var s = new Colon();
            s.Value = @":";
            return s;
        }

        public override object Clone()
        {
            return new Colon(this);
        }
    }

    public partial class BracketOpen : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"("; }
        }

        public BracketOpen()
            :base()
        {
            Value = @"(";
        }

        public BracketOpen(BracketOpen old)
            :base(old)
        {
        }

        public static BracketOpen Create()
        {
            var s = new BracketOpen();
            s.Value = @"(";
            return s;
        }

        public override object Clone()
        {
            return new BracketOpen(this);
        }
    }

    public partial class BracketClose : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @")"; }
        }

        public BracketClose()
            :base()
        {
            Value = @")";
        }

        public BracketClose(BracketClose old)
            :base(old)
        {
        }

        public static BracketClose Create()
        {
            var s = new BracketClose();
            s.Value = @")";
            return s;
        }

        public override object Clone()
        {
            return new BracketClose(this);
        }
    }

    public partial class VectorOpen : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"{"; }
        }

        public VectorOpen()
            :base()
        {
            Value = @"{";
        }

        public VectorOpen(VectorOpen old)
            :base(old)
        {
        }

        public static VectorOpen Create()
        {
            var s = new VectorOpen();
            s.Value = @"{";
            return s;
        }

        public override object Clone()
        {
            return new VectorOpen(this);
        }
    }

    public partial class VectorClose : Jhu.Graywulf.Parsing.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"}"; }
        }

        public VectorClose()
            :base()
        {
            Value = @"}";
        }

        public VectorClose(VectorClose old)
            :base(old)
        {
        }

        public static VectorClose Create()
        {
            var s = new VectorClose();
            s.Value = @"}";
            return s;
        }

        public override object Clone()
        {
            return new VectorClose(this);
        }
    }


    public partial class Number : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public Number()
            :base()
        {
            Value = @"\G([0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?)";
        }

        public Number(Number old)
            :base(old)
        {
        }

        public static Number Create(string value)
        {
            var terminal = new Number();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new Number(this);
        }
    }

    public partial class HexLiteral : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G0[xX][0-9a-fA-F]+", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public HexLiteral()
            :base()
        {
            Value = @"\G0[xX][0-9a-fA-F]+";
        }

        public HexLiteral(HexLiteral old)
            :base(old)
        {
        }

        public static HexLiteral Create(string value)
        {
            var terminal = new HexLiteral();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new HexLiteral(this);
        }
    }

    public partial class StringConstant : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G('([^']|'')*')", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public StringConstant()
            :base()
        {
            Value = @"\G('([^']|'')*')";
        }

        public StringConstant(StringConstant old)
            :base(old)
        {
        }

        public static StringConstant Create(string value)
        {
            var terminal = new StringConstant();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new StringConstant(this);
        }
    }

    public partial class Identifier : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public Identifier()
            :base()
        {
            Value = @"\G([a-zA-Z_]+[0-9a-zA-Z_]*|\[[^\]]+\])";
        }

        public Identifier(Identifier old)
            :base(old)
        {
        }

        public static Identifier Create(string value)
        {
            var terminal = new Identifier();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new Identifier(this);
        }
    }

    public partial class Variable : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G(@[a-zA-Z_][0-9a-zA-Z_]*)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public Variable()
            :base()
        {
            Value = @"\G(@[a-zA-Z_][0-9a-zA-Z_]*)";
        }

        public Variable(Variable old)
            :base(old)
        {
        }

        public static Variable Create(string value)
        {
            var terminal = new Variable();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new Variable(this);
        }
    }

    public partial class Variable2 : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G(@@[a-zA-Z_][0-9a-zA-Z_]*)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public Variable2()
            :base()
        {
            Value = @"\G(@@[a-zA-Z_][0-9a-zA-Z_]*)";
        }

        public Variable2(Variable2 old)
            :base(old)
        {
        }

        public static Variable2 Create(string value)
        {
            var terminal = new Variable2();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new Variable2(this);
        }
    }

    public partial class Cursor : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G([$a-zA-Z_]+)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public Cursor()
            :base()
        {
            Value = @"\G([$a-zA-Z_]+)";
        }

        public Cursor(Cursor old)
            :base(old)
        {
        }

        public static Cursor Create(string value)
        {
            var terminal = new Cursor();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new Cursor(this);
        }
    }


    public partial class Whitespace : Jhu.Graywulf.Parsing.Whitespace, ICloneable
    {
        private static Regex pattern = new Regex(@"\G\s+", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public Whitespace()
            :base()
        {
        }

        public Whitespace(Whitespace old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Whitespace(this);
        }
    }


    public partial class SingleLineComment : Jhu.Graywulf.Parsing.Comment, ICloneable
    {
        private static Regex pattern = new Regex(@"\G--.*", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public SingleLineComment()
            :base()
        {
        }

        public SingleLineComment(SingleLineComment old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new SingleLineComment(this);
        }
    }

    public partial class MultiLineComment : Jhu.Graywulf.Parsing.Comment, ICloneable
    {
        private static Regex pattern = new Regex(@"\G(?sm)/\*.*?\*/", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public MultiLineComment()
            :base()
        {
        }

        public MultiLineComment(MultiLineComment old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new MultiLineComment(this);
        }
    }


    public partial class UnaryOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UnaryOperator()
            :base()
        {
        }

        public UnaryOperator(Jhu.Graywulf.Sql.Parsing.UnaryOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UnaryOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
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
                        r3 = r3 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Plus());
                        CommitOrRollback(r3, parser);
                        a2 = r3;
                    }

                    if (!a2)
                    {
                        Checkpoint(parser); // r4

                        bool r4 = true;
                        r4 = r4 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Minus());
                        CommitOrRollback(r4, parser);
                        a2 = r4;
                    }

                    if (!a2)
                    {
                        Checkpoint(parser); // r5

                        bool r5 = true;
                        r5 = r5 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BitwiseNot());
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

    public partial class ArithmeticOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ArithmeticOperator()
            :base()
        {
        }

        public ArithmeticOperator(Jhu.Graywulf.Sql.Parsing.ArithmeticOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ArithmeticOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
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
                        r8 = r8 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Plus());
                        CommitOrRollback(r8, parser);
                        a7 = r8;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r9

                        bool r9 = true;
                        r9 = r9 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Minus());
                        CommitOrRollback(r9, parser);
                        a7 = r9;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r10

                        bool r10 = true;
                        r10 = r10 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                        CommitOrRollback(r10, parser);
                        a7 = r10;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r11

                        bool r11 = true;
                        r11 = r11 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Div());
                        CommitOrRollback(r11, parser);
                        a7 = r11;
                    }

                    if (!a7)
                    {
                        Checkpoint(parser); // r12

                        bool r12 = true;
                        r12 = r12 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mod());
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

    public partial class BitwiseOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public BitwiseOperator()
            :base()
        {
        }

        public BitwiseOperator(Jhu.Graywulf.Sql.Parsing.BitwiseOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.BitwiseOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
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
                        r15 = r15 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BitwiseAnd());
                        CommitOrRollback(r15, parser);
                        a14 = r15;
                    }

                    if (!a14)
                    {
                        Checkpoint(parser); // r16

                        bool r16 = true;
                        r16 = r16 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BitwiseOr());
                        CommitOrRollback(r16, parser);
                        a14 = r16;
                    }

                    if (!a14)
                    {
                        Checkpoint(parser); // r17

                        bool r17 = true;
                        r17 = r17 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BitwiseXor());
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

    public partial class ComparisonOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ComparisonOperator()
            :base()
        {
        }

        public ComparisonOperator(Jhu.Graywulf.Sql.Parsing.ComparisonOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ComparisonOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
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
                        r20 = r20 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals2());
                        CommitOrRollback(r20, parser);
                        a19 = r20;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r21

                        bool r21 = true;
                        r21 = r21 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        CommitOrRollback(r21, parser);
                        a19 = r21;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r22

                        bool r22 = true;
                        r22 = r22 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LessOrGreaterThan());
                        CommitOrRollback(r22, parser);
                        a19 = r22;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r23

                        bool r23 = true;
                        r23 = r23 && Match(parser, new Jhu.Graywulf.Sql.Parsing.NotEquals());
                        CommitOrRollback(r23, parser);
                        a19 = r23;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r24

                        bool r24 = true;
                        r24 = r24 && Match(parser, new Jhu.Graywulf.Sql.Parsing.NotLessThan());
                        CommitOrRollback(r24, parser);
                        a19 = r24;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r25

                        bool r25 = true;
                        r25 = r25 && Match(parser, new Jhu.Graywulf.Sql.Parsing.NotGreaterThan());
                        CommitOrRollback(r25, parser);
                        a19 = r25;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r26

                        bool r26 = true;
                        r26 = r26 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LessThanOrEqual());
                        CommitOrRollback(r26, parser);
                        a19 = r26;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r27

                        bool r27 = true;
                        r27 = r27 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GreaterThanOrEqual());
                        CommitOrRollback(r27, parser);
                        a19 = r27;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r28

                        bool r28 = true;
                        r28 = r28 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LessThan());
                        CommitOrRollback(r28, parser);
                        a19 = r28;
                    }

                    if (!a19)
                    {
                        Checkpoint(parser); // r29

                        bool r29 = true;
                        r29 = r29 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GreaterThan());
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

    public partial class LogicalNot : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public LogicalNot()
            :base()
        {
        }

        public LogicalNot(Jhu.Graywulf.Sql.Parsing.LogicalNot old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.LogicalNot(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r30

                bool r30 = true;
                r30 = r30 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                CommitOrRollback(r30, parser);
                res = r30;
            }



            return res;
        }
    }

    public partial class LogicalOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public LogicalOperator()
            :base()
        {
        }

        public LogicalOperator(Jhu.Graywulf.Sql.Parsing.LogicalOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.LogicalOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
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
                        r33 = r33 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AND"));
                        CommitOrRollback(r33, parser);
                        a32 = r33;
                    }

                    if (!a32)
                    {
                        Checkpoint(parser); // r34

                        bool r34 = true;
                        r34 = r34 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OR"));
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

    public partial class DatasetName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DatasetName()
            :base()
        {
        }

        public DatasetName(Jhu.Graywulf.Sql.Parsing.DatasetName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DatasetName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r35

                bool r35 = true;
                r35 = r35 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r35, parser);
                res = r35;
            }



            return res;
        }
    }

    public partial class DatabaseName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DatabaseName()
            :base()
        {
        }

        public DatabaseName(Jhu.Graywulf.Sql.Parsing.DatabaseName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DatabaseName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r36

                bool r36 = true;
                r36 = r36 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r36, parser);
                res = r36;
            }



            return res;
        }
    }

    public partial class SchemaName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SchemaName()
            :base()
        {
        }

        public SchemaName(Jhu.Graywulf.Sql.Parsing.SchemaName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SchemaName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r37

                bool r37 = true;
                r37 = r37 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r37, parser);
                res = r37;
            }



            return res;
        }
    }

    public partial class TableName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableName()
            :base()
        {
        }

        public TableName(Jhu.Graywulf.Sql.Parsing.TableName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r38

                bool r38 = true;
                r38 = r38 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r38, parser);
                res = r38;
            }



            return res;
        }
    }

    public partial class ConstraintName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ConstraintName()
            :base()
        {
        }

        public ConstraintName(Jhu.Graywulf.Sql.Parsing.ConstraintName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ConstraintName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r39

                bool r39 = true;
                r39 = r39 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r39, parser);
                res = r39;
            }



            return res;
        }
    }

    public partial class IndexName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IndexName()
            :base()
        {
        }

        public IndexName(Jhu.Graywulf.Sql.Parsing.IndexName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IndexName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r40

                bool r40 = true;
                r40 = r40 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r40, parser);
                res = r40;
            }



            return res;
        }
    }

    public partial class TypeName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TypeName()
            :base()
        {
        }

        public TypeName(Jhu.Graywulf.Sql.Parsing.TypeName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TypeName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r41

                bool r41 = true;
                r41 = r41 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r41, parser);
                res = r41;
            }



            return res;
        }
    }

    public partial class DerivedTable : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DerivedTable()
            :base()
        {
        }

        public DerivedTable(Jhu.Graywulf.Sql.Parsing.DerivedTable old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DerivedTable(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r42

                bool r42 = true;
                r42 = r42 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r42, parser);
                res = r42;
            }



            return res;
        }
    }

    public partial class TableAlias : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableAlias()
            :base()
        {
        }

        public TableAlias(Jhu.Graywulf.Sql.Parsing.TableAlias old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableAlias(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r43

                bool r43 = true;
                r43 = r43 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r43, parser);
                res = r43;
            }



            return res;
        }
    }

    public partial class FunctionName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FunctionName()
            :base()
        {
        }

        public FunctionName(Jhu.Graywulf.Sql.Parsing.FunctionName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FunctionName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r44

                bool r44 = true;
                r44 = r44 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r44, parser);
                res = r44;
            }



            return res;
        }
    }

    public partial class ColumnName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnName()
            :base()
        {
        }

        public ColumnName(Jhu.Graywulf.Sql.Parsing.ColumnName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r45

                bool r45 = true;
                r45 = r45 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r45, parser);
                res = r45;
            }



            return res;
        }
    }

    public partial class ColumnAlias : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnAlias()
            :base()
        {
        }

        public ColumnAlias(Jhu.Graywulf.Sql.Parsing.ColumnAlias old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnAlias(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r46

                bool r46 = true;
                r46 = r46 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r46, parser);
                res = r46;
            }



            return res;
        }
    }

    public partial class UdtColumnName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UdtColumnName()
            :base()
        {
        }

        public UdtColumnName(Jhu.Graywulf.Sql.Parsing.UdtColumnName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UdtColumnName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r47

                bool r47 = true;
                r47 = r47 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r47, parser);
                res = r47;
            }



            return res;
        }
    }

    public partial class PropertyName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public PropertyName()
            :base()
        {
        }

        public PropertyName(Jhu.Graywulf.Sql.Parsing.PropertyName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.PropertyName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r48

                bool r48 = true;
                r48 = r48 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r48, parser);
                res = r48;
            }



            return res;
        }
    }

    public partial class SampleNumber : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SampleNumber()
            :base()
        {
        }

        public SampleNumber(Jhu.Graywulf.Sql.Parsing.SampleNumber old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SampleNumber(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r49

                bool r49 = true;
                r49 = r49 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                CommitOrRollback(r49, parser);
                res = r49;
            }



            return res;
        }
    }

    public partial class RepeatSeed : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public RepeatSeed()
            :base()
        {
        }

        public RepeatSeed(Jhu.Graywulf.Sql.Parsing.RepeatSeed old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.RepeatSeed(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r50

                bool r50 = true;
                r50 = r50 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                CommitOrRollback(r50, parser);
                res = r50;
            }



            return res;
        }
    }

    public partial class IndexValue : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IndexValue()
            :base()
        {
        }

        public IndexValue(Jhu.Graywulf.Sql.Parsing.IndexValue old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IndexValue(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r51

                bool r51 = true;
                r51 = r51 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r51, parser);
                res = r51;
            }



            return res;
        }
    }

    public partial class CommentOrWhitespace : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CommentOrWhitespace()
            :base()
        {
        }

        public CommentOrWhitespace(Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r52

                bool r52 = true;
                if (r52)
                { // alternatives a53 must
                    bool a53 = false;
                    if (!a53)
                    {
                        Checkpoint(parser); // r54

                        bool r54 = true;
                        r54 = r54 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MultiLineComment());
                        CommitOrRollback(r54, parser);
                        a53 = r54;
                    }

                    if (!a53)
                    {
                        Checkpoint(parser); // r55

                        bool r55 = true;
                        r55 = r55 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SingleLineComment());
                        CommitOrRollback(r55, parser);
                        a53 = r55;
                    }

                    if (!a53)
                    {
                        Checkpoint(parser); // r56

                        bool r56 = true;
                        r56 = r56 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Whitespace());
                        CommitOrRollback(r56, parser);
                        a53 = r56;
                    }

                    r52 &= a53;

                } // end alternatives a53

                if (r52)
                { // may a57
                    bool a57 = false;
                    {
                        Checkpoint(parser); // r58

                        bool r58 = true;
                        r58 = r58 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r58, parser);
                        a57 = r58;
                    }

                    r52 |= a57;
                } // end may a57

                CommitOrRollback(r52, parser);
                res = r52;
            }



            return res;
        }
    }

    public partial class Expression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Expression()
            :base()
        {
        }

        public Expression(Jhu.Graywulf.Sql.Parsing.Expression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Expression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r59

                bool r59 = true;
                if (r59)
                { // may a60
                    bool a60 = false;
                    {
                        Checkpoint(parser); // r61

                        bool r61 = true;
                        r61 = r61 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UnaryOperator());
                        CommitOrRollback(r61, parser);
                        a60 = r61;
                    }

                    r59 |= a60;
                } // end may a60

                if (r59)
                { // may a62
                    bool a62 = false;
                    {
                        Checkpoint(parser); // r63

                        bool r63 = true;
                        r63 = r63 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r63, parser);
                        a62 = r63;
                    }

                    r59 |= a62;
                } // end may a62

                if (r59)
                { // alternatives a64 must
                    bool a64 = false;
                    if (!a64)
                    {
                        Checkpoint(parser); // r65

                        bool r65 = true;
                        r65 = r65 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r65, parser);
                        a64 = r65;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r66

                        bool r66 = true;
                        r66 = r66 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ExpressionBrackets());
                        CommitOrRollback(r66, parser);
                        a64 = r66;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r67

                        bool r67 = true;
                        r67 = r67 && Match(parser, new Jhu.Graywulf.Sql.Parsing.RankingFunctionCall());
                        CommitOrRollback(r67, parser);
                        a64 = r67;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r68

                        bool r68 = true;
                        r68 = r68 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdtFunctionCall());
                        CommitOrRollback(r68, parser);
                        a64 = r68;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r69

                        bool r69 = true;
                        r69 = r69 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionCall());
                        CommitOrRollback(r69, parser);
                        a64 = r69;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r70

                        bool r70 = true;
                        r70 = r70 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Null());
                        CommitOrRollback(r70, parser);
                        a64 = r70;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r71

                        bool r71 = true;
                        r71 = r71 && Match(parser, new Jhu.Graywulf.Sql.Parsing.HexLiteral());
                        CommitOrRollback(r71, parser);
                        a64 = r71;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r72

                        bool r72 = true;
                        r72 = r72 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r72, parser);
                        a64 = r72;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r73

                        bool r73 = true;
                        r73 = r73 && Match(parser, new Jhu.Graywulf.Sql.Parsing.AnyVariable());
                        CommitOrRollback(r73, parser);
                        a64 = r73;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r74

                        bool r74 = true;
                        r74 = r74 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StringConstant());
                        CommitOrRollback(r74, parser);
                        a64 = r74;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r75

                        bool r75 = true;
                        r75 = r75 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseExpression());
                        CommitOrRollback(r75, parser);
                        a64 = r75;
                    }

                    if (!a64)
                    {
                        Checkpoint(parser); // r76

                        bool r76 = true;
                        r76 = r76 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseExpression());
                        CommitOrRollback(r76, parser);
                        a64 = r76;
                    }

                    r59 &= a64;

                } // end alternatives a64

                if (r59)
                { // may a77
                    bool a77 = false;
                    {
                        Checkpoint(parser); // r78

                        bool r78 = true;
                        if (r78)
                        { // may a79
                            bool a79 = false;
                            {
                                Checkpoint(parser); // r80

                                bool r80 = true;
                                r80 = r80 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r80, parser);
                                a79 = r80;
                            }

                            r78 |= a79;
                        } // end may a79

                        if (r78)
                        { // alternatives a81 must
                            bool a81 = false;
                            if (!a81)
                            {
                                Checkpoint(parser); // r82

                                bool r82 = true;
                                r82 = r82 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArithmeticOperator());
                                CommitOrRollback(r82, parser);
                                a81 = r82;
                            }

                            if (!a81)
                            {
                                Checkpoint(parser); // r83

                                bool r83 = true;
                                r83 = r83 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BitwiseOperator());
                                CommitOrRollback(r83, parser);
                                a81 = r83;
                            }

                            r78 &= a81;

                        } // end alternatives a81

                        if (r78)
                        { // may a84
                            bool a84 = false;
                            {
                                Checkpoint(parser); // r85

                                bool r85 = true;
                                r85 = r85 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r85, parser);
                                a84 = r85;
                            }

                            r78 |= a84;
                        } // end may a84

                        r78 = r78 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r78, parser);
                        a77 = r78;
                    }

                    r59 |= a77;
                } // end may a77

                CommitOrRollback(r59, parser);
                res = r59;
            }



            return res;
        }
    }

    public partial class ExpressionBrackets : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ExpressionBrackets()
            :base()
        {
        }

        public ExpressionBrackets(Jhu.Graywulf.Sql.Parsing.ExpressionBrackets old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ExpressionBrackets(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r86

                bool r86 = true;
                r86 = r86 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r86)
                { // may a87
                    bool a87 = false;
                    {
                        Checkpoint(parser); // r88

                        bool r88 = true;
                        r88 = r88 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r88, parser);
                        a87 = r88;
                    }

                    r86 |= a87;
                } // end may a87

                r86 = r86 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r86)
                { // may a89
                    bool a89 = false;
                    {
                        Checkpoint(parser); // r90

                        bool r90 = true;
                        r90 = r90 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r90, parser);
                        a89 = r90;
                    }

                    r86 |= a89;
                } // end may a89

                r86 = r86 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r86, parser);
                res = r86;
            }



            return res;
        }
    }

    public partial class Null : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Null()
            :base()
        {
        }

        public Null(Jhu.Graywulf.Sql.Parsing.Null old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Null(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r91

                bool r91 = true;
                r91 = r91 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                CommitOrRollback(r91, parser);
                res = r91;
            }



            return res;
        }
    }

    public partial class AnyVariable : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public AnyVariable()
            :base()
        {
        }

        public AnyVariable(Jhu.Graywulf.Sql.Parsing.AnyVariable old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.AnyVariable(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r92

                bool r92 = true;
                if (r92)
                { // alternatives a93 must
                    bool a93 = false;
                    if (!a93)
                    {
                        Checkpoint(parser); // r94

                        bool r94 = true;
                        r94 = r94 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentifier());
                        CommitOrRollback(r94, parser);
                        a93 = r94;
                    }

                    if (!a93)
                    {
                        Checkpoint(parser); // r95

                        bool r95 = true;
                        r95 = r95 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SystemVariable());
                        CommitOrRollback(r95, parser);
                        a93 = r95;
                    }

                    if (!a93)
                    {
                        Checkpoint(parser); // r96

                        bool r96 = true;
                        r96 = r96 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r96, parser);
                        a93 = r96;
                    }

                    r92 &= a93;

                } // end alternatives a93

                CommitOrRollback(r92, parser);
                res = r92;
            }



            return res;
        }
    }

    public partial class UserVariable : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UserVariable()
            :base()
        {
        }

        public UserVariable(Jhu.Graywulf.Sql.Parsing.UserVariable old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UserVariable(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r97

                bool r97 = true;
                r97 = r97 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                CommitOrRollback(r97, parser);
                res = r97;
            }



            return res;
        }
    }

    public partial class SystemVariable : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SystemVariable()
            :base()
        {
        }

        public SystemVariable(Jhu.Graywulf.Sql.Parsing.SystemVariable old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SystemVariable(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r98

                bool r98 = true;
                r98 = r98 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable2());
                CommitOrRollback(r98, parser);
                res = r98;
            }



            return res;
        }
    }

    public partial class BooleanExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public BooleanExpression()
            :base()
        {
        }

        public BooleanExpression(Jhu.Graywulf.Sql.Parsing.BooleanExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.BooleanExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r99

                bool r99 = true;
                if (r99)
                { // may a100
                    bool a100 = false;
                    {
                        Checkpoint(parser); // r101

                        bool r101 = true;
                        r101 = r101 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LogicalNot());
                        CommitOrRollback(r101, parser);
                        a100 = r101;
                    }

                    r99 |= a100;
                } // end may a100

                if (r99)
                { // may a102
                    bool a102 = false;
                    {
                        Checkpoint(parser); // r103

                        bool r103 = true;
                        r103 = r103 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r103, parser);
                        a102 = r103;
                    }

                    r99 |= a102;
                } // end may a102

                if (r99)
                { // alternatives a104 must
                    bool a104 = false;
                    if (!a104)
                    {
                        Checkpoint(parser); // r105

                        bool r105 = true;
                        r105 = r105 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Predicate());
                        CommitOrRollback(r105, parser);
                        a104 = r105;
                    }

                    if (!a104)
                    {
                        Checkpoint(parser); // r106

                        bool r106 = true;
                        r106 = r106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpressionBrackets());
                        CommitOrRollback(r106, parser);
                        a104 = r106;
                    }

                    r99 &= a104;

                } // end alternatives a104

                if (r99)
                { // may a107
                    bool a107 = false;
                    {
                        Checkpoint(parser); // r108

                        bool r108 = true;
                        if (r108)
                        { // may a109
                            bool a109 = false;
                            {
                                Checkpoint(parser); // r110

                                bool r110 = true;
                                r110 = r110 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r110, parser);
                                a109 = r110;
                            }

                            r108 |= a109;
                        } // end may a109

                        r108 = r108 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LogicalOperator());
                        if (r108)
                        { // may a111
                            bool a111 = false;
                            {
                                Checkpoint(parser); // r112

                                bool r112 = true;
                                r112 = r112 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r112, parser);
                                a111 = r112;
                            }

                            r108 |= a111;
                        } // end may a111

                        r108 = r108 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                        CommitOrRollback(r108, parser);
                        a107 = r108;
                    }

                    r99 |= a107;
                } // end may a107

                CommitOrRollback(r99, parser);
                res = r99;
            }



            return res;
        }
    }

    public partial class BooleanExpressionBrackets : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public BooleanExpressionBrackets()
            :base()
        {
        }

        public BooleanExpressionBrackets(Jhu.Graywulf.Sql.Parsing.BooleanExpressionBrackets old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.BooleanExpressionBrackets(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r113

                bool r113 = true;
                r113 = r113 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r113)
                { // may a114
                    bool a114 = false;
                    {
                        Checkpoint(parser); // r115

                        bool r115 = true;
                        r115 = r115 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r115, parser);
                        a114 = r115;
                    }

                    r113 |= a114;
                } // end may a114

                r113 = r113 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r113)
                { // may a116
                    bool a116 = false;
                    {
                        Checkpoint(parser); // r117

                        bool r117 = true;
                        r117 = r117 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r117, parser);
                        a116 = r117;
                    }

                    r113 |= a116;
                } // end may a116

                r113 = r113 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r113, parser);
                res = r113;
            }



            return res;
        }
    }

    public partial class Predicate : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Predicate()
            :base()
        {
        }

        public Predicate(Jhu.Graywulf.Sql.Parsing.Predicate old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Predicate(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r118

                bool r118 = true;
                if (r118)
                { // alternatives a119 must
                    bool a119 = false;
                    if (!a119)
                    {
                        Checkpoint(parser); // r120

                        bool r120 = true;
                        r120 = r120 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r120)
                        { // may a121
                            bool a121 = false;
                            {
                                Checkpoint(parser); // r122

                                bool r122 = true;
                                r122 = r122 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r122, parser);
                                a121 = r122;
                            }

                            r120 |= a121;
                        } // end may a121

                        r120 = r120 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ComparisonOperator());
                        if (r120)
                        { // may a123
                            bool a123 = false;
                            {
                                Checkpoint(parser); // r124

                                bool r124 = true;
                                r124 = r124 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r124, parser);
                                a123 = r124;
                            }

                            r120 |= a123;
                        } // end may a123

                        r120 = r120 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r120, parser);
                        a119 = r120;
                    }

                    if (!a119)
                    {
                        Checkpoint(parser); // r125

                        bool r125 = true;
                        r125 = r125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r125)
                        { // may a126
                            bool a126 = false;
                            {
                                Checkpoint(parser); // r127

                                bool r127 = true;
                                r127 = r127 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r127, parser);
                                a126 = r127;
                            }

                            r125 |= a126;
                        } // end may a126

                        if (r125)
                        { // may a128
                            bool a128 = false;
                            {
                                Checkpoint(parser); // r129

                                bool r129 = true;
                                r129 = r129 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r129, parser);
                                a128 = r129;
                            }

                            r125 |= a128;
                        } // end may a128

                        r125 = r125 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LIKE"));
                        if (r125)
                        { // may a130
                            bool a130 = false;
                            {
                                Checkpoint(parser); // r131

                                bool r131 = true;
                                r131 = r131 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r131, parser);
                                a130 = r131;
                            }

                            r125 |= a130;
                        } // end may a130

                        r125 = r125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r125)
                        { // may a132
                            bool a132 = false;
                            {
                                Checkpoint(parser); // r133

                                bool r133 = true;
                                if (r133)
                                { // may a134
                                    bool a134 = false;
                                    {
                                        Checkpoint(parser); // r135

                                        bool r135 = true;
                                        r135 = r135 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r135, parser);
                                        a134 = r135;
                                    }

                                    r133 |= a134;
                                } // end may a134

                                r133 = r133 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ESCAPE"));
                                if (r133)
                                { // may a136
                                    bool a136 = false;
                                    {
                                        Checkpoint(parser); // r137

                                        bool r137 = true;
                                        r137 = r137 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r137, parser);
                                        a136 = r137;
                                    }

                                    r133 |= a136;
                                } // end may a136

                                r133 = r133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                                CommitOrRollback(r133, parser);
                                a132 = r133;
                            }

                            r125 |= a132;
                        } // end may a132

                        CommitOrRollback(r125, parser);
                        a119 = r125;
                    }

                    if (!a119)
                    {
                        Checkpoint(parser); // r138

                        bool r138 = true;
                        r138 = r138 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r138)
                        { // may a139
                            bool a139 = false;
                            {
                                Checkpoint(parser); // r140

                                bool r140 = true;
                                if (r140)
                                { // may a141
                                    bool a141 = false;
                                    {
                                        Checkpoint(parser); // r142

                                        bool r142 = true;
                                        r142 = r142 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r142, parser);
                                        a141 = r142;
                                    }

                                    r140 |= a141;
                                } // end may a141

                                r140 = r140 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r140, parser);
                                a139 = r140;
                            }

                            r138 |= a139;
                        } // end may a139

                        if (r138)
                        { // may a143
                            bool a143 = false;
                            {
                                Checkpoint(parser); // r144

                                bool r144 = true;
                                r144 = r144 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r144, parser);
                                a143 = r144;
                            }

                            r138 |= a143;
                        } // end may a143

                        r138 = r138 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BETWEEN"));
                        if (r138)
                        { // may a145
                            bool a145 = false;
                            {
                                Checkpoint(parser); // r146

                                bool r146 = true;
                                r146 = r146 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r146, parser);
                                a145 = r146;
                            }

                            r138 |= a145;
                        } // end may a145

                        r138 = r138 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r138)
                        { // may a147
                            bool a147 = false;
                            {
                                Checkpoint(parser); // r148

                                bool r148 = true;
                                r148 = r148 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r148, parser);
                                a147 = r148;
                            }

                            r138 |= a147;
                        } // end may a147

                        r138 = r138 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AND"));
                        if (r138)
                        { // may a149
                            bool a149 = false;
                            {
                                Checkpoint(parser); // r150

                                bool r150 = true;
                                r150 = r150 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r150, parser);
                                a149 = r150;
                            }

                            r138 |= a149;
                        } // end may a149

                        r138 = r138 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r138, parser);
                        a119 = r138;
                    }

                    if (!a119)
                    {
                        Checkpoint(parser); // r151

                        bool r151 = true;
                        r151 = r151 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r151)
                        { // may a152
                            bool a152 = false;
                            {
                                Checkpoint(parser); // r153

                                bool r153 = true;
                                r153 = r153 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r153, parser);
                                a152 = r153;
                            }

                            r151 |= a152;
                        } // end may a152

                        r151 = r151 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IS"));
                        if (r151)
                        { // may a154
                            bool a154 = false;
                            {
                                Checkpoint(parser); // r155

                                bool r155 = true;
                                r155 = r155 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r155 = r155 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r155, parser);
                                a154 = r155;
                            }

                            r151 |= a154;
                        } // end may a154

                        r151 = r151 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r151 = r151 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                        CommitOrRollback(r151, parser);
                        a119 = r151;
                    }

                    if (!a119)
                    {
                        Checkpoint(parser); // r156

                        bool r156 = true;
                        r156 = r156 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r156)
                        { // may a157
                            bool a157 = false;
                            {
                                Checkpoint(parser); // r158

                                bool r158 = true;
                                r158 = r158 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r158, parser);
                                a157 = r158;
                            }

                            r156 |= a157;
                        } // end may a157

                        if (r156)
                        { // may a159
                            bool a159 = false;
                            {
                                Checkpoint(parser); // r160

                                bool r160 = true;
                                r160 = r160 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r160, parser);
                                a159 = r160;
                            }

                            r156 |= a159;
                        } // end may a159

                        r156 = r156 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IN"));
                        if (r156)
                        { // may a161
                            bool a161 = false;
                            {
                                Checkpoint(parser); // r162

                                bool r162 = true;
                                r162 = r162 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r162, parser);
                                a161 = r162;
                            }

                            r156 |= a161;
                        } // end may a161

                        if (r156)
                        { // alternatives a163 must
                            bool a163 = false;
                            if (!a163)
                            {
                                Checkpoint(parser); // r164

                                bool r164 = true;
                                r164 = r164 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                                CommitOrRollback(r164, parser);
                                a163 = r164;
                            }

                            if (!a163)
                            {
                                Checkpoint(parser); // r165

                                bool r165 = true;
                                r165 = r165 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                                if (r165)
                                { // may a166
                                    bool a166 = false;
                                    {
                                        Checkpoint(parser); // r167

                                        bool r167 = true;
                                        r167 = r167 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r167, parser);
                                        a166 = r167;
                                    }

                                    r165 |= a166;
                                } // end may a166

                                r165 = r165 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                                if (r165)
                                { // may a168
                                    bool a168 = false;
                                    {
                                        Checkpoint(parser); // r169

                                        bool r169 = true;
                                        r169 = r169 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r169, parser);
                                        a168 = r169;
                                    }

                                    r165 |= a168;
                                } // end may a168

                                r165 = r165 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                                CommitOrRollback(r165, parser);
                                a163 = r165;
                            }

                            r156 &= a163;

                        } // end alternatives a163

                        CommitOrRollback(r156, parser);
                        a119 = r156;
                    }

                    if (!a119)
                    {
                        Checkpoint(parser); // r170

                        bool r170 = true;
                        r170 = r170 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r170)
                        { // may a171
                            bool a171 = false;
                            {
                                Checkpoint(parser); // r172

                                bool r172 = true;
                                r172 = r172 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r172, parser);
                                a171 = r172;
                            }

                            r170 |= a171;
                        } // end may a171

                        r170 = r170 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ComparisonOperator());
                        if (r170)
                        { // may a173
                            bool a173 = false;
                            {
                                Checkpoint(parser); // r174

                                bool r174 = true;
                                r174 = r174 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r174, parser);
                                a173 = r174;
                            }

                            r170 |= a173;
                        } // end may a173

                        if (r170)
                        { // alternatives a175 must
                            bool a175 = false;
                            if (!a175)
                            {
                                Checkpoint(parser); // r176

                                bool r176 = true;
                                r176 = r176 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r176, parser);
                                a175 = r176;
                            }

                            if (!a175)
                            {
                                Checkpoint(parser); // r177

                                bool r177 = true;
                                r177 = r177 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SOME"));
                                CommitOrRollback(r177, parser);
                                a175 = r177;
                            }

                            if (!a175)
                            {
                                Checkpoint(parser); // r178

                                bool r178 = true;
                                r178 = r178 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ANY"));
                                CommitOrRollback(r178, parser);
                                a175 = r178;
                            }

                            r170 &= a175;

                        } // end alternatives a175

                        if (r170)
                        { // may a179
                            bool a179 = false;
                            {
                                Checkpoint(parser); // r180

                                bool r180 = true;
                                r180 = r180 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r180, parser);
                                a179 = r180;
                            }

                            r170 |= a179;
                        } // end may a179

                        r170 = r170 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r170, parser);
                        a119 = r170;
                    }

                    if (!a119)
                    {
                        Checkpoint(parser); // r181

                        bool r181 = true;
                        r181 = r181 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"EXISTS"));
                        if (r181)
                        { // may a182
                            bool a182 = false;
                            {
                                Checkpoint(parser); // r183

                                bool r183 = true;
                                r183 = r183 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r183, parser);
                                a182 = r183;
                            }

                            r181 |= a182;
                        } // end may a182

                        r181 = r181 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r181, parser);
                        a119 = r181;
                    }

                    r118 &= a119;

                } // end alternatives a119

                CommitOrRollback(r118, parser);
                res = r118;
            }



            return res;
        }
    }

    public partial class SimpleCaseExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SimpleCaseExpression()
            :base()
        {
        }

        public SimpleCaseExpression(Jhu.Graywulf.Sql.Parsing.SimpleCaseExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SimpleCaseExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r184

                bool r184 = true;
                r184 = r184 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CASE"));
                if (r184)
                { // may a185
                    bool a185 = false;
                    {
                        Checkpoint(parser); // r186

                        bool r186 = true;
                        r186 = r186 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r186, parser);
                        a185 = r186;
                    }

                    r184 |= a185;
                } // end may a185

                r184 = r184 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r184)
                { // may a187
                    bool a187 = false;
                    {
                        Checkpoint(parser); // r188

                        bool r188 = true;
                        r188 = r188 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r188, parser);
                        a187 = r188;
                    }

                    r184 |= a187;
                } // end may a187

                r184 = r184 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList());
                if (r184)
                { // may a189
                    bool a189 = false;
                    {
                        Checkpoint(parser); // r190

                        bool r190 = true;
                        r190 = r190 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CaseElse());
                        CommitOrRollback(r190, parser);
                        a189 = r190;
                    }

                    r184 |= a189;
                } // end may a189

                r184 = r184 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r184, parser);
                res = r184;
            }



            return res;
        }
    }

    public partial class SimpleCaseWhenList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SimpleCaseWhenList()
            :base()
        {
        }

        public SimpleCaseWhenList(Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r191

                bool r191 = true;
                r191 = r191 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhen());
                if (r191)
                { // may a192
                    bool a192 = false;
                    {
                        Checkpoint(parser); // r193

                        bool r193 = true;
                        r193 = r193 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList());
                        CommitOrRollback(r193, parser);
                        a192 = r193;
                    }

                    r191 |= a192;
                } // end may a192

                CommitOrRollback(r191, parser);
                res = r191;
            }



            return res;
        }
    }

    public partial class SimpleCaseWhen : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SimpleCaseWhen()
            :base()
        {
        }

        public SimpleCaseWhen(Jhu.Graywulf.Sql.Parsing.SimpleCaseWhen old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhen(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r194

                bool r194 = true;
                r194 = r194 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHEN"));
                r194 = r194 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                r194 = r194 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THEN"));
                r194 = r194 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r194, parser);
                res = r194;
            }



            return res;
        }
    }

    public partial class SearchedCaseExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SearchedCaseExpression()
            :base()
        {
        }

        public SearchedCaseExpression(Jhu.Graywulf.Sql.Parsing.SearchedCaseExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SearchedCaseExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r195

                bool r195 = true;
                r195 = r195 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CASE"));
                r195 = r195 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList());
                if (r195)
                { // may a196
                    bool a196 = false;
                    {
                        Checkpoint(parser); // r197

                        bool r197 = true;
                        r197 = r197 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CaseElse());
                        CommitOrRollback(r197, parser);
                        a196 = r197;
                    }

                    r195 |= a196;
                } // end may a196

                r195 = r195 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r195, parser);
                res = r195;
            }



            return res;
        }
    }

    public partial class SearchedCaseWhenList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SearchedCaseWhenList()
            :base()
        {
        }

        public SearchedCaseWhenList(Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r198

                bool r198 = true;
                r198 = r198 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhen());
                if (r198)
                { // may a199
                    bool a199 = false;
                    {
                        Checkpoint(parser); // r200

                        bool r200 = true;
                        r200 = r200 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList());
                        CommitOrRollback(r200, parser);
                        a199 = r200;
                    }

                    r198 |= a199;
                } // end may a199

                CommitOrRollback(r198, parser);
                res = r198;
            }



            return res;
        }
    }

    public partial class SearchedCaseWhen : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SearchedCaseWhen()
            :base()
        {
        }

        public SearchedCaseWhen(Jhu.Graywulf.Sql.Parsing.SearchedCaseWhen old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhen(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r201

                bool r201 = true;
                r201 = r201 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHEN"));
                r201 = r201 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THEN"));
                r201 = r201 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r201, parser);
                res = r201;
            }



            return res;
        }
    }

    public partial class CaseElse : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CaseElse()
            :base()
        {
        }

        public CaseElse(Jhu.Graywulf.Sql.Parsing.CaseElse old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CaseElse(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r202

                bool r202 = true;
                r202 = r202 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ELSE"));
                r202 = r202 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r202, parser);
                res = r202;
            }



            return res;
        }
    }

    public partial class TableOrViewName : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableOrViewName()
            :base()
        {
        }

        public TableOrViewName(Jhu.Graywulf.Sql.Parsing.TableOrViewName old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableOrViewName(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r203

                bool r203 = true;
                if (r203)
                { // may a204
                    bool a204 = false;
                    {
                        Checkpoint(parser); // r205

                        bool r205 = true;
                        r205 = r205 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                        if (r205)
                        { // may a206
                            bool a206 = false;
                            {
                                Checkpoint(parser); // r207

                                bool r207 = true;
                                r207 = r207 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r207, parser);
                                a206 = r207;
                            }

                            r205 |= a206;
                        } // end may a206

                        r205 = r205 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                        if (r205)
                        { // may a208
                            bool a208 = false;
                            {
                                Checkpoint(parser); // r209

                                bool r209 = true;
                                r209 = r209 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r209, parser);
                                a208 = r209;
                            }

                            r205 |= a208;
                        } // end may a208

                        CommitOrRollback(r205, parser);
                        a204 = r205;
                    }

                    r203 |= a204;
                } // end may a204

                if (r203)
                { // alternatives a210 must
                    bool a210 = false;
                    if (!a210)
                    {
                        Checkpoint(parser); // r211

                        bool r211 = true;
                        r211 = r211 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                        if (r211)
                        { // may a212
                            bool a212 = false;
                            {
                                Checkpoint(parser); // r213

                                bool r213 = true;
                                r213 = r213 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r213, parser);
                                a212 = r213;
                            }

                            r211 |= a212;
                        } // end may a212

                        r211 = r211 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r211)
                        { // may a214
                            bool a214 = false;
                            {
                                Checkpoint(parser); // r215

                                bool r215 = true;
                                if (r215)
                                { // may a216
                                    bool a216 = false;
                                    {
                                        Checkpoint(parser); // r217

                                        bool r217 = true;
                                        r217 = r217 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r217, parser);
                                        a216 = r217;
                                    }

                                    r215 |= a216;
                                } // end may a216

                                r215 = r215 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                CommitOrRollback(r215, parser);
                                a214 = r215;
                            }

                            r211 |= a214;
                        } // end may a214

                        if (r211)
                        { // may a218
                            bool a218 = false;
                            {
                                Checkpoint(parser); // r219

                                bool r219 = true;
                                r219 = r219 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r219, parser);
                                a218 = r219;
                            }

                            r211 |= a218;
                        } // end may a218

                        r211 = r211 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r211)
                        { // may a220
                            bool a220 = false;
                            {
                                Checkpoint(parser); // r221

                                bool r221 = true;
                                r221 = r221 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r221, parser);
                                a220 = r221;
                            }

                            r211 |= a220;
                        } // end may a220

                        r211 = r211 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r211, parser);
                        a210 = r211;
                    }

                    if (!a210)
                    {
                        Checkpoint(parser); // r222

                        bool r222 = true;
                        r222 = r222 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                        if (r222)
                        { // may a223
                            bool a223 = false;
                            {
                                Checkpoint(parser); // r224

                                bool r224 = true;
                                r224 = r224 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r224, parser);
                                a223 = r224;
                            }

                            r222 |= a223;
                        } // end may a223

                        r222 = r222 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r222)
                        { // may a225
                            bool a225 = false;
                            {
                                Checkpoint(parser); // r226

                                bool r226 = true;
                                r226 = r226 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r226, parser);
                                a225 = r226;
                            }

                            r222 |= a225;
                        } // end may a225

                        r222 = r222 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r222, parser);
                        a210 = r222;
                    }

                    if (!a210)
                    {
                        Checkpoint(parser); // r227

                        bool r227 = true;
                        r227 = r227 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r227, parser);
                        a210 = r227;
                    }

                    r203 &= a210;

                } // end alternatives a210

                CommitOrRollback(r203, parser);
                res = r203;
            }



            return res;
        }
    }

    public partial class ColumnIdentifier : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnIdentifier()
            :base()
        {
        }

        public ColumnIdentifier(Jhu.Graywulf.Sql.Parsing.ColumnIdentifier old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnIdentifier(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r228

                bool r228 = true;
                if (r228)
                { // alternatives a229 must
                    bool a229 = false;
                    if (!a229)
                    {
                        Checkpoint(parser); // r230

                        bool r230 = true;
                        if (r230)
                        { // may a231
                            bool a231 = false;
                            {
                                Checkpoint(parser); // r232

                                bool r232 = true;
                                r232 = r232 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                                if (r232)
                                { // may a233
                                    bool a233 = false;
                                    {
                                        Checkpoint(parser); // r234

                                        bool r234 = true;
                                        r234 = r234 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r234, parser);
                                        a233 = r234;
                                    }

                                    r232 |= a233;
                                } // end may a233

                                r232 = r232 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                                if (r232)
                                { // may a235
                                    bool a235 = false;
                                    {
                                        Checkpoint(parser); // r236

                                        bool r236 = true;
                                        r236 = r236 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r236, parser);
                                        a235 = r236;
                                    }

                                    r232 |= a235;
                                } // end may a235

                                CommitOrRollback(r232, parser);
                                a231 = r232;
                            }

                            r230 |= a231;
                        } // end may a231

                        if (r230)
                        { // alternatives a237 must
                            bool a237 = false;
                            if (!a237)
                            {
                                Checkpoint(parser); // r238

                                bool r238 = true;
                                r238 = r238 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                                if (r238)
                                { // may a239
                                    bool a239 = false;
                                    {
                                        Checkpoint(parser); // r240

                                        bool r240 = true;
                                        r240 = r240 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r240, parser);
                                        a239 = r240;
                                    }

                                    r238 |= a239;
                                } // end may a239

                                r238 = r238 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r238)
                                { // may a241
                                    bool a241 = false;
                                    {
                                        Checkpoint(parser); // r242

                                        bool r242 = true;
                                        r242 = r242 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r242, parser);
                                        a241 = r242;
                                    }

                                    r238 |= a241;
                                } // end may a241

                                if (r238)
                                { // may a243
                                    bool a243 = false;
                                    {
                                        Checkpoint(parser); // r244

                                        bool r244 = true;
                                        r244 = r244 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                        if (r244)
                                        { // may a245
                                            bool a245 = false;
                                            {
                                                Checkpoint(parser); // r246

                                                bool r246 = true;
                                                r246 = r246 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                                CommitOrRollback(r246, parser);
                                                a245 = r246;
                                            }

                                            r244 |= a245;
                                        } // end may a245

                                        CommitOrRollback(r244, parser);
                                        a243 = r244;
                                    }

                                    r238 |= a243;
                                } // end may a243

                                r238 = r238 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r238)
                                { // may a247
                                    bool a247 = false;
                                    {
                                        Checkpoint(parser); // r248

                                        bool r248 = true;
                                        r248 = r248 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r248, parser);
                                        a247 = r248;
                                    }

                                    r238 |= a247;
                                } // end may a247

                                r238 = r238 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r238)
                                { // may a249
                                    bool a249 = false;
                                    {
                                        Checkpoint(parser); // r250

                                        bool r250 = true;
                                        r250 = r250 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r250, parser);
                                        a249 = r250;
                                    }

                                    r238 |= a249;
                                } // end may a249

                                r238 = r238 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r238)
                                { // may a251
                                    bool a251 = false;
                                    {
                                        Checkpoint(parser); // r252

                                        bool r252 = true;
                                        r252 = r252 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r252, parser);
                                        a251 = r252;
                                    }

                                    r238 |= a251;
                                } // end may a251

                                if (r238)
                                { // alternatives a253 must
                                    bool a253 = false;
                                    if (!a253)
                                    {
                                        Checkpoint(parser); // r254

                                        bool r254 = true;
                                        r254 = r254 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r254, parser);
                                        a253 = r254;
                                    }

                                    if (!a253)
                                    {
                                        Checkpoint(parser); // r255

                                        bool r255 = true;
                                        r255 = r255 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r255, parser);
                                        a253 = r255;
                                    }

                                    r238 &= a253;

                                } // end alternatives a253

                                CommitOrRollback(r238, parser);
                                a237 = r238;
                            }

                            if (!a237)
                            {
                                Checkpoint(parser); // r256

                                bool r256 = true;
                                r256 = r256 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                if (r256)
                                { // may a257
                                    bool a257 = false;
                                    {
                                        Checkpoint(parser); // r258

                                        bool r258 = true;
                                        r258 = r258 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r258, parser);
                                        a257 = r258;
                                    }

                                    r256 |= a257;
                                } // end may a257

                                r256 = r256 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r256)
                                { // may a259
                                    bool a259 = false;
                                    {
                                        Checkpoint(parser); // r260

                                        bool r260 = true;
                                        r260 = r260 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r260, parser);
                                        a259 = r260;
                                    }

                                    r256 |= a259;
                                } // end may a259

                                r256 = r256 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r256)
                                { // may a261
                                    bool a261 = false;
                                    {
                                        Checkpoint(parser); // r262

                                        bool r262 = true;
                                        r262 = r262 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r262, parser);
                                        a261 = r262;
                                    }

                                    r256 |= a261;
                                } // end may a261

                                r256 = r256 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r256)
                                { // may a263
                                    bool a263 = false;
                                    {
                                        Checkpoint(parser); // r264

                                        bool r264 = true;
                                        r264 = r264 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r264, parser);
                                        a263 = r264;
                                    }

                                    r256 |= a263;
                                } // end may a263

                                if (r256)
                                { // alternatives a265 must
                                    bool a265 = false;
                                    if (!a265)
                                    {
                                        Checkpoint(parser); // r266

                                        bool r266 = true;
                                        r266 = r266 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r266, parser);
                                        a265 = r266;
                                    }

                                    if (!a265)
                                    {
                                        Checkpoint(parser); // r267

                                        bool r267 = true;
                                        r267 = r267 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r267, parser);
                                        a265 = r267;
                                    }

                                    r256 &= a265;

                                } // end alternatives a265

                                CommitOrRollback(r256, parser);
                                a237 = r256;
                            }

                            if (!a237)
                            {
                                Checkpoint(parser); // r268

                                bool r268 = true;
                                r268 = r268 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r268)
                                { // may a269
                                    bool a269 = false;
                                    {
                                        Checkpoint(parser); // r270

                                        bool r270 = true;
                                        r270 = r270 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r270, parser);
                                        a269 = r270;
                                    }

                                    r268 |= a269;
                                } // end may a269

                                r268 = r268 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r268)
                                { // may a271
                                    bool a271 = false;
                                    {
                                        Checkpoint(parser); // r272

                                        bool r272 = true;
                                        r272 = r272 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r272, parser);
                                        a271 = r272;
                                    }

                                    r268 |= a271;
                                } // end may a271

                                if (r268)
                                { // alternatives a273 must
                                    bool a273 = false;
                                    if (!a273)
                                    {
                                        Checkpoint(parser); // r274

                                        bool r274 = true;
                                        r274 = r274 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r274, parser);
                                        a273 = r274;
                                    }

                                    if (!a273)
                                    {
                                        Checkpoint(parser); // r275

                                        bool r275 = true;
                                        r275 = r275 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r275, parser);
                                        a273 = r275;
                                    }

                                    r268 &= a273;

                                } // end alternatives a273

                                CommitOrRollback(r268, parser);
                                a237 = r268;
                            }

                            r230 &= a237;

                        } // end alternatives a237

                        CommitOrRollback(r230, parser);
                        a229 = r230;
                    }

                    if (!a229)
                    {
                        Checkpoint(parser); // r276

                        bool r276 = true;
                        if (r276)
                        { // alternatives a277 must
                            bool a277 = false;
                            if (!a277)
                            {
                                Checkpoint(parser); // r278

                                bool r278 = true;
                                r278 = r278 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                CommitOrRollback(r278, parser);
                                a277 = r278;
                            }

                            if (!a277)
                            {
                                Checkpoint(parser); // r279

                                bool r279 = true;
                                r279 = r279 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                CommitOrRollback(r279, parser);
                                a277 = r279;
                            }

                            r276 &= a277;

                        } // end alternatives a277

                        CommitOrRollback(r276, parser);
                        a229 = r276;
                    }

                    r228 &= a229;

                } // end alternatives a229

                CommitOrRollback(r228, parser);
                res = r228;
            }



            return res;
        }
    }

    public partial class DataType : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DataType()
            :base()
        {
        }

        public DataType(Jhu.Graywulf.Sql.Parsing.DataType old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DataType(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r280

                bool r280 = true;
                r280 = r280 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TypeName());
                if (r280)
                { // may a281
                    bool a281 = false;
                    {
                        Checkpoint(parser); // r282

                        bool r282 = true;
                        if (r282)
                        { // may a283
                            bool a283 = false;
                            {
                                Checkpoint(parser); // r284

                                bool r284 = true;
                                r284 = r284 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r284, parser);
                                a283 = r284;
                            }

                            r282 |= a283;
                        } // end may a283

                        if (r282)
                        { // alternatives a285 must
                            bool a285 = false;
                            if (!a285)
                            {
                                Checkpoint(parser); // r286

                                bool r286 = true;
                                r286 = r286 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataTypeScaleAndPrecision());
                                CommitOrRollback(r286, parser);
                                a285 = r286;
                            }

                            if (!a285)
                            {
                                Checkpoint(parser); // r287

                                bool r287 = true;
                                r287 = r287 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataTypeSize());
                                CommitOrRollback(r287, parser);
                                a285 = r287;
                            }

                            r282 &= a285;

                        } // end alternatives a285

                        CommitOrRollback(r282, parser);
                        a281 = r282;
                    }

                    r280 |= a281;
                } // end may a281

                if (r280)
                { // may a288
                    bool a288 = false;
                    {
                        Checkpoint(parser); // r289

                        bool r289 = true;
                        if (r289)
                        { // may a290
                            bool a290 = false;
                            {
                                Checkpoint(parser); // r291

                                bool r291 = true;
                                r291 = r291 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r291, parser);
                                a290 = r291;
                            }

                            r289 |= a290;
                        } // end may a290

                        if (r289)
                        { // may a292
                            bool a292 = false;
                            {
                                Checkpoint(parser); // r293

                                bool r293 = true;
                                r293 = r293 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                r293 = r293 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r293, parser);
                                a292 = r293;
                            }

                            r289 |= a292;
                        } // end may a292

                        r289 = r289 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                        CommitOrRollback(r289, parser);
                        a288 = r289;
                    }

                    r280 |= a288;
                } // end may a288

                CommitOrRollback(r280, parser);
                res = r280;
            }



            return res;
        }
    }

    public partial class DataTypeSize : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DataTypeSize()
            :base()
        {
        }

        public DataTypeSize(Jhu.Graywulf.Sql.Parsing.DataTypeSize old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DataTypeSize(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r294

                bool r294 = true;
                r294 = r294 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r294)
                { // may a295
                    bool a295 = false;
                    {
                        Checkpoint(parser); // r296

                        bool r296 = true;
                        r296 = r296 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r296, parser);
                        a295 = r296;
                    }

                    r294 |= a295;
                } // end may a295

                if (r294)
                { // alternatives a297 must
                    bool a297 = false;
                    if (!a297)
                    {
                        Checkpoint(parser); // r298

                        bool r298 = true;
                        r298 = r298 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"MAX"));
                        CommitOrRollback(r298, parser);
                        a297 = r298;
                    }

                    if (!a297)
                    {
                        Checkpoint(parser); // r299

                        bool r299 = true;
                        r299 = r299 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r299, parser);
                        a297 = r299;
                    }

                    r294 &= a297;

                } // end alternatives a297

                if (r294)
                { // may a300
                    bool a300 = false;
                    {
                        Checkpoint(parser); // r301

                        bool r301 = true;
                        r301 = r301 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r301, parser);
                        a300 = r301;
                    }

                    r294 |= a300;
                } // end may a300

                r294 = r294 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r294, parser);
                res = r294;
            }



            return res;
        }
    }

    public partial class DataTypeScaleAndPrecision : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DataTypeScaleAndPrecision()
            :base()
        {
        }

        public DataTypeScaleAndPrecision(Jhu.Graywulf.Sql.Parsing.DataTypeScaleAndPrecision old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DataTypeScaleAndPrecision(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r302

                bool r302 = true;
                r302 = r302 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r302)
                { // may a303
                    bool a303 = false;
                    {
                        Checkpoint(parser); // r304

                        bool r304 = true;
                        r304 = r304 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r304, parser);
                        a303 = r304;
                    }

                    r302 |= a303;
                } // end may a303

                r302 = r302 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                if (r302)
                { // may a305
                    bool a305 = false;
                    {
                        Checkpoint(parser); // r306

                        bool r306 = true;
                        r306 = r306 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r306, parser);
                        a305 = r306;
                    }

                    r302 |= a305;
                } // end may a305

                r302 = r302 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                if (r302)
                { // may a307
                    bool a307 = false;
                    {
                        Checkpoint(parser); // r308

                        bool r308 = true;
                        r308 = r308 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r308, parser);
                        a307 = r308;
                    }

                    r302 |= a307;
                } // end may a307

                r302 = r302 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                if (r302)
                { // may a309
                    bool a309 = false;
                    {
                        Checkpoint(parser); // r310

                        bool r310 = true;
                        r310 = r310 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r310, parser);
                        a309 = r310;
                    }

                    r302 |= a309;
                } // end may a309

                r302 = r302 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r302, parser);
                res = r302;
            }



            return res;
        }
    }

    public partial class FunctionIdentifier : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FunctionIdentifier()
            :base()
        {
        }

        public FunctionIdentifier(Jhu.Graywulf.Sql.Parsing.FunctionIdentifier old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FunctionIdentifier(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r311

                bool r311 = true;
                if (r311)
                { // alternatives a312 must
                    bool a312 = false;
                    if (!a312)
                    {
                        Checkpoint(parser); // r313

                        bool r313 = true;
                        r313 = r313 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdfIdentifier());
                        CommitOrRollback(r313, parser);
                        a312 = r313;
                    }

                    if (!a312)
                    {
                        Checkpoint(parser); // r314

                        bool r314 = true;
                        r314 = r314 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r314, parser);
                        a312 = r314;
                    }

                    r311 &= a312;

                } // end alternatives a312

                CommitOrRollback(r311, parser);
                res = r311;
            }



            return res;
        }
    }

    public partial class UdfIdentifier : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UdfIdentifier()
            :base()
        {
        }

        public UdfIdentifier(Jhu.Graywulf.Sql.Parsing.UdfIdentifier old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UdfIdentifier(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r315

                bool r315 = true;
                if (r315)
                { // may a316
                    bool a316 = false;
                    {
                        Checkpoint(parser); // r317

                        bool r317 = true;
                        r317 = r317 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                        if (r317)
                        { // may a318
                            bool a318 = false;
                            {
                                Checkpoint(parser); // r319

                                bool r319 = true;
                                r319 = r319 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r319, parser);
                                a318 = r319;
                            }

                            r317 |= a318;
                        } // end may a318

                        r317 = r317 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                        if (r317)
                        { // may a320
                            bool a320 = false;
                            {
                                Checkpoint(parser); // r321

                                bool r321 = true;
                                r321 = r321 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r321, parser);
                                a320 = r321;
                            }

                            r317 |= a320;
                        } // end may a320

                        CommitOrRollback(r317, parser);
                        a316 = r317;
                    }

                    r315 |= a316;
                } // end may a316

                if (r315)
                { // alternatives a322 must
                    bool a322 = false;
                    if (!a322)
                    {
                        Checkpoint(parser); // r323

                        bool r323 = true;
                        r323 = r323 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                        if (r323)
                        { // may a324
                            bool a324 = false;
                            {
                                Checkpoint(parser); // r325

                                bool r325 = true;
                                r325 = r325 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r325, parser);
                                a324 = r325;
                            }

                            r323 |= a324;
                        } // end may a324

                        r323 = r323 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r323)
                        { // may a326
                            bool a326 = false;
                            {
                                Checkpoint(parser); // r327

                                bool r327 = true;
                                if (r327)
                                { // may a328
                                    bool a328 = false;
                                    {
                                        Checkpoint(parser); // r329

                                        bool r329 = true;
                                        r329 = r329 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r329, parser);
                                        a328 = r329;
                                    }

                                    r327 |= a328;
                                } // end may a328

                                r327 = r327 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                CommitOrRollback(r327, parser);
                                a326 = r327;
                            }

                            r323 |= a326;
                        } // end may a326

                        if (r323)
                        { // may a330
                            bool a330 = false;
                            {
                                Checkpoint(parser); // r331

                                bool r331 = true;
                                r331 = r331 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r331, parser);
                                a330 = r331;
                            }

                            r323 |= a330;
                        } // end may a330

                        r323 = r323 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r323)
                        { // may a332
                            bool a332 = false;
                            {
                                Checkpoint(parser); // r333

                                bool r333 = true;
                                r333 = r333 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r333, parser);
                                a332 = r333;
                            }

                            r323 |= a332;
                        } // end may a332

                        r323 = r323 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r323, parser);
                        a322 = r323;
                    }

                    if (!a322)
                    {
                        Checkpoint(parser); // r334

                        bool r334 = true;
                        r334 = r334 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                        if (r334)
                        { // may a335
                            bool a335 = false;
                            {
                                Checkpoint(parser); // r336

                                bool r336 = true;
                                r336 = r336 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r336, parser);
                                a335 = r336;
                            }

                            r334 |= a335;
                        } // end may a335

                        r334 = r334 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r334)
                        { // may a337
                            bool a337 = false;
                            {
                                Checkpoint(parser); // r338

                                bool r338 = true;
                                r338 = r338 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r338, parser);
                                a337 = r338;
                            }

                            r334 |= a337;
                        } // end may a337

                        r334 = r334 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r334, parser);
                        a322 = r334;
                    }

                    r315 &= a322;

                } // end alternatives a322

                CommitOrRollback(r315, parser);
                res = r315;
            }



            return res;
        }
    }

    public partial class UdtFunctionIdentifier : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UdtFunctionIdentifier()
            :base()
        {
        }

        public UdtFunctionIdentifier(Jhu.Graywulf.Sql.Parsing.UdtFunctionIdentifier old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UdtFunctionIdentifier(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r339

                bool r339 = true;
                r339 = r339 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r339)
                { // may a340
                    bool a340 = false;
                    {
                        Checkpoint(parser); // r341

                        bool r341 = true;
                        r341 = r341 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r341, parser);
                        a340 = r341;
                    }

                    r339 |= a340;
                } // end may a340

                r339 = r339 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                if (r339)
                { // may a342
                    bool a342 = false;
                    {
                        Checkpoint(parser); // r343

                        bool r343 = true;
                        r343 = r343 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r343, parser);
                        a342 = r343;
                    }

                    r339 |= a342;
                } // end may a342

                r339 = r339 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                CommitOrRollback(r339, parser);
                res = r339;
            }



            return res;
        }
    }

    public partial class Argument : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Argument()
            :base()
        {
        }

        public Argument(Jhu.Graywulf.Sql.Parsing.Argument old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Argument(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r344

                bool r344 = true;
                r344 = r344 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r344, parser);
                res = r344;
            }



            return res;
        }
    }

    public partial class ArgumentList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ArgumentList()
            :base()
        {
        }

        public ArgumentList(Jhu.Graywulf.Sql.Parsing.ArgumentList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ArgumentList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r345

                bool r345 = true;
                r345 = r345 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Argument());
                if (r345)
                { // may a346
                    bool a346 = false;
                    {
                        Checkpoint(parser); // r347

                        bool r347 = true;
                        if (r347)
                        { // may a348
                            bool a348 = false;
                            {
                                Checkpoint(parser); // r349

                                bool r349 = true;
                                r349 = r349 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r349, parser);
                                a348 = r349;
                            }

                            r347 |= a348;
                        } // end may a348

                        r347 = r347 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r347)
                        { // may a350
                            bool a350 = false;
                            {
                                Checkpoint(parser); // r351

                                bool r351 = true;
                                r351 = r351 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r351, parser);
                                a350 = r351;
                            }

                            r347 |= a350;
                        } // end may a350

                        r347 = r347 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                        CommitOrRollback(r347, parser);
                        a346 = r347;
                    }

                    r345 |= a346;
                } // end may a346

                CommitOrRollback(r345, parser);
                res = r345;
            }



            return res;
        }
    }

    public partial class UdtFunctionCall : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UdtFunctionCall()
            :base()
        {
        }

        public UdtFunctionCall(Jhu.Graywulf.Sql.Parsing.UdtFunctionCall old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UdtFunctionCall(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r352

                bool r352 = true;
                r352 = r352 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdtFunctionIdentifier());
                if (r352)
                { // may a353
                    bool a353 = false;
                    {
                        Checkpoint(parser); // r354

                        bool r354 = true;
                        r354 = r354 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r354, parser);
                        a353 = r354;
                    }

                    r352 |= a353;
                } // end may a353

                r352 = r352 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r352, parser);
                res = r352;
            }



            return res;
        }
    }

    public partial class FunctionCall : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FunctionCall()
            :base()
        {
        }

        public FunctionCall(Jhu.Graywulf.Sql.Parsing.FunctionCall old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FunctionCall(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r355

                bool r355 = true;
                r355 = r355 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionIdentifier());
                if (r355)
                { // may a356
                    bool a356 = false;
                    {
                        Checkpoint(parser); // r357

                        bool r357 = true;
                        r357 = r357 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r357, parser);
                        a356 = r357;
                    }

                    r355 |= a356;
                } // end may a356

                r355 = r355 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r355, parser);
                res = r355;
            }



            return res;
        }
    }

    public partial class TableValuedFunctionCall : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableValuedFunctionCall()
            :base()
        {
        }

        public TableValuedFunctionCall(Jhu.Graywulf.Sql.Parsing.TableValuedFunctionCall old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableValuedFunctionCall(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r358

                bool r358 = true;
                r358 = r358 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionIdentifier());
                if (r358)
                { // may a359
                    bool a359 = false;
                    {
                        Checkpoint(parser); // r360

                        bool r360 = true;
                        r360 = r360 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r360, parser);
                        a359 = r360;
                    }

                    r358 |= a359;
                } // end may a359

                r358 = r358 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r358, parser);
                res = r358;
            }



            return res;
        }
    }

    public partial class RankingFunctionCall : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public RankingFunctionCall()
            :base()
        {
        }

        public RankingFunctionCall(Jhu.Graywulf.Sql.Parsing.RankingFunctionCall old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.RankingFunctionCall(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r361

                bool r361 = true;
                r361 = r361 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                if (r361)
                { // may a362
                    bool a362 = false;
                    {
                        Checkpoint(parser); // r363

                        bool r363 = true;
                        r363 = r363 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r363, parser);
                        a362 = r363;
                    }

                    r361 |= a362;
                } // end may a362

                r361 = r361 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                if (r361)
                { // may a364
                    bool a364 = false;
                    {
                        Checkpoint(parser); // r365

                        bool r365 = true;
                        r365 = r365 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r365, parser);
                        a364 = r365;
                    }

                    r361 |= a364;
                } // end may a364

                r361 = r361 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OverClause());
                CommitOrRollback(r361, parser);
                res = r361;
            }



            return res;
        }
    }

    public partial class FunctionArguments : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FunctionArguments()
            :base()
        {
        }

        public FunctionArguments(Jhu.Graywulf.Sql.Parsing.FunctionArguments old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FunctionArguments(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r366

                bool r366 = true;
                r366 = r366 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r366)
                { // may a367
                    bool a367 = false;
                    {
                        Checkpoint(parser); // r368

                        bool r368 = true;
                        r368 = r368 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r368, parser);
                        a367 = r368;
                    }

                    r366 |= a367;
                } // end may a367

                if (r366)
                { // may a369
                    bool a369 = false;
                    {
                        Checkpoint(parser); // r370

                        bool r370 = true;
                        r370 = r370 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                        CommitOrRollback(r370, parser);
                        a369 = r370;
                    }

                    r366 |= a369;
                } // end may a369

                if (r366)
                { // may a371
                    bool a371 = false;
                    {
                        Checkpoint(parser); // r372

                        bool r372 = true;
                        r372 = r372 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r372, parser);
                        a371 = r372;
                    }

                    r366 |= a371;
                } // end may a371

                r366 = r366 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r366, parser);
                res = r366;
            }



            return res;
        }
    }

    public partial class OverClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public OverClause()
            :base()
        {
        }

        public OverClause(Jhu.Graywulf.Sql.Parsing.OverClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.OverClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r373

                bool r373 = true;
                r373 = r373 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OVER"));
                if (r373)
                { // may a374
                    bool a374 = false;
                    {
                        Checkpoint(parser); // r375

                        bool r375 = true;
                        r375 = r375 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r375, parser);
                        a374 = r375;
                    }

                    r373 |= a374;
                } // end may a374

                r373 = r373 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r373)
                { // may a376
                    bool a376 = false;
                    {
                        Checkpoint(parser); // r377

                        bool r377 = true;
                        r377 = r377 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r377, parser);
                        a376 = r377;
                    }

                    r373 |= a376;
                } // end may a376

                if (r373)
                { // may a378
                    bool a378 = false;
                    {
                        Checkpoint(parser); // r379

                        bool r379 = true;
                        r379 = r379 && Match(parser, new Jhu.Graywulf.Sql.Parsing.PartitionByClause());
                        if (r379)
                        { // may a380
                            bool a380 = false;
                            {
                                Checkpoint(parser); // r381

                                bool r381 = true;
                                r381 = r381 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r381, parser);
                                a380 = r381;
                            }

                            r379 |= a380;
                        } // end may a380

                        CommitOrRollback(r379, parser);
                        a378 = r379;
                    }

                    r373 |= a378;
                } // end may a378

                if (r373)
                { // may a382
                    bool a382 = false;
                    {
                        Checkpoint(parser); // r383

                        bool r383 = true;
                        r383 = r383 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        if (r383)
                        { // may a384
                            bool a384 = false;
                            {
                                Checkpoint(parser); // r385

                                bool r385 = true;
                                r385 = r385 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r385, parser);
                                a384 = r385;
                            }

                            r383 |= a384;
                        } // end may a384

                        CommitOrRollback(r383, parser);
                        a382 = r383;
                    }

                    r373 |= a382;
                } // end may a382

                r373 = r373 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r373, parser);
                res = r373;
            }



            return res;
        }
    }

    public partial class PartitionByClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public PartitionByClause()
            :base()
        {
        }

        public PartitionByClause(Jhu.Graywulf.Sql.Parsing.PartitionByClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.PartitionByClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r386

                bool r386 = true;
                r386 = r386 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PARTITION"));
                r386 = r386 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r386 = r386 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                if (r386)
                { // may a387
                    bool a387 = false;
                    {
                        Checkpoint(parser); // r388

                        bool r388 = true;
                        r388 = r388 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r388, parser);
                        a387 = r388;
                    }

                    r386 |= a387;
                } // end may a387

                r386 = r386 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                CommitOrRollback(r386, parser);
                res = r386;
            }



            return res;
        }
    }

    public partial class StatementBlock : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public StatementBlock()
            :base()
        {
        }

        public StatementBlock(Jhu.Graywulf.Sql.Parsing.StatementBlock old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.StatementBlock(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r389

                bool r389 = true;
                if (r389)
                { // may a390
                    bool a390 = false;
                    {
                        Checkpoint(parser); // r391

                        bool r391 = true;
                        r391 = r391 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r391, parser);
                        a390 = r391;
                    }

                    r389 |= a390;
                } // end may a390

                if (r389)
                { // may a392
                    bool a392 = false;
                    {
                        Checkpoint(parser); // r393

                        bool r393 = true;
                        r393 = r393 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                        CommitOrRollback(r393, parser);
                        a392 = r393;
                    }

                    r389 |= a392;
                } // end may a392

                if (r389)
                { // may a394
                    bool a394 = false;
                    {
                        Checkpoint(parser); // r395

                        bool r395 = true;
                        r395 = r395 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementSeparator());
                        if (r395)
                        { // may a396
                            bool a396 = false;
                            {
                                Checkpoint(parser); // r397

                                bool r397 = true;
                                r397 = r397 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                                CommitOrRollback(r397, parser);
                                a396 = r397;
                            }

                            r395 |= a396;
                        } // end may a396

                        CommitOrRollback(r395, parser);
                        a394 = r395;
                    }

                    r389 |= a394;
                } // end may a394

                if (r389)
                { // may a398
                    bool a398 = false;
                    {
                        Checkpoint(parser); // r399

                        bool r399 = true;
                        r399 = r399 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r399, parser);
                        a398 = r399;
                    }

                    r389 |= a398;
                } // end may a398

                CommitOrRollback(r389, parser);
                res = r389;
            }



            return res;
        }
    }

    public partial class StatementSeparator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public StatementSeparator()
            :base()
        {
        }

        public StatementSeparator(Jhu.Graywulf.Sql.Parsing.StatementSeparator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.StatementSeparator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r400

                bool r400 = true;
                if (r400)
                { // alternatives a401 must
                    bool a401 = false;
                    if (!a401)
                    {
                        Checkpoint(parser); // r402

                        bool r402 = true;
                        if (r402)
                        { // may a403
                            bool a403 = false;
                            {
                                Checkpoint(parser); // r404

                                bool r404 = true;
                                r404 = r404 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r404, parser);
                                a403 = r404;
                            }

                            r402 |= a403;
                        } // end may a403

                        r402 = r402 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Semicolon());
                        if (r402)
                        { // may a405
                            bool a405 = false;
                            {
                                Checkpoint(parser); // r406

                                bool r406 = true;
                                r406 = r406 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r406, parser);
                                a405 = r406;
                            }

                            r402 |= a405;
                        } // end may a405

                        CommitOrRollback(r402, parser);
                        a401 = r402;
                    }

                    if (!a401)
                    {
                        Checkpoint(parser); // r407

                        bool r407 = true;
                        r407 = r407 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r407, parser);
                        a401 = r407;
                    }

                    r400 &= a401;

                } // end alternatives a401

                CommitOrRollback(r400, parser);
                res = r400;
            }



            return res;
        }
    }

    public partial class Statement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Statement()
            :base()
        {
        }

        public Statement(Jhu.Graywulf.Sql.Parsing.Statement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Statement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r408

                bool r408 = true;
                if (r408)
                { // alternatives a409 must
                    bool a409 = false;
                    if (!a409)
                    {
                        Checkpoint(parser); // r410

                        bool r410 = true;
                        r410 = r410 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Label());
                        CommitOrRollback(r410, parser);
                        a409 = r410;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r411

                        bool r411 = true;
                        r411 = r411 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GotoStatement());
                        CommitOrRollback(r411, parser);
                        a409 = r411;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r412

                        bool r412 = true;
                        r412 = r412 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BeginEndStatement());
                        CommitOrRollback(r412, parser);
                        a409 = r412;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r413

                        bool r413 = true;
                        r413 = r413 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhileStatement());
                        CommitOrRollback(r413, parser);
                        a409 = r413;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r414

                        bool r414 = true;
                        r414 = r414 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BreakStatement());
                        CommitOrRollback(r414, parser);
                        a409 = r414;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r415

                        bool r415 = true;
                        r415 = r415 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ContinueStatement());
                        CommitOrRollback(r415, parser);
                        a409 = r415;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r416

                        bool r416 = true;
                        r416 = r416 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ReturnStatement());
                        CommitOrRollback(r416, parser);
                        a409 = r416;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r417

                        bool r417 = true;
                        r417 = r417 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IfStatement());
                        CommitOrRollback(r417, parser);
                        a409 = r417;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r418

                        bool r418 = true;
                        r418 = r418 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TryCatchStatement());
                        CommitOrRollback(r418, parser);
                        a409 = r418;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r419

                        bool r419 = true;
                        r419 = r419 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ThrowStatement());
                        CommitOrRollback(r419, parser);
                        a409 = r419;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r420

                        bool r420 = true;
                        r420 = r420 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareCursorStatement());
                        CommitOrRollback(r420, parser);
                        a409 = r420;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r421

                        bool r421 = true;
                        r421 = r421 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SetCursorStatement());
                        CommitOrRollback(r421, parser);
                        a409 = r421;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r422

                        bool r422 = true;
                        r422 = r422 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorOperationStatement());
                        CommitOrRollback(r422, parser);
                        a409 = r422;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r423

                        bool r423 = true;
                        r423 = r423 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FetchStatement());
                        CommitOrRollback(r423, parser);
                        a409 = r423;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r424

                        bool r424 = true;
                        r424 = r424 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareVariableStatement());
                        CommitOrRollback(r424, parser);
                        a409 = r424;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r425

                        bool r425 = true;
                        r425 = r425 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SetVariableStatement());
                        CommitOrRollback(r425, parser);
                        a409 = r425;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r426

                        bool r426 = true;
                        r426 = r426 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareTableStatement());
                        CommitOrRollback(r426, parser);
                        a409 = r426;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r427

                        bool r427 = true;
                        r427 = r427 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CreateTableStatement());
                        CommitOrRollback(r427, parser);
                        a409 = r427;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r428

                        bool r428 = true;
                        r428 = r428 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DropTableStatement());
                        CommitOrRollback(r428, parser);
                        a409 = r428;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r429

                        bool r429 = true;
                        r429 = r429 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TruncateTableStatement());
                        CommitOrRollback(r429, parser);
                        a409 = r429;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r430

                        bool r430 = true;
                        r430 = r430 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CreateIndexStatement());
                        CommitOrRollback(r430, parser);
                        a409 = r430;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r431

                        bool r431 = true;
                        r431 = r431 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DropIndexStatement());
                        CommitOrRollback(r431, parser);
                        a409 = r431;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r432

                        bool r432 = true;
                        r432 = r432 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectStatement());
                        CommitOrRollback(r432, parser);
                        a409 = r432;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r433

                        bool r433 = true;
                        r433 = r433 && Match(parser, new Jhu.Graywulf.Sql.Parsing.InsertStatement());
                        CommitOrRollback(r433, parser);
                        a409 = r433;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r434

                        bool r434 = true;
                        r434 = r434 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateStatement());
                        CommitOrRollback(r434, parser);
                        a409 = r434;
                    }

                    if (!a409)
                    {
                        Checkpoint(parser); // r435

                        bool r435 = true;
                        r435 = r435 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeleteStatement());
                        CommitOrRollback(r435, parser);
                        a409 = r435;
                    }

                    r408 &= a409;

                } // end alternatives a409

                CommitOrRollback(r408, parser);
                res = r408;
            }



            return res;
        }
    }

    public partial class Label : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Label()
            :base()
        {
        }

        public Label(Jhu.Graywulf.Sql.Parsing.Label old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Label(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r436

                bool r436 = true;
                r436 = r436 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                r436 = r436 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                CommitOrRollback(r436, parser);
                res = r436;
            }



            return res;
        }
    }

    public partial class GotoStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public GotoStatement()
            :base()
        {
        }

        public GotoStatement(Jhu.Graywulf.Sql.Parsing.GotoStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.GotoStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r437

                bool r437 = true;
                r437 = r437 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"GOTO"));
                r437 = r437 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r437 = r437 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r437, parser);
                res = r437;
            }



            return res;
        }
    }

    public partial class BeginEndStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public BeginEndStatement()
            :base()
        {
        }

        public BeginEndStatement(Jhu.Graywulf.Sql.Parsing.BeginEndStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.BeginEndStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r438

                bool r438 = true;
                r438 = r438 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r438 = r438 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                r438 = r438 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r438, parser);
                res = r438;
            }



            return res;
        }
    }

    public partial class WhileStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public WhileStatement()
            :base()
        {
        }

        public WhileStatement(Jhu.Graywulf.Sql.Parsing.WhileStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.WhileStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r439

                bool r439 = true;
                r439 = r439 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHILE"));
                if (r439)
                { // may a440
                    bool a440 = false;
                    {
                        Checkpoint(parser); // r441

                        bool r441 = true;
                        r441 = r441 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r441, parser);
                        a440 = r441;
                    }

                    r439 |= a440;
                } // end may a440

                r439 = r439 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r439)
                { // may a442
                    bool a442 = false;
                    {
                        Checkpoint(parser); // r443

                        bool r443 = true;
                        r443 = r443 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r443, parser);
                        a442 = r443;
                    }

                    r439 |= a442;
                } // end may a442

                r439 = r439 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                CommitOrRollback(r439, parser);
                res = r439;
            }



            return res;
        }
    }

    public partial class BreakStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public BreakStatement()
            :base()
        {
        }

        public BreakStatement(Jhu.Graywulf.Sql.Parsing.BreakStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.BreakStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r444

                bool r444 = true;
                r444 = r444 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BREAK"));
                CommitOrRollback(r444, parser);
                res = r444;
            }



            return res;
        }
    }

    public partial class ContinueStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ContinueStatement()
            :base()
        {
        }

        public ContinueStatement(Jhu.Graywulf.Sql.Parsing.ContinueStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ContinueStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r445

                bool r445 = true;
                r445 = r445 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CONTINUE"));
                CommitOrRollback(r445, parser);
                res = r445;
            }



            return res;
        }
    }

    public partial class ReturnStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ReturnStatement()
            :base()
        {
        }

        public ReturnStatement(Jhu.Graywulf.Sql.Parsing.ReturnStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ReturnStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r446

                bool r446 = true;
                r446 = r446 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RETURN"));
                CommitOrRollback(r446, parser);
                res = r446;
            }



            return res;
        }
    }

    public partial class IfStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IfStatement()
            :base()
        {
        }

        public IfStatement(Jhu.Graywulf.Sql.Parsing.IfStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IfStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r447

                bool r447 = true;
                r447 = r447 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IF"));
                if (r447)
                { // may a448
                    bool a448 = false;
                    {
                        Checkpoint(parser); // r449

                        bool r449 = true;
                        r449 = r449 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r449, parser);
                        a448 = r449;
                    }

                    r447 |= a448;
                } // end may a448

                r447 = r447 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r447)
                { // may a450
                    bool a450 = false;
                    {
                        Checkpoint(parser); // r451

                        bool r451 = true;
                        r451 = r451 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r451, parser);
                        a450 = r451;
                    }

                    r447 |= a450;
                } // end may a450

                r447 = r447 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                if (r447)
                { // may a452
                    bool a452 = false;
                    {
                        Checkpoint(parser); // r453

                        bool r453 = true;
                        r453 = r453 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementSeparator());
                        r453 = r453 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ELSE"));
                        r453 = r453 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r453 = r453 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                        CommitOrRollback(r453, parser);
                        a452 = r453;
                    }

                    r447 |= a452;
                } // end may a452

                CommitOrRollback(r447, parser);
                res = r447;
            }



            return res;
        }
    }

    public partial class ThrowStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ThrowStatement()
            :base()
        {
        }

        public ThrowStatement(Jhu.Graywulf.Sql.Parsing.ThrowStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ThrowStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r454

                bool r454 = true;
                r454 = r454 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THROW"));
                if (r454)
                { // may a455
                    bool a455 = false;
                    {
                        Checkpoint(parser); // r456

                        bool r456 = true;
                        r456 = r456 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r456)
                        { // alternatives a457 must
                            bool a457 = false;
                            if (!a457)
                            {
                                Checkpoint(parser); // r458

                                bool r458 = true;
                                r458 = r458 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                CommitOrRollback(r458, parser);
                                a457 = r458;
                            }

                            if (!a457)
                            {
                                Checkpoint(parser); // r459

                                bool r459 = true;
                                r459 = r459 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r459, parser);
                                a457 = r459;
                            }

                            r456 &= a457;

                        } // end alternatives a457

                        if (r456)
                        { // may a460
                            bool a460 = false;
                            {
                                Checkpoint(parser); // r461

                                bool r461 = true;
                                r461 = r461 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r461, parser);
                                a460 = r461;
                            }

                            r456 |= a460;
                        } // end may a460

                        r456 = r456 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r456)
                        { // may a462
                            bool a462 = false;
                            {
                                Checkpoint(parser); // r463

                                bool r463 = true;
                                r463 = r463 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r463, parser);
                                a462 = r463;
                            }

                            r456 |= a462;
                        } // end may a462

                        if (r456)
                        { // alternatives a464 must
                            bool a464 = false;
                            if (!a464)
                            {
                                Checkpoint(parser); // r465

                                bool r465 = true;
                                r465 = r465 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StringConstant());
                                CommitOrRollback(r465, parser);
                                a464 = r465;
                            }

                            if (!a464)
                            {
                                Checkpoint(parser); // r466

                                bool r466 = true;
                                r466 = r466 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r466, parser);
                                a464 = r466;
                            }

                            r456 &= a464;

                        } // end alternatives a464

                        if (r456)
                        { // may a467
                            bool a467 = false;
                            {
                                Checkpoint(parser); // r468

                                bool r468 = true;
                                r468 = r468 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r468, parser);
                                a467 = r468;
                            }

                            r456 |= a467;
                        } // end may a467

                        r456 = r456 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r456)
                        { // may a469
                            bool a469 = false;
                            {
                                Checkpoint(parser); // r470

                                bool r470 = true;
                                r470 = r470 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r470, parser);
                                a469 = r470;
                            }

                            r456 |= a469;
                        } // end may a469

                        if (r456)
                        { // alternatives a471 must
                            bool a471 = false;
                            if (!a471)
                            {
                                Checkpoint(parser); // r472

                                bool r472 = true;
                                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                CommitOrRollback(r472, parser);
                                a471 = r472;
                            }

                            if (!a471)
                            {
                                Checkpoint(parser); // r473

                                bool r473 = true;
                                r473 = r473 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r473, parser);
                                a471 = r473;
                            }

                            r456 &= a471;

                        } // end alternatives a471

                        if (r456)
                        { // may a474
                            bool a474 = false;
                            {
                                Checkpoint(parser); // r475

                                bool r475 = true;
                                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r475, parser);
                                a474 = r475;
                            }

                            r456 |= a474;
                        } // end may a474

                        CommitOrRollback(r456, parser);
                        a455 = r456;
                    }

                    r454 |= a455;
                } // end may a455

                CommitOrRollback(r454, parser);
                res = r454;
            }



            return res;
        }
    }

    public partial class TryCatchStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TryCatchStatement()
            :base()
        {
        }

        public TryCatchStatement(Jhu.Graywulf.Sql.Parsing.TryCatchStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TryCatchStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r476

                bool r476 = true;
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRY"));
                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRY"));
                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CATCH"));
                if (r476)
                { // alternatives a477 must
                    bool a477 = false;
                    if (!a477)
                    {
                        Checkpoint(parser); // r478

                        bool r478 = true;
                        r478 = r478 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                        CommitOrRollback(r478, parser);
                        a477 = r478;
                    }

                    if (!a477)
                    {
                        Checkpoint(parser); // r479

                        bool r479 = true;
                        r479 = r479 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r479, parser);
                        a477 = r479;
                    }

                    r476 &= a477;

                } // end alternatives a477

                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r476 = r476 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CATCH"));
                CommitOrRollback(r476, parser);
                res = r476;
            }



            return res;
        }
    }

    public partial class VariableList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public VariableList()
            :base()
        {
        }

        public VariableList(Jhu.Graywulf.Sql.Parsing.VariableList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.VariableList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r480

                bool r480 = true;
                if (r480)
                { // may a481
                    bool a481 = false;
                    {
                        Checkpoint(parser); // r482

                        bool r482 = true;
                        r482 = r482 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r482, parser);
                        a481 = r482;
                    }

                    r480 |= a481;
                } // end may a481

                r480 = r480 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r480)
                { // may a483
                    bool a483 = false;
                    {
                        Checkpoint(parser); // r484

                        bool r484 = true;
                        if (r484)
                        { // may a485
                            bool a485 = false;
                            {
                                Checkpoint(parser); // r486

                                bool r486 = true;
                                r486 = r486 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r486, parser);
                                a485 = r486;
                            }

                            r484 |= a485;
                        } // end may a485

                        r484 = r484 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        r484 = r484 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableList());
                        CommitOrRollback(r484, parser);
                        a483 = r484;
                    }

                    r480 |= a483;
                } // end may a483

                CommitOrRollback(r480, parser);
                res = r480;
            }



            return res;
        }
    }

    public partial class DeclareVariableStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DeclareVariableStatement()
            :base()
        {
        }

        public DeclareVariableStatement(Jhu.Graywulf.Sql.Parsing.DeclareVariableStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DeclareVariableStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r487

                bool r487 = true;
                r487 = r487 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                if (r487)
                { // may a488
                    bool a488 = false;
                    {
                        Checkpoint(parser); // r489

                        bool r489 = true;
                        r489 = r489 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r489, parser);
                        a488 = r489;
                    }

                    r487 |= a488;
                } // end may a488

                r487 = r487 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList());
                CommitOrRollback(r487, parser);
                res = r487;
            }



            return res;
        }
    }

    public partial class VariableDeclarationList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public VariableDeclarationList()
            :base()
        {
        }

        public VariableDeclarationList(Jhu.Graywulf.Sql.Parsing.VariableDeclarationList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r490

                bool r490 = true;
                r490 = r490 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclaration());
                if (r490)
                { // may a491
                    bool a491 = false;
                    {
                        Checkpoint(parser); // r492

                        bool r492 = true;
                        if (r492)
                        { // may a493
                            bool a493 = false;
                            {
                                Checkpoint(parser); // r494

                                bool r494 = true;
                                r494 = r494 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r494, parser);
                                a493 = r494;
                            }

                            r492 |= a493;
                        } // end may a493

                        r492 = r492 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r492)
                        { // may a495
                            bool a495 = false;
                            {
                                Checkpoint(parser); // r496

                                bool r496 = true;
                                r496 = r496 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r496, parser);
                                a495 = r496;
                            }

                            r492 |= a495;
                        } // end may a495

                        r492 = r492 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList());
                        CommitOrRollback(r492, parser);
                        a491 = r492;
                    }

                    r490 |= a491;
                } // end may a491

                CommitOrRollback(r490, parser);
                res = r490;
            }



            return res;
        }
    }

    public partial class VariableDeclaration : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public VariableDeclaration()
            :base()
        {
        }

        public VariableDeclaration(Jhu.Graywulf.Sql.Parsing.VariableDeclaration old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.VariableDeclaration(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r497

                bool r497 = true;
                r497 = r497 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r497)
                { // may a498
                    bool a498 = false;
                    {
                        Checkpoint(parser); // r499

                        bool r499 = true;
                        r499 = r499 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r499 = r499 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        CommitOrRollback(r499, parser);
                        a498 = r499;
                    }

                    r497 |= a498;
                } // end may a498

                r497 = r497 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r497)
                { // alternatives a500 must
                    bool a500 = false;
                    if (!a500)
                    {
                        Checkpoint(parser); // r501

                        bool r501 = true;
                        r501 = r501 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CURSOR"));
                        CommitOrRollback(r501, parser);
                        a500 = r501;
                    }

                    if (!a500)
                    {
                        Checkpoint(parser); // r502

                        bool r502 = true;
                        r502 = r502 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataType());
                        if (r502)
                        { // may a503
                            bool a503 = false;
                            {
                                Checkpoint(parser); // r504

                                bool r504 = true;
                                if (r504)
                                { // may a505
                                    bool a505 = false;
                                    {
                                        Checkpoint(parser); // r506

                                        bool r506 = true;
                                        r506 = r506 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r506, parser);
                                        a505 = r506;
                                    }

                                    r504 |= a505;
                                } // end may a505

                                r504 = r504 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                                if (r504)
                                { // may a507
                                    bool a507 = false;
                                    {
                                        Checkpoint(parser); // r508

                                        bool r508 = true;
                                        r508 = r508 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r508, parser);
                                        a507 = r508;
                                    }

                                    r504 |= a507;
                                } // end may a507

                                r504 = r504 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                                CommitOrRollback(r504, parser);
                                a503 = r504;
                            }

                            r502 |= a503;
                        } // end may a503

                        CommitOrRollback(r502, parser);
                        a500 = r502;
                    }

                    r497 &= a500;

                } // end alternatives a500

                CommitOrRollback(r497, parser);
                res = r497;
            }



            return res;
        }
    }

    public partial class SetVariableStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SetVariableStatement()
            :base()
        {
        }

        public SetVariableStatement(Jhu.Graywulf.Sql.Parsing.SetVariableStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SetVariableStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r509

                bool r509 = true;
                r509 = r509 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                r509 = r509 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r509 = r509 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r509)
                { // may a510
                    bool a510 = false;
                    {
                        Checkpoint(parser); // r511

                        bool r511 = true;
                        r511 = r511 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r511, parser);
                        a510 = r511;
                    }

                    r509 |= a510;
                } // end may a510

                r509 = r509 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator());
                if (r509)
                { // may a512
                    bool a512 = false;
                    {
                        Checkpoint(parser); // r513

                        bool r513 = true;
                        r513 = r513 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r513, parser);
                        a512 = r513;
                    }

                    r509 |= a512;
                } // end may a512

                r509 = r509 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r509, parser);
                res = r509;
            }



            return res;
        }
    }

    public partial class ValueAssignmentOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ValueAssignmentOperator()
            :base()
        {
        }

        public ValueAssignmentOperator(Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r514

                bool r514 = true;
                if (r514)
                { // alternatives a515 must
                    bool a515 = false;
                    if (!a515)
                    {
                        Checkpoint(parser); // r516

                        bool r516 = true;
                        r516 = r516 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        CommitOrRollback(r516, parser);
                        a515 = r516;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r517

                        bool r517 = true;
                        r517 = r517 && Match(parser, new Jhu.Graywulf.Sql.Parsing.PlusEquals());
                        CommitOrRollback(r517, parser);
                        a515 = r517;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r518

                        bool r518 = true;
                        r518 = r518 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MinusEquals());
                        CommitOrRollback(r518, parser);
                        a515 = r518;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r519

                        bool r519 = true;
                        r519 = r519 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MulEquals());
                        CommitOrRollback(r519, parser);
                        a515 = r519;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r520

                        bool r520 = true;
                        r520 = r520 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DivEquals());
                        CommitOrRollback(r520, parser);
                        a515 = r520;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r521

                        bool r521 = true;
                        r521 = r521 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ModEquals());
                        CommitOrRollback(r521, parser);
                        a515 = r521;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r522

                        bool r522 = true;
                        r522 = r522 && Match(parser, new Jhu.Graywulf.Sql.Parsing.AndEquals());
                        CommitOrRollback(r522, parser);
                        a515 = r522;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r523

                        bool r523 = true;
                        r523 = r523 && Match(parser, new Jhu.Graywulf.Sql.Parsing.XorEquals());
                        CommitOrRollback(r523, parser);
                        a515 = r523;
                    }

                    if (!a515)
                    {
                        Checkpoint(parser); // r524

                        bool r524 = true;
                        r524 = r524 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrEquals());
                        CommitOrRollback(r524, parser);
                        a515 = r524;
                    }

                    r514 &= a515;

                } // end alternatives a515

                CommitOrRollback(r514, parser);
                res = r514;
            }



            return res;
        }
    }

    public partial class DeclareCursorStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DeclareCursorStatement()
            :base()
        {
        }

        public DeclareCursorStatement(Jhu.Graywulf.Sql.Parsing.DeclareCursorStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DeclareCursorStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r525

                bool r525 = true;
                r525 = r525 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r525 = r525 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r525)
                { // alternatives a526 must
                    bool a526 = false;
                    if (!a526)
                    {
                        Checkpoint(parser); // r527

                        bool r527 = true;
                        r527 = r527 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r527, parser);
                        a526 = r527;
                    }

                    if (!a526)
                    {
                        Checkpoint(parser); // r528

                        bool r528 = true;
                        r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r528, parser);
                        a526 = r528;
                    }

                    r525 &= a526;

                } // end alternatives a526

                r525 = r525 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r525 = r525 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorDefinition());
                CommitOrRollback(r525, parser);
                res = r525;
            }



            return res;
        }
    }

    public partial class SetCursorStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SetCursorStatement()
            :base()
        {
        }

        public SetCursorStatement(Jhu.Graywulf.Sql.Parsing.SetCursorStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SetCursorStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r529

                bool r529 = true;
                r529 = r529 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                r529 = r529 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r529 = r529 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r529)
                { // may a530
                    bool a530 = false;
                    {
                        Checkpoint(parser); // r531

                        bool r531 = true;
                        r531 = r531 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r531, parser);
                        a530 = r531;
                    }

                    r529 |= a530;
                } // end may a530

                r529 = r529 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                if (r529)
                { // may a532
                    bool a532 = false;
                    {
                        Checkpoint(parser); // r533

                        bool r533 = true;
                        r533 = r533 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r533, parser);
                        a532 = r533;
                    }

                    r529 |= a532;
                } // end may a532

                r529 = r529 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorDefinition());
                CommitOrRollback(r529, parser);
                res = r529;
            }



            return res;
        }
    }

    public partial class CursorDefinition : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CursorDefinition()
            :base()
        {
        }

        public CursorDefinition(Jhu.Graywulf.Sql.Parsing.CursorDefinition old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CursorDefinition(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r534

                bool r534 = true;
                r534 = r534 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CURSOR"));
                r534 = r534 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r534 = r534 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FOR"));
                r534 = r534 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r534 = r534 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectStatement());
                CommitOrRollback(r534, parser);
                res = r534;
            }



            return res;
        }
    }

    public partial class CursorOperationStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CursorOperationStatement()
            :base()
        {
        }

        public CursorOperationStatement(Jhu.Graywulf.Sql.Parsing.CursorOperationStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CursorOperationStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r535

                bool r535 = true;
                if (r535)
                { // alternatives a536 must
                    bool a536 = false;
                    if (!a536)
                    {
                        Checkpoint(parser); // r537

                        bool r537 = true;
                        r537 = r537 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OPEN"));
                        CommitOrRollback(r537, parser);
                        a536 = r537;
                    }

                    if (!a536)
                    {
                        Checkpoint(parser); // r538

                        bool r538 = true;
                        r538 = r538 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLOSE"));
                        CommitOrRollback(r538, parser);
                        a536 = r538;
                    }

                    if (!a536)
                    {
                        Checkpoint(parser); // r539

                        bool r539 = true;
                        r539 = r539 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEALLOCATE"));
                        CommitOrRollback(r539, parser);
                        a536 = r539;
                    }

                    r535 &= a536;

                } // end alternatives a536

                r535 = r535 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r535)
                { // alternatives a540 must
                    bool a540 = false;
                    if (!a540)
                    {
                        Checkpoint(parser); // r541

                        bool r541 = true;
                        r541 = r541 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r541, parser);
                        a540 = r541;
                    }

                    if (!a540)
                    {
                        Checkpoint(parser); // r542

                        bool r542 = true;
                        r542 = r542 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r542, parser);
                        a540 = r542;
                    }

                    r535 &= a540;

                } // end alternatives a540

                CommitOrRollback(r535, parser);
                res = r535;
            }



            return res;
        }
    }

    public partial class FetchStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FetchStatement()
            :base()
        {
        }

        public FetchStatement(Jhu.Graywulf.Sql.Parsing.FetchStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FetchStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r543

                bool r543 = true;
                r543 = r543 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FETCH"));
                r543 = r543 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r543)
                { // may a544
                    bool a544 = false;
                    {
                        Checkpoint(parser); // r545

                        bool r545 = true;
                        if (r545)
                        { // alternatives a546 must
                            bool a546 = false;
                            if (!a546)
                            {
                                Checkpoint(parser); // r547

                                bool r547 = true;
                                r547 = r547 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NEXT"));
                                CommitOrRollback(r547, parser);
                                a546 = r547;
                            }

                            if (!a546)
                            {
                                Checkpoint(parser); // r548

                                bool r548 = true;
                                r548 = r548 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PRIOR"));
                                CommitOrRollback(r548, parser);
                                a546 = r548;
                            }

                            if (!a546)
                            {
                                Checkpoint(parser); // r549

                                bool r549 = true;
                                r549 = r549 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FIRST"));
                                CommitOrRollback(r549, parser);
                                a546 = r549;
                            }

                            if (!a546)
                            {
                                Checkpoint(parser); // r550

                                bool r550 = true;
                                r550 = r550 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LAST"));
                                CommitOrRollback(r550, parser);
                                a546 = r550;
                            }

                            if (!a546)
                            {
                                Checkpoint(parser); // r551

                                bool r551 = true;
                                if (r551)
                                { // alternatives a552 must
                                    bool a552 = false;
                                    if (!a552)
                                    {
                                        Checkpoint(parser); // r553

                                        bool r553 = true;
                                        r553 = r553 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ABSOLUTE"));
                                        CommitOrRollback(r553, parser);
                                        a552 = r553;
                                    }

                                    if (!a552)
                                    {
                                        Checkpoint(parser); // r554

                                        bool r554 = true;
                                        r554 = r554 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RELATIVE"));
                                        CommitOrRollback(r554, parser);
                                        a552 = r554;
                                    }

                                    r551 &= a552;

                                } // end alternatives a552

                                r551 = r551 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                if (r551)
                                { // alternatives a555 must
                                    bool a555 = false;
                                    if (!a555)
                                    {
                                        Checkpoint(parser); // r556

                                        bool r556 = true;
                                        r556 = r556 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                        CommitOrRollback(r556, parser);
                                        a555 = r556;
                                    }

                                    if (!a555)
                                    {
                                        Checkpoint(parser); // r557

                                        bool r557 = true;
                                        r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                        CommitOrRollback(r557, parser);
                                        a555 = r557;
                                    }

                                    r551 &= a555;

                                } // end alternatives a555

                                CommitOrRollback(r551, parser);
                                a546 = r551;
                            }

                            r545 &= a546;

                        } // end alternatives a546

                        CommitOrRollback(r545, parser);
                        a544 = r545;
                    }

                    r543 |= a544;
                } // end may a544

                r543 = r543 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r543 = r543 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                r543 = r543 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r543)
                { // alternatives a558 must
                    bool a558 = false;
                    if (!a558)
                    {
                        Checkpoint(parser); // r559

                        bool r559 = true;
                        r559 = r559 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r559, parser);
                        a558 = r559;
                    }

                    if (!a558)
                    {
                        Checkpoint(parser); // r560

                        bool r560 = true;
                        r560 = r560 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r560, parser);
                        a558 = r560;
                    }

                    r543 &= a558;

                } // end alternatives a558

                if (r543)
                { // may a561
                    bool a561 = false;
                    {
                        Checkpoint(parser); // r562

                        bool r562 = true;
                        r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r562 = r562 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTO"));
                        r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableList());
                        CommitOrRollback(r562, parser);
                        a561 = r562;
                    }

                    r543 |= a561;
                } // end may a561

                CommitOrRollback(r543, parser);
                res = r543;
            }



            return res;
        }
    }

    public partial class DeclareTableStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DeclareTableStatement()
            :base()
        {
        }

        public DeclareTableStatement(Jhu.Graywulf.Sql.Parsing.DeclareTableStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DeclareTableStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r563

                bool r563 = true;
                r563 = r563 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r563 = r563 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r563 = r563 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDeclaration());
                CommitOrRollback(r563, parser);
                res = r563;
            }



            return res;
        }
    }

    public partial class TableDeclaration : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableDeclaration()
            :base()
        {
        }

        public TableDeclaration(Jhu.Graywulf.Sql.Parsing.TableDeclaration old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableDeclaration(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r564

                bool r564 = true;
                r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r564 = r564 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r564)
                { // may a565
                    bool a565 = false;
                    {
                        Checkpoint(parser); // r566

                        bool r566 = true;
                        r566 = r566 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r566, parser);
                        a565 = r566;
                    }

                    r564 |= a565;
                } // end may a565

                r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r564)
                { // may a567
                    bool a567 = false;
                    {
                        Checkpoint(parser); // r568

                        bool r568 = true;
                        r568 = r568 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r568, parser);
                        a567 = r568;
                    }

                    r564 |= a567;
                } // end may a567

                r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                if (r564)
                { // may a569
                    bool a569 = false;
                    {
                        Checkpoint(parser); // r570

                        bool r570 = true;
                        r570 = r570 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r570, parser);
                        a569 = r570;
                    }

                    r564 |= a569;
                } // end may a569

                r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r564, parser);
                res = r564;
            }



            return res;
        }
    }

    public partial class CommonTableExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CommonTableExpression()
            :base()
        {
        }

        public CommonTableExpression(Jhu.Graywulf.Sql.Parsing.CommonTableExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CommonTableExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r571

                bool r571 = true;
                r571 = r571 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                if (r571)
                { // may a572
                    bool a572 = false;
                    {
                        Checkpoint(parser); // r573

                        bool r573 = true;
                        r573 = r573 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r573, parser);
                        a572 = r573;
                    }

                    r571 |= a572;
                } // end may a572

                r571 = r571 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList());
                CommitOrRollback(r571, parser);
                res = r571;
            }



            return res;
        }
    }

    public partial class CommonTableSpecificationList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CommonTableSpecificationList()
            :base()
        {
        }

        public CommonTableSpecificationList(Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r574

                bool r574 = true;
                r574 = r574 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecification());
                if (r574)
                { // may a575
                    bool a575 = false;
                    {
                        Checkpoint(parser); // r576

                        bool r576 = true;
                        if (r576)
                        { // may a577
                            bool a577 = false;
                            {
                                Checkpoint(parser); // r578

                                bool r578 = true;
                                r578 = r578 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r578, parser);
                                a577 = r578;
                            }

                            r576 |= a577;
                        } // end may a577

                        r576 = r576 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r576)
                        { // may a579
                            bool a579 = false;
                            {
                                Checkpoint(parser); // r580

                                bool r580 = true;
                                r580 = r580 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r580, parser);
                                a579 = r580;
                            }

                            r576 |= a579;
                        } // end may a579

                        r576 = r576 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList());
                        CommitOrRollback(r576, parser);
                        a575 = r576;
                    }

                    r574 |= a575;
                } // end may a575

                CommitOrRollback(r574, parser);
                res = r574;
            }



            return res;
        }
    }

    public partial class CommonTableSpecification : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CommonTableSpecification()
            :base()
        {
        }

        public CommonTableSpecification(Jhu.Graywulf.Sql.Parsing.CommonTableSpecification old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CommonTableSpecification(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r581

                bool r581 = true;
                r581 = r581 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                if (r581)
                { // may a582
                    bool a582 = false;
                    {
                        Checkpoint(parser); // r583

                        bool r583 = true;
                        if (r583)
                        { // may a584
                            bool a584 = false;
                            {
                                Checkpoint(parser); // r585

                                bool r585 = true;
                                r585 = r585 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r585, parser);
                                a584 = r585;
                            }

                            r583 |= a584;
                        } // end may a584

                        r583 = r583 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets());
                        CommitOrRollback(r583, parser);
                        a582 = r583;
                    }

                    r581 |= a582;
                } // end may a582

                if (r581)
                { // may a586
                    bool a586 = false;
                    {
                        Checkpoint(parser); // r587

                        bool r587 = true;
                        r587 = r587 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r587, parser);
                        a586 = r587;
                    }

                    r581 |= a586;
                } // end may a586

                r581 = r581 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                if (r581)
                { // may a588
                    bool a588 = false;
                    {
                        Checkpoint(parser); // r589

                        bool r589 = true;
                        r589 = r589 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r589, parser);
                        a588 = r589;
                    }

                    r581 |= a588;
                } // end may a588

                r581 = r581 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                CommitOrRollback(r581, parser);
                res = r581;
            }



            return res;
        }
    }

    public partial class SelectStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SelectStatement()
            :base()
        {
        }

        public SelectStatement(Jhu.Graywulf.Sql.Parsing.SelectStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SelectStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r590

                bool r590 = true;
                if (r590)
                { // may a591
                    bool a591 = false;
                    {
                        Checkpoint(parser); // r592

                        bool r592 = true;
                        r592 = r592 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r592)
                        { // may a593
                            bool a593 = false;
                            {
                                Checkpoint(parser); // r594

                                bool r594 = true;
                                r594 = r594 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r594, parser);
                                a593 = r594;
                            }

                            r592 |= a593;
                        } // end may a593

                        CommitOrRollback(r592, parser);
                        a591 = r592;
                    }

                    r590 |= a591;
                } // end may a591

                r590 = r590 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r590)
                { // may a595
                    bool a595 = false;
                    {
                        Checkpoint(parser); // r596

                        bool r596 = true;
                        if (r596)
                        { // may a597
                            bool a597 = false;
                            {
                                Checkpoint(parser); // r598

                                bool r598 = true;
                                r598 = r598 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r598, parser);
                                a597 = r598;
                            }

                            r596 |= a597;
                        } // end may a597

                        r596 = r596 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        CommitOrRollback(r596, parser);
                        a595 = r596;
                    }

                    r590 |= a595;
                } // end may a595

                if (r590)
                { // may a599
                    bool a599 = false;
                    {
                        Checkpoint(parser); // r600

                        bool r600 = true;
                        if (r600)
                        { // may a601
                            bool a601 = false;
                            {
                                Checkpoint(parser); // r602

                                bool r602 = true;
                                r602 = r602 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r602, parser);
                                a601 = r602;
                            }

                            r600 |= a601;
                        } // end may a601

                        r600 = r600 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r600, parser);
                        a599 = r600;
                    }

                    r590 |= a599;
                } // end may a599

                CommitOrRollback(r590, parser);
                res = r590;
            }



            return res;
        }
    }

    public partial class Subquery : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public Subquery()
            :base()
        {
        }

        public Subquery(Jhu.Graywulf.Sql.Parsing.Subquery old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.Subquery(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r603

                bool r603 = true;
                r603 = r603 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r603)
                { // may a604
                    bool a604 = false;
                    {
                        Checkpoint(parser); // r605

                        bool r605 = true;
                        r605 = r605 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r605, parser);
                        a604 = r605;
                    }

                    r603 |= a604;
                } // end may a604

                r603 = r603 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r603)
                { // may a606
                    bool a606 = false;
                    {
                        Checkpoint(parser); // r607

                        bool r607 = true;
                        if (r607)
                        { // may a608
                            bool a608 = false;
                            {
                                Checkpoint(parser); // r609

                                bool r609 = true;
                                r609 = r609 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r609, parser);
                                a608 = r609;
                            }

                            r607 |= a608;
                        } // end may a608

                        r607 = r607 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        CommitOrRollback(r607, parser);
                        a606 = r607;
                    }

                    r603 |= a606;
                } // end may a606

                if (r603)
                { // may a610
                    bool a610 = false;
                    {
                        Checkpoint(parser); // r611

                        bool r611 = true;
                        r611 = r611 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r611, parser);
                        a610 = r611;
                    }

                    r603 |= a610;
                } // end may a610

                r603 = r603 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r603, parser);
                res = r603;
            }



            return res;
        }
    }

    public partial class QueryExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryExpression()
            :base()
        {
        }

        public QueryExpression(Jhu.Graywulf.Sql.Parsing.QueryExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r612

                bool r612 = true;
                if (r612)
                { // alternatives a613 must
                    bool a613 = false;
                    if (!a613)
                    {
                        Checkpoint(parser); // r614

                        bool r614 = true;
                        r614 = r614 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpressionBrackets());
                        CommitOrRollback(r614, parser);
                        a613 = r614;
                    }

                    if (!a613)
                    {
                        Checkpoint(parser); // r615

                        bool r615 = true;
                        r615 = r615 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QuerySpecification());
                        CommitOrRollback(r615, parser);
                        a613 = r615;
                    }

                    r612 &= a613;

                } // end alternatives a613

                if (r612)
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
                                r619 = r619 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r619, parser);
                                a618 = r619;
                            }

                            r617 |= a618;
                        } // end may a618

                        r617 = r617 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryOperator());
                        if (r617)
                        { // may a620
                            bool a620 = false;
                            {
                                Checkpoint(parser); // r621

                                bool r621 = true;
                                r621 = r621 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r621, parser);
                                a620 = r621;
                            }

                            r617 |= a620;
                        } // end may a620

                        r617 = r617 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                        CommitOrRollback(r617, parser);
                        a616 = r617;
                    }

                    r612 |= a616;
                } // end may a616

                CommitOrRollback(r612, parser);
                res = r612;
            }



            return res;
        }
    }

    public partial class QueryExpressionBrackets : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryExpressionBrackets()
            :base()
        {
        }

        public QueryExpressionBrackets(Jhu.Graywulf.Sql.Parsing.QueryExpressionBrackets old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryExpressionBrackets(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r622

                bool r622 = true;
                r622 = r622 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r622)
                { // may a623
                    bool a623 = false;
                    {
                        Checkpoint(parser); // r624

                        bool r624 = true;
                        r624 = r624 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r624, parser);
                        a623 = r624;
                    }

                    r622 |= a623;
                } // end may a623

                r622 = r622 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r622)
                { // may a625
                    bool a625 = false;
                    {
                        Checkpoint(parser); // r626

                        bool r626 = true;
                        r626 = r626 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r626, parser);
                        a625 = r626;
                    }

                    r622 |= a625;
                } // end may a625

                r622 = r622 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r622, parser);
                res = r622;
            }



            return res;
        }
    }

    public partial class QueryOperator : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryOperator()
            :base()
        {
        }

        public QueryOperator(Jhu.Graywulf.Sql.Parsing.QueryOperator old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryOperator(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r627

                bool r627 = true;
                if (r627)
                { // alternatives a628 must
                    bool a628 = false;
                    if (!a628)
                    {
                        Checkpoint(parser); // r629

                        bool r629 = true;
                        r629 = r629 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNION"));
                        if (r629)
                        { // may a630
                            bool a630 = false;
                            {
                                Checkpoint(parser); // r631

                                bool r631 = true;
                                r631 = r631 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r631 = r631 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r631, parser);
                                a630 = r631;
                            }

                            r629 |= a630;
                        } // end may a630

                        CommitOrRollback(r629, parser);
                        a628 = r629;
                    }

                    if (!a628)
                    {
                        Checkpoint(parser); // r632

                        bool r632 = true;
                        r632 = r632 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"EXCEPT"));
                        CommitOrRollback(r632, parser);
                        a628 = r632;
                    }

                    if (!a628)
                    {
                        Checkpoint(parser); // r633

                        bool r633 = true;
                        r633 = r633 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTERSECT"));
                        CommitOrRollback(r633, parser);
                        a628 = r633;
                    }

                    r627 &= a628;

                } // end alternatives a628

                CommitOrRollback(r627, parser);
                res = r627;
            }



            return res;
        }
    }

    public partial class QuerySpecification : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QuerySpecification()
            :base()
        {
        }

        public QuerySpecification(Jhu.Graywulf.Sql.Parsing.QuerySpecification old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QuerySpecification(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r634

                bool r634 = true;
                r634 = r634 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SELECT"));
                if (r634)
                { // may a635
                    bool a635 = false;
                    {
                        Checkpoint(parser); // r636

                        bool r636 = true;
                        r636 = r636 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r636)
                        { // alternatives a637 must
                            bool a637 = false;
                            if (!a637)
                            {
                                Checkpoint(parser); // r638

                                bool r638 = true;
                                r638 = r638 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r638, parser);
                                a637 = r638;
                            }

                            if (!a637)
                            {
                                Checkpoint(parser); // r639

                                bool r639 = true;
                                r639 = r639 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DISTINCT"));
                                CommitOrRollback(r639, parser);
                                a637 = r639;
                            }

                            r636 &= a637;

                        } // end alternatives a637

                        CommitOrRollback(r636, parser);
                        a635 = r636;
                    }

                    r634 |= a635;
                } // end may a635

                if (r634)
                { // may a640
                    bool a640 = false;
                    {
                        Checkpoint(parser); // r641

                        bool r641 = true;
                        r641 = r641 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r641 = r641 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TopExpression());
                        CommitOrRollback(r641, parser);
                        a640 = r641;
                    }

                    r634 |= a640;
                } // end may a640

                if (r634)
                { // may a642
                    bool a642 = false;
                    {
                        Checkpoint(parser); // r643

                        bool r643 = true;
                        r643 = r643 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r643, parser);
                        a642 = r643;
                    }

                    r634 |= a642;
                } // end may a642

                r634 = r634 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectList());
                if (r634)
                { // may a644
                    bool a644 = false;
                    {
                        Checkpoint(parser); // r645

                        bool r645 = true;
                        if (r645)
                        { // may a646
                            bool a646 = false;
                            {
                                Checkpoint(parser); // r647

                                bool r647 = true;
                                r647 = r647 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r647, parser);
                                a646 = r647;
                            }

                            r645 |= a646;
                        } // end may a646

                        r645 = r645 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IntoClause());
                        CommitOrRollback(r645, parser);
                        a644 = r645;
                    }

                    r634 |= a644;
                } // end may a644

                if (r634)
                { // may a648
                    bool a648 = false;
                    {
                        Checkpoint(parser); // r649

                        bool r649 = true;
                        if (r649)
                        { // may a650
                            bool a650 = false;
                            {
                                Checkpoint(parser); // r651

                                bool r651 = true;
                                r651 = r651 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r651, parser);
                                a650 = r651;
                            }

                            r649 |= a650;
                        } // end may a650

                        r649 = r649 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r649, parser);
                        a648 = r649;
                    }

                    r634 |= a648;
                } // end may a648

                if (r634)
                { // may a652
                    bool a652 = false;
                    {
                        Checkpoint(parser); // r653

                        bool r653 = true;
                        if (r653)
                        { // may a654
                            bool a654 = false;
                            {
                                Checkpoint(parser); // r655

                                bool r655 = true;
                                r655 = r655 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r655, parser);
                                a654 = r655;
                            }

                            r653 |= a654;
                        } // end may a654

                        r653 = r653 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r653, parser);
                        a652 = r653;
                    }

                    r634 |= a652;
                } // end may a652

                if (r634)
                { // may a656
                    bool a656 = false;
                    {
                        Checkpoint(parser); // r657

                        bool r657 = true;
                        if (r657)
                        { // may a658
                            bool a658 = false;
                            {
                                Checkpoint(parser); // r659

                                bool r659 = true;
                                r659 = r659 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r659, parser);
                                a658 = r659;
                            }

                            r657 |= a658;
                        } // end may a658

                        r657 = r657 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByClause());
                        CommitOrRollback(r657, parser);
                        a656 = r657;
                    }

                    r634 |= a656;
                } // end may a656

                if (r634)
                { // may a660
                    bool a660 = false;
                    {
                        Checkpoint(parser); // r661

                        bool r661 = true;
                        if (r661)
                        { // may a662
                            bool a662 = false;
                            {
                                Checkpoint(parser); // r663

                                bool r663 = true;
                                r663 = r663 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r663, parser);
                                a662 = r663;
                            }

                            r661 |= a662;
                        } // end may a662

                        r661 = r661 && Match(parser, new Jhu.Graywulf.Sql.Parsing.HavingClause());
                        CommitOrRollback(r661, parser);
                        a660 = r661;
                    }

                    r634 |= a660;
                } // end may a660

                CommitOrRollback(r634, parser);
                res = r634;
            }



            return res;
        }
    }

    public partial class TopExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TopExpression()
            :base()
        {
        }

        public TopExpression(Jhu.Graywulf.Sql.Parsing.TopExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TopExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r664

                bool r664 = true;
                r664 = r664 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TOP"));
                r664 = r664 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r664 = r664 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r664)
                { // may a665
                    bool a665 = false;
                    {
                        Checkpoint(parser); // r666

                        bool r666 = true;
                        r666 = r666 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r666 = r666 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PERCENT"));
                        CommitOrRollback(r666, parser);
                        a665 = r666;
                    }

                    r664 |= a665;
                } // end may a665

                if (r664)
                { // may a667
                    bool a667 = false;
                    {
                        Checkpoint(parser); // r668

                        bool r668 = true;
                        r668 = r668 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r668 = r668 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                        r668 = r668 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r668 = r668 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TIES"));
                        CommitOrRollback(r668, parser);
                        a667 = r668;
                    }

                    r664 |= a667;
                } // end may a667

                CommitOrRollback(r664, parser);
                res = r664;
            }



            return res;
        }
    }

    public partial class SelectList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SelectList()
            :base()
        {
        }

        public SelectList(Jhu.Graywulf.Sql.Parsing.SelectList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SelectList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r669

                bool r669 = true;
                r669 = r669 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnExpression());
                if (r669)
                { // may a670
                    bool a670 = false;
                    {
                        Checkpoint(parser); // r671

                        bool r671 = true;
                        if (r671)
                        { // may a672
                            bool a672 = false;
                            {
                                Checkpoint(parser); // r673

                                bool r673 = true;
                                r673 = r673 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r673, parser);
                                a672 = r673;
                            }

                            r671 |= a672;
                        } // end may a672

                        r671 = r671 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r671)
                        { // may a674
                            bool a674 = false;
                            {
                                Checkpoint(parser); // r675

                                bool r675 = true;
                                r675 = r675 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r675, parser);
                                a674 = r675;
                            }

                            r671 |= a674;
                        } // end may a674

                        r671 = r671 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectList());
                        CommitOrRollback(r671, parser);
                        a670 = r671;
                    }

                    r669 |= a670;
                } // end may a670

                CommitOrRollback(r669, parser);
                res = r669;
            }



            return res;
        }
    }

    public partial class ColumnExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnExpression()
            :base()
        {
        }

        public ColumnExpression(Jhu.Graywulf.Sql.Parsing.ColumnExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r676

                bool r676 = true;
                if (r676)
                { // alternatives a677 must
                    bool a677 = false;
                    if (!a677)
                    {
                        Checkpoint(parser); // r678

                        bool r678 = true;
                        r678 = r678 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        if (r678)
                        { // may a679
                            bool a679 = false;
                            {
                                Checkpoint(parser); // r680

                                bool r680 = true;
                                r680 = r680 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r680, parser);
                                a679 = r680;
                            }

                            r678 |= a679;
                        } // end may a679

                        r678 = r678 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r678)
                        { // may a681
                            bool a681 = false;
                            {
                                Checkpoint(parser); // r682

                                bool r682 = true;
                                r682 = r682 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r682, parser);
                                a681 = r682;
                            }

                            r678 |= a681;
                        } // end may a681

                        r678 = r678 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r678, parser);
                        a677 = r678;
                    }

                    if (!a677)
                    {
                        Checkpoint(parser); // r683

                        bool r683 = true;
                        r683 = r683 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                        if (r683)
                        { // may a684
                            bool a684 = false;
                            {
                                Checkpoint(parser); // r685

                                bool r685 = true;
                                r685 = r685 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r685, parser);
                                a684 = r685;
                            }

                            r683 |= a684;
                        } // end may a684

                        r683 = r683 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r683)
                        { // may a686
                            bool a686 = false;
                            {
                                Checkpoint(parser); // r687

                                bool r687 = true;
                                r687 = r687 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r687, parser);
                                a686 = r687;
                            }

                            r683 |= a686;
                        } // end may a686

                        r683 = r683 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r683, parser);
                        a677 = r683;
                    }

                    if (!a677)
                    {
                        Checkpoint(parser); // r688

                        bool r688 = true;
                        r688 = r688 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r688)
                        { // may a689
                            bool a689 = false;
                            {
                                Checkpoint(parser); // r690

                                bool r690 = true;
                                if (r690)
                                { // may a691
                                    bool a691 = false;
                                    {
                                        Checkpoint(parser); // r692

                                        bool r692 = true;
                                        r692 = r692 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        r692 = r692 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                        CommitOrRollback(r692, parser);
                                        a691 = r692;
                                    }

                                    r690 |= a691;
                                } // end may a691

                                r690 = r690 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r690 = r690 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                                CommitOrRollback(r690, parser);
                                a689 = r690;
                            }

                            r688 |= a689;
                        } // end may a689

                        CommitOrRollback(r688, parser);
                        a677 = r688;
                    }

                    r676 &= a677;

                } // end alternatives a677

                CommitOrRollback(r676, parser);
                res = r676;
            }



            return res;
        }
    }

    public partial class IntoClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IntoClause()
            :base()
        {
        }

        public IntoClause(Jhu.Graywulf.Sql.Parsing.IntoClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IntoClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r693

                bool r693 = true;
                r693 = r693 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTO"));
                if (r693)
                { // may a694
                    bool a694 = false;
                    {
                        Checkpoint(parser); // r695

                        bool r695 = true;
                        r695 = r695 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r695, parser);
                        a694 = r695;
                    }

                    r693 |= a694;
                } // end may a694

                r693 = r693 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                CommitOrRollback(r693, parser);
                res = r693;
            }



            return res;
        }
    }

    public partial class TargetTableSpecification : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TargetTableSpecification()
            :base()
        {
        }

        public TargetTableSpecification(Jhu.Graywulf.Sql.Parsing.TargetTableSpecification old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r696

                bool r696 = true;
                if (r696)
                { // alternatives a697 must
                    bool a697 = false;
                    if (!a697)
                    {
                        Checkpoint(parser); // r698

                        bool r698 = true;
                        r698 = r698 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r698, parser);
                        a697 = r698;
                    }

                    if (!a697)
                    {
                        Checkpoint(parser); // r699

                        bool r699 = true;
                        r699 = r699 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                        CommitOrRollback(r699, parser);
                        a697 = r699;
                    }

                    r696 &= a697;

                } // end alternatives a697

                if (r696)
                { // may a700
                    bool a700 = false;
                    {
                        Checkpoint(parser); // r701

                        bool r701 = true;
                        if (r701)
                        { // may a702
                            bool a702 = false;
                            {
                                Checkpoint(parser); // r703

                                bool r703 = true;
                                r703 = r703 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r703, parser);
                                a702 = r703;
                            }

                            r701 |= a702;
                        } // end may a702

                        r701 = r701 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintClause());
                        CommitOrRollback(r701, parser);
                        a700 = r701;
                    }

                    r696 |= a700;
                } // end may a700

                CommitOrRollback(r696, parser);
                res = r696;
            }



            return res;
        }
    }

    public partial class FromClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FromClause()
            :base()
        {
        }

        public FromClause(Jhu.Graywulf.Sql.Parsing.FromClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FromClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r704

                bool r704 = true;
                r704 = r704 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                if (r704)
                { // may a705
                    bool a705 = false;
                    {
                        Checkpoint(parser); // r706

                        bool r706 = true;
                        r706 = r706 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r706, parser);
                        a705 = r706;
                    }

                    r704 |= a705;
                } // end may a705

                r704 = r704 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSourceExpression());
                CommitOrRollback(r704, parser);
                res = r704;
            }



            return res;
        }
    }

    public partial class TableSourceExpression : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableSourceExpression()
            :base()
        {
        }

        public TableSourceExpression(Jhu.Graywulf.Sql.Parsing.TableSourceExpression old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableSourceExpression(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r707

                bool r707 = true;
                r707 = r707 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                if (r707)
                { // may a708
                    bool a708 = false;
                    {
                        Checkpoint(parser); // r709

                        bool r709 = true;
                        if (r709)
                        { // may a710
                            bool a710 = false;
                            {
                                Checkpoint(parser); // r711

                                bool r711 = true;
                                r711 = r711 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r711, parser);
                                a710 = r711;
                            }

                            r709 |= a710;
                        } // end may a710

                        r709 = r709 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinedTable());
                        CommitOrRollback(r709, parser);
                        a708 = r709;
                    }

                    r707 |= a708;
                } // end may a708

                CommitOrRollback(r707, parser);
                res = r707;
            }



            return res;
        }
    }

    public partial class JoinedTable : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public JoinedTable()
            :base()
        {
        }

        public JoinedTable(Jhu.Graywulf.Sql.Parsing.JoinedTable old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.JoinedTable(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r712

                bool r712 = true;
                if (r712)
                { // alternatives a713 must
                    bool a713 = false;
                    if (!a713)
                    {
                        Checkpoint(parser); // r714

                        bool r714 = true;
                        r714 = r714 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinType());
                        if (r714)
                        { // may a715
                            bool a715 = false;
                            {
                                Checkpoint(parser); // r716

                                bool r716 = true;
                                r716 = r716 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r716, parser);
                                a715 = r716;
                            }

                            r714 |= a715;
                        } // end may a715

                        r714 = r714 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        if (r714)
                        { // may a717
                            bool a717 = false;
                            {
                                Checkpoint(parser); // r718

                                bool r718 = true;
                                r718 = r718 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r718, parser);
                                a717 = r718;
                            }

                            r714 |= a717;
                        } // end may a717

                        r714 = r714 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                        if (r714)
                        { // may a719
                            bool a719 = false;
                            {
                                Checkpoint(parser); // r720

                                bool r720 = true;
                                r720 = r720 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r720, parser);
                                a719 = r720;
                            }

                            r714 |= a719;
                        } // end may a719

                        r714 = r714 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                        CommitOrRollback(r714, parser);
                        a713 = r714;
                    }

                    if (!a713)
                    {
                        Checkpoint(parser); // r721

                        bool r721 = true;
                        r721 = r721 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CROSS"));
                        r721 = r721 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r721 = r721 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"JOIN"));
                        if (r721)
                        { // may a722
                            bool a722 = false;
                            {
                                Checkpoint(parser); // r723

                                bool r723 = true;
                                r723 = r723 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r723, parser);
                                a722 = r723;
                            }

                            r721 |= a722;
                        } // end may a722

                        r721 = r721 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r721, parser);
                        a713 = r721;
                    }

                    if (!a713)
                    {
                        Checkpoint(parser); // r724

                        bool r724 = true;
                        r724 = r724 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r724)
                        { // may a725
                            bool a725 = false;
                            {
                                Checkpoint(parser); // r726

                                bool r726 = true;
                                r726 = r726 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r726, parser);
                                a725 = r726;
                            }

                            r724 |= a725;
                        } // end may a725

                        r724 = r724 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r724, parser);
                        a713 = r724;
                    }

                    if (!a713)
                    {
                        Checkpoint(parser); // r727

                        bool r727 = true;
                        if (r727)
                        { // alternatives a728 must
                            bool a728 = false;
                            if (!a728)
                            {
                                Checkpoint(parser); // r729

                                bool r729 = true;
                                r729 = r729 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CROSS"));
                                CommitOrRollback(r729, parser);
                                a728 = r729;
                            }

                            if (!a728)
                            {
                                Checkpoint(parser); // r730

                                bool r730 = true;
                                r730 = r730 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OUTER"));
                                CommitOrRollback(r730, parser);
                                a728 = r730;
                            }

                            r727 &= a728;

                        } // end alternatives a728

                        r727 = r727 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r727 = r727 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"APPLY"));
                        if (r727)
                        { // may a731
                            bool a731 = false;
                            {
                                Checkpoint(parser); // r732

                                bool r732 = true;
                                r732 = r732 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r732, parser);
                                a731 = r732;
                            }

                            r727 |= a731;
                        } // end may a731

                        r727 = r727 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r727, parser);
                        a713 = r727;
                    }

                    r712 &= a713;

                } // end alternatives a713

                if (r712)
                { // may a733
                    bool a733 = false;
                    {
                        Checkpoint(parser); // r734

                        bool r734 = true;
                        if (r734)
                        { // may a735
                            bool a735 = false;
                            {
                                Checkpoint(parser); // r736

                                bool r736 = true;
                                r736 = r736 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r736, parser);
                                a735 = r736;
                            }

                            r734 |= a735;
                        } // end may a735

                        r734 = r734 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinedTable());
                        CommitOrRollback(r734, parser);
                        a733 = r734;
                    }

                    r712 |= a733;
                } // end may a733

                CommitOrRollback(r712, parser);
                res = r712;
            }



            return res;
        }
    }

    public partial class JoinType : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public JoinType()
            :base()
        {
        }

        public JoinType(Jhu.Graywulf.Sql.Parsing.JoinType old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.JoinType(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r737

                bool r737 = true;
                if (r737)
                { // may a738
                    bool a738 = false;
                    {
                        Checkpoint(parser); // r739

                        bool r739 = true;
                        if (r739)
                        { // alternatives a740 must
                            bool a740 = false;
                            if (!a740)
                            {
                                Checkpoint(parser); // r741

                                bool r741 = true;
                                r741 = r741 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INNER"));
                                CommitOrRollback(r741, parser);
                                a740 = r741;
                            }

                            if (!a740)
                            {
                                Checkpoint(parser); // r742

                                bool r742 = true;
                                if (r742)
                                { // alternatives a743 must
                                    bool a743 = false;
                                    if (!a743)
                                    {
                                        Checkpoint(parser); // r744

                                        bool r744 = true;
                                        r744 = r744 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LEFT"));
                                        CommitOrRollback(r744, parser);
                                        a743 = r744;
                                    }

                                    if (!a743)
                                    {
                                        Checkpoint(parser); // r745

                                        bool r745 = true;
                                        r745 = r745 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RIGHT"));
                                        CommitOrRollback(r745, parser);
                                        a743 = r745;
                                    }

                                    if (!a743)
                                    {
                                        Checkpoint(parser); // r746

                                        bool r746 = true;
                                        r746 = r746 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FULL"));
                                        CommitOrRollback(r746, parser);
                                        a743 = r746;
                                    }

                                    r742 &= a743;

                                } // end alternatives a743

                                if (r742)
                                { // may a747
                                    bool a747 = false;
                                    {
                                        Checkpoint(parser); // r748

                                        bool r748 = true;
                                        r748 = r748 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        r748 = r748 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OUTER"));
                                        CommitOrRollback(r748, parser);
                                        a747 = r748;
                                    }

                                    r742 |= a747;
                                } // end may a747

                                CommitOrRollback(r742, parser);
                                a740 = r742;
                            }

                            r739 &= a740;

                        } // end alternatives a740

                        if (r739)
                        { // may a749
                            bool a749 = false;
                            {
                                Checkpoint(parser); // r750

                                bool r750 = true;
                                r750 = r750 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r750, parser);
                                a749 = r750;
                            }

                            r739 |= a749;
                        } // end may a749

                        if (r739)
                        { // may a751
                            bool a751 = false;
                            {
                                Checkpoint(parser); // r752

                                bool r752 = true;
                                r752 = r752 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinHint());
                                CommitOrRollback(r752, parser);
                                a751 = r752;
                            }

                            r739 |= a751;
                        } // end may a751

                        CommitOrRollback(r739, parser);
                        a738 = r739;
                    }

                    r737 |= a738;
                } // end may a738

                if (r737)
                { // may a753
                    bool a753 = false;
                    {
                        Checkpoint(parser); // r754

                        bool r754 = true;
                        r754 = r754 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r754, parser);
                        a753 = r754;
                    }

                    r737 |= a753;
                } // end may a753

                r737 = r737 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"JOIN"));
                CommitOrRollback(r737, parser);
                res = r737;
            }



            return res;
        }
    }

    public partial class JoinHint : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public JoinHint()
            :base()
        {
        }

        public JoinHint(Jhu.Graywulf.Sql.Parsing.JoinHint old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.JoinHint(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r755

                bool r755 = true;
                if (r755)
                { // alternatives a756 must
                    bool a756 = false;
                    if (!a756)
                    {
                        Checkpoint(parser); // r757

                        bool r757 = true;
                        r757 = r757 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LOOP"));
                        CommitOrRollback(r757, parser);
                        a756 = r757;
                    }

                    if (!a756)
                    {
                        Checkpoint(parser); // r758

                        bool r758 = true;
                        r758 = r758 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"HASH"));
                        CommitOrRollback(r758, parser);
                        a756 = r758;
                    }

                    if (!a756)
                    {
                        Checkpoint(parser); // r759

                        bool r759 = true;
                        r759 = r759 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"MERGE"));
                        CommitOrRollback(r759, parser);
                        a756 = r759;
                    }

                    if (!a756)
                    {
                        Checkpoint(parser); // r760

                        bool r760 = true;
                        r760 = r760 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"REMOTE"));
                        CommitOrRollback(r760, parser);
                        a756 = r760;
                    }

                    r755 &= a756;

                } // end alternatives a756

                CommitOrRollback(r755, parser);
                res = r755;
            }



            return res;
        }
    }

    public partial class TableSource : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableSource()
            :base()
        {
        }

        public TableSource(Jhu.Graywulf.Sql.Parsing.TableSource old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableSource(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r761

                bool r761 = true;
                if (r761)
                { // alternatives a762 must
                    bool a762 = false;
                    if (!a762)
                    {
                        Checkpoint(parser); // r763

                        bool r763 = true;
                        r763 = r763 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionTableSource());
                        CommitOrRollback(r763, parser);
                        a762 = r763;
                    }

                    if (!a762)
                    {
                        Checkpoint(parser); // r764

                        bool r764 = true;
                        r764 = r764 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleTableSource());
                        CommitOrRollback(r764, parser);
                        a762 = r764;
                    }

                    if (!a762)
                    {
                        Checkpoint(parser); // r765

                        bool r765 = true;
                        r765 = r765 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableTableSource());
                        CommitOrRollback(r765, parser);
                        a762 = r765;
                    }

                    if (!a762)
                    {
                        Checkpoint(parser); // r766

                        bool r766 = true;
                        r766 = r766 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SubqueryTableSource());
                        CommitOrRollback(r766, parser);
                        a762 = r766;
                    }

                    r761 &= a762;

                } // end alternatives a762

                CommitOrRollback(r761, parser);
                res = r761;
            }



            return res;
        }
    }

    public partial class SimpleTableSource : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SimpleTableSource()
            :base()
        {
        }

        public SimpleTableSource(Jhu.Graywulf.Sql.Parsing.SimpleTableSource old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SimpleTableSource(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r767

                bool r767 = true;
                r767 = r767 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r767)
                { // may a768
                    bool a768 = false;
                    {
                        Checkpoint(parser); // r769

                        bool r769 = true;
                        r769 = r769 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r769)
                        { // may a770
                            bool a770 = false;
                            {
                                Checkpoint(parser); // r771

                                bool r771 = true;
                                r771 = r771 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                r771 = r771 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r771, parser);
                                a770 = r771;
                            }

                            r769 |= a770;
                        } // end may a770

                        r769 = r769 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                        CommitOrRollback(r769, parser);
                        a768 = r769;
                    }

                    r767 |= a768;
                } // end may a768

                if (r767)
                { // may a772
                    bool a772 = false;
                    {
                        Checkpoint(parser); // r773

                        bool r773 = true;
                        r773 = r773 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r773 = r773 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSampleClause());
                        CommitOrRollback(r773, parser);
                        a772 = r773;
                    }

                    r767 |= a772;
                } // end may a772

                if (r767)
                { // may a774
                    bool a774 = false;
                    {
                        Checkpoint(parser); // r775

                        bool r775 = true;
                        r775 = r775 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r775 = r775 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintClause());
                        CommitOrRollback(r775, parser);
                        a774 = r775;
                    }

                    r767 |= a774;
                } // end may a774

                if (r767)
                { // may a776
                    bool a776 = false;
                    {
                        Checkpoint(parser); // r777

                        bool r777 = true;
                        r777 = r777 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r777 = r777 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TablePartitionClause());
                        CommitOrRollback(r777, parser);
                        a776 = r777;
                    }

                    r767 |= a776;
                } // end may a776

                CommitOrRollback(r767, parser);
                res = r767;
            }



            return res;
        }
    }

    public partial class FunctionTableSource : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public FunctionTableSource()
            :base()
        {
        }

        public FunctionTableSource(Jhu.Graywulf.Sql.Parsing.FunctionTableSource old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.FunctionTableSource(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r778

                bool r778 = true;
                r778 = r778 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableValuedFunctionCall());
                if (r778)
                { // may a779
                    bool a779 = false;
                    {
                        Checkpoint(parser); // r780

                        bool r780 = true;
                        r780 = r780 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r780, parser);
                        a779 = r780;
                    }

                    r778 |= a779;
                } // end may a779

                if (r778)
                { // may a781
                    bool a781 = false;
                    {
                        Checkpoint(parser); // r782

                        bool r782 = true;
                        r782 = r782 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        r782 = r782 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r782, parser);
                        a781 = r782;
                    }

                    r778 |= a781;
                } // end may a781

                r778 = r778 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                if (r778)
                { // may a783
                    bool a783 = false;
                    {
                        Checkpoint(parser); // r784

                        bool r784 = true;
                        if (r784)
                        { // may a785
                            bool a785 = false;
                            {
                                Checkpoint(parser); // r786

                                bool r786 = true;
                                r786 = r786 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r786, parser);
                                a785 = r786;
                            }

                            r784 |= a785;
                        } // end may a785

                        r784 = r784 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r784)
                        { // may a787
                            bool a787 = false;
                            {
                                Checkpoint(parser); // r788

                                bool r788 = true;
                                r788 = r788 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r788, parser);
                                a787 = r788;
                            }

                            r784 |= a787;
                        } // end may a787

                        r784 = r784 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAliasList());
                        if (r784)
                        { // may a789
                            bool a789 = false;
                            {
                                Checkpoint(parser); // r790

                                bool r790 = true;
                                r790 = r790 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r790, parser);
                                a789 = r790;
                            }

                            r784 |= a789;
                        } // end may a789

                        r784 = r784 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r784, parser);
                        a783 = r784;
                    }

                    r778 |= a783;
                } // end may a783

                CommitOrRollback(r778, parser);
                res = r778;
            }



            return res;
        }
    }

    public partial class VariableTableSource : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public VariableTableSource()
            :base()
        {
        }

        public VariableTableSource(Jhu.Graywulf.Sql.Parsing.VariableTableSource old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.VariableTableSource(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r791

                bool r791 = true;
                r791 = r791 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableVariable());
                if (r791)
                { // may a792
                    bool a792 = false;
                    {
                        Checkpoint(parser); // r793

                        bool r793 = true;
                        if (r793)
                        { // may a794
                            bool a794 = false;
                            {
                                Checkpoint(parser); // r795

                                bool r795 = true;
                                r795 = r795 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r795, parser);
                                a794 = r795;
                            }

                            r793 |= a794;
                        } // end may a794

                        if (r793)
                        { // may a796
                            bool a796 = false;
                            {
                                Checkpoint(parser); // r797

                                bool r797 = true;
                                r797 = r797 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                if (r797)
                                { // may a798
                                    bool a798 = false;
                                    {
                                        Checkpoint(parser); // r799

                                        bool r799 = true;
                                        r799 = r799 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r799, parser);
                                        a798 = r799;
                                    }

                                    r797 |= a798;
                                } // end may a798

                                CommitOrRollback(r797, parser);
                                a796 = r797;
                            }

                            r793 |= a796;
                        } // end may a796

                        r793 = r793 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                        CommitOrRollback(r793, parser);
                        a792 = r793;
                    }

                    r791 |= a792;
                } // end may a792

                CommitOrRollback(r791, parser);
                res = r791;
            }



            return res;
        }
    }

    public partial class TableVariable : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableVariable()
            :base()
        {
        }

        public TableVariable(Jhu.Graywulf.Sql.Parsing.TableVariable old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableVariable(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r800

                bool r800 = true;
                r800 = r800 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                CommitOrRollback(r800, parser);
                res = r800;
            }



            return res;
        }
    }

    public partial class SubqueryTableSource : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public SubqueryTableSource()
            :base()
        {
        }

        public SubqueryTableSource(Jhu.Graywulf.Sql.Parsing.SubqueryTableSource old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.SubqueryTableSource(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r801

                bool r801 = true;
                r801 = r801 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                if (r801)
                { // may a802
                    bool a802 = false;
                    {
                        Checkpoint(parser); // r803

                        bool r803 = true;
                        r803 = r803 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r803, parser);
                        a802 = r803;
                    }

                    r801 |= a802;
                } // end may a802

                if (r801)
                { // may a804
                    bool a804 = false;
                    {
                        Checkpoint(parser); // r805

                        bool r805 = true;
                        r805 = r805 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        r805 = r805 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r805, parser);
                        a804 = r805;
                    }

                    r801 |= a804;
                } // end may a804

                r801 = r801 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                CommitOrRollback(r801, parser);
                res = r801;
            }



            return res;
        }
    }

    public partial class ColumnAliasList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnAliasList()
            :base()
        {
        }

        public ColumnAliasList(Jhu.Graywulf.Sql.Parsing.ColumnAliasList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnAliasList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r806

                bool r806 = true;
                r806 = r806 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                if (r806)
                { // may a807
                    bool a807 = false;
                    {
                        Checkpoint(parser); // r808

                        bool r808 = true;
                        if (r808)
                        { // may a809
                            bool a809 = false;
                            {
                                Checkpoint(parser); // r810

                                bool r810 = true;
                                r810 = r810 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r810, parser);
                                a809 = r810;
                            }

                            r808 |= a809;
                        } // end may a809

                        r808 = r808 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        r808 = r808 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAliasList());
                        CommitOrRollback(r808, parser);
                        a807 = r808;
                    }

                    r806 |= a807;
                } // end may a807

                CommitOrRollback(r806, parser);
                res = r806;
            }



            return res;
        }
    }

    public partial class TableSampleClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableSampleClause()
            :base()
        {
        }

        public TableSampleClause(Jhu.Graywulf.Sql.Parsing.TableSampleClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableSampleClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r811

                bool r811 = true;
                r811 = r811 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLESAMPLE"));
                if (r811)
                { // may a812
                    bool a812 = false;
                    {
                        Checkpoint(parser); // r813

                        bool r813 = true;
                        r813 = r813 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r813 = r813 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SYSTEM"));
                        CommitOrRollback(r813, parser);
                        a812 = r813;
                    }

                    r811 |= a812;
                } // end may a812

                if (r811)
                { // may a814
                    bool a814 = false;
                    {
                        Checkpoint(parser); // r815

                        bool r815 = true;
                        r815 = r815 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r815, parser);
                        a814 = r815;
                    }

                    r811 |= a814;
                } // end may a814

                r811 = r811 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r811)
                { // may a816
                    bool a816 = false;
                    {
                        Checkpoint(parser); // r817

                        bool r817 = true;
                        r817 = r817 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r817, parser);
                        a816 = r817;
                    }

                    r811 |= a816;
                } // end may a816

                r811 = r811 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SampleNumber());
                if (r811)
                { // may a818
                    bool a818 = false;
                    {
                        Checkpoint(parser); // r819

                        bool r819 = true;
                        r819 = r819 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r819)
                        { // may a820
                            bool a820 = false;
                            {
                                Checkpoint(parser); // r821

                                bool r821 = true;
                                if (r821)
                                { // alternatives a822 must
                                    bool a822 = false;
                                    if (!a822)
                                    {
                                        Checkpoint(parser); // r823

                                        bool r823 = true;
                                        r823 = r823 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PERCENT"));
                                        CommitOrRollback(r823, parser);
                                        a822 = r823;
                                    }

                                    if (!a822)
                                    {
                                        Checkpoint(parser); // r824

                                        bool r824 = true;
                                        r824 = r824 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ROWS"));
                                        CommitOrRollback(r824, parser);
                                        a822 = r824;
                                    }

                                    r821 &= a822;

                                } // end alternatives a822

                                CommitOrRollback(r821, parser);
                                a820 = r821;
                            }

                            r819 |= a820;
                        } // end may a820

                        CommitOrRollback(r819, parser);
                        a818 = r819;
                    }

                    r811 |= a818;
                } // end may a818

                if (r811)
                { // may a825
                    bool a825 = false;
                    {
                        Checkpoint(parser); // r826

                        bool r826 = true;
                        r826 = r826 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r826, parser);
                        a825 = r826;
                    }

                    r811 |= a825;
                } // end may a825

                r811 = r811 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                if (r811)
                { // may a827
                    bool a827 = false;
                    {
                        Checkpoint(parser); // r828

                        bool r828 = true;
                        r828 = r828 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r828 = r828 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"REPEATABLE"));
                        if (r828)
                        { // may a829
                            bool a829 = false;
                            {
                                Checkpoint(parser); // r830

                                bool r830 = true;
                                r830 = r830 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r830, parser);
                                a829 = r830;
                            }

                            r828 |= a829;
                        } // end may a829

                        r828 = r828 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r828)
                        { // may a831
                            bool a831 = false;
                            {
                                Checkpoint(parser); // r832

                                bool r832 = true;
                                r832 = r832 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r832, parser);
                                a831 = r832;
                            }

                            r828 |= a831;
                        } // end may a831

                        r828 = r828 && Match(parser, new Jhu.Graywulf.Sql.Parsing.RepeatSeed());
                        if (r828)
                        { // may a833
                            bool a833 = false;
                            {
                                Checkpoint(parser); // r834

                                bool r834 = true;
                                r834 = r834 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r834, parser);
                                a833 = r834;
                            }

                            r828 |= a833;
                        } // end may a833

                        r828 = r828 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r828, parser);
                        a827 = r828;
                    }

                    r811 |= a827;
                } // end may a827

                CommitOrRollback(r811, parser);
                res = r811;
            }



            return res;
        }
    }

    public partial class TablePartitionClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TablePartitionClause()
            :base()
        {
        }

        public TablePartitionClause(Jhu.Graywulf.Sql.Parsing.TablePartitionClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TablePartitionClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r835

                bool r835 = true;
                r835 = r835 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PARTITION"));
                r835 = r835 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r835 = r835 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                r835 = r835 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r835 = r835 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentifier());
                CommitOrRollback(r835, parser);
                res = r835;
            }



            return res;
        }
    }

    public partial class WhereClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public WhereClause()
            :base()
        {
        }

        public WhereClause(Jhu.Graywulf.Sql.Parsing.WhereClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.WhereClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r836

                bool r836 = true;
                r836 = r836 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHERE"));
                if (r836)
                { // may a837
                    bool a837 = false;
                    {
                        Checkpoint(parser); // r838

                        bool r838 = true;
                        r838 = r838 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r838, parser);
                        a837 = r838;
                    }

                    r836 |= a837;
                } // end may a837

                r836 = r836 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                CommitOrRollback(r836, parser);
                res = r836;
            }



            return res;
        }
    }

    public partial class GroupByClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public GroupByClause()
            :base()
        {
        }

        public GroupByClause(Jhu.Graywulf.Sql.Parsing.GroupByClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.GroupByClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r839

                bool r839 = true;
                r839 = r839 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"GROUP"));
                r839 = r839 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r839 = r839 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                if (r839)
                { // alternatives a840 must
                    bool a840 = false;
                    if (!a840)
                    {
                        Checkpoint(parser); // r841

                        bool r841 = true;
                        r841 = r841 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r841 = r841 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                        CommitOrRollback(r841, parser);
                        a840 = r841;
                    }

                    if (!a840)
                    {
                        Checkpoint(parser); // r842

                        bool r842 = true;
                        if (r842)
                        { // may a843
                            bool a843 = false;
                            {
                                Checkpoint(parser); // r844

                                bool r844 = true;
                                r844 = r844 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r844, parser);
                                a843 = r844;
                            }

                            r842 |= a843;
                        } // end may a843

                        r842 = r842 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByList());
                        CommitOrRollback(r842, parser);
                        a840 = r842;
                    }

                    r839 &= a840;

                } // end alternatives a840

                CommitOrRollback(r839, parser);
                res = r839;
            }



            return res;
        }
    }

    public partial class GroupByList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public GroupByList()
            :base()
        {
        }

        public GroupByList(Jhu.Graywulf.Sql.Parsing.GroupByList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.GroupByList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r845

                bool r845 = true;
                r845 = r845 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r845)
                { // may a846
                    bool a846 = false;
                    {
                        Checkpoint(parser); // r847

                        bool r847 = true;
                        if (r847)
                        { // may a848
                            bool a848 = false;
                            {
                                Checkpoint(parser); // r849

                                bool r849 = true;
                                r849 = r849 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r849, parser);
                                a848 = r849;
                            }

                            r847 |= a848;
                        } // end may a848

                        r847 = r847 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r847)
                        { // may a850
                            bool a850 = false;
                            {
                                Checkpoint(parser); // r851

                                bool r851 = true;
                                r851 = r851 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r851, parser);
                                a850 = r851;
                            }

                            r847 |= a850;
                        } // end may a850

                        r847 = r847 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByList());
                        CommitOrRollback(r847, parser);
                        a846 = r847;
                    }

                    r845 |= a846;
                } // end may a846

                CommitOrRollback(r845, parser);
                res = r845;
            }



            return res;
        }
    }

    public partial class HavingClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public HavingClause()
            :base()
        {
        }

        public HavingClause(Jhu.Graywulf.Sql.Parsing.HavingClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.HavingClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r852

                bool r852 = true;
                r852 = r852 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"HAVING"));
                if (r852)
                { // may a853
                    bool a853 = false;
                    {
                        Checkpoint(parser); // r854

                        bool r854 = true;
                        r854 = r854 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r854, parser);
                        a853 = r854;
                    }

                    r852 |= a853;
                } // end may a853

                r852 = r852 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                CommitOrRollback(r852, parser);
                res = r852;
            }



            return res;
        }
    }

    public partial class OrderByClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public OrderByClause()
            :base()
        {
        }

        public OrderByClause(Jhu.Graywulf.Sql.Parsing.OrderByClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.OrderByClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r855

                bool r855 = true;
                r855 = r855 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ORDER"));
                r855 = r855 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r855 = r855 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                if (r855)
                { // may a856
                    bool a856 = false;
                    {
                        Checkpoint(parser); // r857

                        bool r857 = true;
                        r857 = r857 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r857, parser);
                        a856 = r857;
                    }

                    r855 |= a856;
                } // end may a856

                r855 = r855 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByList());
                CommitOrRollback(r855, parser);
                res = r855;
            }



            return res;
        }
    }

    public partial class OrderByList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public OrderByList()
            :base()
        {
        }

        public OrderByList(Jhu.Graywulf.Sql.Parsing.OrderByList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.OrderByList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r858

                bool r858 = true;
                r858 = r858 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByArgument());
                if (r858)
                { // may a859
                    bool a859 = false;
                    {
                        Checkpoint(parser); // r860

                        bool r860 = true;
                        if (r860)
                        { // may a861
                            bool a861 = false;
                            {
                                Checkpoint(parser); // r862

                                bool r862 = true;
                                r862 = r862 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r862, parser);
                                a861 = r862;
                            }

                            r860 |= a861;
                        } // end may a861

                        r860 = r860 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r860)
                        { // may a863
                            bool a863 = false;
                            {
                                Checkpoint(parser); // r864

                                bool r864 = true;
                                r864 = r864 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r864, parser);
                                a863 = r864;
                            }

                            r860 |= a863;
                        } // end may a863

                        r860 = r860 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByList());
                        CommitOrRollback(r860, parser);
                        a859 = r860;
                    }

                    r858 |= a859;
                } // end may a859

                CommitOrRollback(r858, parser);
                res = r858;
            }



            return res;
        }
    }

    public partial class OrderByArgument : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public OrderByArgument()
            :base()
        {
        }

        public OrderByArgument(Jhu.Graywulf.Sql.Parsing.OrderByArgument old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.OrderByArgument(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r865

                bool r865 = true;
                r865 = r865 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r865)
                { // may a866
                    bool a866 = false;
                    {
                        Checkpoint(parser); // r867

                        bool r867 = true;
                        if (r867)
                        { // may a868
                            bool a868 = false;
                            {
                                Checkpoint(parser); // r869

                                bool r869 = true;
                                r869 = r869 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r869, parser);
                                a868 = r869;
                            }

                            r867 |= a868;
                        } // end may a868

                        if (r867)
                        { // alternatives a870 must
                            bool a870 = false;
                            if (!a870)
                            {
                                Checkpoint(parser); // r871

                                bool r871 = true;
                                r871 = r871 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ASC"));
                                CommitOrRollback(r871, parser);
                                a870 = r871;
                            }

                            if (!a870)
                            {
                                Checkpoint(parser); // r872

                                bool r872 = true;
                                r872 = r872 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DESC"));
                                CommitOrRollback(r872, parser);
                                a870 = r872;
                            }

                            r867 &= a870;

                        } // end alternatives a870

                        CommitOrRollback(r867, parser);
                        a866 = r867;
                    }

                    r865 |= a866;
                } // end may a866

                CommitOrRollback(r865, parser);
                res = r865;
            }



            return res;
        }
    }

    public partial class TableHintClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableHintClause()
            :base()
        {
        }

        public TableHintClause(Jhu.Graywulf.Sql.Parsing.TableHintClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableHintClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r873

                bool r873 = true;
                r873 = r873 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                if (r873)
                { // may a874
                    bool a874 = false;
                    {
                        Checkpoint(parser); // r875

                        bool r875 = true;
                        r875 = r875 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r875, parser);
                        a874 = r875;
                    }

                    r873 |= a874;
                } // end may a874

                r873 = r873 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r873)
                { // may a876
                    bool a876 = false;
                    {
                        Checkpoint(parser); // r877

                        bool r877 = true;
                        r877 = r877 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r877, parser);
                        a876 = r877;
                    }

                    r873 |= a876;
                } // end may a876

                r873 = r873 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintList());
                if (r873)
                { // may a878
                    bool a878 = false;
                    {
                        Checkpoint(parser); // r879

                        bool r879 = true;
                        r879 = r879 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r879, parser);
                        a878 = r879;
                    }

                    r873 |= a878;
                } // end may a878

                r873 = r873 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r873, parser);
                res = r873;
            }



            return res;
        }
    }

    public partial class TableHintList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableHintList()
            :base()
        {
        }

        public TableHintList(Jhu.Graywulf.Sql.Parsing.TableHintList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableHintList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r880

                bool r880 = true;
                r880 = r880 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHint());
                if (r880)
                { // may a881
                    bool a881 = false;
                    {
                        Checkpoint(parser); // r882

                        bool r882 = true;
                        if (r882)
                        { // may a883
                            bool a883 = false;
                            {
                                Checkpoint(parser); // r884

                                bool r884 = true;
                                r884 = r884 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r884, parser);
                                a883 = r884;
                            }

                            r882 |= a883;
                        } // end may a883

                        r882 = r882 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r882)
                        { // may a885
                            bool a885 = false;
                            {
                                Checkpoint(parser); // r886

                                bool r886 = true;
                                r886 = r886 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r886, parser);
                                a885 = r886;
                            }

                            r882 |= a885;
                        } // end may a885

                        r882 = r882 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintList());
                        CommitOrRollback(r882, parser);
                        a881 = r882;
                    }

                    r880 |= a881;
                } // end may a881

                CommitOrRollback(r880, parser);
                res = r880;
            }



            return res;
        }
    }

    public partial class TableHint : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableHint()
            :base()
        {
        }

        public TableHint(Jhu.Graywulf.Sql.Parsing.TableHint old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableHint(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r887

                bool r887 = true;
                if (r887)
                { // alternatives a888 must
                    bool a888 = false;
                    if (!a888)
                    {
                        Checkpoint(parser); // r889

                        bool r889 = true;
                        r889 = r889 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r889)
                        { // may a890
                            bool a890 = false;
                            {
                                Checkpoint(parser); // r891

                                bool r891 = true;
                                r891 = r891 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r891, parser);
                                a890 = r891;
                            }

                            r889 |= a890;
                        } // end may a890

                        r889 = r889 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r889, parser);
                        a888 = r889;
                    }

                    if (!a888)
                    {
                        Checkpoint(parser); // r892

                        bool r892 = true;
                        r892 = r892 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r892, parser);
                        a888 = r892;
                    }

                    r887 &= a888;

                } // end alternatives a888

                CommitOrRollback(r887, parser);
                res = r887;
            }



            return res;
        }
    }

    public partial class QueryHintClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryHintClause()
            :base()
        {
        }

        public QueryHintClause(Jhu.Graywulf.Sql.Parsing.QueryHintClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryHintClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r893

                bool r893 = true;
                r893 = r893 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OPTION"));
                if (r893)
                { // may a894
                    bool a894 = false;
                    {
                        Checkpoint(parser); // r895

                        bool r895 = true;
                        r895 = r895 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r895, parser);
                        a894 = r895;
                    }

                    r893 |= a894;
                } // end may a894

                r893 = r893 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r893)
                { // may a896
                    bool a896 = false;
                    {
                        Checkpoint(parser); // r897

                        bool r897 = true;
                        r897 = r897 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r897, parser);
                        a896 = r897;
                    }

                    r893 |= a896;
                } // end may a896

                r893 = r893 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintList());
                if (r893)
                { // may a898
                    bool a898 = false;
                    {
                        Checkpoint(parser); // r899

                        bool r899 = true;
                        r899 = r899 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r899, parser);
                        a898 = r899;
                    }

                    r893 |= a898;
                } // end may a898

                r893 = r893 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r893, parser);
                res = r893;
            }



            return res;
        }
    }

    public partial class QueryHintList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryHintList()
            :base()
        {
        }

        public QueryHintList(Jhu.Graywulf.Sql.Parsing.QueryHintList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryHintList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r900

                bool r900 = true;
                r900 = r900 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHint());
                if (r900)
                { // may a901
                    bool a901 = false;
                    {
                        Checkpoint(parser); // r902

                        bool r902 = true;
                        if (r902)
                        { // may a903
                            bool a903 = false;
                            {
                                Checkpoint(parser); // r904

                                bool r904 = true;
                                r904 = r904 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r904, parser);
                                a903 = r904;
                            }

                            r902 |= a903;
                        } // end may a903

                        r902 = r902 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r902)
                        { // may a905
                            bool a905 = false;
                            {
                                Checkpoint(parser); // r906

                                bool r906 = true;
                                r906 = r906 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r906, parser);
                                a905 = r906;
                            }

                            r902 |= a905;
                        } // end may a905

                        r902 = r902 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintList());
                        CommitOrRollback(r902, parser);
                        a901 = r902;
                    }

                    r900 |= a901;
                } // end may a901

                CommitOrRollback(r900, parser);
                res = r900;
            }



            return res;
        }
    }

    public partial class QueryHint : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryHint()
            :base()
        {
        }

        public QueryHint(Jhu.Graywulf.Sql.Parsing.QueryHint old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryHint(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r907

                bool r907 = true;
                if (r907)
                { // alternatives a908 must
                    bool a908 = false;
                    if (!a908)
                    {
                        Checkpoint(parser); // r909

                        bool r909 = true;
                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r909)
                        { // may a910
                            bool a910 = false;
                            {
                                Checkpoint(parser); // r911

                                bool r911 = true;
                                r911 = r911 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r911, parser);
                                a910 = r911;
                            }

                            r909 |= a910;
                        } // end may a910

                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r909, parser);
                        a908 = r909;
                    }

                    if (!a908)
                    {
                        Checkpoint(parser); // r912

                        bool r912 = true;
                        r912 = r912 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r912)
                        { // may a913
                            bool a913 = false;
                            {
                                Checkpoint(parser); // r914

                                bool r914 = true;
                                r914 = r914 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r914, parser);
                                a913 = r914;
                            }

                            r912 |= a913;
                        } // end may a913

                        r912 = r912 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r912)
                        { // may a915
                            bool a915 = false;
                            {
                                Checkpoint(parser); // r916

                                bool r916 = true;
                                r916 = r916 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r916, parser);
                                a915 = r916;
                            }

                            r912 |= a915;
                        } // end may a915

                        r912 = r912 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r912, parser);
                        a908 = r912;
                    }

                    if (!a908)
                    {
                        Checkpoint(parser); // r917

                        bool r917 = true;
                        r917 = r917 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        r917 = r917 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r917 = r917 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r917, parser);
                        a908 = r917;
                    }

                    if (!a908)
                    {
                        Checkpoint(parser); // r918

                        bool r918 = true;
                        r918 = r918 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintIdentifierList());
                        CommitOrRollback(r918, parser);
                        a908 = r918;
                    }

                    if (!a908)
                    {
                        Checkpoint(parser); // r919

                        bool r919 = true;
                        r919 = r919 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r919, parser);
                        a908 = r919;
                    }

                    r907 &= a908;

                } // end alternatives a908

                CommitOrRollback(r907, parser);
                res = r907;
            }



            return res;
        }
    }

    public partial class QueryHintIdentifierList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public QueryHintIdentifierList()
            :base()
        {
        }

        public QueryHintIdentifierList(Jhu.Graywulf.Sql.Parsing.QueryHintIdentifierList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.QueryHintIdentifierList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r920

                bool r920 = true;
                r920 = r920 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                if (r920)
                { // may a921
                    bool a921 = false;
                    {
                        Checkpoint(parser); // r922

                        bool r922 = true;
                        r922 = r922 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r922 = r922 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r922, parser);
                        a921 = r922;
                    }

                    r920 |= a921;
                } // end may a921

                CommitOrRollback(r920, parser);
                res = r920;
            }



            return res;
        }
    }

    public partial class InsertStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public InsertStatement()
            :base()
        {
        }

        public InsertStatement(Jhu.Graywulf.Sql.Parsing.InsertStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.InsertStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r923

                bool r923 = true;
                if (r923)
                { // may a924
                    bool a924 = false;
                    {
                        Checkpoint(parser); // r925

                        bool r925 = true;
                        r925 = r925 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r925)
                        { // may a926
                            bool a926 = false;
                            {
                                Checkpoint(parser); // r927

                                bool r927 = true;
                                r927 = r927 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r927, parser);
                                a926 = r927;
                            }

                            r925 |= a926;
                        } // end may a926

                        CommitOrRollback(r925, parser);
                        a924 = r925;
                    }

                    r923 |= a924;
                } // end may a924

                r923 = r923 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INSERT"));
                if (r923)
                { // may a928
                    bool a928 = false;
                    {
                        Checkpoint(parser); // r929

                        bool r929 = true;
                        r929 = r929 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r929, parser);
                        a928 = r929;
                    }

                    r923 |= a928;
                } // end may a928

                if (r923)
                { // alternatives a930 must
                    bool a930 = false;
                    if (!a930)
                    {
                        Checkpoint(parser); // r931

                        bool r931 = true;
                        r931 = r931 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IntoClause());
                        CommitOrRollback(r931, parser);
                        a930 = r931;
                    }

                    if (!a930)
                    {
                        Checkpoint(parser); // r932

                        bool r932 = true;
                        r932 = r932 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                        CommitOrRollback(r932, parser);
                        a930 = r932;
                    }

                    r923 &= a930;

                } // end alternatives a930

                if (r923)
                { // may a933
                    bool a933 = false;
                    {
                        Checkpoint(parser); // r934

                        bool r934 = true;
                        if (r934)
                        { // may a935
                            bool a935 = false;
                            {
                                Checkpoint(parser); // r936

                                bool r936 = true;
                                r936 = r936 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r936, parser);
                                a935 = r936;
                            }

                            r934 |= a935;
                        } // end may a935

                        r934 = r934 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets());
                        CommitOrRollback(r934, parser);
                        a933 = r934;
                    }

                    r923 |= a933;
                } // end may a933

                if (r923)
                { // may a937
                    bool a937 = false;
                    {
                        Checkpoint(parser); // r938

                        bool r938 = true;
                        r938 = r938 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r938, parser);
                        a937 = r938;
                    }

                    r923 |= a937;
                } // end may a937

                if (r923)
                { // alternatives a939 must
                    bool a939 = false;
                    if (!a939)
                    {
                        Checkpoint(parser); // r940

                        bool r940 = true;
                        r940 = r940 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesClause());
                        CommitOrRollback(r940, parser);
                        a939 = r940;
                    }

                    if (!a939)
                    {
                        Checkpoint(parser); // r941

                        bool r941 = true;
                        r941 = r941 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        r941 = r941 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r941 = r941 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"VALUES"));
                        CommitOrRollback(r941, parser);
                        a939 = r941;
                    }

                    if (!a939)
                    {
                        Checkpoint(parser); // r942

                        bool r942 = true;
                        r942 = r942 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                        if (r942)
                        { // may a943
                            bool a943 = false;
                            {
                                Checkpoint(parser); // r944

                                bool r944 = true;
                                if (r944)
                                { // may a945
                                    bool a945 = false;
                                    {
                                        Checkpoint(parser); // r946

                                        bool r946 = true;
                                        r946 = r946 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r946, parser);
                                        a945 = r946;
                                    }

                                    r944 |= a945;
                                } // end may a945

                                r944 = r944 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                                CommitOrRollback(r944, parser);
                                a943 = r944;
                            }

                            r942 |= a943;
                        } // end may a943

                        CommitOrRollback(r942, parser);
                        a939 = r942;
                    }

                    r923 &= a939;

                } // end alternatives a939

                if (r923)
                { // may a947
                    bool a947 = false;
                    {
                        Checkpoint(parser); // r948

                        bool r948 = true;
                        if (r948)
                        { // may a949
                            bool a949 = false;
                            {
                                Checkpoint(parser); // r950

                                bool r950 = true;
                                r950 = r950 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r950, parser);
                                a949 = r950;
                            }

                            r948 |= a949;
                        } // end may a949

                        r948 = r948 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r948, parser);
                        a947 = r948;
                    }

                    r923 |= a947;
                } // end may a947

                CommitOrRollback(r923, parser);
                res = r923;
            }



            return res;
        }
    }

    public partial class ColumnListBrackets : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnListBrackets()
            :base()
        {
        }

        public ColumnListBrackets(Jhu.Graywulf.Sql.Parsing.ColumnListBrackets old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r951

                bool r951 = true;
                r951 = r951 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r951)
                { // may a952
                    bool a952 = false;
                    {
                        Checkpoint(parser); // r953

                        bool r953 = true;
                        r953 = r953 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r953, parser);
                        a952 = r953;
                    }

                    r951 |= a952;
                } // end may a952

                r951 = r951 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnList());
                if (r951)
                { // may a954
                    bool a954 = false;
                    {
                        Checkpoint(parser); // r955

                        bool r955 = true;
                        r955 = r955 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r955, parser);
                        a954 = r955;
                    }

                    r951 |= a954;
                } // end may a954

                r951 = r951 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r951, parser);
                res = r951;
            }



            return res;
        }
    }

    public partial class ColumnList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnList()
            :base()
        {
        }

        public ColumnList(Jhu.Graywulf.Sql.Parsing.ColumnList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r956

                bool r956 = true;
                r956 = r956 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r956)
                { // may a957
                    bool a957 = false;
                    {
                        Checkpoint(parser); // r958

                        bool r958 = true;
                        if (r958)
                        { // may a959
                            bool a959 = false;
                            {
                                Checkpoint(parser); // r960

                                bool r960 = true;
                                r960 = r960 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r960, parser);
                                a959 = r960;
                            }

                            r958 |= a959;
                        } // end may a959

                        r958 = r958 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r958)
                        { // may a961
                            bool a961 = false;
                            {
                                Checkpoint(parser); // r962

                                bool r962 = true;
                                r962 = r962 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r962, parser);
                                a961 = r962;
                            }

                            r958 |= a961;
                        } // end may a961

                        r958 = r958 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnList());
                        CommitOrRollback(r958, parser);
                        a957 = r958;
                    }

                    r956 |= a957;
                } // end may a957

                CommitOrRollback(r956, parser);
                res = r956;
            }



            return res;
        }
    }

    public partial class ValuesClause : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ValuesClause()
            :base()
        {
        }

        public ValuesClause(Jhu.Graywulf.Sql.Parsing.ValuesClause old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ValuesClause(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r963

                bool r963 = true;
                r963 = r963 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"VALUES"));
                if (r963)
                { // may a964
                    bool a964 = false;
                    {
                        Checkpoint(parser); // r965

                        bool r965 = true;
                        r965 = r965 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r965, parser);
                        a964 = r965;
                    }

                    r963 |= a964;
                } // end may a964

                r963 = r963 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroupList());
                if (r963)
                { // may a966
                    bool a966 = false;
                    {
                        Checkpoint(parser); // r967

                        bool r967 = true;
                        r967 = r967 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r967, parser);
                        a966 = r967;
                    }

                    r963 |= a966;
                } // end may a966

                CommitOrRollback(r963, parser);
                res = r963;
            }



            return res;
        }
    }

    public partial class ValuesGroupList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ValuesGroupList()
            :base()
        {
        }

        public ValuesGroupList(Jhu.Graywulf.Sql.Parsing.ValuesGroupList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ValuesGroupList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r968

                bool r968 = true;
                r968 = r968 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroup());
                if (r968)
                { // may a969
                    bool a969 = false;
                    {
                        Checkpoint(parser); // r970

                        bool r970 = true;
                        if (r970)
                        { // may a971
                            bool a971 = false;
                            {
                                Checkpoint(parser); // r972

                                bool r972 = true;
                                r972 = r972 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r972, parser);
                                a971 = r972;
                            }

                            r970 |= a971;
                        } // end may a971

                        r970 = r970 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r970)
                        { // may a973
                            bool a973 = false;
                            {
                                Checkpoint(parser); // r974

                                bool r974 = true;
                                r974 = r974 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r974, parser);
                                a973 = r974;
                            }

                            r970 |= a973;
                        } // end may a973

                        r970 = r970 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroupList());
                        CommitOrRollback(r970, parser);
                        a969 = r970;
                    }

                    r968 |= a969;
                } // end may a969

                CommitOrRollback(r968, parser);
                res = r968;
            }



            return res;
        }
    }

    public partial class ValuesGroup : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ValuesGroup()
            :base()
        {
        }

        public ValuesGroup(Jhu.Graywulf.Sql.Parsing.ValuesGroup old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ValuesGroup(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r975

                bool r975 = true;
                r975 = r975 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r975)
                { // may a976
                    bool a976 = false;
                    {
                        Checkpoint(parser); // r977

                        bool r977 = true;
                        r977 = r977 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r977, parser);
                        a976 = r977;
                    }

                    r975 |= a976;
                } // end may a976

                r975 = r975 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesList());
                if (r975)
                { // may a978
                    bool a978 = false;
                    {
                        Checkpoint(parser); // r979

                        bool r979 = true;
                        r979 = r979 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r979, parser);
                        a978 = r979;
                    }

                    r975 |= a978;
                } // end may a978

                r975 = r975 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r975, parser);
                res = r975;
            }



            return res;
        }
    }

    public partial class ValuesList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ValuesList()
            :base()
        {
        }

        public ValuesList(Jhu.Graywulf.Sql.Parsing.ValuesList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ValuesList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r980

                bool r980 = true;
                if (r980)
                { // alternatives a981 must
                    bool a981 = false;
                    if (!a981)
                    {
                        Checkpoint(parser); // r982

                        bool r982 = true;
                        r982 = r982 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        CommitOrRollback(r982, parser);
                        a981 = r982;
                    }

                    if (!a981)
                    {
                        Checkpoint(parser); // r983

                        bool r983 = true;
                        r983 = r983 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r983, parser);
                        a981 = r983;
                    }

                    r980 &= a981;

                } // end alternatives a981

                if (r980)
                { // may a984
                    bool a984 = false;
                    {
                        Checkpoint(parser); // r985

                        bool r985 = true;
                        if (r985)
                        { // may a986
                            bool a986 = false;
                            {
                                Checkpoint(parser); // r987

                                bool r987 = true;
                                r987 = r987 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r987, parser);
                                a986 = r987;
                            }

                            r985 |= a986;
                        } // end may a986

                        r985 = r985 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r985)
                        { // may a988
                            bool a988 = false;
                            {
                                Checkpoint(parser); // r989

                                bool r989 = true;
                                r989 = r989 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r989, parser);
                                a988 = r989;
                            }

                            r985 |= a988;
                        } // end may a988

                        r985 = r985 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesList());
                        CommitOrRollback(r985, parser);
                        a984 = r985;
                    }

                    r980 |= a984;
                } // end may a984

                CommitOrRollback(r980, parser);
                res = r980;
            }



            return res;
        }
    }

    public partial class UpdateStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UpdateStatement()
            :base()
        {
        }

        public UpdateStatement(Jhu.Graywulf.Sql.Parsing.UpdateStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UpdateStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r990

                bool r990 = true;
                if (r990)
                { // may a991
                    bool a991 = false;
                    {
                        Checkpoint(parser); // r992

                        bool r992 = true;
                        r992 = r992 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r992)
                        { // may a993
                            bool a993 = false;
                            {
                                Checkpoint(parser); // r994

                                bool r994 = true;
                                r994 = r994 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r994, parser);
                                a993 = r994;
                            }

                            r992 |= a993;
                        } // end may a993

                        CommitOrRollback(r992, parser);
                        a991 = r992;
                    }

                    r990 |= a991;
                } // end may a991

                r990 = r990 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UPDATE"));
                if (r990)
                { // may a995
                    bool a995 = false;
                    {
                        Checkpoint(parser); // r996

                        bool r996 = true;
                        r996 = r996 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r996, parser);
                        a995 = r996;
                    }

                    r990 |= a995;
                } // end may a995

                r990 = r990 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                if (r990)
                { // may a997
                    bool a997 = false;
                    {
                        Checkpoint(parser); // r998

                        bool r998 = true;
                        r998 = r998 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r998, parser);
                        a997 = r998;
                    }

                    r990 |= a997;
                } // end may a997

                r990 = r990 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                if (r990)
                { // may a999
                    bool a999 = false;
                    {
                        Checkpoint(parser); // r1000

                        bool r1000 = true;
                        r1000 = r1000 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1000, parser);
                        a999 = r1000;
                    }

                    r990 |= a999;
                } // end may a999

                r990 = r990 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetList());
                if (r990)
                { // may a1001
                    bool a1001 = false;
                    {
                        Checkpoint(parser); // r1002

                        bool r1002 = true;
                        if (r1002)
                        { // may a1003
                            bool a1003 = false;
                            {
                                Checkpoint(parser); // r1004

                                bool r1004 = true;
                                r1004 = r1004 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1004, parser);
                                a1003 = r1004;
                            }

                            r1002 |= a1003;
                        } // end may a1003

                        r1002 = r1002 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r1002, parser);
                        a1001 = r1002;
                    }

                    r990 |= a1001;
                } // end may a1001

                if (r990)
                { // may a1005
                    bool a1005 = false;
                    {
                        Checkpoint(parser); // r1006

                        bool r1006 = true;
                        if (r1006)
                        { // may a1007
                            bool a1007 = false;
                            {
                                Checkpoint(parser); // r1008

                                bool r1008 = true;
                                r1008 = r1008 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1008, parser);
                                a1007 = r1008;
                            }

                            r1006 |= a1007;
                        } // end may a1007

                        r1006 = r1006 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r1006, parser);
                        a1005 = r1006;
                    }

                    r990 |= a1005;
                } // end may a1005

                if (r990)
                { // may a1009
                    bool a1009 = false;
                    {
                        Checkpoint(parser); // r1010

                        bool r1010 = true;
                        if (r1010)
                        { // may a1011
                            bool a1011 = false;
                            {
                                Checkpoint(parser); // r1012

                                bool r1012 = true;
                                r1012 = r1012 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1012, parser);
                                a1011 = r1012;
                            }

                            r1010 |= a1011;
                        } // end may a1011

                        r1010 = r1010 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r1010, parser);
                        a1009 = r1010;
                    }

                    r990 |= a1009;
                } // end may a1009

                CommitOrRollback(r990, parser);
                res = r990;
            }



            return res;
        }
    }

    public partial class UpdateSetList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UpdateSetList()
            :base()
        {
        }

        public UpdateSetList(Jhu.Graywulf.Sql.Parsing.UpdateSetList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UpdateSetList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1013

                bool r1013 = true;
                r1013 = r1013 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetColumn());
                if (r1013)
                { // may a1014
                    bool a1014 = false;
                    {
                        Checkpoint(parser); // r1015

                        bool r1015 = true;
                        if (r1015)
                        { // may a1016
                            bool a1016 = false;
                            {
                                Checkpoint(parser); // r1017

                                bool r1017 = true;
                                r1017 = r1017 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1017, parser);
                                a1016 = r1017;
                            }

                            r1015 |= a1016;
                        } // end may a1016

                        r1015 = r1015 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1015)
                        { // may a1018
                            bool a1018 = false;
                            {
                                Checkpoint(parser); // r1019

                                bool r1019 = true;
                                r1019 = r1019 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1019, parser);
                                a1018 = r1019;
                            }

                            r1015 |= a1018;
                        } // end may a1018

                        r1015 = r1015 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetList());
                        CommitOrRollback(r1015, parser);
                        a1014 = r1015;
                    }

                    r1013 |= a1014;
                } // end may a1014

                CommitOrRollback(r1013, parser);
                res = r1013;
            }



            return res;
        }
    }

    public partial class UpdateSetColumn : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public UpdateSetColumn()
            :base()
        {
        }

        public UpdateSetColumn(Jhu.Graywulf.Sql.Parsing.UpdateSetColumn old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.UpdateSetColumn(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1020

                bool r1020 = true;
                if (r1020)
                { // alternatives a1021 must
                    bool a1021 = false;
                    if (!a1021)
                    {
                        Checkpoint(parser); // r1022

                        bool r1022 = true;
                        r1022 = r1022 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        if (r1022)
                        { // may a1023
                            bool a1023 = false;
                            {
                                Checkpoint(parser); // r1024

                                bool r1024 = true;
                                r1024 = r1024 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1024, parser);
                                a1023 = r1024;
                            }

                            r1022 |= a1023;
                        } // end may a1023

                        r1022 = r1022 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r1022)
                        { // may a1025
                            bool a1025 = false;
                            {
                                Checkpoint(parser); // r1026

                                bool r1026 = true;
                                r1026 = r1026 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1026, parser);
                                a1025 = r1026;
                            }

                            r1022 |= a1025;
                        } // end may a1025

                        r1022 = r1022 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                        CommitOrRollback(r1022, parser);
                        a1021 = r1022;
                    }

                    if (!a1021)
                    {
                        Checkpoint(parser); // r1027

                        bool r1027 = true;
                        if (r1027)
                        { // alternatives a1028 must
                            bool a1028 = false;
                            if (!a1028)
                            {
                                Checkpoint(parser); // r1029

                                bool r1029 = true;
                                r1029 = r1029 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r1029, parser);
                                a1028 = r1029;
                            }

                            if (!a1028)
                            {
                                Checkpoint(parser); // r1030

                                bool r1030 = true;
                                r1030 = r1030 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                CommitOrRollback(r1030, parser);
                                a1028 = r1030;
                            }

                            r1027 &= a1028;

                        } // end alternatives a1028

                        CommitOrRollback(r1027, parser);
                        a1021 = r1027;
                    }

                    r1020 &= a1021;

                } // end alternatives a1021

                if (r1020)
                { // may a1031
                    bool a1031 = false;
                    {
                        Checkpoint(parser); // r1032

                        bool r1032 = true;
                        r1032 = r1032 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1032, parser);
                        a1031 = r1032;
                    }

                    r1020 |= a1031;
                } // end may a1031

                r1020 = r1020 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator());
                if (r1020)
                { // may a1033
                    bool a1033 = false;
                    {
                        Checkpoint(parser); // r1034

                        bool r1034 = true;
                        r1034 = r1034 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1034, parser);
                        a1033 = r1034;
                    }

                    r1020 |= a1033;
                } // end may a1033

                if (r1020)
                { // alternatives a1035 must
                    bool a1035 = false;
                    if (!a1035)
                    {
                        Checkpoint(parser); // r1036

                        bool r1036 = true;
                        r1036 = r1036 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        CommitOrRollback(r1036, parser);
                        a1035 = r1036;
                    }

                    if (!a1035)
                    {
                        Checkpoint(parser); // r1037

                        bool r1037 = true;
                        r1037 = r1037 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r1037, parser);
                        a1035 = r1037;
                    }

                    r1020 &= a1035;

                } // end alternatives a1035

                CommitOrRollback(r1020, parser);
                res = r1020;
            }



            return res;
        }
    }

    public partial class DeleteStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DeleteStatement()
            :base()
        {
        }

        public DeleteStatement(Jhu.Graywulf.Sql.Parsing.DeleteStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DeleteStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1038

                bool r1038 = true;
                r1038 = r1038 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeleteSpecification());
                CommitOrRollback(r1038, parser);
                res = r1038;
            }



            return res;
        }
    }

    public partial class DeleteSpecification : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DeleteSpecification()
            :base()
        {
        }

        public DeleteSpecification(Jhu.Graywulf.Sql.Parsing.DeleteSpecification old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DeleteSpecification(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1039

                bool r1039 = true;
                if (r1039)
                { // may a1040
                    bool a1040 = false;
                    {
                        Checkpoint(parser); // r1041

                        bool r1041 = true;
                        r1041 = r1041 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r1041)
                        { // may a1042
                            bool a1042 = false;
                            {
                                Checkpoint(parser); // r1043

                                bool r1043 = true;
                                r1043 = r1043 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1043, parser);
                                a1042 = r1043;
                            }

                            r1041 |= a1042;
                        } // end may a1042

                        CommitOrRollback(r1041, parser);
                        a1040 = r1041;
                    }

                    r1039 |= a1040;
                } // end may a1040

                r1039 = r1039 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DELETE"));
                if (r1039)
                { // may a1044
                    bool a1044 = false;
                    {
                        Checkpoint(parser); // r1045

                        bool r1045 = true;
                        r1045 = r1045 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1045, parser);
                        a1044 = r1045;
                    }

                    r1039 |= a1044;
                } // end may a1044

                if (r1039)
                { // may a1046
                    bool a1046 = false;
                    {
                        Checkpoint(parser); // r1047

                        bool r1047 = true;
                        r1047 = r1047 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                        CommitOrRollback(r1047, parser);
                        a1046 = r1047;
                    }

                    r1039 |= a1046;
                } // end may a1046

                if (r1039)
                { // may a1048
                    bool a1048 = false;
                    {
                        Checkpoint(parser); // r1049

                        bool r1049 = true;
                        r1049 = r1049 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1049, parser);
                        a1048 = r1049;
                    }

                    r1039 |= a1048;
                } // end may a1048

                r1039 = r1039 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                if (r1039)
                { // may a1050
                    bool a1050 = false;
                    {
                        Checkpoint(parser); // r1051

                        bool r1051 = true;
                        if (r1051)
                        { // may a1052
                            bool a1052 = false;
                            {
                                Checkpoint(parser); // r1053

                                bool r1053 = true;
                                r1053 = r1053 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1053, parser);
                                a1052 = r1053;
                            }

                            r1051 |= a1052;
                        } // end may a1052

                        r1051 = r1051 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r1051, parser);
                        a1050 = r1051;
                    }

                    r1039 |= a1050;
                } // end may a1050

                if (r1039)
                { // may a1054
                    bool a1054 = false;
                    {
                        Checkpoint(parser); // r1055

                        bool r1055 = true;
                        if (r1055)
                        { // may a1056
                            bool a1056 = false;
                            {
                                Checkpoint(parser); // r1057

                                bool r1057 = true;
                                r1057 = r1057 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1057, parser);
                                a1056 = r1057;
                            }

                            r1055 |= a1056;
                        } // end may a1056

                        r1055 = r1055 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r1055, parser);
                        a1054 = r1055;
                    }

                    r1039 |= a1054;
                } // end may a1054

                if (r1039)
                { // may a1058
                    bool a1058 = false;
                    {
                        Checkpoint(parser); // r1059

                        bool r1059 = true;
                        if (r1059)
                        { // may a1060
                            bool a1060 = false;
                            {
                                Checkpoint(parser); // r1061

                                bool r1061 = true;
                                r1061 = r1061 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1061, parser);
                                a1060 = r1061;
                            }

                            r1059 |= a1060;
                        } // end may a1060

                        r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r1059, parser);
                        a1058 = r1059;
                    }

                    r1039 |= a1058;
                } // end may a1058

                CommitOrRollback(r1039, parser);
                res = r1039;
            }



            return res;
        }
    }

    public partial class CreateTableStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CreateTableStatement()
            :base()
        {
        }

        public CreateTableStatement(Jhu.Graywulf.Sql.Parsing.CreateTableStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CreateTableStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1062

                bool r1062 = true;
                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CREATE"));
                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r1062)
                { // may a1063
                    bool a1063 = false;
                    {
                        Checkpoint(parser); // r1064

                        bool r1064 = true;
                        r1064 = r1064 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1064, parser);
                        a1063 = r1064;
                    }

                    r1062 |= a1063;
                } // end may a1063

                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1062)
                { // may a1065
                    bool a1065 = false;
                    {
                        Checkpoint(parser); // r1066

                        bool r1066 = true;
                        r1066 = r1066 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1066, parser);
                        a1065 = r1066;
                    }

                    r1062 |= a1065;
                } // end may a1065

                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                if (r1062)
                { // may a1067
                    bool a1067 = false;
                    {
                        Checkpoint(parser); // r1068

                        bool r1068 = true;
                        r1068 = r1068 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1068, parser);
                        a1067 = r1068;
                    }

                    r1062 |= a1067;
                } // end may a1067

                r1062 = r1062 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r1062, parser);
                res = r1062;
            }



            return res;
        }
    }

    public partial class TableDefinitionList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableDefinitionList()
            :base()
        {
        }

        public TableDefinitionList(Jhu.Graywulf.Sql.Parsing.TableDefinitionList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableDefinitionList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1069

                bool r1069 = true;
                if (r1069)
                { // alternatives a1070 must
                    bool a1070 = false;
                    if (!a1070)
                    {
                        Checkpoint(parser); // r1071

                        bool r1071 = true;
                        r1071 = r1071 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnDefinition());
                        CommitOrRollback(r1071, parser);
                        a1070 = r1071;
                    }

                    if (!a1070)
                    {
                        Checkpoint(parser); // r1072

                        bool r1072 = true;
                        r1072 = r1072 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableConstraint());
                        CommitOrRollback(r1072, parser);
                        a1070 = r1072;
                    }

                    r1069 &= a1070;

                } // end alternatives a1070

                if (r1069)
                { // may a1073
                    bool a1073 = false;
                    {
                        Checkpoint(parser); // r1074

                        bool r1074 = true;
                        if (r1074)
                        { // may a1075
                            bool a1075 = false;
                            {
                                Checkpoint(parser); // r1076

                                bool r1076 = true;
                                r1076 = r1076 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1076, parser);
                                a1075 = r1076;
                            }

                            r1074 |= a1075;
                        } // end may a1075

                        r1074 = r1074 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1074)
                        { // may a1077
                            bool a1077 = false;
                            {
                                Checkpoint(parser); // r1078

                                bool r1078 = true;
                                r1078 = r1078 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1078, parser);
                                a1077 = r1078;
                            }

                            r1074 |= a1077;
                        } // end may a1077

                        r1074 = r1074 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                        CommitOrRollback(r1074, parser);
                        a1073 = r1074;
                    }

                    r1069 |= a1073;
                } // end may a1073

                CommitOrRollback(r1069, parser);
                res = r1069;
            }



            return res;
        }
    }

    public partial class ColumnDefinition : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnDefinition()
            :base()
        {
        }

        public ColumnDefinition(Jhu.Graywulf.Sql.Parsing.ColumnDefinition old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnDefinition(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1079

                bool r1079 = true;
                r1079 = r1079 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                r1079 = r1079 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1079 = r1079 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataType());
                if (r1079)
                { // may a1080
                    bool a1080 = false;
                    {
                        Checkpoint(parser); // r1081

                        bool r1081 = true;
                        if (r1081)
                        { // may a1082
                            bool a1082 = false;
                            {
                                Checkpoint(parser); // r1083

                                bool r1083 = true;
                                r1083 = r1083 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1083, parser);
                                a1082 = r1083;
                            }

                            r1081 |= a1082;
                        } // end may a1082

                        if (r1081)
                        { // alternatives a1084 must
                            bool a1084 = false;
                            if (!a1084)
                            {
                                Checkpoint(parser); // r1085

                                bool r1085 = true;
                                r1085 = r1085 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnDefaultDefinition());
                                CommitOrRollback(r1085, parser);
                                a1084 = r1085;
                            }

                            if (!a1084)
                            {
                                Checkpoint(parser); // r1086

                                bool r1086 = true;
                                r1086 = r1086 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentityDefinition());
                                CommitOrRollback(r1086, parser);
                                a1084 = r1086;
                            }

                            r1081 &= a1084;

                        } // end alternatives a1084

                        CommitOrRollback(r1081, parser);
                        a1080 = r1081;
                    }

                    r1079 |= a1080;
                } // end may a1080

                if (r1079)
                { // may a1087
                    bool a1087 = false;
                    {
                        Checkpoint(parser); // r1088

                        bool r1088 = true;
                        if (r1088)
                        { // may a1089
                            bool a1089 = false;
                            {
                                Checkpoint(parser); // r1090

                                bool r1090 = true;
                                r1090 = r1090 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1090, parser);
                                a1089 = r1090;
                            }

                            r1088 |= a1089;
                        } // end may a1089

                        r1088 = r1088 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnConstraint());
                        CommitOrRollback(r1088, parser);
                        a1087 = r1088;
                    }

                    r1079 |= a1087;
                } // end may a1087

                CommitOrRollback(r1079, parser);
                res = r1079;
            }



            return res;
        }
    }

    public partial class ColumnDefaultDefinition : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnDefaultDefinition()
            :base()
        {
        }

        public ColumnDefaultDefinition(Jhu.Graywulf.Sql.Parsing.ColumnDefaultDefinition old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnDefaultDefinition(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1091

                bool r1091 = true;
                if (r1091)
                { // may a1092
                    bool a1092 = false;
                    {
                        Checkpoint(parser); // r1093

                        bool r1093 = true;
                        r1093 = r1093 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1093, parser);
                        a1092 = r1093;
                    }

                    r1091 |= a1092;
                } // end may a1092

                r1091 = r1091 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                if (r1091)
                { // may a1094
                    bool a1094 = false;
                    {
                        Checkpoint(parser); // r1095

                        bool r1095 = true;
                        r1095 = r1095 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1095, parser);
                        a1094 = r1095;
                    }

                    r1091 |= a1094;
                } // end may a1094

                r1091 = r1091 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r1091, parser);
                res = r1091;
            }



            return res;
        }
    }

    public partial class ConstraintNameSpecification : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ConstraintNameSpecification()
            :base()
        {
        }

        public ConstraintNameSpecification(Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1096

                bool r1096 = true;
                r1096 = r1096 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CONSTRAINT"));
                if (r1096)
                { // may a1097
                    bool a1097 = false;
                    {
                        Checkpoint(parser); // r1098

                        bool r1098 = true;
                        r1098 = r1098 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1098, parser);
                        a1097 = r1098;
                    }

                    r1096 |= a1097;
                } // end may a1097

                r1096 = r1096 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintName());
                if (r1096)
                { // may a1099
                    bool a1099 = false;
                    {
                        Checkpoint(parser); // r1100

                        bool r1100 = true;
                        r1100 = r1100 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1100, parser);
                        a1099 = r1100;
                    }

                    r1096 |= a1099;
                } // end may a1099

                CommitOrRollback(r1096, parser);
                res = r1096;
            }



            return res;
        }
    }

    public partial class ColumnIdentityDefinition : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnIdentityDefinition()
            :base()
        {
        }

        public ColumnIdentityDefinition(Jhu.Graywulf.Sql.Parsing.ColumnIdentityDefinition old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnIdentityDefinition(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1101

                bool r1101 = true;
                r1101 = r1101 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IDENTITY"));
                if (r1101)
                { // may a1102
                    bool a1102 = false;
                    {
                        Checkpoint(parser); // r1103

                        bool r1103 = true;
                        if (r1103)
                        { // may a1104
                            bool a1104 = false;
                            {
                                Checkpoint(parser); // r1105

                                bool r1105 = true;
                                r1105 = r1105 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1105, parser);
                                a1104 = r1105;
                            }

                            r1103 |= a1104;
                        } // end may a1104

                        r1103 = r1103 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r1103, parser);
                        a1102 = r1103;
                    }

                    r1101 |= a1102;
                } // end may a1102

                CommitOrRollback(r1101, parser);
                res = r1101;
            }



            return res;
        }
    }

    public partial class ColumnConstraint : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ColumnConstraint()
            :base()
        {
        }

        public ColumnConstraint(Jhu.Graywulf.Sql.Parsing.ColumnConstraint old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ColumnConstraint(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1106

                bool r1106 = true;
                if (r1106)
                { // may a1107
                    bool a1107 = false;
                    {
                        Checkpoint(parser); // r1108

                        bool r1108 = true;
                        r1108 = r1108 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1108, parser);
                        a1107 = r1108;
                    }

                    r1106 |= a1107;
                } // end may a1107

                r1106 = r1106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintSpecification());
                CommitOrRollback(r1106, parser);
                res = r1106;
            }



            return res;
        }
    }

    public partial class TableConstraint : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TableConstraint()
            :base()
        {
        }

        public TableConstraint(Jhu.Graywulf.Sql.Parsing.TableConstraint old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TableConstraint(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1109

                bool r1109 = true;
                if (r1109)
                { // may a1110
                    bool a1110 = false;
                    {
                        Checkpoint(parser); // r1111

                        bool r1111 = true;
                        r1111 = r1111 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1111, parser);
                        a1110 = r1111;
                    }

                    r1109 |= a1110;
                } // end may a1110

                r1109 = r1109 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintSpecification());
                if (r1109)
                { // may a1112
                    bool a1112 = false;
                    {
                        Checkpoint(parser); // r1113

                        bool r1113 = true;
                        r1113 = r1113 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1113, parser);
                        a1112 = r1113;
                    }

                    r1109 |= a1112;
                } // end may a1112

                r1109 = r1109 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1109)
                { // may a1114
                    bool a1114 = false;
                    {
                        Checkpoint(parser); // r1115

                        bool r1115 = true;
                        r1115 = r1115 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1115, parser);
                        a1114 = r1115;
                    }

                    r1109 |= a1114;
                } // end may a1114

                r1109 = r1109 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                if (r1109)
                { // may a1116
                    bool a1116 = false;
                    {
                        Checkpoint(parser); // r1117

                        bool r1117 = true;
                        r1117 = r1117 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1117, parser);
                        a1116 = r1117;
                    }

                    r1109 |= a1116;
                } // end may a1116

                r1109 = r1109 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r1109, parser);
                res = r1109;
            }



            return res;
        }
    }

    public partial class ConstraintSpecification : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public ConstraintSpecification()
            :base()
        {
        }

        public ConstraintSpecification(Jhu.Graywulf.Sql.Parsing.ConstraintSpecification old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.ConstraintSpecification(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1118

                bool r1118 = true;
                if (r1118)
                { // alternatives a1119 must
                    bool a1119 = false;
                    if (!a1119)
                    {
                        Checkpoint(parser); // r1120

                        bool r1120 = true;
                        r1120 = r1120 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PRIMARY"));
                        r1120 = r1120 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r1120 = r1120 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"KEY"));
                        CommitOrRollback(r1120, parser);
                        a1119 = r1120;
                    }

                    if (!a1119)
                    {
                        Checkpoint(parser); // r1121

                        bool r1121 = true;
                        r1121 = r1121 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNIQUE"));
                        CommitOrRollback(r1121, parser);
                        a1119 = r1121;
                    }

                    r1118 &= a1119;

                } // end alternatives a1119

                if (r1118)
                { // may a1122
                    bool a1122 = false;
                    {
                        Checkpoint(parser); // r1123

                        bool r1123 = true;
                        r1123 = r1123 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r1123)
                        { // alternatives a1124 must
                            bool a1124 = false;
                            if (!a1124)
                            {
                                Checkpoint(parser); // r1125

                                bool r1125 = true;
                                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLUSTERED"));
                                CommitOrRollback(r1125, parser);
                                a1124 = r1125;
                            }

                            if (!a1124)
                            {
                                Checkpoint(parser); // r1126

                                bool r1126 = true;
                                r1126 = r1126 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NONCLUSTERED"));
                                CommitOrRollback(r1126, parser);
                                a1124 = r1126;
                            }

                            r1123 &= a1124;

                        } // end alternatives a1124

                        CommitOrRollback(r1123, parser);
                        a1122 = r1123;
                    }

                    r1118 |= a1122;
                } // end may a1122

                CommitOrRollback(r1118, parser);
                res = r1118;
            }



            return res;
        }
    }

    public partial class DropTableStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DropTableStatement()
            :base()
        {
        }

        public DropTableStatement(Jhu.Graywulf.Sql.Parsing.DropTableStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DropTableStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1127

                bool r1127 = true;
                r1127 = r1127 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DROP"));
                r1127 = r1127 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1127 = r1127 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r1127)
                { // may a1128
                    bool a1128 = false;
                    {
                        Checkpoint(parser); // r1129

                        bool r1129 = true;
                        r1129 = r1129 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1129, parser);
                        a1128 = r1129;
                    }

                    r1127 |= a1128;
                } // end may a1128

                r1127 = r1127 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1127, parser);
                res = r1127;
            }



            return res;
        }
    }

    public partial class TruncateTableStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public TruncateTableStatement()
            :base()
        {
        }

        public TruncateTableStatement(Jhu.Graywulf.Sql.Parsing.TruncateTableStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.TruncateTableStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1130

                bool r1130 = true;
                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRUNCATE"));
                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r1130)
                { // may a1131
                    bool a1131 = false;
                    {
                        Checkpoint(parser); // r1132

                        bool r1132 = true;
                        r1132 = r1132 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1132, parser);
                        a1131 = r1132;
                    }

                    r1130 |= a1131;
                } // end may a1131

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1130, parser);
                res = r1130;
            }



            return res;
        }
    }

    public partial class CreateIndexStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public CreateIndexStatement()
            :base()
        {
        }

        public CreateIndexStatement(Jhu.Graywulf.Sql.Parsing.CreateIndexStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.CreateIndexStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1133

                bool r1133 = true;
                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CREATE"));
                if (r1133)
                { // may a1134
                    bool a1134 = false;
                    {
                        Checkpoint(parser); // r1135

                        bool r1135 = true;
                        r1135 = r1135 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r1135 = r1135 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNIQUE"));
                        CommitOrRollback(r1135, parser);
                        a1134 = r1135;
                    }

                    r1133 |= a1134;
                } // end may a1134

                if (r1133)
                { // may a1136
                    bool a1136 = false;
                    {
                        Checkpoint(parser); // r1137

                        bool r1137 = true;
                        r1137 = r1137 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r1137)
                        { // alternatives a1138 must
                            bool a1138 = false;
                            if (!a1138)
                            {
                                Checkpoint(parser); // r1139

                                bool r1139 = true;
                                r1139 = r1139 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLUSTERED"));
                                CommitOrRollback(r1139, parser);
                                a1138 = r1139;
                            }

                            if (!a1138)
                            {
                                Checkpoint(parser); // r1140

                                bool r1140 = true;
                                r1140 = r1140 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NONCLUSTERED"));
                                CommitOrRollback(r1140, parser);
                                a1138 = r1140;
                            }

                            r1137 &= a1138;

                        } // end alternatives a1138

                        CommitOrRollback(r1137, parser);
                        a1136 = r1137;
                    }

                    r1133 |= a1136;
                } // end may a1136

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INDEX"));
                if (r1133)
                { // may a1141
                    bool a1141 = false;
                    {
                        Checkpoint(parser); // r1142

                        bool r1142 = true;
                        r1142 = r1142 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1142, parser);
                        a1141 = r1142;
                    }

                    r1133 |= a1141;
                } // end may a1141

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexName());
                if (r1133)
                { // may a1143
                    bool a1143 = false;
                    {
                        Checkpoint(parser); // r1144

                        bool r1144 = true;
                        r1144 = r1144 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1144, parser);
                        a1143 = r1144;
                    }

                    r1133 |= a1143;
                } // end may a1143

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                if (r1133)
                { // may a1145
                    bool a1145 = false;
                    {
                        Checkpoint(parser); // r1146

                        bool r1146 = true;
                        r1146 = r1146 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1146, parser);
                        a1145 = r1146;
                    }

                    r1133 |= a1145;
                } // end may a1145

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r1133)
                { // may a1147
                    bool a1147 = false;
                    {
                        Checkpoint(parser); // r1148

                        bool r1148 = true;
                        r1148 = r1148 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1148, parser);
                        a1147 = r1148;
                    }

                    r1133 |= a1147;
                } // end may a1147

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1133)
                { // may a1149
                    bool a1149 = false;
                    {
                        Checkpoint(parser); // r1150

                        bool r1150 = true;
                        r1150 = r1150 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1150, parser);
                        a1149 = r1150;
                    }

                    r1133 |= a1149;
                } // end may a1149

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                if (r1133)
                { // may a1151
                    bool a1151 = false;
                    {
                        Checkpoint(parser); // r1152

                        bool r1152 = true;
                        r1152 = r1152 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1152, parser);
                        a1151 = r1152;
                    }

                    r1133 |= a1151;
                } // end may a1151

                r1133 = r1133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                if (r1133)
                { // may a1153
                    bool a1153 = false;
                    {
                        Checkpoint(parser); // r1154

                        bool r1154 = true;
                        if (r1154)
                        { // may a1155
                            bool a1155 = false;
                            {
                                Checkpoint(parser); // r1156

                                bool r1156 = true;
                                r1156 = r1156 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1156, parser);
                                a1155 = r1156;
                            }

                            r1154 |= a1155;
                        } // end may a1155

                        r1154 = r1154 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INCLUDE"));
                        if (r1154)
                        { // may a1157
                            bool a1157 = false;
                            {
                                Checkpoint(parser); // r1158

                                bool r1158 = true;
                                r1158 = r1158 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1158, parser);
                                a1157 = r1158;
                            }

                            r1154 |= a1157;
                        } // end may a1157

                        r1154 = r1154 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r1154)
                        { // may a1159
                            bool a1159 = false;
                            {
                                Checkpoint(parser); // r1160

                                bool r1160 = true;
                                r1160 = r1160 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1160, parser);
                                a1159 = r1160;
                            }

                            r1154 |= a1159;
                        } // end may a1159

                        r1154 = r1154 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IncludedColumnList());
                        if (r1154)
                        { // may a1161
                            bool a1161 = false;
                            {
                                Checkpoint(parser); // r1162

                                bool r1162 = true;
                                r1162 = r1162 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1162, parser);
                                a1161 = r1162;
                            }

                            r1154 |= a1161;
                        } // end may a1161

                        r1154 = r1154 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r1154, parser);
                        a1153 = r1154;
                    }

                    r1133 |= a1153;
                } // end may a1153

                CommitOrRollback(r1133, parser);
                res = r1133;
            }



            return res;
        }
    }

    public partial class IndexColumnList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IndexColumnList()
            :base()
        {
        }

        public IndexColumnList(Jhu.Graywulf.Sql.Parsing.IndexColumnList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IndexColumnList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1163

                bool r1163 = true;
                r1163 = r1163 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumn());
                if (r1163)
                { // may a1164
                    bool a1164 = false;
                    {
                        Checkpoint(parser); // r1165

                        bool r1165 = true;
                        if (r1165)
                        { // may a1166
                            bool a1166 = false;
                            {
                                Checkpoint(parser); // r1167

                                bool r1167 = true;
                                r1167 = r1167 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1167, parser);
                                a1166 = r1167;
                            }

                            r1165 |= a1166;
                        } // end may a1166

                        r1165 = r1165 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1165)
                        { // may a1168
                            bool a1168 = false;
                            {
                                Checkpoint(parser); // r1169

                                bool r1169 = true;
                                r1169 = r1169 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1169, parser);
                                a1168 = r1169;
                            }

                            r1165 |= a1168;
                        } // end may a1168

                        r1165 = r1165 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                        CommitOrRollback(r1165, parser);
                        a1164 = r1165;
                    }

                    r1163 |= a1164;
                } // end may a1164

                CommitOrRollback(r1163, parser);
                res = r1163;
            }



            return res;
        }
    }

    public partial class IndexColumn : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IndexColumn()
            :base()
        {
        }

        public IndexColumn(Jhu.Graywulf.Sql.Parsing.IndexColumn old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IndexColumn(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1170

                bool r1170 = true;
                r1170 = r1170 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r1170)
                { // may a1171
                    bool a1171 = false;
                    {
                        Checkpoint(parser); // r1172

                        bool r1172 = true;
                        if (r1172)
                        { // may a1173
                            bool a1173 = false;
                            {
                                Checkpoint(parser); // r1174

                                bool r1174 = true;
                                r1174 = r1174 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1174, parser);
                                a1173 = r1174;
                            }

                            r1172 |= a1173;
                        } // end may a1173

                        if (r1172)
                        { // alternatives a1175 must
                            bool a1175 = false;
                            if (!a1175)
                            {
                                Checkpoint(parser); // r1176

                                bool r1176 = true;
                                r1176 = r1176 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ASC"));
                                CommitOrRollback(r1176, parser);
                                a1175 = r1176;
                            }

                            if (!a1175)
                            {
                                Checkpoint(parser); // r1177

                                bool r1177 = true;
                                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DESC"));
                                CommitOrRollback(r1177, parser);
                                a1175 = r1177;
                            }

                            r1172 &= a1175;

                        } // end alternatives a1175

                        CommitOrRollback(r1172, parser);
                        a1171 = r1172;
                    }

                    r1170 |= a1171;
                } // end may a1171

                CommitOrRollback(r1170, parser);
                res = r1170;
            }



            return res;
        }
    }

    public partial class IncludedColumnList : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public IncludedColumnList()
            :base()
        {
        }

        public IncludedColumnList(Jhu.Graywulf.Sql.Parsing.IncludedColumnList old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.IncludedColumnList(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1178

                bool r1178 = true;
                r1178 = r1178 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r1178)
                { // may a1179
                    bool a1179 = false;
                    {
                        Checkpoint(parser); // r1180

                        bool r1180 = true;
                        if (r1180)
                        { // may a1181
                            bool a1181 = false;
                            {
                                Checkpoint(parser); // r1182

                                bool r1182 = true;
                                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1182, parser);
                                a1181 = r1182;
                            }

                            r1180 |= a1181;
                        } // end may a1181

                        r1180 = r1180 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1180)
                        { // may a1183
                            bool a1183 = false;
                            {
                                Checkpoint(parser); // r1184

                                bool r1184 = true;
                                r1184 = r1184 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1184, parser);
                                a1183 = r1184;
                            }

                            r1180 |= a1183;
                        } // end may a1183

                        r1180 = r1180 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IncludedColumnList());
                        CommitOrRollback(r1180, parser);
                        a1179 = r1180;
                    }

                    r1178 |= a1179;
                } // end may a1179

                CommitOrRollback(r1178, parser);
                res = r1178;
            }



            return res;
        }
    }

    public partial class DropIndexStatement : Jhu.Graywulf.Parsing.Node, ICloneable
    {
        public DropIndexStatement()
            :base()
        {
        }

        public DropIndexStatement(Jhu.Graywulf.Sql.Parsing.DropIndexStatement old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new Jhu.Graywulf.Sql.Parsing.DropIndexStatement(this);
        }

        public override bool Match(Jhu.Graywulf.Parsing.Parser parser)
        {
            bool res = true;

            {
                Checkpoint(parser); // r1185

                bool r1185 = true;
                r1185 = r1185 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DROP"));
                r1185 = r1185 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1185 = r1185 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INDEX"));
                if (r1185)
                { // may a1186
                    bool a1186 = false;
                    {
                        Checkpoint(parser); // r1187

                        bool r1187 = true;
                        r1187 = r1187 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1187, parser);
                        a1186 = r1187;
                    }

                    r1185 |= a1186;
                } // end may a1186

                r1185 = r1185 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexName());
                if (r1185)
                { // may a1188
                    bool a1188 = false;
                    {
                        Checkpoint(parser); // r1189

                        bool r1189 = true;
                        r1189 = r1189 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1189, parser);
                        a1188 = r1189;
                    }

                    r1185 |= a1188;
                } // end may a1188

                r1185 = r1185 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                if (r1185)
                { // may a1190
                    bool a1190 = false;
                    {
                        Checkpoint(parser); // r1191

                        bool r1191 = true;
                        r1191 = r1191 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1191, parser);
                        a1190 = r1191;
                    }

                    r1185 |= a1190;
                } // end may a1190

                r1185 = r1185 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1185, parser);
                res = r1185;
            }



            return res;
        }
    }


}