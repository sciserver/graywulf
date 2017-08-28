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

    public partial class SystemVariable : Jhu.Graywulf.Parsing.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"\G(@@[a-zA-Z_][0-9a-zA-Z_]*)", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public SystemVariable()
            :base()
        {
            Value = @"\G(@@[a-zA-Z_][0-9a-zA-Z_]*)";
        }

        public SystemVariable(SystemVariable old)
            :base(old)
        {
        }

        public static SystemVariable Create(string value)
        {
            var terminal = new SystemVariable();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new SystemVariable(this);
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
                Checkpoint(parser); // r98

                bool r98 = true;
                if (r98)
                { // may a99
                    bool a99 = false;
                    {
                        Checkpoint(parser); // r100

                        bool r100 = true;
                        r100 = r100 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LogicalNot());
                        CommitOrRollback(r100, parser);
                        a99 = r100;
                    }

                    r98 |= a99;
                } // end may a99

                if (r98)
                { // may a101
                    bool a101 = false;
                    {
                        Checkpoint(parser); // r102

                        bool r102 = true;
                        r102 = r102 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r102, parser);
                        a101 = r102;
                    }

                    r98 |= a101;
                } // end may a101

                if (r98)
                { // alternatives a103 must
                    bool a103 = false;
                    if (!a103)
                    {
                        Checkpoint(parser); // r104

                        bool r104 = true;
                        r104 = r104 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Predicate());
                        CommitOrRollback(r104, parser);
                        a103 = r104;
                    }

                    if (!a103)
                    {
                        Checkpoint(parser); // r105

                        bool r105 = true;
                        r105 = r105 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpressionBrackets());
                        CommitOrRollback(r105, parser);
                        a103 = r105;
                    }

                    r98 &= a103;

                } // end alternatives a103

                if (r98)
                { // may a106
                    bool a106 = false;
                    {
                        Checkpoint(parser); // r107

                        bool r107 = true;
                        if (r107)
                        { // may a108
                            bool a108 = false;
                            {
                                Checkpoint(parser); // r109

                                bool r109 = true;
                                r109 = r109 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r109, parser);
                                a108 = r109;
                            }

                            r107 |= a108;
                        } // end may a108

                        r107 = r107 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LogicalOperator());
                        if (r107)
                        { // may a110
                            bool a110 = false;
                            {
                                Checkpoint(parser); // r111

                                bool r111 = true;
                                r111 = r111 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r111, parser);
                                a110 = r111;
                            }

                            r107 |= a110;
                        } // end may a110

                        r107 = r107 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                        CommitOrRollback(r107, parser);
                        a106 = r107;
                    }

                    r98 |= a106;
                } // end may a106

                CommitOrRollback(r98, parser);
                res = r98;
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
                Checkpoint(parser); // r112

                bool r112 = true;
                r112 = r112 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r112)
                { // may a113
                    bool a113 = false;
                    {
                        Checkpoint(parser); // r114

                        bool r114 = true;
                        r114 = r114 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r114, parser);
                        a113 = r114;
                    }

                    r112 |= a113;
                } // end may a113

                r112 = r112 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r112)
                { // may a115
                    bool a115 = false;
                    {
                        Checkpoint(parser); // r116

                        bool r116 = true;
                        r116 = r116 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r116, parser);
                        a115 = r116;
                    }

                    r112 |= a115;
                } // end may a115

                r112 = r112 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r112, parser);
                res = r112;
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
                Checkpoint(parser); // r117

                bool r117 = true;
                if (r117)
                { // alternatives a118 must
                    bool a118 = false;
                    if (!a118)
                    {
                        Checkpoint(parser); // r119

                        bool r119 = true;
                        r119 = r119 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r119)
                        { // may a120
                            bool a120 = false;
                            {
                                Checkpoint(parser); // r121

                                bool r121 = true;
                                r121 = r121 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r121, parser);
                                a120 = r121;
                            }

                            r119 |= a120;
                        } // end may a120

                        r119 = r119 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ComparisonOperator());
                        if (r119)
                        { // may a122
                            bool a122 = false;
                            {
                                Checkpoint(parser); // r123

                                bool r123 = true;
                                r123 = r123 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r123, parser);
                                a122 = r123;
                            }

                            r119 |= a122;
                        } // end may a122

                        r119 = r119 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r119, parser);
                        a118 = r119;
                    }

                    if (!a118)
                    {
                        Checkpoint(parser); // r124

                        bool r124 = true;
                        r124 = r124 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r124)
                        { // may a125
                            bool a125 = false;
                            {
                                Checkpoint(parser); // r126

                                bool r126 = true;
                                r126 = r126 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r126, parser);
                                a125 = r126;
                            }

                            r124 |= a125;
                        } // end may a125

                        if (r124)
                        { // may a127
                            bool a127 = false;
                            {
                                Checkpoint(parser); // r128

                                bool r128 = true;
                                r128 = r128 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r128, parser);
                                a127 = r128;
                            }

                            r124 |= a127;
                        } // end may a127

                        r124 = r124 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LIKE"));
                        if (r124)
                        { // may a129
                            bool a129 = false;
                            {
                                Checkpoint(parser); // r130

                                bool r130 = true;
                                r130 = r130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r130, parser);
                                a129 = r130;
                            }

                            r124 |= a129;
                        } // end may a129

                        r124 = r124 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r124)
                        { // may a131
                            bool a131 = false;
                            {
                                Checkpoint(parser); // r132

                                bool r132 = true;
                                if (r132)
                                { // may a133
                                    bool a133 = false;
                                    {
                                        Checkpoint(parser); // r134

                                        bool r134 = true;
                                        r134 = r134 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r134, parser);
                                        a133 = r134;
                                    }

                                    r132 |= a133;
                                } // end may a133

                                r132 = r132 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ESCAPE"));
                                if (r132)
                                { // may a135
                                    bool a135 = false;
                                    {
                                        Checkpoint(parser); // r136

                                        bool r136 = true;
                                        r136 = r136 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r136, parser);
                                        a135 = r136;
                                    }

                                    r132 |= a135;
                                } // end may a135

                                r132 = r132 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                                CommitOrRollback(r132, parser);
                                a131 = r132;
                            }

                            r124 |= a131;
                        } // end may a131

                        CommitOrRollback(r124, parser);
                        a118 = r124;
                    }

                    if (!a118)
                    {
                        Checkpoint(parser); // r137

                        bool r137 = true;
                        r137 = r137 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r137)
                        { // may a138
                            bool a138 = false;
                            {
                                Checkpoint(parser); // r139

                                bool r139 = true;
                                if (r139)
                                { // may a140
                                    bool a140 = false;
                                    {
                                        Checkpoint(parser); // r141

                                        bool r141 = true;
                                        r141 = r141 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r141, parser);
                                        a140 = r141;
                                    }

                                    r139 |= a140;
                                } // end may a140

                                r139 = r139 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r139, parser);
                                a138 = r139;
                            }

                            r137 |= a138;
                        } // end may a138

                        if (r137)
                        { // may a142
                            bool a142 = false;
                            {
                                Checkpoint(parser); // r143

                                bool r143 = true;
                                r143 = r143 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r143, parser);
                                a142 = r143;
                            }

                            r137 |= a142;
                        } // end may a142

                        r137 = r137 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BETWEEN"));
                        if (r137)
                        { // may a144
                            bool a144 = false;
                            {
                                Checkpoint(parser); // r145

                                bool r145 = true;
                                r145 = r145 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r145, parser);
                                a144 = r145;
                            }

                            r137 |= a144;
                        } // end may a144

                        r137 = r137 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r137)
                        { // may a146
                            bool a146 = false;
                            {
                                Checkpoint(parser); // r147

                                bool r147 = true;
                                r147 = r147 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r147, parser);
                                a146 = r147;
                            }

                            r137 |= a146;
                        } // end may a146

                        r137 = r137 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AND"));
                        if (r137)
                        { // may a148
                            bool a148 = false;
                            {
                                Checkpoint(parser); // r149

                                bool r149 = true;
                                r149 = r149 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r149, parser);
                                a148 = r149;
                            }

                            r137 |= a148;
                        } // end may a148

                        r137 = r137 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r137, parser);
                        a118 = r137;
                    }

                    if (!a118)
                    {
                        Checkpoint(parser); // r150

                        bool r150 = true;
                        r150 = r150 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r150)
                        { // may a151
                            bool a151 = false;
                            {
                                Checkpoint(parser); // r152

                                bool r152 = true;
                                r152 = r152 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r152, parser);
                                a151 = r152;
                            }

                            r150 |= a151;
                        } // end may a151

                        r150 = r150 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IS"));
                        if (r150)
                        { // may a153
                            bool a153 = false;
                            {
                                Checkpoint(parser); // r154

                                bool r154 = true;
                                r154 = r154 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r154 = r154 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r154, parser);
                                a153 = r154;
                            }

                            r150 |= a153;
                        } // end may a153

                        r150 = r150 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r150 = r150 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                        CommitOrRollback(r150, parser);
                        a118 = r150;
                    }

                    if (!a118)
                    {
                        Checkpoint(parser); // r155

                        bool r155 = true;
                        r155 = r155 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r155)
                        { // may a156
                            bool a156 = false;
                            {
                                Checkpoint(parser); // r157

                                bool r157 = true;
                                r157 = r157 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r157, parser);
                                a156 = r157;
                            }

                            r155 |= a156;
                        } // end may a156

                        if (r155)
                        { // may a158
                            bool a158 = false;
                            {
                                Checkpoint(parser); // r159

                                bool r159 = true;
                                r159 = r159 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r159, parser);
                                a158 = r159;
                            }

                            r155 |= a158;
                        } // end may a158

                        r155 = r155 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IN"));
                        if (r155)
                        { // may a160
                            bool a160 = false;
                            {
                                Checkpoint(parser); // r161

                                bool r161 = true;
                                r161 = r161 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r161, parser);
                                a160 = r161;
                            }

                            r155 |= a160;
                        } // end may a160

                        if (r155)
                        { // alternatives a162 must
                            bool a162 = false;
                            if (!a162)
                            {
                                Checkpoint(parser); // r163

                                bool r163 = true;
                                r163 = r163 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                                CommitOrRollback(r163, parser);
                                a162 = r163;
                            }

                            if (!a162)
                            {
                                Checkpoint(parser); // r164

                                bool r164 = true;
                                r164 = r164 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                                if (r164)
                                { // may a165
                                    bool a165 = false;
                                    {
                                        Checkpoint(parser); // r166

                                        bool r166 = true;
                                        r166 = r166 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r166, parser);
                                        a165 = r166;
                                    }

                                    r164 |= a165;
                                } // end may a165

                                r164 = r164 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                                if (r164)
                                { // may a167
                                    bool a167 = false;
                                    {
                                        Checkpoint(parser); // r168

                                        bool r168 = true;
                                        r168 = r168 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r168, parser);
                                        a167 = r168;
                                    }

                                    r164 |= a167;
                                } // end may a167

                                r164 = r164 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                                CommitOrRollback(r164, parser);
                                a162 = r164;
                            }

                            r155 &= a162;

                        } // end alternatives a162

                        CommitOrRollback(r155, parser);
                        a118 = r155;
                    }

                    if (!a118)
                    {
                        Checkpoint(parser); // r169

                        bool r169 = true;
                        r169 = r169 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r169)
                        { // may a170
                            bool a170 = false;
                            {
                                Checkpoint(parser); // r171

                                bool r171 = true;
                                r171 = r171 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r171, parser);
                                a170 = r171;
                            }

                            r169 |= a170;
                        } // end may a170

                        r169 = r169 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ComparisonOperator());
                        if (r169)
                        { // may a172
                            bool a172 = false;
                            {
                                Checkpoint(parser); // r173

                                bool r173 = true;
                                r173 = r173 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r173, parser);
                                a172 = r173;
                            }

                            r169 |= a172;
                        } // end may a172

                        if (r169)
                        { // alternatives a174 must
                            bool a174 = false;
                            if (!a174)
                            {
                                Checkpoint(parser); // r175

                                bool r175 = true;
                                r175 = r175 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r175, parser);
                                a174 = r175;
                            }

                            if (!a174)
                            {
                                Checkpoint(parser); // r176

                                bool r176 = true;
                                r176 = r176 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SOME"));
                                CommitOrRollback(r176, parser);
                                a174 = r176;
                            }

                            if (!a174)
                            {
                                Checkpoint(parser); // r177

                                bool r177 = true;
                                r177 = r177 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ANY"));
                                CommitOrRollback(r177, parser);
                                a174 = r177;
                            }

                            r169 &= a174;

                        } // end alternatives a174

                        if (r169)
                        { // may a178
                            bool a178 = false;
                            {
                                Checkpoint(parser); // r179

                                bool r179 = true;
                                r179 = r179 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r179, parser);
                                a178 = r179;
                            }

                            r169 |= a178;
                        } // end may a178

                        r169 = r169 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r169, parser);
                        a118 = r169;
                    }

                    if (!a118)
                    {
                        Checkpoint(parser); // r180

                        bool r180 = true;
                        r180 = r180 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"EXISTS"));
                        if (r180)
                        { // may a181
                            bool a181 = false;
                            {
                                Checkpoint(parser); // r182

                                bool r182 = true;
                                r182 = r182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r182, parser);
                                a181 = r182;
                            }

                            r180 |= a181;
                        } // end may a181

                        r180 = r180 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r180, parser);
                        a118 = r180;
                    }

                    r117 &= a118;

                } // end alternatives a118

                CommitOrRollback(r117, parser);
                res = r117;
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
                Checkpoint(parser); // r183

                bool r183 = true;
                r183 = r183 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CASE"));
                if (r183)
                { // may a184
                    bool a184 = false;
                    {
                        Checkpoint(parser); // r185

                        bool r185 = true;
                        r185 = r185 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r185, parser);
                        a184 = r185;
                    }

                    r183 |= a184;
                } // end may a184

                r183 = r183 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r183)
                { // may a186
                    bool a186 = false;
                    {
                        Checkpoint(parser); // r187

                        bool r187 = true;
                        r187 = r187 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r187, parser);
                        a186 = r187;
                    }

                    r183 |= a186;
                } // end may a186

                r183 = r183 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList());
                if (r183)
                { // may a188
                    bool a188 = false;
                    {
                        Checkpoint(parser); // r189

                        bool r189 = true;
                        r189 = r189 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CaseElse());
                        CommitOrRollback(r189, parser);
                        a188 = r189;
                    }

                    r183 |= a188;
                } // end may a188

                r183 = r183 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r183, parser);
                res = r183;
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
                Checkpoint(parser); // r190

                bool r190 = true;
                r190 = r190 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhen());
                if (r190)
                { // may a191
                    bool a191 = false;
                    {
                        Checkpoint(parser); // r192

                        bool r192 = true;
                        r192 = r192 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList());
                        CommitOrRollback(r192, parser);
                        a191 = r192;
                    }

                    r190 |= a191;
                } // end may a191

                CommitOrRollback(r190, parser);
                res = r190;
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
                Checkpoint(parser); // r193

                bool r193 = true;
                r193 = r193 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHEN"));
                r193 = r193 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                r193 = r193 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THEN"));
                r193 = r193 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r193, parser);
                res = r193;
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
                Checkpoint(parser); // r194

                bool r194 = true;
                r194 = r194 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CASE"));
                r194 = r194 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList());
                if (r194)
                { // may a195
                    bool a195 = false;
                    {
                        Checkpoint(parser); // r196

                        bool r196 = true;
                        r196 = r196 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CaseElse());
                        CommitOrRollback(r196, parser);
                        a195 = r196;
                    }

                    r194 |= a195;
                } // end may a195

                r194 = r194 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r194, parser);
                res = r194;
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
                Checkpoint(parser); // r197

                bool r197 = true;
                r197 = r197 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhen());
                if (r197)
                { // may a198
                    bool a198 = false;
                    {
                        Checkpoint(parser); // r199

                        bool r199 = true;
                        r199 = r199 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList());
                        CommitOrRollback(r199, parser);
                        a198 = r199;
                    }

                    r197 |= a198;
                } // end may a198

                CommitOrRollback(r197, parser);
                res = r197;
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
                Checkpoint(parser); // r200

                bool r200 = true;
                r200 = r200 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHEN"));
                r200 = r200 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THEN"));
                r200 = r200 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r200, parser);
                res = r200;
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
                Checkpoint(parser); // r201

                bool r201 = true;
                r201 = r201 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ELSE"));
                r201 = r201 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r201, parser);
                res = r201;
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
                Checkpoint(parser); // r202

                bool r202 = true;
                if (r202)
                { // may a203
                    bool a203 = false;
                    {
                        Checkpoint(parser); // r204

                        bool r204 = true;
                        r204 = r204 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                        if (r204)
                        { // may a205
                            bool a205 = false;
                            {
                                Checkpoint(parser); // r206

                                bool r206 = true;
                                r206 = r206 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r206, parser);
                                a205 = r206;
                            }

                            r204 |= a205;
                        } // end may a205

                        r204 = r204 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                        if (r204)
                        { // may a207
                            bool a207 = false;
                            {
                                Checkpoint(parser); // r208

                                bool r208 = true;
                                r208 = r208 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r208, parser);
                                a207 = r208;
                            }

                            r204 |= a207;
                        } // end may a207

                        CommitOrRollback(r204, parser);
                        a203 = r204;
                    }

                    r202 |= a203;
                } // end may a203

                if (r202)
                { // alternatives a209 must
                    bool a209 = false;
                    if (!a209)
                    {
                        Checkpoint(parser); // r210

                        bool r210 = true;
                        r210 = r210 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                        if (r210)
                        { // may a211
                            bool a211 = false;
                            {
                                Checkpoint(parser); // r212

                                bool r212 = true;
                                r212 = r212 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r212, parser);
                                a211 = r212;
                            }

                            r210 |= a211;
                        } // end may a211

                        r210 = r210 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r210)
                        { // may a213
                            bool a213 = false;
                            {
                                Checkpoint(parser); // r214

                                bool r214 = true;
                                if (r214)
                                { // may a215
                                    bool a215 = false;
                                    {
                                        Checkpoint(parser); // r216

                                        bool r216 = true;
                                        r216 = r216 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r216, parser);
                                        a215 = r216;
                                    }

                                    r214 |= a215;
                                } // end may a215

                                r214 = r214 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                CommitOrRollback(r214, parser);
                                a213 = r214;
                            }

                            r210 |= a213;
                        } // end may a213

                        if (r210)
                        { // may a217
                            bool a217 = false;
                            {
                                Checkpoint(parser); // r218

                                bool r218 = true;
                                r218 = r218 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r218, parser);
                                a217 = r218;
                            }

                            r210 |= a217;
                        } // end may a217

                        r210 = r210 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r210)
                        { // may a219
                            bool a219 = false;
                            {
                                Checkpoint(parser); // r220

                                bool r220 = true;
                                r220 = r220 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r220, parser);
                                a219 = r220;
                            }

                            r210 |= a219;
                        } // end may a219

                        r210 = r210 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r210, parser);
                        a209 = r210;
                    }

                    if (!a209)
                    {
                        Checkpoint(parser); // r221

                        bool r221 = true;
                        r221 = r221 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                        if (r221)
                        { // may a222
                            bool a222 = false;
                            {
                                Checkpoint(parser); // r223

                                bool r223 = true;
                                r223 = r223 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r223, parser);
                                a222 = r223;
                            }

                            r221 |= a222;
                        } // end may a222

                        r221 = r221 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r221)
                        { // may a224
                            bool a224 = false;
                            {
                                Checkpoint(parser); // r225

                                bool r225 = true;
                                r225 = r225 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r225, parser);
                                a224 = r225;
                            }

                            r221 |= a224;
                        } // end may a224

                        r221 = r221 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r221, parser);
                        a209 = r221;
                    }

                    if (!a209)
                    {
                        Checkpoint(parser); // r226

                        bool r226 = true;
                        r226 = r226 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r226, parser);
                        a209 = r226;
                    }

                    r202 &= a209;

                } // end alternatives a209

                CommitOrRollback(r202, parser);
                res = r202;
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
                Checkpoint(parser); // r227

                bool r227 = true;
                if (r227)
                { // alternatives a228 must
                    bool a228 = false;
                    if (!a228)
                    {
                        Checkpoint(parser); // r229

                        bool r229 = true;
                        if (r229)
                        { // may a230
                            bool a230 = false;
                            {
                                Checkpoint(parser); // r231

                                bool r231 = true;
                                r231 = r231 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                                if (r231)
                                { // may a232
                                    bool a232 = false;
                                    {
                                        Checkpoint(parser); // r233

                                        bool r233 = true;
                                        r233 = r233 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r233, parser);
                                        a232 = r233;
                                    }

                                    r231 |= a232;
                                } // end may a232

                                r231 = r231 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                                if (r231)
                                { // may a234
                                    bool a234 = false;
                                    {
                                        Checkpoint(parser); // r235

                                        bool r235 = true;
                                        r235 = r235 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r235, parser);
                                        a234 = r235;
                                    }

                                    r231 |= a234;
                                } // end may a234

                                CommitOrRollback(r231, parser);
                                a230 = r231;
                            }

                            r229 |= a230;
                        } // end may a230

                        if (r229)
                        { // alternatives a236 must
                            bool a236 = false;
                            if (!a236)
                            {
                                Checkpoint(parser); // r237

                                bool r237 = true;
                                r237 = r237 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                                if (r237)
                                { // may a238
                                    bool a238 = false;
                                    {
                                        Checkpoint(parser); // r239

                                        bool r239 = true;
                                        r239 = r239 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r239, parser);
                                        a238 = r239;
                                    }

                                    r237 |= a238;
                                } // end may a238

                                r237 = r237 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r237)
                                { // may a240
                                    bool a240 = false;
                                    {
                                        Checkpoint(parser); // r241

                                        bool r241 = true;
                                        r241 = r241 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r241, parser);
                                        a240 = r241;
                                    }

                                    r237 |= a240;
                                } // end may a240

                                if (r237)
                                { // may a242
                                    bool a242 = false;
                                    {
                                        Checkpoint(parser); // r243

                                        bool r243 = true;
                                        r243 = r243 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                        if (r243)
                                        { // may a244
                                            bool a244 = false;
                                            {
                                                Checkpoint(parser); // r245

                                                bool r245 = true;
                                                r245 = r245 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                                CommitOrRollback(r245, parser);
                                                a244 = r245;
                                            }

                                            r243 |= a244;
                                        } // end may a244

                                        CommitOrRollback(r243, parser);
                                        a242 = r243;
                                    }

                                    r237 |= a242;
                                } // end may a242

                                r237 = r237 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r237)
                                { // may a246
                                    bool a246 = false;
                                    {
                                        Checkpoint(parser); // r247

                                        bool r247 = true;
                                        r247 = r247 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r247, parser);
                                        a246 = r247;
                                    }

                                    r237 |= a246;
                                } // end may a246

                                r237 = r237 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r237)
                                { // may a248
                                    bool a248 = false;
                                    {
                                        Checkpoint(parser); // r249

                                        bool r249 = true;
                                        r249 = r249 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r249, parser);
                                        a248 = r249;
                                    }

                                    r237 |= a248;
                                } // end may a248

                                r237 = r237 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r237)
                                { // may a250
                                    bool a250 = false;
                                    {
                                        Checkpoint(parser); // r251

                                        bool r251 = true;
                                        r251 = r251 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r251, parser);
                                        a250 = r251;
                                    }

                                    r237 |= a250;
                                } // end may a250

                                if (r237)
                                { // alternatives a252 must
                                    bool a252 = false;
                                    if (!a252)
                                    {
                                        Checkpoint(parser); // r253

                                        bool r253 = true;
                                        r253 = r253 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r253, parser);
                                        a252 = r253;
                                    }

                                    if (!a252)
                                    {
                                        Checkpoint(parser); // r254

                                        bool r254 = true;
                                        r254 = r254 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r254, parser);
                                        a252 = r254;
                                    }

                                    r237 &= a252;

                                } // end alternatives a252

                                CommitOrRollback(r237, parser);
                                a236 = r237;
                            }

                            if (!a236)
                            {
                                Checkpoint(parser); // r255

                                bool r255 = true;
                                r255 = r255 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                if (r255)
                                { // may a256
                                    bool a256 = false;
                                    {
                                        Checkpoint(parser); // r257

                                        bool r257 = true;
                                        r257 = r257 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r257, parser);
                                        a256 = r257;
                                    }

                                    r255 |= a256;
                                } // end may a256

                                r255 = r255 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r255)
                                { // may a258
                                    bool a258 = false;
                                    {
                                        Checkpoint(parser); // r259

                                        bool r259 = true;
                                        r259 = r259 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r259, parser);
                                        a258 = r259;
                                    }

                                    r255 |= a258;
                                } // end may a258

                                r255 = r255 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r255)
                                { // may a260
                                    bool a260 = false;
                                    {
                                        Checkpoint(parser); // r261

                                        bool r261 = true;
                                        r261 = r261 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r261, parser);
                                        a260 = r261;
                                    }

                                    r255 |= a260;
                                } // end may a260

                                r255 = r255 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r255)
                                { // may a262
                                    bool a262 = false;
                                    {
                                        Checkpoint(parser); // r263

                                        bool r263 = true;
                                        r263 = r263 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r263, parser);
                                        a262 = r263;
                                    }

                                    r255 |= a262;
                                } // end may a262

                                if (r255)
                                { // alternatives a264 must
                                    bool a264 = false;
                                    if (!a264)
                                    {
                                        Checkpoint(parser); // r265

                                        bool r265 = true;
                                        r265 = r265 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r265, parser);
                                        a264 = r265;
                                    }

                                    if (!a264)
                                    {
                                        Checkpoint(parser); // r266

                                        bool r266 = true;
                                        r266 = r266 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r266, parser);
                                        a264 = r266;
                                    }

                                    r255 &= a264;

                                } // end alternatives a264

                                CommitOrRollback(r255, parser);
                                a236 = r255;
                            }

                            if (!a236)
                            {
                                Checkpoint(parser); // r267

                                bool r267 = true;
                                r267 = r267 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r267)
                                { // may a268
                                    bool a268 = false;
                                    {
                                        Checkpoint(parser); // r269

                                        bool r269 = true;
                                        r269 = r269 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r269, parser);
                                        a268 = r269;
                                    }

                                    r267 |= a268;
                                } // end may a268

                                r267 = r267 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r267)
                                { // may a270
                                    bool a270 = false;
                                    {
                                        Checkpoint(parser); // r271

                                        bool r271 = true;
                                        r271 = r271 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r271, parser);
                                        a270 = r271;
                                    }

                                    r267 |= a270;
                                } // end may a270

                                if (r267)
                                { // alternatives a272 must
                                    bool a272 = false;
                                    if (!a272)
                                    {
                                        Checkpoint(parser); // r273

                                        bool r273 = true;
                                        r273 = r273 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r273, parser);
                                        a272 = r273;
                                    }

                                    if (!a272)
                                    {
                                        Checkpoint(parser); // r274

                                        bool r274 = true;
                                        r274 = r274 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r274, parser);
                                        a272 = r274;
                                    }

                                    r267 &= a272;

                                } // end alternatives a272

                                CommitOrRollback(r267, parser);
                                a236 = r267;
                            }

                            r229 &= a236;

                        } // end alternatives a236

                        CommitOrRollback(r229, parser);
                        a228 = r229;
                    }

                    if (!a228)
                    {
                        Checkpoint(parser); // r275

                        bool r275 = true;
                        if (r275)
                        { // alternatives a276 must
                            bool a276 = false;
                            if (!a276)
                            {
                                Checkpoint(parser); // r277

                                bool r277 = true;
                                r277 = r277 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                CommitOrRollback(r277, parser);
                                a276 = r277;
                            }

                            if (!a276)
                            {
                                Checkpoint(parser); // r278

                                bool r278 = true;
                                r278 = r278 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                CommitOrRollback(r278, parser);
                                a276 = r278;
                            }

                            r275 &= a276;

                        } // end alternatives a276

                        CommitOrRollback(r275, parser);
                        a228 = r275;
                    }

                    r227 &= a228;

                } // end alternatives a228

                CommitOrRollback(r227, parser);
                res = r227;
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
                Checkpoint(parser); // r279

                bool r279 = true;
                r279 = r279 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TypeName());
                if (r279)
                { // may a280
                    bool a280 = false;
                    {
                        Checkpoint(parser); // r281

                        bool r281 = true;
                        if (r281)
                        { // may a282
                            bool a282 = false;
                            {
                                Checkpoint(parser); // r283

                                bool r283 = true;
                                r283 = r283 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r283, parser);
                                a282 = r283;
                            }

                            r281 |= a282;
                        } // end may a282

                        if (r281)
                        { // alternatives a284 must
                            bool a284 = false;
                            if (!a284)
                            {
                                Checkpoint(parser); // r285

                                bool r285 = true;
                                r285 = r285 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataTypeScaleAndPrecision());
                                CommitOrRollback(r285, parser);
                                a284 = r285;
                            }

                            if (!a284)
                            {
                                Checkpoint(parser); // r286

                                bool r286 = true;
                                r286 = r286 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataTypeSize());
                                CommitOrRollback(r286, parser);
                                a284 = r286;
                            }

                            r281 &= a284;

                        } // end alternatives a284

                        CommitOrRollback(r281, parser);
                        a280 = r281;
                    }

                    r279 |= a280;
                } // end may a280

                if (r279)
                { // may a287
                    bool a287 = false;
                    {
                        Checkpoint(parser); // r288

                        bool r288 = true;
                        if (r288)
                        { // may a289
                            bool a289 = false;
                            {
                                Checkpoint(parser); // r290

                                bool r290 = true;
                                r290 = r290 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r290, parser);
                                a289 = r290;
                            }

                            r288 |= a289;
                        } // end may a289

                        if (r288)
                        { // may a291
                            bool a291 = false;
                            {
                                Checkpoint(parser); // r292

                                bool r292 = true;
                                r292 = r292 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                r292 = r292 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r292, parser);
                                a291 = r292;
                            }

                            r288 |= a291;
                        } // end may a291

                        r288 = r288 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                        CommitOrRollback(r288, parser);
                        a287 = r288;
                    }

                    r279 |= a287;
                } // end may a287

                CommitOrRollback(r279, parser);
                res = r279;
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
                Checkpoint(parser); // r293

                bool r293 = true;
                r293 = r293 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r293)
                { // may a294
                    bool a294 = false;
                    {
                        Checkpoint(parser); // r295

                        bool r295 = true;
                        r295 = r295 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r295, parser);
                        a294 = r295;
                    }

                    r293 |= a294;
                } // end may a294

                if (r293)
                { // alternatives a296 must
                    bool a296 = false;
                    if (!a296)
                    {
                        Checkpoint(parser); // r297

                        bool r297 = true;
                        r297 = r297 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"MAX"));
                        CommitOrRollback(r297, parser);
                        a296 = r297;
                    }

                    if (!a296)
                    {
                        Checkpoint(parser); // r298

                        bool r298 = true;
                        r298 = r298 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r298, parser);
                        a296 = r298;
                    }

                    r293 &= a296;

                } // end alternatives a296

                if (r293)
                { // may a299
                    bool a299 = false;
                    {
                        Checkpoint(parser); // r300

                        bool r300 = true;
                        r300 = r300 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r300, parser);
                        a299 = r300;
                    }

                    r293 |= a299;
                } // end may a299

                r293 = r293 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r293, parser);
                res = r293;
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
                Checkpoint(parser); // r301

                bool r301 = true;
                r301 = r301 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r301)
                { // may a302
                    bool a302 = false;
                    {
                        Checkpoint(parser); // r303

                        bool r303 = true;
                        r303 = r303 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r303, parser);
                        a302 = r303;
                    }

                    r301 |= a302;
                } // end may a302

                r301 = r301 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                if (r301)
                { // may a304
                    bool a304 = false;
                    {
                        Checkpoint(parser); // r305

                        bool r305 = true;
                        r305 = r305 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r305, parser);
                        a304 = r305;
                    }

                    r301 |= a304;
                } // end may a304

                r301 = r301 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                if (r301)
                { // may a306
                    bool a306 = false;
                    {
                        Checkpoint(parser); // r307

                        bool r307 = true;
                        r307 = r307 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r307, parser);
                        a306 = r307;
                    }

                    r301 |= a306;
                } // end may a306

                r301 = r301 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                if (r301)
                { // may a308
                    bool a308 = false;
                    {
                        Checkpoint(parser); // r309

                        bool r309 = true;
                        r309 = r309 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r309, parser);
                        a308 = r309;
                    }

                    r301 |= a308;
                } // end may a308

                r301 = r301 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r301, parser);
                res = r301;
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
                Checkpoint(parser); // r310

                bool r310 = true;
                if (r310)
                { // alternatives a311 must
                    bool a311 = false;
                    if (!a311)
                    {
                        Checkpoint(parser); // r312

                        bool r312 = true;
                        r312 = r312 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdfIdentifier());
                        CommitOrRollback(r312, parser);
                        a311 = r312;
                    }

                    if (!a311)
                    {
                        Checkpoint(parser); // r313

                        bool r313 = true;
                        r313 = r313 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r313, parser);
                        a311 = r313;
                    }

                    r310 &= a311;

                } // end alternatives a311

                CommitOrRollback(r310, parser);
                res = r310;
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
                Checkpoint(parser); // r314

                bool r314 = true;
                if (r314)
                { // may a315
                    bool a315 = false;
                    {
                        Checkpoint(parser); // r316

                        bool r316 = true;
                        r316 = r316 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                        if (r316)
                        { // may a317
                            bool a317 = false;
                            {
                                Checkpoint(parser); // r318

                                bool r318 = true;
                                r318 = r318 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r318, parser);
                                a317 = r318;
                            }

                            r316 |= a317;
                        } // end may a317

                        r316 = r316 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                        if (r316)
                        { // may a319
                            bool a319 = false;
                            {
                                Checkpoint(parser); // r320

                                bool r320 = true;
                                r320 = r320 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r320, parser);
                                a319 = r320;
                            }

                            r316 |= a319;
                        } // end may a319

                        CommitOrRollback(r316, parser);
                        a315 = r316;
                    }

                    r314 |= a315;
                } // end may a315

                if (r314)
                { // alternatives a321 must
                    bool a321 = false;
                    if (!a321)
                    {
                        Checkpoint(parser); // r322

                        bool r322 = true;
                        r322 = r322 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                        if (r322)
                        { // may a323
                            bool a323 = false;
                            {
                                Checkpoint(parser); // r324

                                bool r324 = true;
                                r324 = r324 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r324, parser);
                                a323 = r324;
                            }

                            r322 |= a323;
                        } // end may a323

                        r322 = r322 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r322)
                        { // may a325
                            bool a325 = false;
                            {
                                Checkpoint(parser); // r326

                                bool r326 = true;
                                if (r326)
                                { // may a327
                                    bool a327 = false;
                                    {
                                        Checkpoint(parser); // r328

                                        bool r328 = true;
                                        r328 = r328 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r328, parser);
                                        a327 = r328;
                                    }

                                    r326 |= a327;
                                } // end may a327

                                r326 = r326 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                CommitOrRollback(r326, parser);
                                a325 = r326;
                            }

                            r322 |= a325;
                        } // end may a325

                        if (r322)
                        { // may a329
                            bool a329 = false;
                            {
                                Checkpoint(parser); // r330

                                bool r330 = true;
                                r330 = r330 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r330, parser);
                                a329 = r330;
                            }

                            r322 |= a329;
                        } // end may a329

                        r322 = r322 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r322)
                        { // may a331
                            bool a331 = false;
                            {
                                Checkpoint(parser); // r332

                                bool r332 = true;
                                r332 = r332 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r332, parser);
                                a331 = r332;
                            }

                            r322 |= a331;
                        } // end may a331

                        r322 = r322 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r322, parser);
                        a321 = r322;
                    }

                    if (!a321)
                    {
                        Checkpoint(parser); // r333

                        bool r333 = true;
                        r333 = r333 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                        if (r333)
                        { // may a334
                            bool a334 = false;
                            {
                                Checkpoint(parser); // r335

                                bool r335 = true;
                                r335 = r335 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r335, parser);
                                a334 = r335;
                            }

                            r333 |= a334;
                        } // end may a334

                        r333 = r333 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r333)
                        { // may a336
                            bool a336 = false;
                            {
                                Checkpoint(parser); // r337

                                bool r337 = true;
                                r337 = r337 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r337, parser);
                                a336 = r337;
                            }

                            r333 |= a336;
                        } // end may a336

                        r333 = r333 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r333, parser);
                        a321 = r333;
                    }

                    r314 &= a321;

                } // end alternatives a321

                CommitOrRollback(r314, parser);
                res = r314;
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
                Checkpoint(parser); // r338

                bool r338 = true;
                r338 = r338 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r338)
                { // may a339
                    bool a339 = false;
                    {
                        Checkpoint(parser); // r340

                        bool r340 = true;
                        r340 = r340 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r340, parser);
                        a339 = r340;
                    }

                    r338 |= a339;
                } // end may a339

                r338 = r338 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                if (r338)
                { // may a341
                    bool a341 = false;
                    {
                        Checkpoint(parser); // r342

                        bool r342 = true;
                        r342 = r342 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r342, parser);
                        a341 = r342;
                    }

                    r338 |= a341;
                } // end may a341

                r338 = r338 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                CommitOrRollback(r338, parser);
                res = r338;
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
                Checkpoint(parser); // r343

                bool r343 = true;
                r343 = r343 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r343, parser);
                res = r343;
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
                Checkpoint(parser); // r344

                bool r344 = true;
                r344 = r344 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Argument());
                if (r344)
                { // may a345
                    bool a345 = false;
                    {
                        Checkpoint(parser); // r346

                        bool r346 = true;
                        if (r346)
                        { // may a347
                            bool a347 = false;
                            {
                                Checkpoint(parser); // r348

                                bool r348 = true;
                                r348 = r348 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r348, parser);
                                a347 = r348;
                            }

                            r346 |= a347;
                        } // end may a347

                        r346 = r346 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r346)
                        { // may a349
                            bool a349 = false;
                            {
                                Checkpoint(parser); // r350

                                bool r350 = true;
                                r350 = r350 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r350, parser);
                                a349 = r350;
                            }

                            r346 |= a349;
                        } // end may a349

                        r346 = r346 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                        CommitOrRollback(r346, parser);
                        a345 = r346;
                    }

                    r344 |= a345;
                } // end may a345

                CommitOrRollback(r344, parser);
                res = r344;
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
                Checkpoint(parser); // r351

                bool r351 = true;
                r351 = r351 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdtFunctionIdentifier());
                if (r351)
                { // may a352
                    bool a352 = false;
                    {
                        Checkpoint(parser); // r353

                        bool r353 = true;
                        r353 = r353 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r353, parser);
                        a352 = r353;
                    }

                    r351 |= a352;
                } // end may a352

                r351 = r351 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r351, parser);
                res = r351;
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
                Checkpoint(parser); // r354

                bool r354 = true;
                r354 = r354 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionIdentifier());
                if (r354)
                { // may a355
                    bool a355 = false;
                    {
                        Checkpoint(parser); // r356

                        bool r356 = true;
                        r356 = r356 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r356, parser);
                        a355 = r356;
                    }

                    r354 |= a355;
                } // end may a355

                r354 = r354 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r354, parser);
                res = r354;
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
                Checkpoint(parser); // r357

                bool r357 = true;
                r357 = r357 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionIdentifier());
                if (r357)
                { // may a358
                    bool a358 = false;
                    {
                        Checkpoint(parser); // r359

                        bool r359 = true;
                        r359 = r359 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r359, parser);
                        a358 = r359;
                    }

                    r357 |= a358;
                } // end may a358

                r357 = r357 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r357, parser);
                res = r357;
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
                Checkpoint(parser); // r360

                bool r360 = true;
                r360 = r360 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                if (r360)
                { // may a361
                    bool a361 = false;
                    {
                        Checkpoint(parser); // r362

                        bool r362 = true;
                        r362 = r362 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r362, parser);
                        a361 = r362;
                    }

                    r360 |= a361;
                } // end may a361

                r360 = r360 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                if (r360)
                { // may a363
                    bool a363 = false;
                    {
                        Checkpoint(parser); // r364

                        bool r364 = true;
                        r364 = r364 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r364, parser);
                        a363 = r364;
                    }

                    r360 |= a363;
                } // end may a363

                r360 = r360 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OverClause());
                CommitOrRollback(r360, parser);
                res = r360;
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
                Checkpoint(parser); // r365

                bool r365 = true;
                r365 = r365 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r365)
                { // may a366
                    bool a366 = false;
                    {
                        Checkpoint(parser); // r367

                        bool r367 = true;
                        r367 = r367 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r367, parser);
                        a366 = r367;
                    }

                    r365 |= a366;
                } // end may a366

                if (r365)
                { // may a368
                    bool a368 = false;
                    {
                        Checkpoint(parser); // r369

                        bool r369 = true;
                        r369 = r369 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                        CommitOrRollback(r369, parser);
                        a368 = r369;
                    }

                    r365 |= a368;
                } // end may a368

                if (r365)
                { // may a370
                    bool a370 = false;
                    {
                        Checkpoint(parser); // r371

                        bool r371 = true;
                        r371 = r371 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r371, parser);
                        a370 = r371;
                    }

                    r365 |= a370;
                } // end may a370

                r365 = r365 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r365, parser);
                res = r365;
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
                Checkpoint(parser); // r372

                bool r372 = true;
                r372 = r372 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OVER"));
                if (r372)
                { // may a373
                    bool a373 = false;
                    {
                        Checkpoint(parser); // r374

                        bool r374 = true;
                        r374 = r374 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r374, parser);
                        a373 = r374;
                    }

                    r372 |= a373;
                } // end may a373

                r372 = r372 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r372)
                { // may a375
                    bool a375 = false;
                    {
                        Checkpoint(parser); // r376

                        bool r376 = true;
                        r376 = r376 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r376, parser);
                        a375 = r376;
                    }

                    r372 |= a375;
                } // end may a375

                if (r372)
                { // may a377
                    bool a377 = false;
                    {
                        Checkpoint(parser); // r378

                        bool r378 = true;
                        r378 = r378 && Match(parser, new Jhu.Graywulf.Sql.Parsing.PartitionByClause());
                        if (r378)
                        { // may a379
                            bool a379 = false;
                            {
                                Checkpoint(parser); // r380

                                bool r380 = true;
                                r380 = r380 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r380, parser);
                                a379 = r380;
                            }

                            r378 |= a379;
                        } // end may a379

                        CommitOrRollback(r378, parser);
                        a377 = r378;
                    }

                    r372 |= a377;
                } // end may a377

                if (r372)
                { // may a381
                    bool a381 = false;
                    {
                        Checkpoint(parser); // r382

                        bool r382 = true;
                        r382 = r382 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        if (r382)
                        { // may a383
                            bool a383 = false;
                            {
                                Checkpoint(parser); // r384

                                bool r384 = true;
                                r384 = r384 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r384, parser);
                                a383 = r384;
                            }

                            r382 |= a383;
                        } // end may a383

                        CommitOrRollback(r382, parser);
                        a381 = r382;
                    }

                    r372 |= a381;
                } // end may a381

                r372 = r372 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r372, parser);
                res = r372;
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
                Checkpoint(parser); // r385

                bool r385 = true;
                r385 = r385 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PARTITION"));
                r385 = r385 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r385 = r385 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                if (r385)
                { // may a386
                    bool a386 = false;
                    {
                        Checkpoint(parser); // r387

                        bool r387 = true;
                        r387 = r387 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r387, parser);
                        a386 = r387;
                    }

                    r385 |= a386;
                } // end may a386

                r385 = r385 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                CommitOrRollback(r385, parser);
                res = r385;
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
                Checkpoint(parser); // r388

                bool r388 = true;
                if (r388)
                { // may a389
                    bool a389 = false;
                    {
                        Checkpoint(parser); // r390

                        bool r390 = true;
                        r390 = r390 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r390, parser);
                        a389 = r390;
                    }

                    r388 |= a389;
                } // end may a389

                if (r388)
                { // may a391
                    bool a391 = false;
                    {
                        Checkpoint(parser); // r392

                        bool r392 = true;
                        r392 = r392 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                        CommitOrRollback(r392, parser);
                        a391 = r392;
                    }

                    r388 |= a391;
                } // end may a391

                if (r388)
                { // may a393
                    bool a393 = false;
                    {
                        Checkpoint(parser); // r394

                        bool r394 = true;
                        r394 = r394 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementSeparator());
                        if (r394)
                        { // may a395
                            bool a395 = false;
                            {
                                Checkpoint(parser); // r396

                                bool r396 = true;
                                r396 = r396 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                                CommitOrRollback(r396, parser);
                                a395 = r396;
                            }

                            r394 |= a395;
                        } // end may a395

                        CommitOrRollback(r394, parser);
                        a393 = r394;
                    }

                    r388 |= a393;
                } // end may a393

                if (r388)
                { // may a397
                    bool a397 = false;
                    {
                        Checkpoint(parser); // r398

                        bool r398 = true;
                        r398 = r398 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r398, parser);
                        a397 = r398;
                    }

                    r388 |= a397;
                } // end may a397

                CommitOrRollback(r388, parser);
                res = r388;
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
                Checkpoint(parser); // r399

                bool r399 = true;
                if (r399)
                { // alternatives a400 must
                    bool a400 = false;
                    if (!a400)
                    {
                        Checkpoint(parser); // r401

                        bool r401 = true;
                        if (r401)
                        { // may a402
                            bool a402 = false;
                            {
                                Checkpoint(parser); // r403

                                bool r403 = true;
                                r403 = r403 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r403, parser);
                                a402 = r403;
                            }

                            r401 |= a402;
                        } // end may a402

                        r401 = r401 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Semicolon());
                        if (r401)
                        { // may a404
                            bool a404 = false;
                            {
                                Checkpoint(parser); // r405

                                bool r405 = true;
                                r405 = r405 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r405, parser);
                                a404 = r405;
                            }

                            r401 |= a404;
                        } // end may a404

                        CommitOrRollback(r401, parser);
                        a400 = r401;
                    }

                    if (!a400)
                    {
                        Checkpoint(parser); // r406

                        bool r406 = true;
                        r406 = r406 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r406, parser);
                        a400 = r406;
                    }

                    r399 &= a400;

                } // end alternatives a400

                CommitOrRollback(r399, parser);
                res = r399;
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
                Checkpoint(parser); // r407

                bool r407 = true;
                if (r407)
                { // alternatives a408 must
                    bool a408 = false;
                    if (!a408)
                    {
                        Checkpoint(parser); // r409

                        bool r409 = true;
                        r409 = r409 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Label());
                        CommitOrRollback(r409, parser);
                        a408 = r409;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r410

                        bool r410 = true;
                        r410 = r410 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GotoStatement());
                        CommitOrRollback(r410, parser);
                        a408 = r410;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r411

                        bool r411 = true;
                        r411 = r411 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BeginEndStatement());
                        CommitOrRollback(r411, parser);
                        a408 = r411;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r412

                        bool r412 = true;
                        r412 = r412 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhileStatement());
                        CommitOrRollback(r412, parser);
                        a408 = r412;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r413

                        bool r413 = true;
                        r413 = r413 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BreakStatement());
                        CommitOrRollback(r413, parser);
                        a408 = r413;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r414

                        bool r414 = true;
                        r414 = r414 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ContinueStatement());
                        CommitOrRollback(r414, parser);
                        a408 = r414;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r415

                        bool r415 = true;
                        r415 = r415 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ReturnStatement());
                        CommitOrRollback(r415, parser);
                        a408 = r415;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r416

                        bool r416 = true;
                        r416 = r416 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IfStatement());
                        CommitOrRollback(r416, parser);
                        a408 = r416;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r417

                        bool r417 = true;
                        r417 = r417 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TryCatchStatement());
                        CommitOrRollback(r417, parser);
                        a408 = r417;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r418

                        bool r418 = true;
                        r418 = r418 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ThrowStatement());
                        CommitOrRollback(r418, parser);
                        a408 = r418;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r419

                        bool r419 = true;
                        r419 = r419 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareCursorStatement());
                        CommitOrRollback(r419, parser);
                        a408 = r419;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r420

                        bool r420 = true;
                        r420 = r420 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SetCursorStatement());
                        CommitOrRollback(r420, parser);
                        a408 = r420;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r421

                        bool r421 = true;
                        r421 = r421 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorOperationStatement());
                        CommitOrRollback(r421, parser);
                        a408 = r421;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r422

                        bool r422 = true;
                        r422 = r422 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FetchStatement());
                        CommitOrRollback(r422, parser);
                        a408 = r422;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r423

                        bool r423 = true;
                        r423 = r423 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareVariableStatement());
                        CommitOrRollback(r423, parser);
                        a408 = r423;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r424

                        bool r424 = true;
                        r424 = r424 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SetVariableStatement());
                        CommitOrRollback(r424, parser);
                        a408 = r424;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r425

                        bool r425 = true;
                        r425 = r425 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareTableStatement());
                        CommitOrRollback(r425, parser);
                        a408 = r425;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r426

                        bool r426 = true;
                        r426 = r426 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CreateTableStatement());
                        CommitOrRollback(r426, parser);
                        a408 = r426;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r427

                        bool r427 = true;
                        r427 = r427 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DropTableStatement());
                        CommitOrRollback(r427, parser);
                        a408 = r427;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r428

                        bool r428 = true;
                        r428 = r428 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TruncateTableStatement());
                        CommitOrRollback(r428, parser);
                        a408 = r428;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r429

                        bool r429 = true;
                        r429 = r429 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CreateIndexStatement());
                        CommitOrRollback(r429, parser);
                        a408 = r429;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r430

                        bool r430 = true;
                        r430 = r430 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DropIndexStatement());
                        CommitOrRollback(r430, parser);
                        a408 = r430;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r431

                        bool r431 = true;
                        r431 = r431 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectStatement());
                        CommitOrRollback(r431, parser);
                        a408 = r431;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r432

                        bool r432 = true;
                        r432 = r432 && Match(parser, new Jhu.Graywulf.Sql.Parsing.InsertStatement());
                        CommitOrRollback(r432, parser);
                        a408 = r432;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r433

                        bool r433 = true;
                        r433 = r433 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateStatement());
                        CommitOrRollback(r433, parser);
                        a408 = r433;
                    }

                    if (!a408)
                    {
                        Checkpoint(parser); // r434

                        bool r434 = true;
                        r434 = r434 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeleteStatement());
                        CommitOrRollback(r434, parser);
                        a408 = r434;
                    }

                    r407 &= a408;

                } // end alternatives a408

                CommitOrRollback(r407, parser);
                res = r407;
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
                Checkpoint(parser); // r435

                bool r435 = true;
                r435 = r435 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                r435 = r435 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                CommitOrRollback(r435, parser);
                res = r435;
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
                Checkpoint(parser); // r436

                bool r436 = true;
                r436 = r436 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"GOTO"));
                r436 = r436 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r436 = r436 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r436, parser);
                res = r436;
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
                Checkpoint(parser); // r437

                bool r437 = true;
                r437 = r437 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r437 = r437 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                r437 = r437 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r437, parser);
                res = r437;
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
                Checkpoint(parser); // r438

                bool r438 = true;
                r438 = r438 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHILE"));
                if (r438)
                { // may a439
                    bool a439 = false;
                    {
                        Checkpoint(parser); // r440

                        bool r440 = true;
                        r440 = r440 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r440, parser);
                        a439 = r440;
                    }

                    r438 |= a439;
                } // end may a439

                r438 = r438 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r438)
                { // may a441
                    bool a441 = false;
                    {
                        Checkpoint(parser); // r442

                        bool r442 = true;
                        r442 = r442 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r442, parser);
                        a441 = r442;
                    }

                    r438 |= a441;
                } // end may a441

                r438 = r438 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                CommitOrRollback(r438, parser);
                res = r438;
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
                Checkpoint(parser); // r443

                bool r443 = true;
                r443 = r443 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BREAK"));
                CommitOrRollback(r443, parser);
                res = r443;
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
                Checkpoint(parser); // r444

                bool r444 = true;
                r444 = r444 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CONTINUE"));
                CommitOrRollback(r444, parser);
                res = r444;
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
                Checkpoint(parser); // r445

                bool r445 = true;
                r445 = r445 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RETURN"));
                CommitOrRollback(r445, parser);
                res = r445;
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
                Checkpoint(parser); // r446

                bool r446 = true;
                r446 = r446 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IF"));
                if (r446)
                { // may a447
                    bool a447 = false;
                    {
                        Checkpoint(parser); // r448

                        bool r448 = true;
                        r448 = r448 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r448, parser);
                        a447 = r448;
                    }

                    r446 |= a447;
                } // end may a447

                r446 = r446 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r446)
                { // may a449
                    bool a449 = false;
                    {
                        Checkpoint(parser); // r450

                        bool r450 = true;
                        r450 = r450 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r450, parser);
                        a449 = r450;
                    }

                    r446 |= a449;
                } // end may a449

                r446 = r446 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                if (r446)
                { // may a451
                    bool a451 = false;
                    {
                        Checkpoint(parser); // r452

                        bool r452 = true;
                        r452 = r452 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementSeparator());
                        r452 = r452 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ELSE"));
                        r452 = r452 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r452 = r452 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                        CommitOrRollback(r452, parser);
                        a451 = r452;
                    }

                    r446 |= a451;
                } // end may a451

                CommitOrRollback(r446, parser);
                res = r446;
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
                Checkpoint(parser); // r453

                bool r453 = true;
                r453 = r453 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THROW"));
                if (r453)
                { // may a454
                    bool a454 = false;
                    {
                        Checkpoint(parser); // r455

                        bool r455 = true;
                        r455 = r455 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r455)
                        { // alternatives a456 must
                            bool a456 = false;
                            if (!a456)
                            {
                                Checkpoint(parser); // r457

                                bool r457 = true;
                                r457 = r457 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                CommitOrRollback(r457, parser);
                                a456 = r457;
                            }

                            if (!a456)
                            {
                                Checkpoint(parser); // r458

                                bool r458 = true;
                                r458 = r458 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r458, parser);
                                a456 = r458;
                            }

                            r455 &= a456;

                        } // end alternatives a456

                        if (r455)
                        { // may a459
                            bool a459 = false;
                            {
                                Checkpoint(parser); // r460

                                bool r460 = true;
                                r460 = r460 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r460, parser);
                                a459 = r460;
                            }

                            r455 |= a459;
                        } // end may a459

                        r455 = r455 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r455)
                        { // may a461
                            bool a461 = false;
                            {
                                Checkpoint(parser); // r462

                                bool r462 = true;
                                r462 = r462 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r462, parser);
                                a461 = r462;
                            }

                            r455 |= a461;
                        } // end may a461

                        if (r455)
                        { // alternatives a463 must
                            bool a463 = false;
                            if (!a463)
                            {
                                Checkpoint(parser); // r464

                                bool r464 = true;
                                r464 = r464 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StringConstant());
                                CommitOrRollback(r464, parser);
                                a463 = r464;
                            }

                            if (!a463)
                            {
                                Checkpoint(parser); // r465

                                bool r465 = true;
                                r465 = r465 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r465, parser);
                                a463 = r465;
                            }

                            r455 &= a463;

                        } // end alternatives a463

                        if (r455)
                        { // may a466
                            bool a466 = false;
                            {
                                Checkpoint(parser); // r467

                                bool r467 = true;
                                r467 = r467 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r467, parser);
                                a466 = r467;
                            }

                            r455 |= a466;
                        } // end may a466

                        r455 = r455 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r455)
                        { // may a468
                            bool a468 = false;
                            {
                                Checkpoint(parser); // r469

                                bool r469 = true;
                                r469 = r469 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r469, parser);
                                a468 = r469;
                            }

                            r455 |= a468;
                        } // end may a468

                        if (r455)
                        { // alternatives a470 must
                            bool a470 = false;
                            if (!a470)
                            {
                                Checkpoint(parser); // r471

                                bool r471 = true;
                                r471 = r471 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                CommitOrRollback(r471, parser);
                                a470 = r471;
                            }

                            if (!a470)
                            {
                                Checkpoint(parser); // r472

                                bool r472 = true;
                                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r472, parser);
                                a470 = r472;
                            }

                            r455 &= a470;

                        } // end alternatives a470

                        if (r455)
                        { // may a473
                            bool a473 = false;
                            {
                                Checkpoint(parser); // r474

                                bool r474 = true;
                                r474 = r474 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r474, parser);
                                a473 = r474;
                            }

                            r455 |= a473;
                        } // end may a473

                        CommitOrRollback(r455, parser);
                        a454 = r455;
                    }

                    r453 |= a454;
                } // end may a454

                CommitOrRollback(r453, parser);
                res = r453;
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
                Checkpoint(parser); // r475

                bool r475 = true;
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRY"));
                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRY"));
                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CATCH"));
                if (r475)
                { // alternatives a476 must
                    bool a476 = false;
                    if (!a476)
                    {
                        Checkpoint(parser); // r477

                        bool r477 = true;
                        r477 = r477 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                        CommitOrRollback(r477, parser);
                        a476 = r477;
                    }

                    if (!a476)
                    {
                        Checkpoint(parser); // r478

                        bool r478 = true;
                        r478 = r478 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r478, parser);
                        a476 = r478;
                    }

                    r475 &= a476;

                } // end alternatives a476

                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r475 = r475 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CATCH"));
                CommitOrRollback(r475, parser);
                res = r475;
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
                Checkpoint(parser); // r479

                bool r479 = true;
                if (r479)
                { // may a480
                    bool a480 = false;
                    {
                        Checkpoint(parser); // r481

                        bool r481 = true;
                        r481 = r481 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r481, parser);
                        a480 = r481;
                    }

                    r479 |= a480;
                } // end may a480

                r479 = r479 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r479)
                { // may a482
                    bool a482 = false;
                    {
                        Checkpoint(parser); // r483

                        bool r483 = true;
                        if (r483)
                        { // may a484
                            bool a484 = false;
                            {
                                Checkpoint(parser); // r485

                                bool r485 = true;
                                r485 = r485 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r485, parser);
                                a484 = r485;
                            }

                            r483 |= a484;
                        } // end may a484

                        r483 = r483 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        r483 = r483 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableList());
                        CommitOrRollback(r483, parser);
                        a482 = r483;
                    }

                    r479 |= a482;
                } // end may a482

                CommitOrRollback(r479, parser);
                res = r479;
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
                Checkpoint(parser); // r486

                bool r486 = true;
                r486 = r486 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                if (r486)
                { // may a487
                    bool a487 = false;
                    {
                        Checkpoint(parser); // r488

                        bool r488 = true;
                        r488 = r488 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r488, parser);
                        a487 = r488;
                    }

                    r486 |= a487;
                } // end may a487

                r486 = r486 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList());
                CommitOrRollback(r486, parser);
                res = r486;
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
                Checkpoint(parser); // r489

                bool r489 = true;
                r489 = r489 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclaration());
                if (r489)
                { // may a490
                    bool a490 = false;
                    {
                        Checkpoint(parser); // r491

                        bool r491 = true;
                        if (r491)
                        { // may a492
                            bool a492 = false;
                            {
                                Checkpoint(parser); // r493

                                bool r493 = true;
                                r493 = r493 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r493, parser);
                                a492 = r493;
                            }

                            r491 |= a492;
                        } // end may a492

                        r491 = r491 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r491)
                        { // may a494
                            bool a494 = false;
                            {
                                Checkpoint(parser); // r495

                                bool r495 = true;
                                r495 = r495 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r495, parser);
                                a494 = r495;
                            }

                            r491 |= a494;
                        } // end may a494

                        r491 = r491 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList());
                        CommitOrRollback(r491, parser);
                        a490 = r491;
                    }

                    r489 |= a490;
                } // end may a490

                CommitOrRollback(r489, parser);
                res = r489;
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
                Checkpoint(parser); // r496

                bool r496 = true;
                r496 = r496 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r496)
                { // may a497
                    bool a497 = false;
                    {
                        Checkpoint(parser); // r498

                        bool r498 = true;
                        r498 = r498 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r498 = r498 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        CommitOrRollback(r498, parser);
                        a497 = r498;
                    }

                    r496 |= a497;
                } // end may a497

                r496 = r496 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r496)
                { // alternatives a499 must
                    bool a499 = false;
                    if (!a499)
                    {
                        Checkpoint(parser); // r500

                        bool r500 = true;
                        r500 = r500 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CURSOR"));
                        CommitOrRollback(r500, parser);
                        a499 = r500;
                    }

                    if (!a499)
                    {
                        Checkpoint(parser); // r501

                        bool r501 = true;
                        r501 = r501 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataType());
                        if (r501)
                        { // may a502
                            bool a502 = false;
                            {
                                Checkpoint(parser); // r503

                                bool r503 = true;
                                if (r503)
                                { // may a504
                                    bool a504 = false;
                                    {
                                        Checkpoint(parser); // r505

                                        bool r505 = true;
                                        r505 = r505 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r505, parser);
                                        a504 = r505;
                                    }

                                    r503 |= a504;
                                } // end may a504

                                r503 = r503 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                                if (r503)
                                { // may a506
                                    bool a506 = false;
                                    {
                                        Checkpoint(parser); // r507

                                        bool r507 = true;
                                        r507 = r507 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r507, parser);
                                        a506 = r507;
                                    }

                                    r503 |= a506;
                                } // end may a506

                                r503 = r503 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                                CommitOrRollback(r503, parser);
                                a502 = r503;
                            }

                            r501 |= a502;
                        } // end may a502

                        CommitOrRollback(r501, parser);
                        a499 = r501;
                    }

                    r496 &= a499;

                } // end alternatives a499

                CommitOrRollback(r496, parser);
                res = r496;
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
                Checkpoint(parser); // r508

                bool r508 = true;
                r508 = r508 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                r508 = r508 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r508 = r508 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r508)
                { // may a509
                    bool a509 = false;
                    {
                        Checkpoint(parser); // r510

                        bool r510 = true;
                        r510 = r510 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r510, parser);
                        a509 = r510;
                    }

                    r508 |= a509;
                } // end may a509

                r508 = r508 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator());
                if (r508)
                { // may a511
                    bool a511 = false;
                    {
                        Checkpoint(parser); // r512

                        bool r512 = true;
                        r512 = r512 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r512, parser);
                        a511 = r512;
                    }

                    r508 |= a511;
                } // end may a511

                r508 = r508 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r508, parser);
                res = r508;
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
                Checkpoint(parser); // r513

                bool r513 = true;
                if (r513)
                { // alternatives a514 must
                    bool a514 = false;
                    if (!a514)
                    {
                        Checkpoint(parser); // r515

                        bool r515 = true;
                        r515 = r515 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        CommitOrRollback(r515, parser);
                        a514 = r515;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r516

                        bool r516 = true;
                        r516 = r516 && Match(parser, new Jhu.Graywulf.Sql.Parsing.PlusEquals());
                        CommitOrRollback(r516, parser);
                        a514 = r516;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r517

                        bool r517 = true;
                        r517 = r517 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MinusEquals());
                        CommitOrRollback(r517, parser);
                        a514 = r517;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r518

                        bool r518 = true;
                        r518 = r518 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MulEquals());
                        CommitOrRollback(r518, parser);
                        a514 = r518;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r519

                        bool r519 = true;
                        r519 = r519 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DivEquals());
                        CommitOrRollback(r519, parser);
                        a514 = r519;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r520

                        bool r520 = true;
                        r520 = r520 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ModEquals());
                        CommitOrRollback(r520, parser);
                        a514 = r520;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r521

                        bool r521 = true;
                        r521 = r521 && Match(parser, new Jhu.Graywulf.Sql.Parsing.AndEquals());
                        CommitOrRollback(r521, parser);
                        a514 = r521;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r522

                        bool r522 = true;
                        r522 = r522 && Match(parser, new Jhu.Graywulf.Sql.Parsing.XorEquals());
                        CommitOrRollback(r522, parser);
                        a514 = r522;
                    }

                    if (!a514)
                    {
                        Checkpoint(parser); // r523

                        bool r523 = true;
                        r523 = r523 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrEquals());
                        CommitOrRollback(r523, parser);
                        a514 = r523;
                    }

                    r513 &= a514;

                } // end alternatives a514

                CommitOrRollback(r513, parser);
                res = r513;
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
                Checkpoint(parser); // r524

                bool r524 = true;
                r524 = r524 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r524 = r524 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r524)
                { // alternatives a525 must
                    bool a525 = false;
                    if (!a525)
                    {
                        Checkpoint(parser); // r526

                        bool r526 = true;
                        r526 = r526 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r526, parser);
                        a525 = r526;
                    }

                    if (!a525)
                    {
                        Checkpoint(parser); // r527

                        bool r527 = true;
                        r527 = r527 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r527, parser);
                        a525 = r527;
                    }

                    r524 &= a525;

                } // end alternatives a525

                r524 = r524 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r524 = r524 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorDefinition());
                CommitOrRollback(r524, parser);
                res = r524;
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
                Checkpoint(parser); // r528

                bool r528 = true;
                r528 = r528 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r528)
                { // may a529
                    bool a529 = false;
                    {
                        Checkpoint(parser); // r530

                        bool r530 = true;
                        r530 = r530 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r530, parser);
                        a529 = r530;
                    }

                    r528 |= a529;
                } // end may a529

                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                if (r528)
                { // may a531
                    bool a531 = false;
                    {
                        Checkpoint(parser); // r532

                        bool r532 = true;
                        r532 = r532 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r532, parser);
                        a531 = r532;
                    }

                    r528 |= a531;
                } // end may a531

                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorDefinition());
                CommitOrRollback(r528, parser);
                res = r528;
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
                Checkpoint(parser); // r533

                bool r533 = true;
                r533 = r533 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CURSOR"));
                r533 = r533 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r533 = r533 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FOR"));
                r533 = r533 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r533 = r533 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectStatement());
                CommitOrRollback(r533, parser);
                res = r533;
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
                Checkpoint(parser); // r534

                bool r534 = true;
                if (r534)
                { // alternatives a535 must
                    bool a535 = false;
                    if (!a535)
                    {
                        Checkpoint(parser); // r536

                        bool r536 = true;
                        r536 = r536 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OPEN"));
                        CommitOrRollback(r536, parser);
                        a535 = r536;
                    }

                    if (!a535)
                    {
                        Checkpoint(parser); // r537

                        bool r537 = true;
                        r537 = r537 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLOSE"));
                        CommitOrRollback(r537, parser);
                        a535 = r537;
                    }

                    if (!a535)
                    {
                        Checkpoint(parser); // r538

                        bool r538 = true;
                        r538 = r538 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEALLOCATE"));
                        CommitOrRollback(r538, parser);
                        a535 = r538;
                    }

                    r534 &= a535;

                } // end alternatives a535

                r534 = r534 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r534)
                { // alternatives a539 must
                    bool a539 = false;
                    if (!a539)
                    {
                        Checkpoint(parser); // r540

                        bool r540 = true;
                        r540 = r540 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r540, parser);
                        a539 = r540;
                    }

                    if (!a539)
                    {
                        Checkpoint(parser); // r541

                        bool r541 = true;
                        r541 = r541 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r541, parser);
                        a539 = r541;
                    }

                    r534 &= a539;

                } // end alternatives a539

                CommitOrRollback(r534, parser);
                res = r534;
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
                Checkpoint(parser); // r542

                bool r542 = true;
                r542 = r542 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FETCH"));
                r542 = r542 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r542)
                { // may a543
                    bool a543 = false;
                    {
                        Checkpoint(parser); // r544

                        bool r544 = true;
                        if (r544)
                        { // alternatives a545 must
                            bool a545 = false;
                            if (!a545)
                            {
                                Checkpoint(parser); // r546

                                bool r546 = true;
                                r546 = r546 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NEXT"));
                                CommitOrRollback(r546, parser);
                                a545 = r546;
                            }

                            if (!a545)
                            {
                                Checkpoint(parser); // r547

                                bool r547 = true;
                                r547 = r547 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PRIOR"));
                                CommitOrRollback(r547, parser);
                                a545 = r547;
                            }

                            if (!a545)
                            {
                                Checkpoint(parser); // r548

                                bool r548 = true;
                                r548 = r548 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FIRST"));
                                CommitOrRollback(r548, parser);
                                a545 = r548;
                            }

                            if (!a545)
                            {
                                Checkpoint(parser); // r549

                                bool r549 = true;
                                r549 = r549 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LAST"));
                                CommitOrRollback(r549, parser);
                                a545 = r549;
                            }

                            if (!a545)
                            {
                                Checkpoint(parser); // r550

                                bool r550 = true;
                                if (r550)
                                { // alternatives a551 must
                                    bool a551 = false;
                                    if (!a551)
                                    {
                                        Checkpoint(parser); // r552

                                        bool r552 = true;
                                        r552 = r552 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ABSOLUTE"));
                                        CommitOrRollback(r552, parser);
                                        a551 = r552;
                                    }

                                    if (!a551)
                                    {
                                        Checkpoint(parser); // r553

                                        bool r553 = true;
                                        r553 = r553 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RELATIVE"));
                                        CommitOrRollback(r553, parser);
                                        a551 = r553;
                                    }

                                    r550 &= a551;

                                } // end alternatives a551

                                r550 = r550 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                if (r550)
                                { // alternatives a554 must
                                    bool a554 = false;
                                    if (!a554)
                                    {
                                        Checkpoint(parser); // r555

                                        bool r555 = true;
                                        r555 = r555 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                        CommitOrRollback(r555, parser);
                                        a554 = r555;
                                    }

                                    if (!a554)
                                    {
                                        Checkpoint(parser); // r556

                                        bool r556 = true;
                                        r556 = r556 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                        CommitOrRollback(r556, parser);
                                        a554 = r556;
                                    }

                                    r550 &= a554;

                                } // end alternatives a554

                                CommitOrRollback(r550, parser);
                                a545 = r550;
                            }

                            r544 &= a545;

                        } // end alternatives a545

                        CommitOrRollback(r544, parser);
                        a543 = r544;
                    }

                    r542 |= a543;
                } // end may a543

                r542 = r542 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r542 = r542 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                r542 = r542 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r542)
                { // alternatives a557 must
                    bool a557 = false;
                    if (!a557)
                    {
                        Checkpoint(parser); // r558

                        bool r558 = true;
                        r558 = r558 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r558, parser);
                        a557 = r558;
                    }

                    if (!a557)
                    {
                        Checkpoint(parser); // r559

                        bool r559 = true;
                        r559 = r559 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r559, parser);
                        a557 = r559;
                    }

                    r542 &= a557;

                } // end alternatives a557

                if (r542)
                { // may a560
                    bool a560 = false;
                    {
                        Checkpoint(parser); // r561

                        bool r561 = true;
                        r561 = r561 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r561 = r561 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTO"));
                        r561 = r561 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r561 = r561 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableList());
                        CommitOrRollback(r561, parser);
                        a560 = r561;
                    }

                    r542 |= a560;
                } // end may a560

                CommitOrRollback(r542, parser);
                res = r542;
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
                Checkpoint(parser); // r562

                bool r562 = true;
                r562 = r562 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r562 = r562 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r562)
                { // may a563
                    bool a563 = false;
                    {
                        Checkpoint(parser); // r564

                        bool r564 = true;
                        r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r564, parser);
                        a563 = r564;
                    }

                    r562 |= a563;
                } // end may a563

                r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r562)
                { // may a565
                    bool a565 = false;
                    {
                        Checkpoint(parser); // r566

                        bool r566 = true;
                        r566 = r566 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r566, parser);
                        a565 = r566;
                    }

                    r562 |= a565;
                } // end may a565

                r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                if (r562)
                { // may a567
                    bool a567 = false;
                    {
                        Checkpoint(parser); // r568

                        bool r568 = true;
                        r568 = r568 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r568, parser);
                        a567 = r568;
                    }

                    r562 |= a567;
                } // end may a567

                r562 = r562 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r562, parser);
                res = r562;
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
                Checkpoint(parser); // r569

                bool r569 = true;
                r569 = r569 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                if (r569)
                { // may a570
                    bool a570 = false;
                    {
                        Checkpoint(parser); // r571

                        bool r571 = true;
                        r571 = r571 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r571, parser);
                        a570 = r571;
                    }

                    r569 |= a570;
                } // end may a570

                r569 = r569 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList());
                CommitOrRollback(r569, parser);
                res = r569;
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
                Checkpoint(parser); // r572

                bool r572 = true;
                r572 = r572 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecification());
                if (r572)
                { // may a573
                    bool a573 = false;
                    {
                        Checkpoint(parser); // r574

                        bool r574 = true;
                        if (r574)
                        { // may a575
                            bool a575 = false;
                            {
                                Checkpoint(parser); // r576

                                bool r576 = true;
                                r576 = r576 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r576, parser);
                                a575 = r576;
                            }

                            r574 |= a575;
                        } // end may a575

                        r574 = r574 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r574)
                        { // may a577
                            bool a577 = false;
                            {
                                Checkpoint(parser); // r578

                                bool r578 = true;
                                r578 = r578 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r578, parser);
                                a577 = r578;
                            }

                            r574 |= a577;
                        } // end may a577

                        r574 = r574 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList());
                        CommitOrRollback(r574, parser);
                        a573 = r574;
                    }

                    r572 |= a573;
                } // end may a573

                CommitOrRollback(r572, parser);
                res = r572;
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
                Checkpoint(parser); // r579

                bool r579 = true;
                r579 = r579 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                if (r579)
                { // may a580
                    bool a580 = false;
                    {
                        Checkpoint(parser); // r581

                        bool r581 = true;
                        if (r581)
                        { // may a582
                            bool a582 = false;
                            {
                                Checkpoint(parser); // r583

                                bool r583 = true;
                                r583 = r583 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r583, parser);
                                a582 = r583;
                            }

                            r581 |= a582;
                        } // end may a582

                        r581 = r581 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets());
                        CommitOrRollback(r581, parser);
                        a580 = r581;
                    }

                    r579 |= a580;
                } // end may a580

                if (r579)
                { // may a584
                    bool a584 = false;
                    {
                        Checkpoint(parser); // r585

                        bool r585 = true;
                        r585 = r585 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r585, parser);
                        a584 = r585;
                    }

                    r579 |= a584;
                } // end may a584

                r579 = r579 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                if (r579)
                { // may a586
                    bool a586 = false;
                    {
                        Checkpoint(parser); // r587

                        bool r587 = true;
                        r587 = r587 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r587, parser);
                        a586 = r587;
                    }

                    r579 |= a586;
                } // end may a586

                r579 = r579 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                CommitOrRollback(r579, parser);
                res = r579;
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
                Checkpoint(parser); // r588

                bool r588 = true;
                if (r588)
                { // may a589
                    bool a589 = false;
                    {
                        Checkpoint(parser); // r590

                        bool r590 = true;
                        r590 = r590 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r590)
                        { // may a591
                            bool a591 = false;
                            {
                                Checkpoint(parser); // r592

                                bool r592 = true;
                                r592 = r592 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r592, parser);
                                a591 = r592;
                            }

                            r590 |= a591;
                        } // end may a591

                        CommitOrRollback(r590, parser);
                        a589 = r590;
                    }

                    r588 |= a589;
                } // end may a589

                r588 = r588 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r588)
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
                                r596 = r596 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r596, parser);
                                a595 = r596;
                            }

                            r594 |= a595;
                        } // end may a595

                        r594 = r594 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        CommitOrRollback(r594, parser);
                        a593 = r594;
                    }

                    r588 |= a593;
                } // end may a593

                if (r588)
                { // may a597
                    bool a597 = false;
                    {
                        Checkpoint(parser); // r598

                        bool r598 = true;
                        if (r598)
                        { // may a599
                            bool a599 = false;
                            {
                                Checkpoint(parser); // r600

                                bool r600 = true;
                                r600 = r600 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r600, parser);
                                a599 = r600;
                            }

                            r598 |= a599;
                        } // end may a599

                        r598 = r598 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r598, parser);
                        a597 = r598;
                    }

                    r588 |= a597;
                } // end may a597

                CommitOrRollback(r588, parser);
                res = r588;
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
                Checkpoint(parser); // r601

                bool r601 = true;
                r601 = r601 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r601)
                { // may a602
                    bool a602 = false;
                    {
                        Checkpoint(parser); // r603

                        bool r603 = true;
                        r603 = r603 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r603, parser);
                        a602 = r603;
                    }

                    r601 |= a602;
                } // end may a602

                r601 = r601 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r601)
                { // may a604
                    bool a604 = false;
                    {
                        Checkpoint(parser); // r605

                        bool r605 = true;
                        if (r605)
                        { // may a606
                            bool a606 = false;
                            {
                                Checkpoint(parser); // r607

                                bool r607 = true;
                                r607 = r607 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r607, parser);
                                a606 = r607;
                            }

                            r605 |= a606;
                        } // end may a606

                        r605 = r605 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        CommitOrRollback(r605, parser);
                        a604 = r605;
                    }

                    r601 |= a604;
                } // end may a604

                if (r601)
                { // may a608
                    bool a608 = false;
                    {
                        Checkpoint(parser); // r609

                        bool r609 = true;
                        r609 = r609 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r609, parser);
                        a608 = r609;
                    }

                    r601 |= a608;
                } // end may a608

                r601 = r601 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r601, parser);
                res = r601;
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
                Checkpoint(parser); // r610

                bool r610 = true;
                if (r610)
                { // alternatives a611 must
                    bool a611 = false;
                    if (!a611)
                    {
                        Checkpoint(parser); // r612

                        bool r612 = true;
                        r612 = r612 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpressionBrackets());
                        CommitOrRollback(r612, parser);
                        a611 = r612;
                    }

                    if (!a611)
                    {
                        Checkpoint(parser); // r613

                        bool r613 = true;
                        r613 = r613 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QuerySpecification());
                        CommitOrRollback(r613, parser);
                        a611 = r613;
                    }

                    r610 &= a611;

                } // end alternatives a611

                if (r610)
                { // may a614
                    bool a614 = false;
                    {
                        Checkpoint(parser); // r615

                        bool r615 = true;
                        if (r615)
                        { // may a616
                            bool a616 = false;
                            {
                                Checkpoint(parser); // r617

                                bool r617 = true;
                                r617 = r617 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r617, parser);
                                a616 = r617;
                            }

                            r615 |= a616;
                        } // end may a616

                        r615 = r615 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryOperator());
                        if (r615)
                        { // may a618
                            bool a618 = false;
                            {
                                Checkpoint(parser); // r619

                                bool r619 = true;
                                r619 = r619 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r619, parser);
                                a618 = r619;
                            }

                            r615 |= a618;
                        } // end may a618

                        r615 = r615 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                        CommitOrRollback(r615, parser);
                        a614 = r615;
                    }

                    r610 |= a614;
                } // end may a614

                CommitOrRollback(r610, parser);
                res = r610;
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
                Checkpoint(parser); // r620

                bool r620 = true;
                r620 = r620 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r620)
                { // may a621
                    bool a621 = false;
                    {
                        Checkpoint(parser); // r622

                        bool r622 = true;
                        r622 = r622 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r622, parser);
                        a621 = r622;
                    }

                    r620 |= a621;
                } // end may a621

                r620 = r620 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r620)
                { // may a623
                    bool a623 = false;
                    {
                        Checkpoint(parser); // r624

                        bool r624 = true;
                        r624 = r624 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r624, parser);
                        a623 = r624;
                    }

                    r620 |= a623;
                } // end may a623

                r620 = r620 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r620, parser);
                res = r620;
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
                Checkpoint(parser); // r625

                bool r625 = true;
                if (r625)
                { // alternatives a626 must
                    bool a626 = false;
                    if (!a626)
                    {
                        Checkpoint(parser); // r627

                        bool r627 = true;
                        r627 = r627 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNION"));
                        if (r627)
                        { // may a628
                            bool a628 = false;
                            {
                                Checkpoint(parser); // r629

                                bool r629 = true;
                                r629 = r629 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r629 = r629 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r629, parser);
                                a628 = r629;
                            }

                            r627 |= a628;
                        } // end may a628

                        CommitOrRollback(r627, parser);
                        a626 = r627;
                    }

                    if (!a626)
                    {
                        Checkpoint(parser); // r630

                        bool r630 = true;
                        r630 = r630 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"EXCEPT"));
                        CommitOrRollback(r630, parser);
                        a626 = r630;
                    }

                    if (!a626)
                    {
                        Checkpoint(parser); // r631

                        bool r631 = true;
                        r631 = r631 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTERSECT"));
                        CommitOrRollback(r631, parser);
                        a626 = r631;
                    }

                    r625 &= a626;

                } // end alternatives a626

                CommitOrRollback(r625, parser);
                res = r625;
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
                Checkpoint(parser); // r632

                bool r632 = true;
                r632 = r632 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SELECT"));
                if (r632)
                { // may a633
                    bool a633 = false;
                    {
                        Checkpoint(parser); // r634

                        bool r634 = true;
                        r634 = r634 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r634)
                        { // alternatives a635 must
                            bool a635 = false;
                            if (!a635)
                            {
                                Checkpoint(parser); // r636

                                bool r636 = true;
                                r636 = r636 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r636, parser);
                                a635 = r636;
                            }

                            if (!a635)
                            {
                                Checkpoint(parser); // r637

                                bool r637 = true;
                                r637 = r637 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DISTINCT"));
                                CommitOrRollback(r637, parser);
                                a635 = r637;
                            }

                            r634 &= a635;

                        } // end alternatives a635

                        CommitOrRollback(r634, parser);
                        a633 = r634;
                    }

                    r632 |= a633;
                } // end may a633

                if (r632)
                { // may a638
                    bool a638 = false;
                    {
                        Checkpoint(parser); // r639

                        bool r639 = true;
                        r639 = r639 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r639 = r639 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TopExpression());
                        CommitOrRollback(r639, parser);
                        a638 = r639;
                    }

                    r632 |= a638;
                } // end may a638

                if (r632)
                { // may a640
                    bool a640 = false;
                    {
                        Checkpoint(parser); // r641

                        bool r641 = true;
                        r641 = r641 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r641, parser);
                        a640 = r641;
                    }

                    r632 |= a640;
                } // end may a640

                r632 = r632 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectList());
                if (r632)
                { // may a642
                    bool a642 = false;
                    {
                        Checkpoint(parser); // r643

                        bool r643 = true;
                        if (r643)
                        { // may a644
                            bool a644 = false;
                            {
                                Checkpoint(parser); // r645

                                bool r645 = true;
                                r645 = r645 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r645, parser);
                                a644 = r645;
                            }

                            r643 |= a644;
                        } // end may a644

                        r643 = r643 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IntoClause());
                        CommitOrRollback(r643, parser);
                        a642 = r643;
                    }

                    r632 |= a642;
                } // end may a642

                if (r632)
                { // may a646
                    bool a646 = false;
                    {
                        Checkpoint(parser); // r647

                        bool r647 = true;
                        if (r647)
                        { // may a648
                            bool a648 = false;
                            {
                                Checkpoint(parser); // r649

                                bool r649 = true;
                                r649 = r649 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r649, parser);
                                a648 = r649;
                            }

                            r647 |= a648;
                        } // end may a648

                        r647 = r647 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r647, parser);
                        a646 = r647;
                    }

                    r632 |= a646;
                } // end may a646

                if (r632)
                { // may a650
                    bool a650 = false;
                    {
                        Checkpoint(parser); // r651

                        bool r651 = true;
                        if (r651)
                        { // may a652
                            bool a652 = false;
                            {
                                Checkpoint(parser); // r653

                                bool r653 = true;
                                r653 = r653 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r653, parser);
                                a652 = r653;
                            }

                            r651 |= a652;
                        } // end may a652

                        r651 = r651 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r651, parser);
                        a650 = r651;
                    }

                    r632 |= a650;
                } // end may a650

                if (r632)
                { // may a654
                    bool a654 = false;
                    {
                        Checkpoint(parser); // r655

                        bool r655 = true;
                        if (r655)
                        { // may a656
                            bool a656 = false;
                            {
                                Checkpoint(parser); // r657

                                bool r657 = true;
                                r657 = r657 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r657, parser);
                                a656 = r657;
                            }

                            r655 |= a656;
                        } // end may a656

                        r655 = r655 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByClause());
                        CommitOrRollback(r655, parser);
                        a654 = r655;
                    }

                    r632 |= a654;
                } // end may a654

                if (r632)
                { // may a658
                    bool a658 = false;
                    {
                        Checkpoint(parser); // r659

                        bool r659 = true;
                        if (r659)
                        { // may a660
                            bool a660 = false;
                            {
                                Checkpoint(parser); // r661

                                bool r661 = true;
                                r661 = r661 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r661, parser);
                                a660 = r661;
                            }

                            r659 |= a660;
                        } // end may a660

                        r659 = r659 && Match(parser, new Jhu.Graywulf.Sql.Parsing.HavingClause());
                        CommitOrRollback(r659, parser);
                        a658 = r659;
                    }

                    r632 |= a658;
                } // end may a658

                CommitOrRollback(r632, parser);
                res = r632;
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
                Checkpoint(parser); // r662

                bool r662 = true;
                r662 = r662 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TOP"));
                r662 = r662 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r662 = r662 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r662)
                { // may a663
                    bool a663 = false;
                    {
                        Checkpoint(parser); // r664

                        bool r664 = true;
                        r664 = r664 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r664 = r664 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PERCENT"));
                        CommitOrRollback(r664, parser);
                        a663 = r664;
                    }

                    r662 |= a663;
                } // end may a663

                if (r662)
                { // may a665
                    bool a665 = false;
                    {
                        Checkpoint(parser); // r666

                        bool r666 = true;
                        r666 = r666 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r666 = r666 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                        r666 = r666 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r666 = r666 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TIES"));
                        CommitOrRollback(r666, parser);
                        a665 = r666;
                    }

                    r662 |= a665;
                } // end may a665

                CommitOrRollback(r662, parser);
                res = r662;
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
                Checkpoint(parser); // r667

                bool r667 = true;
                r667 = r667 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnExpression());
                if (r667)
                { // may a668
                    bool a668 = false;
                    {
                        Checkpoint(parser); // r669

                        bool r669 = true;
                        if (r669)
                        { // may a670
                            bool a670 = false;
                            {
                                Checkpoint(parser); // r671

                                bool r671 = true;
                                r671 = r671 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r671, parser);
                                a670 = r671;
                            }

                            r669 |= a670;
                        } // end may a670

                        r669 = r669 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r669)
                        { // may a672
                            bool a672 = false;
                            {
                                Checkpoint(parser); // r673

                                bool r673 = true;
                                r673 = r673 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r673, parser);
                                a672 = r673;
                            }

                            r669 |= a672;
                        } // end may a672

                        r669 = r669 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectList());
                        CommitOrRollback(r669, parser);
                        a668 = r669;
                    }

                    r667 |= a668;
                } // end may a668

                CommitOrRollback(r667, parser);
                res = r667;
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
                Checkpoint(parser); // r674

                bool r674 = true;
                if (r674)
                { // alternatives a675 must
                    bool a675 = false;
                    if (!a675)
                    {
                        Checkpoint(parser); // r676

                        bool r676 = true;
                        r676 = r676 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        if (r676)
                        { // may a677
                            bool a677 = false;
                            {
                                Checkpoint(parser); // r678

                                bool r678 = true;
                                r678 = r678 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r678, parser);
                                a677 = r678;
                            }

                            r676 |= a677;
                        } // end may a677

                        r676 = r676 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r676)
                        { // may a679
                            bool a679 = false;
                            {
                                Checkpoint(parser); // r680

                                bool r680 = true;
                                r680 = r680 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r680, parser);
                                a679 = r680;
                            }

                            r676 |= a679;
                        } // end may a679

                        r676 = r676 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r676, parser);
                        a675 = r676;
                    }

                    if (!a675)
                    {
                        Checkpoint(parser); // r681

                        bool r681 = true;
                        r681 = r681 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                        if (r681)
                        { // may a682
                            bool a682 = false;
                            {
                                Checkpoint(parser); // r683

                                bool r683 = true;
                                r683 = r683 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r683, parser);
                                a682 = r683;
                            }

                            r681 |= a682;
                        } // end may a682

                        r681 = r681 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r681)
                        { // may a684
                            bool a684 = false;
                            {
                                Checkpoint(parser); // r685

                                bool r685 = true;
                                r685 = r685 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r685, parser);
                                a684 = r685;
                            }

                            r681 |= a684;
                        } // end may a684

                        r681 = r681 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r681, parser);
                        a675 = r681;
                    }

                    if (!a675)
                    {
                        Checkpoint(parser); // r686

                        bool r686 = true;
                        r686 = r686 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r686)
                        { // may a687
                            bool a687 = false;
                            {
                                Checkpoint(parser); // r688

                                bool r688 = true;
                                if (r688)
                                { // may a689
                                    bool a689 = false;
                                    {
                                        Checkpoint(parser); // r690

                                        bool r690 = true;
                                        r690 = r690 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        r690 = r690 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                        CommitOrRollback(r690, parser);
                                        a689 = r690;
                                    }

                                    r688 |= a689;
                                } // end may a689

                                r688 = r688 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r688 = r688 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                                CommitOrRollback(r688, parser);
                                a687 = r688;
                            }

                            r686 |= a687;
                        } // end may a687

                        CommitOrRollback(r686, parser);
                        a675 = r686;
                    }

                    r674 &= a675;

                } // end alternatives a675

                CommitOrRollback(r674, parser);
                res = r674;
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
                Checkpoint(parser); // r691

                bool r691 = true;
                r691 = r691 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTO"));
                if (r691)
                { // may a692
                    bool a692 = false;
                    {
                        Checkpoint(parser); // r693

                        bool r693 = true;
                        r693 = r693 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r693, parser);
                        a692 = r693;
                    }

                    r691 |= a692;
                } // end may a692

                r691 = r691 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                CommitOrRollback(r691, parser);
                res = r691;
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
                Checkpoint(parser); // r694

                bool r694 = true;
                if (r694)
                { // alternatives a695 must
                    bool a695 = false;
                    if (!a695)
                    {
                        Checkpoint(parser); // r696

                        bool r696 = true;
                        r696 = r696 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        CommitOrRollback(r696, parser);
                        a695 = r696;
                    }

                    if (!a695)
                    {
                        Checkpoint(parser); // r697

                        bool r697 = true;
                        r697 = r697 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                        CommitOrRollback(r697, parser);
                        a695 = r697;
                    }

                    r694 &= a695;

                } // end alternatives a695

                if (r694)
                { // may a698
                    bool a698 = false;
                    {
                        Checkpoint(parser); // r699

                        bool r699 = true;
                        if (r699)
                        { // may a700
                            bool a700 = false;
                            {
                                Checkpoint(parser); // r701

                                bool r701 = true;
                                r701 = r701 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r701, parser);
                                a700 = r701;
                            }

                            r699 |= a700;
                        } // end may a700

                        r699 = r699 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintClause());
                        CommitOrRollback(r699, parser);
                        a698 = r699;
                    }

                    r694 |= a698;
                } // end may a698

                CommitOrRollback(r694, parser);
                res = r694;
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
                Checkpoint(parser); // r702

                bool r702 = true;
                r702 = r702 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                if (r702)
                { // may a703
                    bool a703 = false;
                    {
                        Checkpoint(parser); // r704

                        bool r704 = true;
                        r704 = r704 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r704, parser);
                        a703 = r704;
                    }

                    r702 |= a703;
                } // end may a703

                r702 = r702 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSourceExpression());
                CommitOrRollback(r702, parser);
                res = r702;
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
                Checkpoint(parser); // r705

                bool r705 = true;
                r705 = r705 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                if (r705)
                { // may a706
                    bool a706 = false;
                    {
                        Checkpoint(parser); // r707

                        bool r707 = true;
                        if (r707)
                        { // may a708
                            bool a708 = false;
                            {
                                Checkpoint(parser); // r709

                                bool r709 = true;
                                r709 = r709 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r709, parser);
                                a708 = r709;
                            }

                            r707 |= a708;
                        } // end may a708

                        r707 = r707 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinedTable());
                        CommitOrRollback(r707, parser);
                        a706 = r707;
                    }

                    r705 |= a706;
                } // end may a706

                CommitOrRollback(r705, parser);
                res = r705;
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
                Checkpoint(parser); // r710

                bool r710 = true;
                if (r710)
                { // alternatives a711 must
                    bool a711 = false;
                    if (!a711)
                    {
                        Checkpoint(parser); // r712

                        bool r712 = true;
                        r712 = r712 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinType());
                        if (r712)
                        { // may a713
                            bool a713 = false;
                            {
                                Checkpoint(parser); // r714

                                bool r714 = true;
                                r714 = r714 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r714, parser);
                                a713 = r714;
                            }

                            r712 |= a713;
                        } // end may a713

                        r712 = r712 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        if (r712)
                        { // may a715
                            bool a715 = false;
                            {
                                Checkpoint(parser); // r716

                                bool r716 = true;
                                r716 = r716 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r716, parser);
                                a715 = r716;
                            }

                            r712 |= a715;
                        } // end may a715

                        r712 = r712 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                        if (r712)
                        { // may a717
                            bool a717 = false;
                            {
                                Checkpoint(parser); // r718

                                bool r718 = true;
                                r718 = r718 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r718, parser);
                                a717 = r718;
                            }

                            r712 |= a717;
                        } // end may a717

                        r712 = r712 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                        CommitOrRollback(r712, parser);
                        a711 = r712;
                    }

                    if (!a711)
                    {
                        Checkpoint(parser); // r719

                        bool r719 = true;
                        r719 = r719 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CROSS"));
                        r719 = r719 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r719 = r719 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"JOIN"));
                        if (r719)
                        { // may a720
                            bool a720 = false;
                            {
                                Checkpoint(parser); // r721

                                bool r721 = true;
                                r721 = r721 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r721, parser);
                                a720 = r721;
                            }

                            r719 |= a720;
                        } // end may a720

                        r719 = r719 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r719, parser);
                        a711 = r719;
                    }

                    if (!a711)
                    {
                        Checkpoint(parser); // r722

                        bool r722 = true;
                        r722 = r722 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r722)
                        { // may a723
                            bool a723 = false;
                            {
                                Checkpoint(parser); // r724

                                bool r724 = true;
                                r724 = r724 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r724, parser);
                                a723 = r724;
                            }

                            r722 |= a723;
                        } // end may a723

                        r722 = r722 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r722, parser);
                        a711 = r722;
                    }

                    if (!a711)
                    {
                        Checkpoint(parser); // r725

                        bool r725 = true;
                        if (r725)
                        { // alternatives a726 must
                            bool a726 = false;
                            if (!a726)
                            {
                                Checkpoint(parser); // r727

                                bool r727 = true;
                                r727 = r727 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CROSS"));
                                CommitOrRollback(r727, parser);
                                a726 = r727;
                            }

                            if (!a726)
                            {
                                Checkpoint(parser); // r728

                                bool r728 = true;
                                r728 = r728 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OUTER"));
                                CommitOrRollback(r728, parser);
                                a726 = r728;
                            }

                            r725 &= a726;

                        } // end alternatives a726

                        r725 = r725 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r725 = r725 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"APPLY"));
                        if (r725)
                        { // may a729
                            bool a729 = false;
                            {
                                Checkpoint(parser); // r730

                                bool r730 = true;
                                r730 = r730 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r730, parser);
                                a729 = r730;
                            }

                            r725 |= a729;
                        } // end may a729

                        r725 = r725 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r725, parser);
                        a711 = r725;
                    }

                    r710 &= a711;

                } // end alternatives a711

                if (r710)
                { // may a731
                    bool a731 = false;
                    {
                        Checkpoint(parser); // r732

                        bool r732 = true;
                        if (r732)
                        { // may a733
                            bool a733 = false;
                            {
                                Checkpoint(parser); // r734

                                bool r734 = true;
                                r734 = r734 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r734, parser);
                                a733 = r734;
                            }

                            r732 |= a733;
                        } // end may a733

                        r732 = r732 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinedTable());
                        CommitOrRollback(r732, parser);
                        a731 = r732;
                    }

                    r710 |= a731;
                } // end may a731

                CommitOrRollback(r710, parser);
                res = r710;
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
                Checkpoint(parser); // r735

                bool r735 = true;
                if (r735)
                { // may a736
                    bool a736 = false;
                    {
                        Checkpoint(parser); // r737

                        bool r737 = true;
                        if (r737)
                        { // alternatives a738 must
                            bool a738 = false;
                            if (!a738)
                            {
                                Checkpoint(parser); // r739

                                bool r739 = true;
                                r739 = r739 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INNER"));
                                CommitOrRollback(r739, parser);
                                a738 = r739;
                            }

                            if (!a738)
                            {
                                Checkpoint(parser); // r740

                                bool r740 = true;
                                if (r740)
                                { // alternatives a741 must
                                    bool a741 = false;
                                    if (!a741)
                                    {
                                        Checkpoint(parser); // r742

                                        bool r742 = true;
                                        r742 = r742 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LEFT"));
                                        CommitOrRollback(r742, parser);
                                        a741 = r742;
                                    }

                                    if (!a741)
                                    {
                                        Checkpoint(parser); // r743

                                        bool r743 = true;
                                        r743 = r743 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RIGHT"));
                                        CommitOrRollback(r743, parser);
                                        a741 = r743;
                                    }

                                    if (!a741)
                                    {
                                        Checkpoint(parser); // r744

                                        bool r744 = true;
                                        r744 = r744 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FULL"));
                                        CommitOrRollback(r744, parser);
                                        a741 = r744;
                                    }

                                    r740 &= a741;

                                } // end alternatives a741

                                if (r740)
                                { // may a745
                                    bool a745 = false;
                                    {
                                        Checkpoint(parser); // r746

                                        bool r746 = true;
                                        r746 = r746 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        r746 = r746 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OUTER"));
                                        CommitOrRollback(r746, parser);
                                        a745 = r746;
                                    }

                                    r740 |= a745;
                                } // end may a745

                                CommitOrRollback(r740, parser);
                                a738 = r740;
                            }

                            r737 &= a738;

                        } // end alternatives a738

                        if (r737)
                        { // may a747
                            bool a747 = false;
                            {
                                Checkpoint(parser); // r748

                                bool r748 = true;
                                r748 = r748 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r748, parser);
                                a747 = r748;
                            }

                            r737 |= a747;
                        } // end may a747

                        if (r737)
                        { // may a749
                            bool a749 = false;
                            {
                                Checkpoint(parser); // r750

                                bool r750 = true;
                                r750 = r750 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinHint());
                                CommitOrRollback(r750, parser);
                                a749 = r750;
                            }

                            r737 |= a749;
                        } // end may a749

                        CommitOrRollback(r737, parser);
                        a736 = r737;
                    }

                    r735 |= a736;
                } // end may a736

                if (r735)
                { // may a751
                    bool a751 = false;
                    {
                        Checkpoint(parser); // r752

                        bool r752 = true;
                        r752 = r752 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r752, parser);
                        a751 = r752;
                    }

                    r735 |= a751;
                } // end may a751

                r735 = r735 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"JOIN"));
                CommitOrRollback(r735, parser);
                res = r735;
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
                Checkpoint(parser); // r753

                bool r753 = true;
                if (r753)
                { // alternatives a754 must
                    bool a754 = false;
                    if (!a754)
                    {
                        Checkpoint(parser); // r755

                        bool r755 = true;
                        r755 = r755 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LOOP"));
                        CommitOrRollback(r755, parser);
                        a754 = r755;
                    }

                    if (!a754)
                    {
                        Checkpoint(parser); // r756

                        bool r756 = true;
                        r756 = r756 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"HASH"));
                        CommitOrRollback(r756, parser);
                        a754 = r756;
                    }

                    if (!a754)
                    {
                        Checkpoint(parser); // r757

                        bool r757 = true;
                        r757 = r757 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"MERGE"));
                        CommitOrRollback(r757, parser);
                        a754 = r757;
                    }

                    if (!a754)
                    {
                        Checkpoint(parser); // r758

                        bool r758 = true;
                        r758 = r758 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"REMOTE"));
                        CommitOrRollback(r758, parser);
                        a754 = r758;
                    }

                    r753 &= a754;

                } // end alternatives a754

                CommitOrRollback(r753, parser);
                res = r753;
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
                Checkpoint(parser); // r759

                bool r759 = true;
                if (r759)
                { // alternatives a760 must
                    bool a760 = false;
                    if (!a760)
                    {
                        Checkpoint(parser); // r761

                        bool r761 = true;
                        r761 = r761 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionTableSource());
                        CommitOrRollback(r761, parser);
                        a760 = r761;
                    }

                    if (!a760)
                    {
                        Checkpoint(parser); // r762

                        bool r762 = true;
                        r762 = r762 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleTableSource());
                        CommitOrRollback(r762, parser);
                        a760 = r762;
                    }

                    if (!a760)
                    {
                        Checkpoint(parser); // r763

                        bool r763 = true;
                        r763 = r763 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableTableSource());
                        CommitOrRollback(r763, parser);
                        a760 = r763;
                    }

                    if (!a760)
                    {
                        Checkpoint(parser); // r764

                        bool r764 = true;
                        r764 = r764 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SubqueryTableSource());
                        CommitOrRollback(r764, parser);
                        a760 = r764;
                    }

                    r759 &= a760;

                } // end alternatives a760

                CommitOrRollback(r759, parser);
                res = r759;
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
                Checkpoint(parser); // r765

                bool r765 = true;
                r765 = r765 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r765)
                { // may a766
                    bool a766 = false;
                    {
                        Checkpoint(parser); // r767

                        bool r767 = true;
                        r767 = r767 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r767)
                        { // may a768
                            bool a768 = false;
                            {
                                Checkpoint(parser); // r769

                                bool r769 = true;
                                r769 = r769 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                r769 = r769 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r769, parser);
                                a768 = r769;
                            }

                            r767 |= a768;
                        } // end may a768

                        r767 = r767 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                        CommitOrRollback(r767, parser);
                        a766 = r767;
                    }

                    r765 |= a766;
                } // end may a766

                if (r765)
                { // may a770
                    bool a770 = false;
                    {
                        Checkpoint(parser); // r771

                        bool r771 = true;
                        r771 = r771 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r771 = r771 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSampleClause());
                        CommitOrRollback(r771, parser);
                        a770 = r771;
                    }

                    r765 |= a770;
                } // end may a770

                if (r765)
                { // may a772
                    bool a772 = false;
                    {
                        Checkpoint(parser); // r773

                        bool r773 = true;
                        r773 = r773 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r773 = r773 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintClause());
                        CommitOrRollback(r773, parser);
                        a772 = r773;
                    }

                    r765 |= a772;
                } // end may a772

                if (r765)
                { // may a774
                    bool a774 = false;
                    {
                        Checkpoint(parser); // r775

                        bool r775 = true;
                        r775 = r775 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r775 = r775 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TablePartitionClause());
                        CommitOrRollback(r775, parser);
                        a774 = r775;
                    }

                    r765 |= a774;
                } // end may a774

                CommitOrRollback(r765, parser);
                res = r765;
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
                Checkpoint(parser); // r776

                bool r776 = true;
                r776 = r776 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableValuedFunctionCall());
                if (r776)
                { // may a777
                    bool a777 = false;
                    {
                        Checkpoint(parser); // r778

                        bool r778 = true;
                        r778 = r778 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r778, parser);
                        a777 = r778;
                    }

                    r776 |= a777;
                } // end may a777

                if (r776)
                { // may a779
                    bool a779 = false;
                    {
                        Checkpoint(parser); // r780

                        bool r780 = true;
                        r780 = r780 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        r780 = r780 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r780, parser);
                        a779 = r780;
                    }

                    r776 |= a779;
                } // end may a779

                r776 = r776 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                if (r776)
                { // may a781
                    bool a781 = false;
                    {
                        Checkpoint(parser); // r782

                        bool r782 = true;
                        if (r782)
                        { // may a783
                            bool a783 = false;
                            {
                                Checkpoint(parser); // r784

                                bool r784 = true;
                                r784 = r784 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r784, parser);
                                a783 = r784;
                            }

                            r782 |= a783;
                        } // end may a783

                        r782 = r782 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r782)
                        { // may a785
                            bool a785 = false;
                            {
                                Checkpoint(parser); // r786

                                bool r786 = true;
                                r786 = r786 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r786, parser);
                                a785 = r786;
                            }

                            r782 |= a785;
                        } // end may a785

                        r782 = r782 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAliasList());
                        if (r782)
                        { // may a787
                            bool a787 = false;
                            {
                                Checkpoint(parser); // r788

                                bool r788 = true;
                                r788 = r788 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r788, parser);
                                a787 = r788;
                            }

                            r782 |= a787;
                        } // end may a787

                        r782 = r782 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r782, parser);
                        a781 = r782;
                    }

                    r776 |= a781;
                } // end may a781

                CommitOrRollback(r776, parser);
                res = r776;
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
                Checkpoint(parser); // r789

                bool r789 = true;
                r789 = r789 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                if (r789)
                { // may a790
                    bool a790 = false;
                    {
                        Checkpoint(parser); // r791

                        bool r791 = true;
                        if (r791)
                        { // may a792
                            bool a792 = false;
                            {
                                Checkpoint(parser); // r793

                                bool r793 = true;
                                r793 = r793 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r793, parser);
                                a792 = r793;
                            }

                            r791 |= a792;
                        } // end may a792

                        if (r791)
                        { // may a794
                            bool a794 = false;
                            {
                                Checkpoint(parser); // r795

                                bool r795 = true;
                                r795 = r795 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                if (r795)
                                { // may a796
                                    bool a796 = false;
                                    {
                                        Checkpoint(parser); // r797

                                        bool r797 = true;
                                        r797 = r797 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r797, parser);
                                        a796 = r797;
                                    }

                                    r795 |= a796;
                                } // end may a796

                                CommitOrRollback(r795, parser);
                                a794 = r795;
                            }

                            r791 |= a794;
                        } // end may a794

                        r791 = r791 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                        CommitOrRollback(r791, parser);
                        a790 = r791;
                    }

                    r789 |= a790;
                } // end may a790

                CommitOrRollback(r789, parser);
                res = r789;
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
                Checkpoint(parser); // r798

                bool r798 = true;
                r798 = r798 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                if (r798)
                { // may a799
                    bool a799 = false;
                    {
                        Checkpoint(parser); // r800

                        bool r800 = true;
                        r800 = r800 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r800, parser);
                        a799 = r800;
                    }

                    r798 |= a799;
                } // end may a799

                if (r798)
                { // may a801
                    bool a801 = false;
                    {
                        Checkpoint(parser); // r802

                        bool r802 = true;
                        r802 = r802 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        r802 = r802 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r802, parser);
                        a801 = r802;
                    }

                    r798 |= a801;
                } // end may a801

                r798 = r798 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                CommitOrRollback(r798, parser);
                res = r798;
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
                Checkpoint(parser); // r803

                bool r803 = true;
                r803 = r803 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                if (r803)
                { // may a804
                    bool a804 = false;
                    {
                        Checkpoint(parser); // r805

                        bool r805 = true;
                        if (r805)
                        { // may a806
                            bool a806 = false;
                            {
                                Checkpoint(parser); // r807

                                bool r807 = true;
                                r807 = r807 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r807, parser);
                                a806 = r807;
                            }

                            r805 |= a806;
                        } // end may a806

                        r805 = r805 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        r805 = r805 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAliasList());
                        CommitOrRollback(r805, parser);
                        a804 = r805;
                    }

                    r803 |= a804;
                } // end may a804

                CommitOrRollback(r803, parser);
                res = r803;
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
                Checkpoint(parser); // r808

                bool r808 = true;
                r808 = r808 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLESAMPLE"));
                if (r808)
                { // may a809
                    bool a809 = false;
                    {
                        Checkpoint(parser); // r810

                        bool r810 = true;
                        r810 = r810 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r810 = r810 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SYSTEM"));
                        CommitOrRollback(r810, parser);
                        a809 = r810;
                    }

                    r808 |= a809;
                } // end may a809

                if (r808)
                { // may a811
                    bool a811 = false;
                    {
                        Checkpoint(parser); // r812

                        bool r812 = true;
                        r812 = r812 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r812, parser);
                        a811 = r812;
                    }

                    r808 |= a811;
                } // end may a811

                r808 = r808 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r808)
                { // may a813
                    bool a813 = false;
                    {
                        Checkpoint(parser); // r814

                        bool r814 = true;
                        r814 = r814 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r814, parser);
                        a813 = r814;
                    }

                    r808 |= a813;
                } // end may a813

                r808 = r808 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SampleNumber());
                if (r808)
                { // may a815
                    bool a815 = false;
                    {
                        Checkpoint(parser); // r816

                        bool r816 = true;
                        r816 = r816 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r816)
                        { // may a817
                            bool a817 = false;
                            {
                                Checkpoint(parser); // r818

                                bool r818 = true;
                                if (r818)
                                { // alternatives a819 must
                                    bool a819 = false;
                                    if (!a819)
                                    {
                                        Checkpoint(parser); // r820

                                        bool r820 = true;
                                        r820 = r820 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PERCENT"));
                                        CommitOrRollback(r820, parser);
                                        a819 = r820;
                                    }

                                    if (!a819)
                                    {
                                        Checkpoint(parser); // r821

                                        bool r821 = true;
                                        r821 = r821 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ROWS"));
                                        CommitOrRollback(r821, parser);
                                        a819 = r821;
                                    }

                                    r818 &= a819;

                                } // end alternatives a819

                                CommitOrRollback(r818, parser);
                                a817 = r818;
                            }

                            r816 |= a817;
                        } // end may a817

                        CommitOrRollback(r816, parser);
                        a815 = r816;
                    }

                    r808 |= a815;
                } // end may a815

                if (r808)
                { // may a822
                    bool a822 = false;
                    {
                        Checkpoint(parser); // r823

                        bool r823 = true;
                        r823 = r823 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r823, parser);
                        a822 = r823;
                    }

                    r808 |= a822;
                } // end may a822

                r808 = r808 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                if (r808)
                { // may a824
                    bool a824 = false;
                    {
                        Checkpoint(parser); // r825

                        bool r825 = true;
                        r825 = r825 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r825 = r825 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"REPEATABLE"));
                        if (r825)
                        { // may a826
                            bool a826 = false;
                            {
                                Checkpoint(parser); // r827

                                bool r827 = true;
                                r827 = r827 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r827, parser);
                                a826 = r827;
                            }

                            r825 |= a826;
                        } // end may a826

                        r825 = r825 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r825)
                        { // may a828
                            bool a828 = false;
                            {
                                Checkpoint(parser); // r829

                                bool r829 = true;
                                r829 = r829 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r829, parser);
                                a828 = r829;
                            }

                            r825 |= a828;
                        } // end may a828

                        r825 = r825 && Match(parser, new Jhu.Graywulf.Sql.Parsing.RepeatSeed());
                        if (r825)
                        { // may a830
                            bool a830 = false;
                            {
                                Checkpoint(parser); // r831

                                bool r831 = true;
                                r831 = r831 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r831, parser);
                                a830 = r831;
                            }

                            r825 |= a830;
                        } // end may a830

                        r825 = r825 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r825, parser);
                        a824 = r825;
                    }

                    r808 |= a824;
                } // end may a824

                CommitOrRollback(r808, parser);
                res = r808;
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
                Checkpoint(parser); // r832

                bool r832 = true;
                r832 = r832 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PARTITION"));
                r832 = r832 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r832 = r832 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                r832 = r832 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r832 = r832 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentifier());
                CommitOrRollback(r832, parser);
                res = r832;
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
                Checkpoint(parser); // r833

                bool r833 = true;
                r833 = r833 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHERE"));
                if (r833)
                { // may a834
                    bool a834 = false;
                    {
                        Checkpoint(parser); // r835

                        bool r835 = true;
                        r835 = r835 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r835, parser);
                        a834 = r835;
                    }

                    r833 |= a834;
                } // end may a834

                r833 = r833 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                CommitOrRollback(r833, parser);
                res = r833;
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
                Checkpoint(parser); // r836

                bool r836 = true;
                r836 = r836 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"GROUP"));
                r836 = r836 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r836 = r836 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                if (r836)
                { // alternatives a837 must
                    bool a837 = false;
                    if (!a837)
                    {
                        Checkpoint(parser); // r838

                        bool r838 = true;
                        r838 = r838 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r838 = r838 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                        CommitOrRollback(r838, parser);
                        a837 = r838;
                    }

                    if (!a837)
                    {
                        Checkpoint(parser); // r839

                        bool r839 = true;
                        if (r839)
                        { // may a840
                            bool a840 = false;
                            {
                                Checkpoint(parser); // r841

                                bool r841 = true;
                                r841 = r841 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r841, parser);
                                a840 = r841;
                            }

                            r839 |= a840;
                        } // end may a840

                        r839 = r839 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByList());
                        CommitOrRollback(r839, parser);
                        a837 = r839;
                    }

                    r836 &= a837;

                } // end alternatives a837

                CommitOrRollback(r836, parser);
                res = r836;
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
                Checkpoint(parser); // r842

                bool r842 = true;
                r842 = r842 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r842)
                { // may a843
                    bool a843 = false;
                    {
                        Checkpoint(parser); // r844

                        bool r844 = true;
                        if (r844)
                        { // may a845
                            bool a845 = false;
                            {
                                Checkpoint(parser); // r846

                                bool r846 = true;
                                r846 = r846 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r846, parser);
                                a845 = r846;
                            }

                            r844 |= a845;
                        } // end may a845

                        r844 = r844 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r844)
                        { // may a847
                            bool a847 = false;
                            {
                                Checkpoint(parser); // r848

                                bool r848 = true;
                                r848 = r848 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r848, parser);
                                a847 = r848;
                            }

                            r844 |= a847;
                        } // end may a847

                        r844 = r844 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByList());
                        CommitOrRollback(r844, parser);
                        a843 = r844;
                    }

                    r842 |= a843;
                } // end may a843

                CommitOrRollback(r842, parser);
                res = r842;
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
                Checkpoint(parser); // r849

                bool r849 = true;
                r849 = r849 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"HAVING"));
                if (r849)
                { // may a850
                    bool a850 = false;
                    {
                        Checkpoint(parser); // r851

                        bool r851 = true;
                        r851 = r851 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r851, parser);
                        a850 = r851;
                    }

                    r849 |= a850;
                } // end may a850

                r849 = r849 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                CommitOrRollback(r849, parser);
                res = r849;
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
                Checkpoint(parser); // r852

                bool r852 = true;
                r852 = r852 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ORDER"));
                r852 = r852 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r852 = r852 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
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

                r852 = r852 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByList());
                CommitOrRollback(r852, parser);
                res = r852;
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
                Checkpoint(parser); // r855

                bool r855 = true;
                r855 = r855 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByArgument());
                if (r855)
                { // may a856
                    bool a856 = false;
                    {
                        Checkpoint(parser); // r857

                        bool r857 = true;
                        if (r857)
                        { // may a858
                            bool a858 = false;
                            {
                                Checkpoint(parser); // r859

                                bool r859 = true;
                                r859 = r859 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r859, parser);
                                a858 = r859;
                            }

                            r857 |= a858;
                        } // end may a858

                        r857 = r857 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r857)
                        { // may a860
                            bool a860 = false;
                            {
                                Checkpoint(parser); // r861

                                bool r861 = true;
                                r861 = r861 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r861, parser);
                                a860 = r861;
                            }

                            r857 |= a860;
                        } // end may a860

                        r857 = r857 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByList());
                        CommitOrRollback(r857, parser);
                        a856 = r857;
                    }

                    r855 |= a856;
                } // end may a856

                CommitOrRollback(r855, parser);
                res = r855;
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
                Checkpoint(parser); // r862

                bool r862 = true;
                r862 = r862 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r862)
                { // may a863
                    bool a863 = false;
                    {
                        Checkpoint(parser); // r864

                        bool r864 = true;
                        if (r864)
                        { // may a865
                            bool a865 = false;
                            {
                                Checkpoint(parser); // r866

                                bool r866 = true;
                                r866 = r866 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r866, parser);
                                a865 = r866;
                            }

                            r864 |= a865;
                        } // end may a865

                        if (r864)
                        { // alternatives a867 must
                            bool a867 = false;
                            if (!a867)
                            {
                                Checkpoint(parser); // r868

                                bool r868 = true;
                                r868 = r868 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ASC"));
                                CommitOrRollback(r868, parser);
                                a867 = r868;
                            }

                            if (!a867)
                            {
                                Checkpoint(parser); // r869

                                bool r869 = true;
                                r869 = r869 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DESC"));
                                CommitOrRollback(r869, parser);
                                a867 = r869;
                            }

                            r864 &= a867;

                        } // end alternatives a867

                        CommitOrRollback(r864, parser);
                        a863 = r864;
                    }

                    r862 |= a863;
                } // end may a863

                CommitOrRollback(r862, parser);
                res = r862;
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
                Checkpoint(parser); // r870

                bool r870 = true;
                r870 = r870 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                if (r870)
                { // may a871
                    bool a871 = false;
                    {
                        Checkpoint(parser); // r872

                        bool r872 = true;
                        r872 = r872 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r872, parser);
                        a871 = r872;
                    }

                    r870 |= a871;
                } // end may a871

                r870 = r870 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r870)
                { // may a873
                    bool a873 = false;
                    {
                        Checkpoint(parser); // r874

                        bool r874 = true;
                        r874 = r874 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r874, parser);
                        a873 = r874;
                    }

                    r870 |= a873;
                } // end may a873

                r870 = r870 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintList());
                if (r870)
                { // may a875
                    bool a875 = false;
                    {
                        Checkpoint(parser); // r876

                        bool r876 = true;
                        r876 = r876 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r876, parser);
                        a875 = r876;
                    }

                    r870 |= a875;
                } // end may a875

                r870 = r870 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r870, parser);
                res = r870;
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
                Checkpoint(parser); // r877

                bool r877 = true;
                r877 = r877 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHint());
                if (r877)
                { // may a878
                    bool a878 = false;
                    {
                        Checkpoint(parser); // r879

                        bool r879 = true;
                        if (r879)
                        { // may a880
                            bool a880 = false;
                            {
                                Checkpoint(parser); // r881

                                bool r881 = true;
                                r881 = r881 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r881, parser);
                                a880 = r881;
                            }

                            r879 |= a880;
                        } // end may a880

                        r879 = r879 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r879)
                        { // may a882
                            bool a882 = false;
                            {
                                Checkpoint(parser); // r883

                                bool r883 = true;
                                r883 = r883 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r883, parser);
                                a882 = r883;
                            }

                            r879 |= a882;
                        } // end may a882

                        r879 = r879 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintList());
                        CommitOrRollback(r879, parser);
                        a878 = r879;
                    }

                    r877 |= a878;
                } // end may a878

                CommitOrRollback(r877, parser);
                res = r877;
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
                Checkpoint(parser); // r884

                bool r884 = true;
                if (r884)
                { // alternatives a885 must
                    bool a885 = false;
                    if (!a885)
                    {
                        Checkpoint(parser); // r886

                        bool r886 = true;
                        r886 = r886 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r886)
                        { // may a887
                            bool a887 = false;
                            {
                                Checkpoint(parser); // r888

                                bool r888 = true;
                                r888 = r888 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r888, parser);
                                a887 = r888;
                            }

                            r886 |= a887;
                        } // end may a887

                        r886 = r886 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r886, parser);
                        a885 = r886;
                    }

                    if (!a885)
                    {
                        Checkpoint(parser); // r889

                        bool r889 = true;
                        r889 = r889 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r889, parser);
                        a885 = r889;
                    }

                    r884 &= a885;

                } // end alternatives a885

                CommitOrRollback(r884, parser);
                res = r884;
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
                Checkpoint(parser); // r890

                bool r890 = true;
                r890 = r890 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OPTION"));
                if (r890)
                { // may a891
                    bool a891 = false;
                    {
                        Checkpoint(parser); // r892

                        bool r892 = true;
                        r892 = r892 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r892, parser);
                        a891 = r892;
                    }

                    r890 |= a891;
                } // end may a891

                r890 = r890 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r890)
                { // may a893
                    bool a893 = false;
                    {
                        Checkpoint(parser); // r894

                        bool r894 = true;
                        r894 = r894 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r894, parser);
                        a893 = r894;
                    }

                    r890 |= a893;
                } // end may a893

                r890 = r890 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintList());
                if (r890)
                { // may a895
                    bool a895 = false;
                    {
                        Checkpoint(parser); // r896

                        bool r896 = true;
                        r896 = r896 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r896, parser);
                        a895 = r896;
                    }

                    r890 |= a895;
                } // end may a895

                r890 = r890 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r890, parser);
                res = r890;
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
                Checkpoint(parser); // r897

                bool r897 = true;
                r897 = r897 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHint());
                if (r897)
                { // may a898
                    bool a898 = false;
                    {
                        Checkpoint(parser); // r899

                        bool r899 = true;
                        if (r899)
                        { // may a900
                            bool a900 = false;
                            {
                                Checkpoint(parser); // r901

                                bool r901 = true;
                                r901 = r901 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r901, parser);
                                a900 = r901;
                            }

                            r899 |= a900;
                        } // end may a900

                        r899 = r899 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r899)
                        { // may a902
                            bool a902 = false;
                            {
                                Checkpoint(parser); // r903

                                bool r903 = true;
                                r903 = r903 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r903, parser);
                                a902 = r903;
                            }

                            r899 |= a902;
                        } // end may a902

                        r899 = r899 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintList());
                        CommitOrRollback(r899, parser);
                        a898 = r899;
                    }

                    r897 |= a898;
                } // end may a898

                CommitOrRollback(r897, parser);
                res = r897;
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
                Checkpoint(parser); // r904

                bool r904 = true;
                if (r904)
                { // alternatives a905 must
                    bool a905 = false;
                    if (!a905)
                    {
                        Checkpoint(parser); // r906

                        bool r906 = true;
                        r906 = r906 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r906)
                        { // may a907
                            bool a907 = false;
                            {
                                Checkpoint(parser); // r908

                                bool r908 = true;
                                r908 = r908 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r908, parser);
                                a907 = r908;
                            }

                            r906 |= a907;
                        } // end may a907

                        r906 = r906 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r906, parser);
                        a905 = r906;
                    }

                    if (!a905)
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

                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r909)
                        { // may a912
                            bool a912 = false;
                            {
                                Checkpoint(parser); // r913

                                bool r913 = true;
                                r913 = r913 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r913, parser);
                                a912 = r913;
                            }

                            r909 |= a912;
                        } // end may a912

                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r909, parser);
                        a905 = r909;
                    }

                    if (!a905)
                    {
                        Checkpoint(parser); // r914

                        bool r914 = true;
                        r914 = r914 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        r914 = r914 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r914 = r914 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r914, parser);
                        a905 = r914;
                    }

                    if (!a905)
                    {
                        Checkpoint(parser); // r915

                        bool r915 = true;
                        r915 = r915 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintIdentifierList());
                        CommitOrRollback(r915, parser);
                        a905 = r915;
                    }

                    if (!a905)
                    {
                        Checkpoint(parser); // r916

                        bool r916 = true;
                        r916 = r916 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r916, parser);
                        a905 = r916;
                    }

                    r904 &= a905;

                } // end alternatives a905

                CommitOrRollback(r904, parser);
                res = r904;
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
                Checkpoint(parser); // r917

                bool r917 = true;
                r917 = r917 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                if (r917)
                { // may a918
                    bool a918 = false;
                    {
                        Checkpoint(parser); // r919

                        bool r919 = true;
                        r919 = r919 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r919 = r919 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r919, parser);
                        a918 = r919;
                    }

                    r917 |= a918;
                } // end may a918

                CommitOrRollback(r917, parser);
                res = r917;
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
                Checkpoint(parser); // r920

                bool r920 = true;
                if (r920)
                { // may a921
                    bool a921 = false;
                    {
                        Checkpoint(parser); // r922

                        bool r922 = true;
                        r922 = r922 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r922)
                        { // may a923
                            bool a923 = false;
                            {
                                Checkpoint(parser); // r924

                                bool r924 = true;
                                r924 = r924 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r924, parser);
                                a923 = r924;
                            }

                            r922 |= a923;
                        } // end may a923

                        CommitOrRollback(r922, parser);
                        a921 = r922;
                    }

                    r920 |= a921;
                } // end may a921

                r920 = r920 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INSERT"));
                if (r920)
                { // may a925
                    bool a925 = false;
                    {
                        Checkpoint(parser); // r926

                        bool r926 = true;
                        r926 = r926 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r926, parser);
                        a925 = r926;
                    }

                    r920 |= a925;
                } // end may a925

                if (r920)
                { // alternatives a927 must
                    bool a927 = false;
                    if (!a927)
                    {
                        Checkpoint(parser); // r928

                        bool r928 = true;
                        r928 = r928 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IntoClause());
                        CommitOrRollback(r928, parser);
                        a927 = r928;
                    }

                    if (!a927)
                    {
                        Checkpoint(parser); // r929

                        bool r929 = true;
                        r929 = r929 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                        CommitOrRollback(r929, parser);
                        a927 = r929;
                    }

                    r920 &= a927;

                } // end alternatives a927

                if (r920)
                { // may a930
                    bool a930 = false;
                    {
                        Checkpoint(parser); // r931

                        bool r931 = true;
                        if (r931)
                        { // may a932
                            bool a932 = false;
                            {
                                Checkpoint(parser); // r933

                                bool r933 = true;
                                r933 = r933 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r933, parser);
                                a932 = r933;
                            }

                            r931 |= a932;
                        } // end may a932

                        r931 = r931 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets());
                        CommitOrRollback(r931, parser);
                        a930 = r931;
                    }

                    r920 |= a930;
                } // end may a930

                if (r920)
                { // may a934
                    bool a934 = false;
                    {
                        Checkpoint(parser); // r935

                        bool r935 = true;
                        r935 = r935 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r935, parser);
                        a934 = r935;
                    }

                    r920 |= a934;
                } // end may a934

                if (r920)
                { // alternatives a936 must
                    bool a936 = false;
                    if (!a936)
                    {
                        Checkpoint(parser); // r937

                        bool r937 = true;
                        r937 = r937 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesClause());
                        CommitOrRollback(r937, parser);
                        a936 = r937;
                    }

                    if (!a936)
                    {
                        Checkpoint(parser); // r938

                        bool r938 = true;
                        r938 = r938 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        r938 = r938 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r938 = r938 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"VALUES"));
                        CommitOrRollback(r938, parser);
                        a936 = r938;
                    }

                    if (!a936)
                    {
                        Checkpoint(parser); // r939

                        bool r939 = true;
                        r939 = r939 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                        if (r939)
                        { // may a940
                            bool a940 = false;
                            {
                                Checkpoint(parser); // r941

                                bool r941 = true;
                                if (r941)
                                { // may a942
                                    bool a942 = false;
                                    {
                                        Checkpoint(parser); // r943

                                        bool r943 = true;
                                        r943 = r943 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r943, parser);
                                        a942 = r943;
                                    }

                                    r941 |= a942;
                                } // end may a942

                                r941 = r941 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                                CommitOrRollback(r941, parser);
                                a940 = r941;
                            }

                            r939 |= a940;
                        } // end may a940

                        CommitOrRollback(r939, parser);
                        a936 = r939;
                    }

                    r920 &= a936;

                } // end alternatives a936

                if (r920)
                { // may a944
                    bool a944 = false;
                    {
                        Checkpoint(parser); // r945

                        bool r945 = true;
                        if (r945)
                        { // may a946
                            bool a946 = false;
                            {
                                Checkpoint(parser); // r947

                                bool r947 = true;
                                r947 = r947 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r947, parser);
                                a946 = r947;
                            }

                            r945 |= a946;
                        } // end may a946

                        r945 = r945 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r945, parser);
                        a944 = r945;
                    }

                    r920 |= a944;
                } // end may a944

                CommitOrRollback(r920, parser);
                res = r920;
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
                Checkpoint(parser); // r948

                bool r948 = true;
                r948 = r948 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
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

                r948 = r948 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnList());
                if (r948)
                { // may a951
                    bool a951 = false;
                    {
                        Checkpoint(parser); // r952

                        bool r952 = true;
                        r952 = r952 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r952, parser);
                        a951 = r952;
                    }

                    r948 |= a951;
                } // end may a951

                r948 = r948 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r948, parser);
                res = r948;
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
                Checkpoint(parser); // r953

                bool r953 = true;
                r953 = r953 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r953)
                { // may a954
                    bool a954 = false;
                    {
                        Checkpoint(parser); // r955

                        bool r955 = true;
                        if (r955)
                        { // may a956
                            bool a956 = false;
                            {
                                Checkpoint(parser); // r957

                                bool r957 = true;
                                r957 = r957 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r957, parser);
                                a956 = r957;
                            }

                            r955 |= a956;
                        } // end may a956

                        r955 = r955 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r955)
                        { // may a958
                            bool a958 = false;
                            {
                                Checkpoint(parser); // r959

                                bool r959 = true;
                                r959 = r959 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r959, parser);
                                a958 = r959;
                            }

                            r955 |= a958;
                        } // end may a958

                        r955 = r955 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnList());
                        CommitOrRollback(r955, parser);
                        a954 = r955;
                    }

                    r953 |= a954;
                } // end may a954

                CommitOrRollback(r953, parser);
                res = r953;
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
                Checkpoint(parser); // r960

                bool r960 = true;
                r960 = r960 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"VALUES"));
                if (r960)
                { // may a961
                    bool a961 = false;
                    {
                        Checkpoint(parser); // r962

                        bool r962 = true;
                        r962 = r962 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r962, parser);
                        a961 = r962;
                    }

                    r960 |= a961;
                } // end may a961

                r960 = r960 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroupList());
                if (r960)
                { // may a963
                    bool a963 = false;
                    {
                        Checkpoint(parser); // r964

                        bool r964 = true;
                        r964 = r964 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r964, parser);
                        a963 = r964;
                    }

                    r960 |= a963;
                } // end may a963

                CommitOrRollback(r960, parser);
                res = r960;
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
                Checkpoint(parser); // r965

                bool r965 = true;
                r965 = r965 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroup());
                if (r965)
                { // may a966
                    bool a966 = false;
                    {
                        Checkpoint(parser); // r967

                        bool r967 = true;
                        if (r967)
                        { // may a968
                            bool a968 = false;
                            {
                                Checkpoint(parser); // r969

                                bool r969 = true;
                                r969 = r969 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r969, parser);
                                a968 = r969;
                            }

                            r967 |= a968;
                        } // end may a968

                        r967 = r967 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r967)
                        { // may a970
                            bool a970 = false;
                            {
                                Checkpoint(parser); // r971

                                bool r971 = true;
                                r971 = r971 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r971, parser);
                                a970 = r971;
                            }

                            r967 |= a970;
                        } // end may a970

                        r967 = r967 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroupList());
                        CommitOrRollback(r967, parser);
                        a966 = r967;
                    }

                    r965 |= a966;
                } // end may a966

                CommitOrRollback(r965, parser);
                res = r965;
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
                Checkpoint(parser); // r972

                bool r972 = true;
                r972 = r972 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r972)
                { // may a973
                    bool a973 = false;
                    {
                        Checkpoint(parser); // r974

                        bool r974 = true;
                        r974 = r974 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r974, parser);
                        a973 = r974;
                    }

                    r972 |= a973;
                } // end may a973

                r972 = r972 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesList());
                if (r972)
                { // may a975
                    bool a975 = false;
                    {
                        Checkpoint(parser); // r976

                        bool r976 = true;
                        r976 = r976 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r976, parser);
                        a975 = r976;
                    }

                    r972 |= a975;
                } // end may a975

                r972 = r972 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r972, parser);
                res = r972;
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
                Checkpoint(parser); // r977

                bool r977 = true;
                if (r977)
                { // alternatives a978 must
                    bool a978 = false;
                    if (!a978)
                    {
                        Checkpoint(parser); // r979

                        bool r979 = true;
                        r979 = r979 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        CommitOrRollback(r979, parser);
                        a978 = r979;
                    }

                    if (!a978)
                    {
                        Checkpoint(parser); // r980

                        bool r980 = true;
                        r980 = r980 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r980, parser);
                        a978 = r980;
                    }

                    r977 &= a978;

                } // end alternatives a978

                if (r977)
                { // may a981
                    bool a981 = false;
                    {
                        Checkpoint(parser); // r982

                        bool r982 = true;
                        if (r982)
                        { // may a983
                            bool a983 = false;
                            {
                                Checkpoint(parser); // r984

                                bool r984 = true;
                                r984 = r984 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r984, parser);
                                a983 = r984;
                            }

                            r982 |= a983;
                        } // end may a983

                        r982 = r982 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r982)
                        { // may a985
                            bool a985 = false;
                            {
                                Checkpoint(parser); // r986

                                bool r986 = true;
                                r986 = r986 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r986, parser);
                                a985 = r986;
                            }

                            r982 |= a985;
                        } // end may a985

                        r982 = r982 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesList());
                        CommitOrRollback(r982, parser);
                        a981 = r982;
                    }

                    r977 |= a981;
                } // end may a981

                CommitOrRollback(r977, parser);
                res = r977;
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
                Checkpoint(parser); // r987

                bool r987 = true;
                if (r987)
                { // may a988
                    bool a988 = false;
                    {
                        Checkpoint(parser); // r989

                        bool r989 = true;
                        r989 = r989 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r989)
                        { // may a990
                            bool a990 = false;
                            {
                                Checkpoint(parser); // r991

                                bool r991 = true;
                                r991 = r991 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r991, parser);
                                a990 = r991;
                            }

                            r989 |= a990;
                        } // end may a990

                        CommitOrRollback(r989, parser);
                        a988 = r989;
                    }

                    r987 |= a988;
                } // end may a988

                r987 = r987 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UPDATE"));
                if (r987)
                { // may a992
                    bool a992 = false;
                    {
                        Checkpoint(parser); // r993

                        bool r993 = true;
                        r993 = r993 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r993, parser);
                        a992 = r993;
                    }

                    r987 |= a992;
                } // end may a992

                r987 = r987 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                if (r987)
                { // may a994
                    bool a994 = false;
                    {
                        Checkpoint(parser); // r995

                        bool r995 = true;
                        r995 = r995 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r995, parser);
                        a994 = r995;
                    }

                    r987 |= a994;
                } // end may a994

                r987 = r987 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                if (r987)
                { // may a996
                    bool a996 = false;
                    {
                        Checkpoint(parser); // r997

                        bool r997 = true;
                        r997 = r997 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r997, parser);
                        a996 = r997;
                    }

                    r987 |= a996;
                } // end may a996

                r987 = r987 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetList());
                if (r987)
                { // may a998
                    bool a998 = false;
                    {
                        Checkpoint(parser); // r999

                        bool r999 = true;
                        if (r999)
                        { // may a1000
                            bool a1000 = false;
                            {
                                Checkpoint(parser); // r1001

                                bool r1001 = true;
                                r1001 = r1001 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1001, parser);
                                a1000 = r1001;
                            }

                            r999 |= a1000;
                        } // end may a1000

                        r999 = r999 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r999, parser);
                        a998 = r999;
                    }

                    r987 |= a998;
                } // end may a998

                if (r987)
                { // may a1002
                    bool a1002 = false;
                    {
                        Checkpoint(parser); // r1003

                        bool r1003 = true;
                        if (r1003)
                        { // may a1004
                            bool a1004 = false;
                            {
                                Checkpoint(parser); // r1005

                                bool r1005 = true;
                                r1005 = r1005 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1005, parser);
                                a1004 = r1005;
                            }

                            r1003 |= a1004;
                        } // end may a1004

                        r1003 = r1003 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r1003, parser);
                        a1002 = r1003;
                    }

                    r987 |= a1002;
                } // end may a1002

                if (r987)
                { // may a1006
                    bool a1006 = false;
                    {
                        Checkpoint(parser); // r1007

                        bool r1007 = true;
                        if (r1007)
                        { // may a1008
                            bool a1008 = false;
                            {
                                Checkpoint(parser); // r1009

                                bool r1009 = true;
                                r1009 = r1009 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1009, parser);
                                a1008 = r1009;
                            }

                            r1007 |= a1008;
                        } // end may a1008

                        r1007 = r1007 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r1007, parser);
                        a1006 = r1007;
                    }

                    r987 |= a1006;
                } // end may a1006

                CommitOrRollback(r987, parser);
                res = r987;
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
                Checkpoint(parser); // r1010

                bool r1010 = true;
                r1010 = r1010 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetColumn());
                if (r1010)
                { // may a1011
                    bool a1011 = false;
                    {
                        Checkpoint(parser); // r1012

                        bool r1012 = true;
                        if (r1012)
                        { // may a1013
                            bool a1013 = false;
                            {
                                Checkpoint(parser); // r1014

                                bool r1014 = true;
                                r1014 = r1014 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1014, parser);
                                a1013 = r1014;
                            }

                            r1012 |= a1013;
                        } // end may a1013

                        r1012 = r1012 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1012)
                        { // may a1015
                            bool a1015 = false;
                            {
                                Checkpoint(parser); // r1016

                                bool r1016 = true;
                                r1016 = r1016 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1016, parser);
                                a1015 = r1016;
                            }

                            r1012 |= a1015;
                        } // end may a1015

                        r1012 = r1012 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetList());
                        CommitOrRollback(r1012, parser);
                        a1011 = r1012;
                    }

                    r1010 |= a1011;
                } // end may a1011

                CommitOrRollback(r1010, parser);
                res = r1010;
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
                Checkpoint(parser); // r1017

                bool r1017 = true;
                if (r1017)
                { // alternatives a1018 must
                    bool a1018 = false;
                    if (!a1018)
                    {
                        Checkpoint(parser); // r1019

                        bool r1019 = true;
                        r1019 = r1019 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                        if (r1019)
                        { // may a1020
                            bool a1020 = false;
                            {
                                Checkpoint(parser); // r1021

                                bool r1021 = true;
                                r1021 = r1021 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1021, parser);
                                a1020 = r1021;
                            }

                            r1019 |= a1020;
                        } // end may a1020

                        r1019 = r1019 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r1019)
                        { // may a1022
                            bool a1022 = false;
                            {
                                Checkpoint(parser); // r1023

                                bool r1023 = true;
                                r1023 = r1023 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1023, parser);
                                a1022 = r1023;
                            }

                            r1019 |= a1022;
                        } // end may a1022

                        r1019 = r1019 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                        CommitOrRollback(r1019, parser);
                        a1018 = r1019;
                    }

                    if (!a1018)
                    {
                        Checkpoint(parser); // r1024

                        bool r1024 = true;
                        if (r1024)
                        { // alternatives a1025 must
                            bool a1025 = false;
                            if (!a1025)
                            {
                                Checkpoint(parser); // r1026

                                bool r1026 = true;
                                r1026 = r1026 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UserVariable());
                                CommitOrRollback(r1026, parser);
                                a1025 = r1026;
                            }

                            if (!a1025)
                            {
                                Checkpoint(parser); // r1027

                                bool r1027 = true;
                                r1027 = r1027 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                CommitOrRollback(r1027, parser);
                                a1025 = r1027;
                            }

                            r1024 &= a1025;

                        } // end alternatives a1025

                        CommitOrRollback(r1024, parser);
                        a1018 = r1024;
                    }

                    r1017 &= a1018;

                } // end alternatives a1018

                if (r1017)
                { // may a1028
                    bool a1028 = false;
                    {
                        Checkpoint(parser); // r1029

                        bool r1029 = true;
                        r1029 = r1029 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1029, parser);
                        a1028 = r1029;
                    }

                    r1017 |= a1028;
                } // end may a1028

                r1017 = r1017 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator());
                if (r1017)
                { // may a1030
                    bool a1030 = false;
                    {
                        Checkpoint(parser); // r1031

                        bool r1031 = true;
                        r1031 = r1031 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1031, parser);
                        a1030 = r1031;
                    }

                    r1017 |= a1030;
                } // end may a1030

                if (r1017)
                { // alternatives a1032 must
                    bool a1032 = false;
                    if (!a1032)
                    {
                        Checkpoint(parser); // r1033

                        bool r1033 = true;
                        r1033 = r1033 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        CommitOrRollback(r1033, parser);
                        a1032 = r1033;
                    }

                    if (!a1032)
                    {
                        Checkpoint(parser); // r1034

                        bool r1034 = true;
                        r1034 = r1034 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r1034, parser);
                        a1032 = r1034;
                    }

                    r1017 &= a1032;

                } // end alternatives a1032

                CommitOrRollback(r1017, parser);
                res = r1017;
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
                Checkpoint(parser); // r1035

                bool r1035 = true;
                r1035 = r1035 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeleteSpecification());
                CommitOrRollback(r1035, parser);
                res = r1035;
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
                Checkpoint(parser); // r1036

                bool r1036 = true;
                if (r1036)
                { // may a1037
                    bool a1037 = false;
                    {
                        Checkpoint(parser); // r1038

                        bool r1038 = true;
                        r1038 = r1038 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r1038)
                        { // may a1039
                            bool a1039 = false;
                            {
                                Checkpoint(parser); // r1040

                                bool r1040 = true;
                                r1040 = r1040 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1040, parser);
                                a1039 = r1040;
                            }

                            r1038 |= a1039;
                        } // end may a1039

                        CommitOrRollback(r1038, parser);
                        a1037 = r1038;
                    }

                    r1036 |= a1037;
                } // end may a1037

                r1036 = r1036 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DELETE"));
                if (r1036)
                { // may a1041
                    bool a1041 = false;
                    {
                        Checkpoint(parser); // r1042

                        bool r1042 = true;
                        r1042 = r1042 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1042, parser);
                        a1041 = r1042;
                    }

                    r1036 |= a1041;
                } // end may a1041

                if (r1036)
                { // may a1043
                    bool a1043 = false;
                    {
                        Checkpoint(parser); // r1044

                        bool r1044 = true;
                        r1044 = r1044 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                        CommitOrRollback(r1044, parser);
                        a1043 = r1044;
                    }

                    r1036 |= a1043;
                } // end may a1043

                if (r1036)
                { // may a1045
                    bool a1045 = false;
                    {
                        Checkpoint(parser); // r1046

                        bool r1046 = true;
                        r1046 = r1046 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1046, parser);
                        a1045 = r1046;
                    }

                    r1036 |= a1045;
                } // end may a1045

                r1036 = r1036 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                if (r1036)
                { // may a1047
                    bool a1047 = false;
                    {
                        Checkpoint(parser); // r1048

                        bool r1048 = true;
                        if (r1048)
                        { // may a1049
                            bool a1049 = false;
                            {
                                Checkpoint(parser); // r1050

                                bool r1050 = true;
                                r1050 = r1050 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1050, parser);
                                a1049 = r1050;
                            }

                            r1048 |= a1049;
                        } // end may a1049

                        r1048 = r1048 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r1048, parser);
                        a1047 = r1048;
                    }

                    r1036 |= a1047;
                } // end may a1047

                if (r1036)
                { // may a1051
                    bool a1051 = false;
                    {
                        Checkpoint(parser); // r1052

                        bool r1052 = true;
                        if (r1052)
                        { // may a1053
                            bool a1053 = false;
                            {
                                Checkpoint(parser); // r1054

                                bool r1054 = true;
                                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1054, parser);
                                a1053 = r1054;
                            }

                            r1052 |= a1053;
                        } // end may a1053

                        r1052 = r1052 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r1052, parser);
                        a1051 = r1052;
                    }

                    r1036 |= a1051;
                } // end may a1051

                if (r1036)
                { // may a1055
                    bool a1055 = false;
                    {
                        Checkpoint(parser); // r1056

                        bool r1056 = true;
                        if (r1056)
                        { // may a1057
                            bool a1057 = false;
                            {
                                Checkpoint(parser); // r1058

                                bool r1058 = true;
                                r1058 = r1058 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1058, parser);
                                a1057 = r1058;
                            }

                            r1056 |= a1057;
                        } // end may a1057

                        r1056 = r1056 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r1056, parser);
                        a1055 = r1056;
                    }

                    r1036 |= a1055;
                } // end may a1055

                CommitOrRollback(r1036, parser);
                res = r1036;
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
                Checkpoint(parser); // r1059

                bool r1059 = true;
                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CREATE"));
                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
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

                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1059)
                { // may a1062
                    bool a1062 = false;
                    {
                        Checkpoint(parser); // r1063

                        bool r1063 = true;
                        r1063 = r1063 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1063, parser);
                        a1062 = r1063;
                    }

                    r1059 |= a1062;
                } // end may a1062

                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                if (r1059)
                { // may a1064
                    bool a1064 = false;
                    {
                        Checkpoint(parser); // r1065

                        bool r1065 = true;
                        r1065 = r1065 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1065, parser);
                        a1064 = r1065;
                    }

                    r1059 |= a1064;
                } // end may a1064

                r1059 = r1059 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r1059, parser);
                res = r1059;
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
                Checkpoint(parser); // r1066

                bool r1066 = true;
                if (r1066)
                { // alternatives a1067 must
                    bool a1067 = false;
                    if (!a1067)
                    {
                        Checkpoint(parser); // r1068

                        bool r1068 = true;
                        r1068 = r1068 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnDefinition());
                        CommitOrRollback(r1068, parser);
                        a1067 = r1068;
                    }

                    if (!a1067)
                    {
                        Checkpoint(parser); // r1069

                        bool r1069 = true;
                        r1069 = r1069 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableConstraint());
                        CommitOrRollback(r1069, parser);
                        a1067 = r1069;
                    }

                    r1066 &= a1067;

                } // end alternatives a1067

                if (r1066)
                { // may a1070
                    bool a1070 = false;
                    {
                        Checkpoint(parser); // r1071

                        bool r1071 = true;
                        if (r1071)
                        { // may a1072
                            bool a1072 = false;
                            {
                                Checkpoint(parser); // r1073

                                bool r1073 = true;
                                r1073 = r1073 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1073, parser);
                                a1072 = r1073;
                            }

                            r1071 |= a1072;
                        } // end may a1072

                        r1071 = r1071 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1071)
                        { // may a1074
                            bool a1074 = false;
                            {
                                Checkpoint(parser); // r1075

                                bool r1075 = true;
                                r1075 = r1075 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1075, parser);
                                a1074 = r1075;
                            }

                            r1071 |= a1074;
                        } // end may a1074

                        r1071 = r1071 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                        CommitOrRollback(r1071, parser);
                        a1070 = r1071;
                    }

                    r1066 |= a1070;
                } // end may a1070

                CommitOrRollback(r1066, parser);
                res = r1066;
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
                Checkpoint(parser); // r1076

                bool r1076 = true;
                r1076 = r1076 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                r1076 = r1076 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1076 = r1076 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataType());
                if (r1076)
                { // may a1077
                    bool a1077 = false;
                    {
                        Checkpoint(parser); // r1078

                        bool r1078 = true;
                        if (r1078)
                        { // may a1079
                            bool a1079 = false;
                            {
                                Checkpoint(parser); // r1080

                                bool r1080 = true;
                                r1080 = r1080 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1080, parser);
                                a1079 = r1080;
                            }

                            r1078 |= a1079;
                        } // end may a1079

                        if (r1078)
                        { // alternatives a1081 must
                            bool a1081 = false;
                            if (!a1081)
                            {
                                Checkpoint(parser); // r1082

                                bool r1082 = true;
                                r1082 = r1082 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnDefaultDefinition());
                                CommitOrRollback(r1082, parser);
                                a1081 = r1082;
                            }

                            if (!a1081)
                            {
                                Checkpoint(parser); // r1083

                                bool r1083 = true;
                                r1083 = r1083 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentityDefinition());
                                CommitOrRollback(r1083, parser);
                                a1081 = r1083;
                            }

                            r1078 &= a1081;

                        } // end alternatives a1081

                        CommitOrRollback(r1078, parser);
                        a1077 = r1078;
                    }

                    r1076 |= a1077;
                } // end may a1077

                if (r1076)
                { // may a1084
                    bool a1084 = false;
                    {
                        Checkpoint(parser); // r1085

                        bool r1085 = true;
                        if (r1085)
                        { // may a1086
                            bool a1086 = false;
                            {
                                Checkpoint(parser); // r1087

                                bool r1087 = true;
                                r1087 = r1087 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1087, parser);
                                a1086 = r1087;
                            }

                            r1085 |= a1086;
                        } // end may a1086

                        r1085 = r1085 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnConstraint());
                        CommitOrRollback(r1085, parser);
                        a1084 = r1085;
                    }

                    r1076 |= a1084;
                } // end may a1084

                CommitOrRollback(r1076, parser);
                res = r1076;
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
                Checkpoint(parser); // r1088

                bool r1088 = true;
                if (r1088)
                { // may a1089
                    bool a1089 = false;
                    {
                        Checkpoint(parser); // r1090

                        bool r1090 = true;
                        r1090 = r1090 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1090, parser);
                        a1089 = r1090;
                    }

                    r1088 |= a1089;
                } // end may a1089

                r1088 = r1088 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                if (r1088)
                { // may a1091
                    bool a1091 = false;
                    {
                        Checkpoint(parser); // r1092

                        bool r1092 = true;
                        r1092 = r1092 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1092, parser);
                        a1091 = r1092;
                    }

                    r1088 |= a1091;
                } // end may a1091

                r1088 = r1088 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r1088, parser);
                res = r1088;
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
                Checkpoint(parser); // r1093

                bool r1093 = true;
                r1093 = r1093 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CONSTRAINT"));
                if (r1093)
                { // may a1094
                    bool a1094 = false;
                    {
                        Checkpoint(parser); // r1095

                        bool r1095 = true;
                        r1095 = r1095 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1095, parser);
                        a1094 = r1095;
                    }

                    r1093 |= a1094;
                } // end may a1094

                r1093 = r1093 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintName());
                if (r1093)
                { // may a1096
                    bool a1096 = false;
                    {
                        Checkpoint(parser); // r1097

                        bool r1097 = true;
                        r1097 = r1097 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1097, parser);
                        a1096 = r1097;
                    }

                    r1093 |= a1096;
                } // end may a1096

                CommitOrRollback(r1093, parser);
                res = r1093;
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
                Checkpoint(parser); // r1098

                bool r1098 = true;
                r1098 = r1098 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IDENTITY"));
                if (r1098)
                { // may a1099
                    bool a1099 = false;
                    {
                        Checkpoint(parser); // r1100

                        bool r1100 = true;
                        if (r1100)
                        { // may a1101
                            bool a1101 = false;
                            {
                                Checkpoint(parser); // r1102

                                bool r1102 = true;
                                r1102 = r1102 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1102, parser);
                                a1101 = r1102;
                            }

                            r1100 |= a1101;
                        } // end may a1101

                        r1100 = r1100 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r1100, parser);
                        a1099 = r1100;
                    }

                    r1098 |= a1099;
                } // end may a1099

                CommitOrRollback(r1098, parser);
                res = r1098;
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
                Checkpoint(parser); // r1103

                bool r1103 = true;
                if (r1103)
                { // may a1104
                    bool a1104 = false;
                    {
                        Checkpoint(parser); // r1105

                        bool r1105 = true;
                        r1105 = r1105 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1105, parser);
                        a1104 = r1105;
                    }

                    r1103 |= a1104;
                } // end may a1104

                r1103 = r1103 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintSpecification());
                CommitOrRollback(r1103, parser);
                res = r1103;
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
                if (r1106)
                { // may a1109
                    bool a1109 = false;
                    {
                        Checkpoint(parser); // r1110

                        bool r1110 = true;
                        r1110 = r1110 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1110, parser);
                        a1109 = r1110;
                    }

                    r1106 |= a1109;
                } // end may a1109

                r1106 = r1106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1106)
                { // may a1111
                    bool a1111 = false;
                    {
                        Checkpoint(parser); // r1112

                        bool r1112 = true;
                        r1112 = r1112 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1112, parser);
                        a1111 = r1112;
                    }

                    r1106 |= a1111;
                } // end may a1111

                r1106 = r1106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                if (r1106)
                { // may a1113
                    bool a1113 = false;
                    {
                        Checkpoint(parser); // r1114

                        bool r1114 = true;
                        r1114 = r1114 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1114, parser);
                        a1113 = r1114;
                    }

                    r1106 |= a1113;
                } // end may a1113

                r1106 = r1106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r1106, parser);
                res = r1106;
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
                Checkpoint(parser); // r1115

                bool r1115 = true;
                if (r1115)
                { // alternatives a1116 must
                    bool a1116 = false;
                    if (!a1116)
                    {
                        Checkpoint(parser); // r1117

                        bool r1117 = true;
                        r1117 = r1117 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PRIMARY"));
                        r1117 = r1117 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r1117 = r1117 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"KEY"));
                        CommitOrRollback(r1117, parser);
                        a1116 = r1117;
                    }

                    if (!a1116)
                    {
                        Checkpoint(parser); // r1118

                        bool r1118 = true;
                        r1118 = r1118 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNIQUE"));
                        CommitOrRollback(r1118, parser);
                        a1116 = r1118;
                    }

                    r1115 &= a1116;

                } // end alternatives a1116

                if (r1115)
                { // may a1119
                    bool a1119 = false;
                    {
                        Checkpoint(parser); // r1120

                        bool r1120 = true;
                        r1120 = r1120 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r1120)
                        { // alternatives a1121 must
                            bool a1121 = false;
                            if (!a1121)
                            {
                                Checkpoint(parser); // r1122

                                bool r1122 = true;
                                r1122 = r1122 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLUSTERED"));
                                CommitOrRollback(r1122, parser);
                                a1121 = r1122;
                            }

                            if (!a1121)
                            {
                                Checkpoint(parser); // r1123

                                bool r1123 = true;
                                r1123 = r1123 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NONCLUSTERED"));
                                CommitOrRollback(r1123, parser);
                                a1121 = r1123;
                            }

                            r1120 &= a1121;

                        } // end alternatives a1121

                        CommitOrRollback(r1120, parser);
                        a1119 = r1120;
                    }

                    r1115 |= a1119;
                } // end may a1119

                CommitOrRollback(r1115, parser);
                res = r1115;
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
                Checkpoint(parser); // r1124

                bool r1124 = true;
                r1124 = r1124 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DROP"));
                r1124 = r1124 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1124 = r1124 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r1124)
                { // may a1125
                    bool a1125 = false;
                    {
                        Checkpoint(parser); // r1126

                        bool r1126 = true;
                        r1126 = r1126 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1126, parser);
                        a1125 = r1126;
                    }

                    r1124 |= a1125;
                } // end may a1125

                r1124 = r1124 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1124, parser);
                res = r1124;
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
                Checkpoint(parser); // r1127

                bool r1127 = true;
                r1127 = r1127 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRUNCATE"));
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
                Checkpoint(parser); // r1130

                bool r1130 = true;
                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CREATE"));
                if (r1130)
                { // may a1131
                    bool a1131 = false;
                    {
                        Checkpoint(parser); // r1132

                        bool r1132 = true;
                        r1132 = r1132 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r1132 = r1132 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNIQUE"));
                        CommitOrRollback(r1132, parser);
                        a1131 = r1132;
                    }

                    r1130 |= a1131;
                } // end may a1131

                if (r1130)
                { // may a1133
                    bool a1133 = false;
                    {
                        Checkpoint(parser); // r1134

                        bool r1134 = true;
                        r1134 = r1134 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r1134)
                        { // alternatives a1135 must
                            bool a1135 = false;
                            if (!a1135)
                            {
                                Checkpoint(parser); // r1136

                                bool r1136 = true;
                                r1136 = r1136 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLUSTERED"));
                                CommitOrRollback(r1136, parser);
                                a1135 = r1136;
                            }

                            if (!a1135)
                            {
                                Checkpoint(parser); // r1137

                                bool r1137 = true;
                                r1137 = r1137 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NONCLUSTERED"));
                                CommitOrRollback(r1137, parser);
                                a1135 = r1137;
                            }

                            r1134 &= a1135;

                        } // end alternatives a1135

                        CommitOrRollback(r1134, parser);
                        a1133 = r1134;
                    }

                    r1130 |= a1133;
                } // end may a1133

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INDEX"));
                if (r1130)
                { // may a1138
                    bool a1138 = false;
                    {
                        Checkpoint(parser); // r1139

                        bool r1139 = true;
                        r1139 = r1139 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1139, parser);
                        a1138 = r1139;
                    }

                    r1130 |= a1138;
                } // end may a1138

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexName());
                if (r1130)
                { // may a1140
                    bool a1140 = false;
                    {
                        Checkpoint(parser); // r1141

                        bool r1141 = true;
                        r1141 = r1141 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1141, parser);
                        a1140 = r1141;
                    }

                    r1130 |= a1140;
                } // end may a1140

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                if (r1130)
                { // may a1142
                    bool a1142 = false;
                    {
                        Checkpoint(parser); // r1143

                        bool r1143 = true;
                        r1143 = r1143 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1143, parser);
                        a1142 = r1143;
                    }

                    r1130 |= a1142;
                } // end may a1142

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r1130)
                { // may a1144
                    bool a1144 = false;
                    {
                        Checkpoint(parser); // r1145

                        bool r1145 = true;
                        r1145 = r1145 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1145, parser);
                        a1144 = r1145;
                    }

                    r1130 |= a1144;
                } // end may a1144

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1130)
                { // may a1146
                    bool a1146 = false;
                    {
                        Checkpoint(parser); // r1147

                        bool r1147 = true;
                        r1147 = r1147 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1147, parser);
                        a1146 = r1147;
                    }

                    r1130 |= a1146;
                } // end may a1146

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                if (r1130)
                { // may a1148
                    bool a1148 = false;
                    {
                        Checkpoint(parser); // r1149

                        bool r1149 = true;
                        r1149 = r1149 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1149, parser);
                        a1148 = r1149;
                    }

                    r1130 |= a1148;
                } // end may a1148

                r1130 = r1130 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                if (r1130)
                { // may a1150
                    bool a1150 = false;
                    {
                        Checkpoint(parser); // r1151

                        bool r1151 = true;
                        if (r1151)
                        { // may a1152
                            bool a1152 = false;
                            {
                                Checkpoint(parser); // r1153

                                bool r1153 = true;
                                r1153 = r1153 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1153, parser);
                                a1152 = r1153;
                            }

                            r1151 |= a1152;
                        } // end may a1152

                        r1151 = r1151 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INCLUDE"));
                        if (r1151)
                        { // may a1154
                            bool a1154 = false;
                            {
                                Checkpoint(parser); // r1155

                                bool r1155 = true;
                                r1155 = r1155 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1155, parser);
                                a1154 = r1155;
                            }

                            r1151 |= a1154;
                        } // end may a1154

                        r1151 = r1151 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r1151)
                        { // may a1156
                            bool a1156 = false;
                            {
                                Checkpoint(parser); // r1157

                                bool r1157 = true;
                                r1157 = r1157 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1157, parser);
                                a1156 = r1157;
                            }

                            r1151 |= a1156;
                        } // end may a1156

                        r1151 = r1151 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IncludedColumnList());
                        if (r1151)
                        { // may a1158
                            bool a1158 = false;
                            {
                                Checkpoint(parser); // r1159

                                bool r1159 = true;
                                r1159 = r1159 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1159, parser);
                                a1158 = r1159;
                            }

                            r1151 |= a1158;
                        } // end may a1158

                        r1151 = r1151 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r1151, parser);
                        a1150 = r1151;
                    }

                    r1130 |= a1150;
                } // end may a1150

                CommitOrRollback(r1130, parser);
                res = r1130;
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
                Checkpoint(parser); // r1160

                bool r1160 = true;
                r1160 = r1160 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumn());
                if (r1160)
                { // may a1161
                    bool a1161 = false;
                    {
                        Checkpoint(parser); // r1162

                        bool r1162 = true;
                        if (r1162)
                        { // may a1163
                            bool a1163 = false;
                            {
                                Checkpoint(parser); // r1164

                                bool r1164 = true;
                                r1164 = r1164 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1164, parser);
                                a1163 = r1164;
                            }

                            r1162 |= a1163;
                        } // end may a1163

                        r1162 = r1162 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1162)
                        { // may a1165
                            bool a1165 = false;
                            {
                                Checkpoint(parser); // r1166

                                bool r1166 = true;
                                r1166 = r1166 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1166, parser);
                                a1165 = r1166;
                            }

                            r1162 |= a1165;
                        } // end may a1165

                        r1162 = r1162 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                        CommitOrRollback(r1162, parser);
                        a1161 = r1162;
                    }

                    r1160 |= a1161;
                } // end may a1161

                CommitOrRollback(r1160, parser);
                res = r1160;
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
                Checkpoint(parser); // r1167

                bool r1167 = true;
                r1167 = r1167 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r1167)
                { // may a1168
                    bool a1168 = false;
                    {
                        Checkpoint(parser); // r1169

                        bool r1169 = true;
                        if (r1169)
                        { // may a1170
                            bool a1170 = false;
                            {
                                Checkpoint(parser); // r1171

                                bool r1171 = true;
                                r1171 = r1171 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1171, parser);
                                a1170 = r1171;
                            }

                            r1169 |= a1170;
                        } // end may a1170

                        if (r1169)
                        { // alternatives a1172 must
                            bool a1172 = false;
                            if (!a1172)
                            {
                                Checkpoint(parser); // r1173

                                bool r1173 = true;
                                r1173 = r1173 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ASC"));
                                CommitOrRollback(r1173, parser);
                                a1172 = r1173;
                            }

                            if (!a1172)
                            {
                                Checkpoint(parser); // r1174

                                bool r1174 = true;
                                r1174 = r1174 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DESC"));
                                CommitOrRollback(r1174, parser);
                                a1172 = r1174;
                            }

                            r1169 &= a1172;

                        } // end alternatives a1172

                        CommitOrRollback(r1169, parser);
                        a1168 = r1169;
                    }

                    r1167 |= a1168;
                } // end may a1168

                CommitOrRollback(r1167, parser);
                res = r1167;
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
                Checkpoint(parser); // r1175

                bool r1175 = true;
                r1175 = r1175 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r1175)
                { // may a1176
                    bool a1176 = false;
                    {
                        Checkpoint(parser); // r1177

                        bool r1177 = true;
                        if (r1177)
                        { // may a1178
                            bool a1178 = false;
                            {
                                Checkpoint(parser); // r1179

                                bool r1179 = true;
                                r1179 = r1179 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1179, parser);
                                a1178 = r1179;
                            }

                            r1177 |= a1178;
                        } // end may a1178

                        r1177 = r1177 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1177)
                        { // may a1180
                            bool a1180 = false;
                            {
                                Checkpoint(parser); // r1181

                                bool r1181 = true;
                                r1181 = r1181 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1181, parser);
                                a1180 = r1181;
                            }

                            r1177 |= a1180;
                        } // end may a1180

                        r1177 = r1177 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IncludedColumnList());
                        CommitOrRollback(r1177, parser);
                        a1176 = r1177;
                    }

                    r1175 |= a1176;
                } // end may a1176

                CommitOrRollback(r1175, parser);
                res = r1175;
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
                Checkpoint(parser); // r1182

                bool r1182 = true;
                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DROP"));
                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INDEX"));
                if (r1182)
                { // may a1183
                    bool a1183 = false;
                    {
                        Checkpoint(parser); // r1184

                        bool r1184 = true;
                        r1184 = r1184 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1184, parser);
                        a1183 = r1184;
                    }

                    r1182 |= a1183;
                } // end may a1183

                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexName());
                if (r1182)
                { // may a1185
                    bool a1185 = false;
                    {
                        Checkpoint(parser); // r1186

                        bool r1186 = true;
                        r1186 = r1186 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1186, parser);
                        a1185 = r1186;
                    }

                    r1182 |= a1185;
                } // end may a1185

                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                if (r1182)
                { // may a1187
                    bool a1187 = false;
                    {
                        Checkpoint(parser); // r1188

                        bool r1188 = true;
                        r1188 = r1188 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1188, parser);
                        a1187 = r1188;
                    }

                    r1182 |= a1187;
                } // end may a1187

                r1182 = r1182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1182, parser);
                res = r1182;
            }



            return res;
        }
    }


}