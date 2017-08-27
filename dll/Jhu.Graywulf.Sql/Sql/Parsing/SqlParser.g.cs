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
                        r96 = r96 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
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
                Checkpoint(parser); // r97

                bool r97 = true;
                if (r97)
                { // may a98
                    bool a98 = false;
                    {
                        Checkpoint(parser); // r99

                        bool r99 = true;
                        r99 = r99 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LogicalNot());
                        CommitOrRollback(r99, parser);
                        a98 = r99;
                    }

                    r97 |= a98;
                } // end may a98

                if (r97)
                { // may a100
                    bool a100 = false;
                    {
                        Checkpoint(parser); // r101

                        bool r101 = true;
                        r101 = r101 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r101, parser);
                        a100 = r101;
                    }

                    r97 |= a100;
                } // end may a100

                if (r97)
                { // alternatives a102 must
                    bool a102 = false;
                    if (!a102)
                    {
                        Checkpoint(parser); // r103

                        bool r103 = true;
                        r103 = r103 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Predicate());
                        CommitOrRollback(r103, parser);
                        a102 = r103;
                    }

                    if (!a102)
                    {
                        Checkpoint(parser); // r104

                        bool r104 = true;
                        r104 = r104 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpressionBrackets());
                        CommitOrRollback(r104, parser);
                        a102 = r104;
                    }

                    r97 &= a102;

                } // end alternatives a102

                if (r97)
                { // may a105
                    bool a105 = false;
                    {
                        Checkpoint(parser); // r106

                        bool r106 = true;
                        if (r106)
                        { // may a107
                            bool a107 = false;
                            {
                                Checkpoint(parser); // r108

                                bool r108 = true;
                                r108 = r108 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r108, parser);
                                a107 = r108;
                            }

                            r106 |= a107;
                        } // end may a107

                        r106 = r106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.LogicalOperator());
                        if (r106)
                        { // may a109
                            bool a109 = false;
                            {
                                Checkpoint(parser); // r110

                                bool r110 = true;
                                r110 = r110 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r110, parser);
                                a109 = r110;
                            }

                            r106 |= a109;
                        } // end may a109

                        r106 = r106 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                        CommitOrRollback(r106, parser);
                        a105 = r106;
                    }

                    r97 |= a105;
                } // end may a105

                CommitOrRollback(r97, parser);
                res = r97;
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
                Checkpoint(parser); // r111

                bool r111 = true;
                r111 = r111 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r111)
                { // may a112
                    bool a112 = false;
                    {
                        Checkpoint(parser); // r113

                        bool r113 = true;
                        r113 = r113 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r113, parser);
                        a112 = r113;
                    }

                    r111 |= a112;
                } // end may a112

                r111 = r111 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r111)
                { // may a114
                    bool a114 = false;
                    {
                        Checkpoint(parser); // r115

                        bool r115 = true;
                        r115 = r115 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r115, parser);
                        a114 = r115;
                    }

                    r111 |= a114;
                } // end may a114

                r111 = r111 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r111, parser);
                res = r111;
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
                Checkpoint(parser); // r116

                bool r116 = true;
                if (r116)
                { // alternatives a117 must
                    bool a117 = false;
                    if (!a117)
                    {
                        Checkpoint(parser); // r118

                        bool r118 = true;
                        r118 = r118 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r118)
                        { // may a119
                            bool a119 = false;
                            {
                                Checkpoint(parser); // r120

                                bool r120 = true;
                                r120 = r120 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r120, parser);
                                a119 = r120;
                            }

                            r118 |= a119;
                        } // end may a119

                        r118 = r118 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ComparisonOperator());
                        if (r118)
                        { // may a121
                            bool a121 = false;
                            {
                                Checkpoint(parser); // r122

                                bool r122 = true;
                                r122 = r122 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r122, parser);
                                a121 = r122;
                            }

                            r118 |= a121;
                        } // end may a121

                        r118 = r118 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r118, parser);
                        a117 = r118;
                    }

                    if (!a117)
                    {
                        Checkpoint(parser); // r123

                        bool r123 = true;
                        r123 = r123 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r123)
                        { // may a124
                            bool a124 = false;
                            {
                                Checkpoint(parser); // r125

                                bool r125 = true;
                                r125 = r125 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r125, parser);
                                a124 = r125;
                            }

                            r123 |= a124;
                        } // end may a124

                        if (r123)
                        { // may a126
                            bool a126 = false;
                            {
                                Checkpoint(parser); // r127

                                bool r127 = true;
                                r127 = r127 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r127, parser);
                                a126 = r127;
                            }

                            r123 |= a126;
                        } // end may a126

                        r123 = r123 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LIKE"));
                        if (r123)
                        { // may a128
                            bool a128 = false;
                            {
                                Checkpoint(parser); // r129

                                bool r129 = true;
                                r129 = r129 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r129, parser);
                                a128 = r129;
                            }

                            r123 |= a128;
                        } // end may a128

                        r123 = r123 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r123)
                        { // may a130
                            bool a130 = false;
                            {
                                Checkpoint(parser); // r131

                                bool r131 = true;
                                if (r131)
                                { // may a132
                                    bool a132 = false;
                                    {
                                        Checkpoint(parser); // r133

                                        bool r133 = true;
                                        r133 = r133 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r133, parser);
                                        a132 = r133;
                                    }

                                    r131 |= a132;
                                } // end may a132

                                r131 = r131 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ESCAPE"));
                                if (r131)
                                { // may a134
                                    bool a134 = false;
                                    {
                                        Checkpoint(parser); // r135

                                        bool r135 = true;
                                        r135 = r135 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r135, parser);
                                        a134 = r135;
                                    }

                                    r131 |= a134;
                                } // end may a134

                                r131 = r131 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                                CommitOrRollback(r131, parser);
                                a130 = r131;
                            }

                            r123 |= a130;
                        } // end may a130

                        CommitOrRollback(r123, parser);
                        a117 = r123;
                    }

                    if (!a117)
                    {
                        Checkpoint(parser); // r136

                        bool r136 = true;
                        r136 = r136 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r136)
                        { // may a137
                            bool a137 = false;
                            {
                                Checkpoint(parser); // r138

                                bool r138 = true;
                                if (r138)
                                { // may a139
                                    bool a139 = false;
                                    {
                                        Checkpoint(parser); // r140

                                        bool r140 = true;
                                        r140 = r140 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r140, parser);
                                        a139 = r140;
                                    }

                                    r138 |= a139;
                                } // end may a139

                                r138 = r138 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r138, parser);
                                a137 = r138;
                            }

                            r136 |= a137;
                        } // end may a137

                        if (r136)
                        { // may a141
                            bool a141 = false;
                            {
                                Checkpoint(parser); // r142

                                bool r142 = true;
                                r142 = r142 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r142, parser);
                                a141 = r142;
                            }

                            r136 |= a141;
                        } // end may a141

                        r136 = r136 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BETWEEN"));
                        if (r136)
                        { // may a143
                            bool a143 = false;
                            {
                                Checkpoint(parser); // r144

                                bool r144 = true;
                                r144 = r144 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r144, parser);
                                a143 = r144;
                            }

                            r136 |= a143;
                        } // end may a143

                        r136 = r136 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r136)
                        { // may a145
                            bool a145 = false;
                            {
                                Checkpoint(parser); // r146

                                bool r146 = true;
                                r146 = r146 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r146, parser);
                                a145 = r146;
                            }

                            r136 |= a145;
                        } // end may a145

                        r136 = r136 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AND"));
                        if (r136)
                        { // may a147
                            bool a147 = false;
                            {
                                Checkpoint(parser); // r148

                                bool r148 = true;
                                r148 = r148 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r148, parser);
                                a147 = r148;
                            }

                            r136 |= a147;
                        } // end may a147

                        r136 = r136 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r136, parser);
                        a117 = r136;
                    }

                    if (!a117)
                    {
                        Checkpoint(parser); // r149

                        bool r149 = true;
                        r149 = r149 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r149)
                        { // may a150
                            bool a150 = false;
                            {
                                Checkpoint(parser); // r151

                                bool r151 = true;
                                r151 = r151 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r151, parser);
                                a150 = r151;
                            }

                            r149 |= a150;
                        } // end may a150

                        r149 = r149 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IS"));
                        if (r149)
                        { // may a152
                            bool a152 = false;
                            {
                                Checkpoint(parser); // r153

                                bool r153 = true;
                                r153 = r153 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r153 = r153 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r153, parser);
                                a152 = r153;
                            }

                            r149 |= a152;
                        } // end may a152

                        r149 = r149 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r149 = r149 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                        CommitOrRollback(r149, parser);
                        a117 = r149;
                    }

                    if (!a117)
                    {
                        Checkpoint(parser); // r154

                        bool r154 = true;
                        r154 = r154 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r154)
                        { // may a155
                            bool a155 = false;
                            {
                                Checkpoint(parser); // r156

                                bool r156 = true;
                                r156 = r156 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                CommitOrRollback(r156, parser);
                                a155 = r156;
                            }

                            r154 |= a155;
                        } // end may a155

                        if (r154)
                        { // may a157
                            bool a157 = false;
                            {
                                Checkpoint(parser); // r158

                                bool r158 = true;
                                r158 = r158 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r158, parser);
                                a157 = r158;
                            }

                            r154 |= a157;
                        } // end may a157

                        r154 = r154 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IN"));
                        if (r154)
                        { // may a159
                            bool a159 = false;
                            {
                                Checkpoint(parser); // r160

                                bool r160 = true;
                                r160 = r160 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r160, parser);
                                a159 = r160;
                            }

                            r154 |= a159;
                        } // end may a159

                        if (r154)
                        { // alternatives a161 must
                            bool a161 = false;
                            if (!a161)
                            {
                                Checkpoint(parser); // r162

                                bool r162 = true;
                                r162 = r162 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                                CommitOrRollback(r162, parser);
                                a161 = r162;
                            }

                            if (!a161)
                            {
                                Checkpoint(parser); // r163

                                bool r163 = true;
                                r163 = r163 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                                if (r163)
                                { // may a164
                                    bool a164 = false;
                                    {
                                        Checkpoint(parser); // r165

                                        bool r165 = true;
                                        r165 = r165 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r165, parser);
                                        a164 = r165;
                                    }

                                    r163 |= a164;
                                } // end may a164

                                r163 = r163 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                                if (r163)
                                { // may a166
                                    bool a166 = false;
                                    {
                                        Checkpoint(parser); // r167

                                        bool r167 = true;
                                        r167 = r167 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r167, parser);
                                        a166 = r167;
                                    }

                                    r163 |= a166;
                                } // end may a166

                                r163 = r163 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                                CommitOrRollback(r163, parser);
                                a161 = r163;
                            }

                            r154 &= a161;

                        } // end alternatives a161

                        CommitOrRollback(r154, parser);
                        a117 = r154;
                    }

                    if (!a117)
                    {
                        Checkpoint(parser); // r168

                        bool r168 = true;
                        r168 = r168 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r168)
                        { // may a169
                            bool a169 = false;
                            {
                                Checkpoint(parser); // r170

                                bool r170 = true;
                                r170 = r170 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r170, parser);
                                a169 = r170;
                            }

                            r168 |= a169;
                        } // end may a169

                        r168 = r168 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ComparisonOperator());
                        if (r168)
                        { // may a171
                            bool a171 = false;
                            {
                                Checkpoint(parser); // r172

                                bool r172 = true;
                                r172 = r172 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r172, parser);
                                a171 = r172;
                            }

                            r168 |= a171;
                        } // end may a171

                        if (r168)
                        { // alternatives a173 must
                            bool a173 = false;
                            if (!a173)
                            {
                                Checkpoint(parser); // r174

                                bool r174 = true;
                                r174 = r174 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r174, parser);
                                a173 = r174;
                            }

                            if (!a173)
                            {
                                Checkpoint(parser); // r175

                                bool r175 = true;
                                r175 = r175 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SOME"));
                                CommitOrRollback(r175, parser);
                                a173 = r175;
                            }

                            if (!a173)
                            {
                                Checkpoint(parser); // r176

                                bool r176 = true;
                                r176 = r176 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ANY"));
                                CommitOrRollback(r176, parser);
                                a173 = r176;
                            }

                            r168 &= a173;

                        } // end alternatives a173

                        if (r168)
                        { // may a177
                            bool a177 = false;
                            {
                                Checkpoint(parser); // r178

                                bool r178 = true;
                                r178 = r178 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r178, parser);
                                a177 = r178;
                            }

                            r168 |= a177;
                        } // end may a177

                        r168 = r168 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r168, parser);
                        a117 = r168;
                    }

                    if (!a117)
                    {
                        Checkpoint(parser); // r179

                        bool r179 = true;
                        r179 = r179 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"EXISTS"));
                        if (r179)
                        { // may a180
                            bool a180 = false;
                            {
                                Checkpoint(parser); // r181

                                bool r181 = true;
                                r181 = r181 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r181, parser);
                                a180 = r181;
                            }

                            r179 |= a180;
                        } // end may a180

                        r179 = r179 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                        CommitOrRollback(r179, parser);
                        a117 = r179;
                    }

                    r116 &= a117;

                } // end alternatives a117

                CommitOrRollback(r116, parser);
                res = r116;
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
                Checkpoint(parser); // r182

                bool r182 = true;
                r182 = r182 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CASE"));
                if (r182)
                { // may a183
                    bool a183 = false;
                    {
                        Checkpoint(parser); // r184

                        bool r184 = true;
                        r184 = r184 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r184, parser);
                        a183 = r184;
                    }

                    r182 |= a183;
                } // end may a183

                r182 = r182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r182)
                { // may a185
                    bool a185 = false;
                    {
                        Checkpoint(parser); // r186

                        bool r186 = true;
                        r186 = r186 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r186, parser);
                        a185 = r186;
                    }

                    r182 |= a185;
                } // end may a185

                r182 = r182 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList());
                if (r182)
                { // may a187
                    bool a187 = false;
                    {
                        Checkpoint(parser); // r188

                        bool r188 = true;
                        r188 = r188 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CaseElse());
                        CommitOrRollback(r188, parser);
                        a187 = r188;
                    }

                    r182 |= a187;
                } // end may a187

                r182 = r182 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r182, parser);
                res = r182;
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
                Checkpoint(parser); // r189

                bool r189 = true;
                r189 = r189 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhen());
                if (r189)
                { // may a190
                    bool a190 = false;
                    {
                        Checkpoint(parser); // r191

                        bool r191 = true;
                        r191 = r191 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleCaseWhenList());
                        CommitOrRollback(r191, parser);
                        a190 = r191;
                    }

                    r189 |= a190;
                } // end may a190

                CommitOrRollback(r189, parser);
                res = r189;
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
                Checkpoint(parser); // r192

                bool r192 = true;
                r192 = r192 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHEN"));
                r192 = r192 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                r192 = r192 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THEN"));
                r192 = r192 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r192, parser);
                res = r192;
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
                Checkpoint(parser); // r193

                bool r193 = true;
                r193 = r193 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CASE"));
                r193 = r193 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList());
                if (r193)
                { // may a194
                    bool a194 = false;
                    {
                        Checkpoint(parser); // r195

                        bool r195 = true;
                        r195 = r195 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CaseElse());
                        CommitOrRollback(r195, parser);
                        a194 = r195;
                    }

                    r193 |= a194;
                } // end may a194

                r193 = r193 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r193, parser);
                res = r193;
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
                Checkpoint(parser); // r196

                bool r196 = true;
                r196 = r196 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhen());
                if (r196)
                { // may a197
                    bool a197 = false;
                    {
                        Checkpoint(parser); // r198

                        bool r198 = true;
                        r198 = r198 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SearchedCaseWhenList());
                        CommitOrRollback(r198, parser);
                        a197 = r198;
                    }

                    r196 |= a197;
                } // end may a197

                CommitOrRollback(r196, parser);
                res = r196;
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
                Checkpoint(parser); // r199

                bool r199 = true;
                r199 = r199 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHEN"));
                r199 = r199 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THEN"));
                r199 = r199 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r199, parser);
                res = r199;
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
                Checkpoint(parser); // r200

                bool r200 = true;
                r200 = r200 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ELSE"));
                r200 = r200 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r200, parser);
                res = r200;
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
                Checkpoint(parser); // r201

                bool r201 = true;
                if (r201)
                { // may a202
                    bool a202 = false;
                    {
                        Checkpoint(parser); // r203

                        bool r203 = true;
                        r203 = r203 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                        if (r203)
                        { // may a204
                            bool a204 = false;
                            {
                                Checkpoint(parser); // r205

                                bool r205 = true;
                                r205 = r205 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r205, parser);
                                a204 = r205;
                            }

                            r203 |= a204;
                        } // end may a204

                        r203 = r203 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                        if (r203)
                        { // may a206
                            bool a206 = false;
                            {
                                Checkpoint(parser); // r207

                                bool r207 = true;
                                r207 = r207 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r207, parser);
                                a206 = r207;
                            }

                            r203 |= a206;
                        } // end may a206

                        CommitOrRollback(r203, parser);
                        a202 = r203;
                    }

                    r201 |= a202;
                } // end may a202

                if (r201)
                { // alternatives a208 must
                    bool a208 = false;
                    if (!a208)
                    {
                        Checkpoint(parser); // r209

                        bool r209 = true;
                        r209 = r209 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                        if (r209)
                        { // may a210
                            bool a210 = false;
                            {
                                Checkpoint(parser); // r211

                                bool r211 = true;
                                r211 = r211 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r211, parser);
                                a210 = r211;
                            }

                            r209 |= a210;
                        } // end may a210

                        r209 = r209 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r209)
                        { // may a212
                            bool a212 = false;
                            {
                                Checkpoint(parser); // r213

                                bool r213 = true;
                                if (r213)
                                { // may a214
                                    bool a214 = false;
                                    {
                                        Checkpoint(parser); // r215

                                        bool r215 = true;
                                        r215 = r215 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r215, parser);
                                        a214 = r215;
                                    }

                                    r213 |= a214;
                                } // end may a214

                                r213 = r213 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                CommitOrRollback(r213, parser);
                                a212 = r213;
                            }

                            r209 |= a212;
                        } // end may a212

                        if (r209)
                        { // may a216
                            bool a216 = false;
                            {
                                Checkpoint(parser); // r217

                                bool r217 = true;
                                r217 = r217 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r217, parser);
                                a216 = r217;
                            }

                            r209 |= a216;
                        } // end may a216

                        r209 = r209 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r209)
                        { // may a218
                            bool a218 = false;
                            {
                                Checkpoint(parser); // r219

                                bool r219 = true;
                                r219 = r219 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r219, parser);
                                a218 = r219;
                            }

                            r209 |= a218;
                        } // end may a218

                        r209 = r209 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r209, parser);
                        a208 = r209;
                    }

                    if (!a208)
                    {
                        Checkpoint(parser); // r220

                        bool r220 = true;
                        r220 = r220 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                        if (r220)
                        { // may a221
                            bool a221 = false;
                            {
                                Checkpoint(parser); // r222

                                bool r222 = true;
                                r222 = r222 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r222, parser);
                                a221 = r222;
                            }

                            r220 |= a221;
                        } // end may a221

                        r220 = r220 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r220)
                        { // may a223
                            bool a223 = false;
                            {
                                Checkpoint(parser); // r224

                                bool r224 = true;
                                r224 = r224 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r224, parser);
                                a223 = r224;
                            }

                            r220 |= a223;
                        } // end may a223

                        r220 = r220 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r220, parser);
                        a208 = r220;
                    }

                    if (!a208)
                    {
                        Checkpoint(parser); // r225

                        bool r225 = true;
                        r225 = r225 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                        CommitOrRollback(r225, parser);
                        a208 = r225;
                    }

                    r201 &= a208;

                } // end alternatives a208

                CommitOrRollback(r201, parser);
                res = r201;
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
                Checkpoint(parser); // r226

                bool r226 = true;
                if (r226)
                { // alternatives a227 must
                    bool a227 = false;
                    if (!a227)
                    {
                        Checkpoint(parser); // r228

                        bool r228 = true;
                        if (r228)
                        { // may a229
                            bool a229 = false;
                            {
                                Checkpoint(parser); // r230

                                bool r230 = true;
                                r230 = r230 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                                if (r230)
                                { // may a231
                                    bool a231 = false;
                                    {
                                        Checkpoint(parser); // r232

                                        bool r232 = true;
                                        r232 = r232 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r232, parser);
                                        a231 = r232;
                                    }

                                    r230 |= a231;
                                } // end may a231

                                r230 = r230 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                                if (r230)
                                { // may a233
                                    bool a233 = false;
                                    {
                                        Checkpoint(parser); // r234

                                        bool r234 = true;
                                        r234 = r234 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r234, parser);
                                        a233 = r234;
                                    }

                                    r230 |= a233;
                                } // end may a233

                                CommitOrRollback(r230, parser);
                                a229 = r230;
                            }

                            r228 |= a229;
                        } // end may a229

                        if (r228)
                        { // alternatives a235 must
                            bool a235 = false;
                            if (!a235)
                            {
                                Checkpoint(parser); // r236

                                bool r236 = true;
                                r236 = r236 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                                if (r236)
                                { // may a237
                                    bool a237 = false;
                                    {
                                        Checkpoint(parser); // r238

                                        bool r238 = true;
                                        r238 = r238 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r238, parser);
                                        a237 = r238;
                                    }

                                    r236 |= a237;
                                } // end may a237

                                r236 = r236 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r236)
                                { // may a239
                                    bool a239 = false;
                                    {
                                        Checkpoint(parser); // r240

                                        bool r240 = true;
                                        r240 = r240 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r240, parser);
                                        a239 = r240;
                                    }

                                    r236 |= a239;
                                } // end may a239

                                if (r236)
                                { // may a241
                                    bool a241 = false;
                                    {
                                        Checkpoint(parser); // r242

                                        bool r242 = true;
                                        r242 = r242 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                        if (r242)
                                        { // may a243
                                            bool a243 = false;
                                            {
                                                Checkpoint(parser); // r244

                                                bool r244 = true;
                                                r244 = r244 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                                CommitOrRollback(r244, parser);
                                                a243 = r244;
                                            }

                                            r242 |= a243;
                                        } // end may a243

                                        CommitOrRollback(r242, parser);
                                        a241 = r242;
                                    }

                                    r236 |= a241;
                                } // end may a241

                                r236 = r236 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r236)
                                { // may a245
                                    bool a245 = false;
                                    {
                                        Checkpoint(parser); // r246

                                        bool r246 = true;
                                        r246 = r246 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r246, parser);
                                        a245 = r246;
                                    }

                                    r236 |= a245;
                                } // end may a245

                                r236 = r236 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r236)
                                { // may a247
                                    bool a247 = false;
                                    {
                                        Checkpoint(parser); // r248

                                        bool r248 = true;
                                        r248 = r248 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r248, parser);
                                        a247 = r248;
                                    }

                                    r236 |= a247;
                                } // end may a247

                                r236 = r236 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r236)
                                { // may a249
                                    bool a249 = false;
                                    {
                                        Checkpoint(parser); // r250

                                        bool r250 = true;
                                        r250 = r250 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r250, parser);
                                        a249 = r250;
                                    }

                                    r236 |= a249;
                                } // end may a249

                                if (r236)
                                { // alternatives a251 must
                                    bool a251 = false;
                                    if (!a251)
                                    {
                                        Checkpoint(parser); // r252

                                        bool r252 = true;
                                        r252 = r252 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r252, parser);
                                        a251 = r252;
                                    }

                                    if (!a251)
                                    {
                                        Checkpoint(parser); // r253

                                        bool r253 = true;
                                        r253 = r253 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r253, parser);
                                        a251 = r253;
                                    }

                                    r236 &= a251;

                                } // end alternatives a251

                                CommitOrRollback(r236, parser);
                                a235 = r236;
                            }

                            if (!a235)
                            {
                                Checkpoint(parser); // r254

                                bool r254 = true;
                                r254 = r254 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                if (r254)
                                { // may a255
                                    bool a255 = false;
                                    {
                                        Checkpoint(parser); // r256

                                        bool r256 = true;
                                        r256 = r256 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r256, parser);
                                        a255 = r256;
                                    }

                                    r254 |= a255;
                                } // end may a255

                                r254 = r254 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r254)
                                { // may a257
                                    bool a257 = false;
                                    {
                                        Checkpoint(parser); // r258

                                        bool r258 = true;
                                        r258 = r258 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r258, parser);
                                        a257 = r258;
                                    }

                                    r254 |= a257;
                                } // end may a257

                                r254 = r254 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r254)
                                { // may a259
                                    bool a259 = false;
                                    {
                                        Checkpoint(parser); // r260

                                        bool r260 = true;
                                        r260 = r260 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r260, parser);
                                        a259 = r260;
                                    }

                                    r254 |= a259;
                                } // end may a259

                                r254 = r254 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r254)
                                { // may a261
                                    bool a261 = false;
                                    {
                                        Checkpoint(parser); // r262

                                        bool r262 = true;
                                        r262 = r262 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r262, parser);
                                        a261 = r262;
                                    }

                                    r254 |= a261;
                                } // end may a261

                                if (r254)
                                { // alternatives a263 must
                                    bool a263 = false;
                                    if (!a263)
                                    {
                                        Checkpoint(parser); // r264

                                        bool r264 = true;
                                        r264 = r264 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r264, parser);
                                        a263 = r264;
                                    }

                                    if (!a263)
                                    {
                                        Checkpoint(parser); // r265

                                        bool r265 = true;
                                        r265 = r265 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r265, parser);
                                        a263 = r265;
                                    }

                                    r254 &= a263;

                                } // end alternatives a263

                                CommitOrRollback(r254, parser);
                                a235 = r254;
                            }

                            if (!a235)
                            {
                                Checkpoint(parser); // r266

                                bool r266 = true;
                                r266 = r266 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableName());
                                if (r266)
                                { // may a267
                                    bool a267 = false;
                                    {
                                        Checkpoint(parser); // r268

                                        bool r268 = true;
                                        r268 = r268 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r268, parser);
                                        a267 = r268;
                                    }

                                    r266 |= a267;
                                } // end may a267

                                r266 = r266 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                                if (r266)
                                { // may a269
                                    bool a269 = false;
                                    {
                                        Checkpoint(parser); // r270

                                        bool r270 = true;
                                        r270 = r270 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r270, parser);
                                        a269 = r270;
                                    }

                                    r266 |= a269;
                                } // end may a269

                                if (r266)
                                { // alternatives a271 must
                                    bool a271 = false;
                                    if (!a271)
                                    {
                                        Checkpoint(parser); // r272

                                        bool r272 = true;
                                        r272 = r272 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                        CommitOrRollback(r272, parser);
                                        a271 = r272;
                                    }

                                    if (!a271)
                                    {
                                        Checkpoint(parser); // r273

                                        bool r273 = true;
                                        r273 = r273 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                        CommitOrRollback(r273, parser);
                                        a271 = r273;
                                    }

                                    r266 &= a271;

                                } // end alternatives a271

                                CommitOrRollback(r266, parser);
                                a235 = r266;
                            }

                            r228 &= a235;

                        } // end alternatives a235

                        CommitOrRollback(r228, parser);
                        a227 = r228;
                    }

                    if (!a227)
                    {
                        Checkpoint(parser); // r274

                        bool r274 = true;
                        if (r274)
                        { // alternatives a275 must
                            bool a275 = false;
                            if (!a275)
                            {
                                Checkpoint(parser); // r276

                                bool r276 = true;
                                r276 = r276 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Mul());
                                CommitOrRollback(r276, parser);
                                a275 = r276;
                            }

                            if (!a275)
                            {
                                Checkpoint(parser); // r277

                                bool r277 = true;
                                r277 = r277 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                CommitOrRollback(r277, parser);
                                a275 = r277;
                            }

                            r274 &= a275;

                        } // end alternatives a275

                        CommitOrRollback(r274, parser);
                        a227 = r274;
                    }

                    r226 &= a227;

                } // end alternatives a227

                CommitOrRollback(r226, parser);
                res = r226;
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
                Checkpoint(parser); // r278

                bool r278 = true;
                r278 = r278 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TypeName());
                if (r278)
                { // may a279
                    bool a279 = false;
                    {
                        Checkpoint(parser); // r280

                        bool r280 = true;
                        if (r280)
                        { // may a281
                            bool a281 = false;
                            {
                                Checkpoint(parser); // r282

                                bool r282 = true;
                                r282 = r282 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r282, parser);
                                a281 = r282;
                            }

                            r280 |= a281;
                        } // end may a281

                        if (r280)
                        { // alternatives a283 must
                            bool a283 = false;
                            if (!a283)
                            {
                                Checkpoint(parser); // r284

                                bool r284 = true;
                                r284 = r284 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataTypeScaleAndPrecision());
                                CommitOrRollback(r284, parser);
                                a283 = r284;
                            }

                            if (!a283)
                            {
                                Checkpoint(parser); // r285

                                bool r285 = true;
                                r285 = r285 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataTypeSize());
                                CommitOrRollback(r285, parser);
                                a283 = r285;
                            }

                            r280 &= a283;

                        } // end alternatives a283

                        CommitOrRollback(r280, parser);
                        a279 = r280;
                    }

                    r278 |= a279;
                } // end may a279

                if (r278)
                { // may a286
                    bool a286 = false;
                    {
                        Checkpoint(parser); // r287

                        bool r287 = true;
                        r287 = r287 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r287)
                        { // may a288
                            bool a288 = false;
                            {
                                Checkpoint(parser); // r289

                                bool r289 = true;
                                r289 = r289 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NOT"));
                                r289 = r289 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r289, parser);
                                a288 = r289;
                            }

                            r287 |= a288;
                        } // end may a288

                        r287 = r287 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NULL"));
                        CommitOrRollback(r287, parser);
                        a286 = r287;
                    }

                    r278 |= a286;
                } // end may a286

                CommitOrRollback(r278, parser);
                res = r278;
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
                Checkpoint(parser); // r290

                bool r290 = true;
                r290 = r290 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r290)
                { // may a291
                    bool a291 = false;
                    {
                        Checkpoint(parser); // r292

                        bool r292 = true;
                        r292 = r292 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r292, parser);
                        a291 = r292;
                    }

                    r290 |= a291;
                } // end may a291

                if (r290)
                { // alternatives a293 must
                    bool a293 = false;
                    if (!a293)
                    {
                        Checkpoint(parser); // r294

                        bool r294 = true;
                        r294 = r294 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"MAX"));
                        CommitOrRollback(r294, parser);
                        a293 = r294;
                    }

                    if (!a293)
                    {
                        Checkpoint(parser); // r295

                        bool r295 = true;
                        r295 = r295 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r295, parser);
                        a293 = r295;
                    }

                    r290 &= a293;

                } // end alternatives a293

                if (r290)
                { // may a296
                    bool a296 = false;
                    {
                        Checkpoint(parser); // r297

                        bool r297 = true;
                        r297 = r297 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r297, parser);
                        a296 = r297;
                    }

                    r290 |= a296;
                } // end may a296

                r290 = r290 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r290, parser);
                res = r290;
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
                Checkpoint(parser); // r298

                bool r298 = true;
                r298 = r298 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r298)
                { // may a299
                    bool a299 = false;
                    {
                        Checkpoint(parser); // r300

                        bool r300 = true;
                        r300 = r300 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r300, parser);
                        a299 = r300;
                    }

                    r298 |= a299;
                } // end may a299

                r298 = r298 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                if (r298)
                { // may a301
                    bool a301 = false;
                    {
                        Checkpoint(parser); // r302

                        bool r302 = true;
                        r302 = r302 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r302, parser);
                        a301 = r302;
                    }

                    r298 |= a301;
                } // end may a301

                r298 = r298 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                if (r298)
                { // may a303
                    bool a303 = false;
                    {
                        Checkpoint(parser); // r304

                        bool r304 = true;
                        r304 = r304 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r304, parser);
                        a303 = r304;
                    }

                    r298 |= a303;
                } // end may a303

                r298 = r298 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                if (r298)
                { // may a305
                    bool a305 = false;
                    {
                        Checkpoint(parser); // r306

                        bool r306 = true;
                        r306 = r306 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r306, parser);
                        a305 = r306;
                    }

                    r298 |= a305;
                } // end may a305

                r298 = r298 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r298, parser);
                res = r298;
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
                Checkpoint(parser); // r307

                bool r307 = true;
                if (r307)
                { // alternatives a308 must
                    bool a308 = false;
                    if (!a308)
                    {
                        Checkpoint(parser); // r309

                        bool r309 = true;
                        r309 = r309 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdfIdentifier());
                        CommitOrRollback(r309, parser);
                        a308 = r309;
                    }

                    if (!a308)
                    {
                        Checkpoint(parser); // r310

                        bool r310 = true;
                        r310 = r310 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r310, parser);
                        a308 = r310;
                    }

                    r307 &= a308;

                } // end alternatives a308

                CommitOrRollback(r307, parser);
                res = r307;
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
                Checkpoint(parser); // r311

                bool r311 = true;
                if (r311)
                { // may a312
                    bool a312 = false;
                    {
                        Checkpoint(parser); // r313

                        bool r313 = true;
                        r313 = r313 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatasetName());
                        if (r313)
                        { // may a314
                            bool a314 = false;
                            {
                                Checkpoint(parser); // r315

                                bool r315 = true;
                                r315 = r315 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r315, parser);
                                a314 = r315;
                            }

                            r313 |= a314;
                        } // end may a314

                        r313 = r313 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                        if (r313)
                        { // may a316
                            bool a316 = false;
                            {
                                Checkpoint(parser); // r317

                                bool r317 = true;
                                r317 = r317 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r317, parser);
                                a316 = r317;
                            }

                            r313 |= a316;
                        } // end may a316

                        CommitOrRollback(r313, parser);
                        a312 = r313;
                    }

                    r311 |= a312;
                } // end may a312

                if (r311)
                { // alternatives a318 must
                    bool a318 = false;
                    if (!a318)
                    {
                        Checkpoint(parser); // r319

                        bool r319 = true;
                        r319 = r319 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DatabaseName());
                        if (r319)
                        { // may a320
                            bool a320 = false;
                            {
                                Checkpoint(parser); // r321

                                bool r321 = true;
                                r321 = r321 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r321, parser);
                                a320 = r321;
                            }

                            r319 |= a320;
                        } // end may a320

                        r319 = r319 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r319)
                        { // may a322
                            bool a322 = false;
                            {
                                Checkpoint(parser); // r323

                                bool r323 = true;
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

                                r323 = r323 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                                CommitOrRollback(r323, parser);
                                a322 = r323;
                            }

                            r319 |= a322;
                        } // end may a322

                        if (r319)
                        { // may a326
                            bool a326 = false;
                            {
                                Checkpoint(parser); // r327

                                bool r327 = true;
                                r327 = r327 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r327, parser);
                                a326 = r327;
                            }

                            r319 |= a326;
                        } // end may a326

                        r319 = r319 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r319)
                        { // may a328
                            bool a328 = false;
                            {
                                Checkpoint(parser); // r329

                                bool r329 = true;
                                r329 = r329 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r329, parser);
                                a328 = r329;
                            }

                            r319 |= a328;
                        } // end may a328

                        r319 = r319 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r319, parser);
                        a318 = r319;
                    }

                    if (!a318)
                    {
                        Checkpoint(parser); // r330

                        bool r330 = true;
                        r330 = r330 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SchemaName());
                        if (r330)
                        { // may a331
                            bool a331 = false;
                            {
                                Checkpoint(parser); // r332

                                bool r332 = true;
                                r332 = r332 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r332, parser);
                                a331 = r332;
                            }

                            r330 |= a331;
                        } // end may a331

                        r330 = r330 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                        if (r330)
                        { // may a333
                            bool a333 = false;
                            {
                                Checkpoint(parser); // r334

                                bool r334 = true;
                                r334 = r334 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r334, parser);
                                a333 = r334;
                            }

                            r330 |= a333;
                        } // end may a333

                        r330 = r330 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                        CommitOrRollback(r330, parser);
                        a318 = r330;
                    }

                    r311 &= a318;

                } // end alternatives a318

                CommitOrRollback(r311, parser);
                res = r311;
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
                Checkpoint(parser); // r335

                bool r335 = true;
                r335 = r335 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                if (r335)
                { // may a336
                    bool a336 = false;
                    {
                        Checkpoint(parser); // r337

                        bool r337 = true;
                        r337 = r337 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r337, parser);
                        a336 = r337;
                    }

                    r335 |= a336;
                } // end may a336

                r335 = r335 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Dot());
                if (r335)
                { // may a338
                    bool a338 = false;
                    {
                        Checkpoint(parser); // r339

                        bool r339 = true;
                        r339 = r339 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r339, parser);
                        a338 = r339;
                    }

                    r335 |= a338;
                } // end may a338

                r335 = r335 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
                CommitOrRollback(r335, parser);
                res = r335;
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
                Checkpoint(parser); // r340

                bool r340 = true;
                r340 = r340 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r340, parser);
                res = r340;
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
                Checkpoint(parser); // r341

                bool r341 = true;
                r341 = r341 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Argument());
                if (r341)
                { // may a342
                    bool a342 = false;
                    {
                        Checkpoint(parser); // r343

                        bool r343 = true;
                        if (r343)
                        { // may a344
                            bool a344 = false;
                            {
                                Checkpoint(parser); // r345

                                bool r345 = true;
                                r345 = r345 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r345, parser);
                                a344 = r345;
                            }

                            r343 |= a344;
                        } // end may a344

                        r343 = r343 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r343)
                        { // may a346
                            bool a346 = false;
                            {
                                Checkpoint(parser); // r347

                                bool r347 = true;
                                r347 = r347 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r347, parser);
                                a346 = r347;
                            }

                            r343 |= a346;
                        } // end may a346

                        r343 = r343 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                        CommitOrRollback(r343, parser);
                        a342 = r343;
                    }

                    r341 |= a342;
                } // end may a342

                CommitOrRollback(r341, parser);
                res = r341;
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
                Checkpoint(parser); // r348

                bool r348 = true;
                r348 = r348 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UdtFunctionIdentifier());
                if (r348)
                { // may a349
                    bool a349 = false;
                    {
                        Checkpoint(parser); // r350

                        bool r350 = true;
                        r350 = r350 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r350, parser);
                        a349 = r350;
                    }

                    r348 |= a349;
                } // end may a349

                r348 = r348 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                CommitOrRollback(r348, parser);
                res = r348;
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
                Checkpoint(parser); // r351

                bool r351 = true;
                r351 = r351 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionIdentifier());
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
                Checkpoint(parser); // r357

                bool r357 = true;
                r357 = r357 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionName());
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
                if (r357)
                { // may a360
                    bool a360 = false;
                    {
                        Checkpoint(parser); // r361

                        bool r361 = true;
                        r361 = r361 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r361, parser);
                        a360 = r361;
                    }

                    r357 |= a360;
                } // end may a360

                r357 = r357 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OverClause());
                CommitOrRollback(r357, parser);
                res = r357;
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
                Checkpoint(parser); // r362

                bool r362 = true;
                r362 = r362 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r362)
                { // may a363
                    bool a363 = false;
                    {
                        Checkpoint(parser); // r364

                        bool r364 = true;
                        r364 = r364 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r364, parser);
                        a363 = r364;
                    }

                    r362 |= a363;
                } // end may a363

                if (r362)
                { // may a365
                    bool a365 = false;
                    {
                        Checkpoint(parser); // r366

                        bool r366 = true;
                        r366 = r366 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                        CommitOrRollback(r366, parser);
                        a365 = r366;
                    }

                    r362 |= a365;
                } // end may a365

                if (r362)
                { // may a367
                    bool a367 = false;
                    {
                        Checkpoint(parser); // r368

                        bool r368 = true;
                        r368 = r368 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r368, parser);
                        a367 = r368;
                    }

                    r362 |= a367;
                } // end may a367

                r362 = r362 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r362, parser);
                res = r362;
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
                Checkpoint(parser); // r369

                bool r369 = true;
                r369 = r369 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OVER"));
                if (r369)
                { // may a370
                    bool a370 = false;
                    {
                        Checkpoint(parser); // r371

                        bool r371 = true;
                        r371 = r371 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r371, parser);
                        a370 = r371;
                    }

                    r369 |= a370;
                } // end may a370

                r369 = r369 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r369)
                { // may a372
                    bool a372 = false;
                    {
                        Checkpoint(parser); // r373

                        bool r373 = true;
                        r373 = r373 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r373, parser);
                        a372 = r373;
                    }

                    r369 |= a372;
                } // end may a372

                if (r369)
                { // may a374
                    bool a374 = false;
                    {
                        Checkpoint(parser); // r375

                        bool r375 = true;
                        r375 = r375 && Match(parser, new Jhu.Graywulf.Sql.Parsing.PartitionByClause());
                        if (r375)
                        { // may a376
                            bool a376 = false;
                            {
                                Checkpoint(parser); // r377

                                bool r377 = true;
                                r377 = r377 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r377, parser);
                                a376 = r377;
                            }

                            r375 |= a376;
                        } // end may a376

                        CommitOrRollback(r375, parser);
                        a374 = r375;
                    }

                    r369 |= a374;
                } // end may a374

                if (r369)
                { // may a378
                    bool a378 = false;
                    {
                        Checkpoint(parser); // r379

                        bool r379 = true;
                        r379 = r379 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
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

                    r369 |= a378;
                } // end may a378

                r369 = r369 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r369, parser);
                res = r369;
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
                Checkpoint(parser); // r382

                bool r382 = true;
                r382 = r382 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PARTITION"));
                r382 = r382 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r382 = r382 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
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

                r382 = r382 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ArgumentList());
                CommitOrRollback(r382, parser);
                res = r382;
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
                Checkpoint(parser); // r385

                bool r385 = true;
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

                if (r385)
                { // may a388
                    bool a388 = false;
                    {
                        Checkpoint(parser); // r389

                        bool r389 = true;
                        r389 = r389 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                        CommitOrRollback(r389, parser);
                        a388 = r389;
                    }

                    r385 |= a388;
                } // end may a388

                if (r385)
                { // may a390
                    bool a390 = false;
                    {
                        Checkpoint(parser); // r391

                        bool r391 = true;
                        r391 = r391 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementSeparator());
                        if (r391)
                        { // may a392
                            bool a392 = false;
                            {
                                Checkpoint(parser); // r393

                                bool r393 = true;
                                r393 = r393 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                                CommitOrRollback(r393, parser);
                                a392 = r393;
                            }

                            r391 |= a392;
                        } // end may a392

                        CommitOrRollback(r391, parser);
                        a390 = r391;
                    }

                    r385 |= a390;
                } // end may a390

                if (r385)
                { // may a394
                    bool a394 = false;
                    {
                        Checkpoint(parser); // r395

                        bool r395 = true;
                        r395 = r395 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r395, parser);
                        a394 = r395;
                    }

                    r385 |= a394;
                } // end may a394

                CommitOrRollback(r385, parser);
                res = r385;
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
                Checkpoint(parser); // r396

                bool r396 = true;
                if (r396)
                { // alternatives a397 must
                    bool a397 = false;
                    if (!a397)
                    {
                        Checkpoint(parser); // r398

                        bool r398 = true;
                        if (r398)
                        { // may a399
                            bool a399 = false;
                            {
                                Checkpoint(parser); // r400

                                bool r400 = true;
                                r400 = r400 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r400, parser);
                                a399 = r400;
                            }

                            r398 |= a399;
                        } // end may a399

                        r398 = r398 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Semicolon());
                        if (r398)
                        { // may a401
                            bool a401 = false;
                            {
                                Checkpoint(parser); // r402

                                bool r402 = true;
                                r402 = r402 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r402, parser);
                                a401 = r402;
                            }

                            r398 |= a401;
                        } // end may a401

                        CommitOrRollback(r398, parser);
                        a397 = r398;
                    }

                    if (!a397)
                    {
                        Checkpoint(parser); // r403

                        bool r403 = true;
                        r403 = r403 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r403, parser);
                        a397 = r403;
                    }

                    r396 &= a397;

                } // end alternatives a397

                CommitOrRollback(r396, parser);
                res = r396;
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
                Checkpoint(parser); // r404

                bool r404 = true;
                if (r404)
                { // alternatives a405 must
                    bool a405 = false;
                    if (!a405)
                    {
                        Checkpoint(parser); // r406

                        bool r406 = true;
                        r406 = r406 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Label());
                        CommitOrRollback(r406, parser);
                        a405 = r406;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r407

                        bool r407 = true;
                        r407 = r407 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GotoStatement());
                        CommitOrRollback(r407, parser);
                        a405 = r407;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r408

                        bool r408 = true;
                        r408 = r408 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BeginEndStatement());
                        CommitOrRollback(r408, parser);
                        a405 = r408;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r409

                        bool r409 = true;
                        r409 = r409 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhileStatement());
                        CommitOrRollback(r409, parser);
                        a405 = r409;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r410

                        bool r410 = true;
                        r410 = r410 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BreakStatement());
                        CommitOrRollback(r410, parser);
                        a405 = r410;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r411

                        bool r411 = true;
                        r411 = r411 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ContinueStatement());
                        CommitOrRollback(r411, parser);
                        a405 = r411;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r412

                        bool r412 = true;
                        r412 = r412 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ReturnStatement());
                        CommitOrRollback(r412, parser);
                        a405 = r412;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r413

                        bool r413 = true;
                        r413 = r413 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IfStatement());
                        CommitOrRollback(r413, parser);
                        a405 = r413;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r414

                        bool r414 = true;
                        r414 = r414 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TryCatchStatement());
                        CommitOrRollback(r414, parser);
                        a405 = r414;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r415

                        bool r415 = true;
                        r415 = r415 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ThrowStatement());
                        CommitOrRollback(r415, parser);
                        a405 = r415;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r416

                        bool r416 = true;
                        r416 = r416 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareCursorStatement());
                        CommitOrRollback(r416, parser);
                        a405 = r416;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r417

                        bool r417 = true;
                        r417 = r417 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SetCursorStatement());
                        CommitOrRollback(r417, parser);
                        a405 = r417;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r418

                        bool r418 = true;
                        r418 = r418 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorOperationStatement());
                        CommitOrRollback(r418, parser);
                        a405 = r418;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r419

                        bool r419 = true;
                        r419 = r419 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FetchStatement());
                        CommitOrRollback(r419, parser);
                        a405 = r419;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r420

                        bool r420 = true;
                        r420 = r420 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareVariableStatement());
                        CommitOrRollback(r420, parser);
                        a405 = r420;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r421

                        bool r421 = true;
                        r421 = r421 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SetVariableStatement());
                        CommitOrRollback(r421, parser);
                        a405 = r421;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r422

                        bool r422 = true;
                        r422 = r422 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeclareTableStatement());
                        CommitOrRollback(r422, parser);
                        a405 = r422;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r423

                        bool r423 = true;
                        r423 = r423 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CreateTableStatement());
                        CommitOrRollback(r423, parser);
                        a405 = r423;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r424

                        bool r424 = true;
                        r424 = r424 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DropTableStatement());
                        CommitOrRollback(r424, parser);
                        a405 = r424;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r425

                        bool r425 = true;
                        r425 = r425 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TruncateTableStatement());
                        CommitOrRollback(r425, parser);
                        a405 = r425;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r426

                        bool r426 = true;
                        r426 = r426 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CreateIndexStatement());
                        CommitOrRollback(r426, parser);
                        a405 = r426;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r427

                        bool r427 = true;
                        r427 = r427 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DropIndexStatement());
                        CommitOrRollback(r427, parser);
                        a405 = r427;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r428

                        bool r428 = true;
                        r428 = r428 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectStatement());
                        CommitOrRollback(r428, parser);
                        a405 = r428;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r429

                        bool r429 = true;
                        r429 = r429 && Match(parser, new Jhu.Graywulf.Sql.Parsing.InsertStatement());
                        CommitOrRollback(r429, parser);
                        a405 = r429;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r430

                        bool r430 = true;
                        r430 = r430 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateStatement());
                        CommitOrRollback(r430, parser);
                        a405 = r430;
                    }

                    if (!a405)
                    {
                        Checkpoint(parser); // r431

                        bool r431 = true;
                        r431 = r431 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeleteStatement());
                        CommitOrRollback(r431, parser);
                        a405 = r431;
                    }

                    r404 &= a405;

                } // end alternatives a405

                CommitOrRollback(r404, parser);
                res = r404;
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
                Checkpoint(parser); // r432

                bool r432 = true;
                r432 = r432 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                r432 = r432 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Colon());
                CommitOrRollback(r432, parser);
                res = r432;
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
                Checkpoint(parser); // r433

                bool r433 = true;
                r433 = r433 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"GOTO"));
                r433 = r433 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r433 = r433 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                CommitOrRollback(r433, parser);
                res = r433;
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
                Checkpoint(parser); // r434

                bool r434 = true;
                r434 = r434 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r434 = r434 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                r434 = r434 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                CommitOrRollback(r434, parser);
                res = r434;
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
                Checkpoint(parser); // r435

                bool r435 = true;
                r435 = r435 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHILE"));
                if (r435)
                { // may a436
                    bool a436 = false;
                    {
                        Checkpoint(parser); // r437

                        bool r437 = true;
                        r437 = r437 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r437, parser);
                        a436 = r437;
                    }

                    r435 |= a436;
                } // end may a436

                r435 = r435 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r435)
                { // may a438
                    bool a438 = false;
                    {
                        Checkpoint(parser); // r439

                        bool r439 = true;
                        r439 = r439 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r439, parser);
                        a438 = r439;
                    }

                    r435 |= a438;
                } // end may a438

                r435 = r435 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                CommitOrRollback(r435, parser);
                res = r435;
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
                Checkpoint(parser); // r440

                bool r440 = true;
                r440 = r440 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BREAK"));
                CommitOrRollback(r440, parser);
                res = r440;
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
                Checkpoint(parser); // r441

                bool r441 = true;
                r441 = r441 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CONTINUE"));
                CommitOrRollback(r441, parser);
                res = r441;
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
                Checkpoint(parser); // r442

                bool r442 = true;
                r442 = r442 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RETURN"));
                CommitOrRollback(r442, parser);
                res = r442;
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
                Checkpoint(parser); // r443

                bool r443 = true;
                r443 = r443 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IF"));
                if (r443)
                { // may a444
                    bool a444 = false;
                    {
                        Checkpoint(parser); // r445

                        bool r445 = true;
                        r445 = r445 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r445, parser);
                        a444 = r445;
                    }

                    r443 |= a444;
                } // end may a444

                r443 = r443 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                if (r443)
                { // may a446
                    bool a446 = false;
                    {
                        Checkpoint(parser); // r447

                        bool r447 = true;
                        r447 = r447 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r447, parser);
                        a446 = r447;
                    }

                    r443 |= a446;
                } // end may a446

                r443 = r443 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                if (r443)
                { // may a448
                    bool a448 = false;
                    {
                        Checkpoint(parser); // r449

                        bool r449 = true;
                        r449 = r449 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementSeparator());
                        r449 = r449 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ELSE"));
                        r449 = r449 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r449 = r449 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Statement());
                        CommitOrRollback(r449, parser);
                        a448 = r449;
                    }

                    r443 |= a448;
                } // end may a448

                CommitOrRollback(r443, parser);
                res = r443;
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
                Checkpoint(parser); // r450

                bool r450 = true;
                r450 = r450 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"THROW"));
                if (r450)
                { // may a451
                    bool a451 = false;
                    {
                        Checkpoint(parser); // r452

                        bool r452 = true;
                        r452 = r452 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r452)
                        { // alternatives a453 must
                            bool a453 = false;
                            if (!a453)
                            {
                                Checkpoint(parser); // r454

                                bool r454 = true;
                                r454 = r454 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                CommitOrRollback(r454, parser);
                                a453 = r454;
                            }

                            if (!a453)
                            {
                                Checkpoint(parser); // r455

                                bool r455 = true;
                                r455 = r455 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                                CommitOrRollback(r455, parser);
                                a453 = r455;
                            }

                            r452 &= a453;

                        } // end alternatives a453

                        if (r452)
                        { // may a456
                            bool a456 = false;
                            {
                                Checkpoint(parser); // r457

                                bool r457 = true;
                                r457 = r457 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r457, parser);
                                a456 = r457;
                            }

                            r452 |= a456;
                        } // end may a456

                        r452 = r452 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r452)
                        { // may a458
                            bool a458 = false;
                            {
                                Checkpoint(parser); // r459

                                bool r459 = true;
                                r459 = r459 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r459, parser);
                                a458 = r459;
                            }

                            r452 |= a458;
                        } // end may a458

                        if (r452)
                        { // alternatives a460 must
                            bool a460 = false;
                            if (!a460)
                            {
                                Checkpoint(parser); // r461

                                bool r461 = true;
                                r461 = r461 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StringConstant());
                                CommitOrRollback(r461, parser);
                                a460 = r461;
                            }

                            if (!a460)
                            {
                                Checkpoint(parser); // r462

                                bool r462 = true;
                                r462 = r462 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                                CommitOrRollback(r462, parser);
                                a460 = r462;
                            }

                            r452 &= a460;

                        } // end alternatives a460

                        if (r452)
                        { // may a463
                            bool a463 = false;
                            {
                                Checkpoint(parser); // r464

                                bool r464 = true;
                                r464 = r464 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r464, parser);
                                a463 = r464;
                            }

                            r452 |= a463;
                        } // end may a463

                        r452 = r452 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r452)
                        { // may a465
                            bool a465 = false;
                            {
                                Checkpoint(parser); // r466

                                bool r466 = true;
                                r466 = r466 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r466, parser);
                                a465 = r466;
                            }

                            r452 |= a465;
                        } // end may a465

                        if (r452)
                        { // alternatives a467 must
                            bool a467 = false;
                            if (!a467)
                            {
                                Checkpoint(parser); // r468

                                bool r468 = true;
                                r468 = r468 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                CommitOrRollback(r468, parser);
                                a467 = r468;
                            }

                            if (!a467)
                            {
                                Checkpoint(parser); // r469

                                bool r469 = true;
                                r469 = r469 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                                CommitOrRollback(r469, parser);
                                a467 = r469;
                            }

                            r452 &= a467;

                        } // end alternatives a467

                        if (r452)
                        { // may a470
                            bool a470 = false;
                            {
                                Checkpoint(parser); // r471

                                bool r471 = true;
                                r471 = r471 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r471, parser);
                                a470 = r471;
                            }

                            r452 |= a470;
                        } // end may a470

                        CommitOrRollback(r452, parser);
                        a451 = r452;
                    }

                    r450 |= a451;
                } // end may a451

                CommitOrRollback(r450, parser);
                res = r450;
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
                Checkpoint(parser); // r472

                bool r472 = true;
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRY"));
                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRY"));
                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BEGIN"));
                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CATCH"));
                if (r472)
                { // alternatives a473 must
                    bool a473 = false;
                    if (!a473)
                    {
                        Checkpoint(parser); // r474

                        bool r474 = true;
                        r474 = r474 && Match(parser, new Jhu.Graywulf.Sql.Parsing.StatementBlock());
                        CommitOrRollback(r474, parser);
                        a473 = r474;
                    }

                    if (!a473)
                    {
                        Checkpoint(parser); // r475

                        bool r475 = true;
                        r475 = r475 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r475, parser);
                        a473 = r475;
                    }

                    r472 &= a473;

                } // end alternatives a473

                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"END"));
                r472 = r472 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r472 = r472 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CATCH"));
                CommitOrRollback(r472, parser);
                res = r472;
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
                Checkpoint(parser); // r476

                bool r476 = true;
                if (r476)
                { // may a477
                    bool a477 = false;
                    {
                        Checkpoint(parser); // r478

                        bool r478 = true;
                        r478 = r478 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r478, parser);
                        a477 = r478;
                    }

                    r476 |= a477;
                } // end may a477

                r476 = r476 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                if (r476)
                { // may a479
                    bool a479 = false;
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

                        r480 = r480 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        r480 = r480 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableList());
                        CommitOrRollback(r480, parser);
                        a479 = r480;
                    }

                    r476 |= a479;
                } // end may a479

                CommitOrRollback(r476, parser);
                res = r476;
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
                Checkpoint(parser); // r483

                bool r483 = true;
                r483 = r483 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r483 = r483 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r483 = r483 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList());
                CommitOrRollback(r483, parser);
                res = r483;
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
                Checkpoint(parser); // r484

                bool r484 = true;
                r484 = r484 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclaration());
                if (r484)
                { // may a485
                    bool a485 = false;
                    {
                        Checkpoint(parser); // r486

                        bool r486 = true;
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

                        r486 = r486 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r486)
                        { // may a489
                            bool a489 = false;
                            {
                                Checkpoint(parser); // r490

                                bool r490 = true;
                                r490 = r490 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r490, parser);
                                a489 = r490;
                            }

                            r486 |= a489;
                        } // end may a489

                        r486 = r486 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableDeclarationList());
                        CommitOrRollback(r486, parser);
                        a485 = r486;
                    }

                    r484 |= a485;
                } // end may a485

                CommitOrRollback(r484, parser);
                res = r484;
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
                Checkpoint(parser); // r491

                bool r491 = true;
                r491 = r491 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                if (r491)
                { // may a492
                    bool a492 = false;
                    {
                        Checkpoint(parser); // r493

                        bool r493 = true;
                        r493 = r493 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r493 = r493 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        CommitOrRollback(r493, parser);
                        a492 = r493;
                    }

                    r491 |= a492;
                } // end may a492

                r491 = r491 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r491)
                { // alternatives a494 must
                    bool a494 = false;
                    if (!a494)
                    {
                        Checkpoint(parser); // r495

                        bool r495 = true;
                        r495 = r495 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CURSOR"));
                        CommitOrRollback(r495, parser);
                        a494 = r495;
                    }

                    if (!a494)
                    {
                        Checkpoint(parser); // r496

                        bool r496 = true;
                        r496 = r496 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataType());
                        if (r496)
                        { // may a497
                            bool a497 = false;
                            {
                                Checkpoint(parser); // r498

                                bool r498 = true;
                                if (r498)
                                { // may a499
                                    bool a499 = false;
                                    {
                                        Checkpoint(parser); // r500

                                        bool r500 = true;
                                        r500 = r500 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r500, parser);
                                        a499 = r500;
                                    }

                                    r498 |= a499;
                                } // end may a499

                                r498 = r498 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                                if (r498)
                                { // may a501
                                    bool a501 = false;
                                    {
                                        Checkpoint(parser); // r502

                                        bool r502 = true;
                                        r502 = r502 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r502, parser);
                                        a501 = r502;
                                    }

                                    r498 |= a501;
                                } // end may a501

                                r498 = r498 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                                CommitOrRollback(r498, parser);
                                a497 = r498;
                            }

                            r496 |= a497;
                        } // end may a497

                        CommitOrRollback(r496, parser);
                        a494 = r496;
                    }

                    r491 &= a494;

                } // end alternatives a494

                CommitOrRollback(r491, parser);
                res = r491;
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
                Checkpoint(parser); // r503

                bool r503 = true;
                r503 = r503 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                r503 = r503 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r503 = r503 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
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

                r503 = r503 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator());
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
                res = r503;
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
                Checkpoint(parser); // r508

                bool r508 = true;
                if (r508)
                { // alternatives a509 must
                    bool a509 = false;
                    if (!a509)
                    {
                        Checkpoint(parser); // r510

                        bool r510 = true;
                        r510 = r510 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        CommitOrRollback(r510, parser);
                        a509 = r510;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r511

                        bool r511 = true;
                        r511 = r511 && Match(parser, new Jhu.Graywulf.Sql.Parsing.PlusEquals());
                        CommitOrRollback(r511, parser);
                        a509 = r511;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r512

                        bool r512 = true;
                        r512 = r512 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MinusEquals());
                        CommitOrRollback(r512, parser);
                        a509 = r512;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r513

                        bool r513 = true;
                        r513 = r513 && Match(parser, new Jhu.Graywulf.Sql.Parsing.MulEquals());
                        CommitOrRollback(r513, parser);
                        a509 = r513;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r514

                        bool r514 = true;
                        r514 = r514 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DivEquals());
                        CommitOrRollback(r514, parser);
                        a509 = r514;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r515

                        bool r515 = true;
                        r515 = r515 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ModEquals());
                        CommitOrRollback(r515, parser);
                        a509 = r515;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r516

                        bool r516 = true;
                        r516 = r516 && Match(parser, new Jhu.Graywulf.Sql.Parsing.AndEquals());
                        CommitOrRollback(r516, parser);
                        a509 = r516;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r517

                        bool r517 = true;
                        r517 = r517 && Match(parser, new Jhu.Graywulf.Sql.Parsing.XorEquals());
                        CommitOrRollback(r517, parser);
                        a509 = r517;
                    }

                    if (!a509)
                    {
                        Checkpoint(parser); // r518

                        bool r518 = true;
                        r518 = r518 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrEquals());
                        CommitOrRollback(r518, parser);
                        a509 = r518;
                    }

                    r508 &= a509;

                } // end alternatives a509

                CommitOrRollback(r508, parser);
                res = r508;
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
                Checkpoint(parser); // r519

                bool r519 = true;
                r519 = r519 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r519 = r519 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r519)
                { // alternatives a520 must
                    bool a520 = false;
                    if (!a520)
                    {
                        Checkpoint(parser); // r521

                        bool r521 = true;
                        r521 = r521 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r521, parser);
                        a520 = r521;
                    }

                    if (!a520)
                    {
                        Checkpoint(parser); // r522

                        bool r522 = true;
                        r522 = r522 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                        CommitOrRollback(r522, parser);
                        a520 = r522;
                    }

                    r519 &= a520;

                } // end alternatives a520

                r519 = r519 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r519 = r519 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorDefinition());
                CommitOrRollback(r519, parser);
                res = r519;
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
                Checkpoint(parser); // r523

                bool r523 = true;
                r523 = r523 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                r523 = r523 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r523 = r523 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                if (r523)
                { // may a524
                    bool a524 = false;
                    {
                        Checkpoint(parser); // r525

                        bool r525 = true;
                        r525 = r525 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r525, parser);
                        a524 = r525;
                    }

                    r523 |= a524;
                } // end may a524

                r523 = r523 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                if (r523)
                { // may a526
                    bool a526 = false;
                    {
                        Checkpoint(parser); // r527

                        bool r527 = true;
                        r527 = r527 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r527, parser);
                        a526 = r527;
                    }

                    r523 |= a526;
                } // end may a526

                r523 = r523 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CursorDefinition());
                CommitOrRollback(r523, parser);
                res = r523;
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
                Checkpoint(parser); // r528

                bool r528 = true;
                r528 = r528 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CURSOR"));
                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r528 = r528 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FOR"));
                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r528 = r528 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectStatement());
                CommitOrRollback(r528, parser);
                res = r528;
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
                Checkpoint(parser); // r529

                bool r529 = true;
                if (r529)
                { // alternatives a530 must
                    bool a530 = false;
                    if (!a530)
                    {
                        Checkpoint(parser); // r531

                        bool r531 = true;
                        r531 = r531 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OPEN"));
                        CommitOrRollback(r531, parser);
                        a530 = r531;
                    }

                    if (!a530)
                    {
                        Checkpoint(parser); // r532

                        bool r532 = true;
                        r532 = r532 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLOSE"));
                        CommitOrRollback(r532, parser);
                        a530 = r532;
                    }

                    if (!a530)
                    {
                        Checkpoint(parser); // r533

                        bool r533 = true;
                        r533 = r533 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEALLOCATE"));
                        CommitOrRollback(r533, parser);
                        a530 = r533;
                    }

                    r529 &= a530;

                } // end alternatives a530

                r529 = r529 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r529)
                { // alternatives a534 must
                    bool a534 = false;
                    if (!a534)
                    {
                        Checkpoint(parser); // r535

                        bool r535 = true;
                        r535 = r535 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r535, parser);
                        a534 = r535;
                    }

                    if (!a534)
                    {
                        Checkpoint(parser); // r536

                        bool r536 = true;
                        r536 = r536 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                        CommitOrRollback(r536, parser);
                        a534 = r536;
                    }

                    r529 &= a534;

                } // end alternatives a534

                CommitOrRollback(r529, parser);
                res = r529;
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
                Checkpoint(parser); // r537

                bool r537 = true;
                r537 = r537 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FETCH"));
                r537 = r537 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r537)
                { // may a538
                    bool a538 = false;
                    {
                        Checkpoint(parser); // r539

                        bool r539 = true;
                        if (r539)
                        { // alternatives a540 must
                            bool a540 = false;
                            if (!a540)
                            {
                                Checkpoint(parser); // r541

                                bool r541 = true;
                                r541 = r541 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NEXT"));
                                CommitOrRollback(r541, parser);
                                a540 = r541;
                            }

                            if (!a540)
                            {
                                Checkpoint(parser); // r542

                                bool r542 = true;
                                r542 = r542 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PRIOR"));
                                CommitOrRollback(r542, parser);
                                a540 = r542;
                            }

                            if (!a540)
                            {
                                Checkpoint(parser); // r543

                                bool r543 = true;
                                r543 = r543 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FIRST"));
                                CommitOrRollback(r543, parser);
                                a540 = r543;
                            }

                            if (!a540)
                            {
                                Checkpoint(parser); // r544

                                bool r544 = true;
                                r544 = r544 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LAST"));
                                CommitOrRollback(r544, parser);
                                a540 = r544;
                            }

                            if (!a540)
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
                                        r547 = r547 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ABSOLUTE"));
                                        CommitOrRollback(r547, parser);
                                        a546 = r547;
                                    }

                                    if (!a546)
                                    {
                                        Checkpoint(parser); // r548

                                        bool r548 = true;
                                        r548 = r548 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RELATIVE"));
                                        CommitOrRollback(r548, parser);
                                        a546 = r548;
                                    }

                                    r545 &= a546;

                                } // end alternatives a546

                                r545 = r545 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                if (r545)
                                { // alternatives a549 must
                                    bool a549 = false;
                                    if (!a549)
                                    {
                                        Checkpoint(parser); // r550

                                        bool r550 = true;
                                        r550 = r550 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                                        CommitOrRollback(r550, parser);
                                        a549 = r550;
                                    }

                                    if (!a549)
                                    {
                                        Checkpoint(parser); // r551

                                        bool r551 = true;
                                        r551 = r551 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                                        CommitOrRollback(r551, parser);
                                        a549 = r551;
                                    }

                                    r545 &= a549;

                                } // end alternatives a549

                                CommitOrRollback(r545, parser);
                                a540 = r545;
                            }

                            r539 &= a540;

                        } // end alternatives a540

                        CommitOrRollback(r539, parser);
                        a538 = r539;
                    }

                    r537 |= a538;
                } // end may a538

                r537 = r537 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r537 = r537 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                r537 = r537 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                if (r537)
                { // alternatives a552 must
                    bool a552 = false;
                    if (!a552)
                    {
                        Checkpoint(parser); // r553

                        bool r553 = true;
                        r553 = r553 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Cursor());
                        CommitOrRollback(r553, parser);
                        a552 = r553;
                    }

                    if (!a552)
                    {
                        Checkpoint(parser); // r554

                        bool r554 = true;
                        r554 = r554 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                        CommitOrRollback(r554, parser);
                        a552 = r554;
                    }

                    r537 &= a552;

                } // end alternatives a552

                if (r537)
                { // may a555
                    bool a555 = false;
                    {
                        Checkpoint(parser); // r556

                        bool r556 = true;
                        r556 = r556 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r556 = r556 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTO"));
                        r556 = r556 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r556 = r556 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableList());
                        CommitOrRollback(r556, parser);
                        a555 = r556;
                    }

                    r537 |= a555;
                } // end may a555

                CommitOrRollback(r537, parser);
                res = r537;
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
                Checkpoint(parser); // r557

                bool r557 = true;
                r557 = r557 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DECLARE"));
                r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r557 = r557 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r557)
                { // may a558
                    bool a558 = false;
                    {
                        Checkpoint(parser); // r559

                        bool r559 = true;
                        r559 = r559 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r559, parser);
                        a558 = r559;
                    }

                    r557 |= a558;
                } // end may a558

                r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r557)
                { // may a560
                    bool a560 = false;
                    {
                        Checkpoint(parser); // r561

                        bool r561 = true;
                        r561 = r561 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r561, parser);
                        a560 = r561;
                    }

                    r557 |= a560;
                } // end may a560

                r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                if (r557)
                { // may a562
                    bool a562 = false;
                    {
                        Checkpoint(parser); // r563

                        bool r563 = true;
                        r563 = r563 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r563, parser);
                        a562 = r563;
                    }

                    r557 |= a562;
                } // end may a562

                r557 = r557 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r557, parser);
                res = r557;
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
                Checkpoint(parser); // r564

                bool r564 = true;
                r564 = r564 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
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

                r564 = r564 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList());
                CommitOrRollback(r564, parser);
                res = r564;
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
                Checkpoint(parser); // r567

                bool r567 = true;
                r567 = r567 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecification());
                if (r567)
                { // may a568
                    bool a568 = false;
                    {
                        Checkpoint(parser); // r569

                        bool r569 = true;
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

                        r569 = r569 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r569)
                        { // may a572
                            bool a572 = false;
                            {
                                Checkpoint(parser); // r573

                                bool r573 = true;
                                r573 = r573 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r573, parser);
                                a572 = r573;
                            }

                            r569 |= a572;
                        } // end may a572

                        r569 = r569 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableSpecificationList());
                        CommitOrRollback(r569, parser);
                        a568 = r569;
                    }

                    r567 |= a568;
                } // end may a568

                CommitOrRollback(r567, parser);
                res = r567;
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
                Checkpoint(parser); // r574

                bool r574 = true;
                r574 = r574 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
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

                        r576 = r576 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets());
                        CommitOrRollback(r576, parser);
                        a575 = r576;
                    }

                    r574 |= a575;
                } // end may a575

                if (r574)
                { // may a579
                    bool a579 = false;
                    {
                        Checkpoint(parser); // r580

                        bool r580 = true;
                        r580 = r580 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r580, parser);
                        a579 = r580;
                    }

                    r574 |= a579;
                } // end may a579

                r574 = r574 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                if (r574)
                { // may a581
                    bool a581 = false;
                    {
                        Checkpoint(parser); // r582

                        bool r582 = true;
                        r582 = r582 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r582, parser);
                        a581 = r582;
                    }

                    r574 |= a581;
                } // end may a581

                r574 = r574 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
                CommitOrRollback(r574, parser);
                res = r574;
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
                Checkpoint(parser); // r583

                bool r583 = true;
                if (r583)
                { // may a584
                    bool a584 = false;
                    {
                        Checkpoint(parser); // r585

                        bool r585 = true;
                        r585 = r585 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r585)
                        { // may a586
                            bool a586 = false;
                            {
                                Checkpoint(parser); // r587

                                bool r587 = true;
                                r587 = r587 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r587, parser);
                                a586 = r587;
                            }

                            r585 |= a586;
                        } // end may a586

                        CommitOrRollback(r585, parser);
                        a584 = r585;
                    }

                    r583 |= a584;
                } // end may a584

                r583 = r583 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r583)
                { // may a588
                    bool a588 = false;
                    {
                        Checkpoint(parser); // r589

                        bool r589 = true;
                        if (r589)
                        { // may a590
                            bool a590 = false;
                            {
                                Checkpoint(parser); // r591

                                bool r591 = true;
                                r591 = r591 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r591, parser);
                                a590 = r591;
                            }

                            r589 |= a590;
                        } // end may a590

                        r589 = r589 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        CommitOrRollback(r589, parser);
                        a588 = r589;
                    }

                    r583 |= a588;
                } // end may a588

                if (r583)
                { // may a592
                    bool a592 = false;
                    {
                        Checkpoint(parser); // r593

                        bool r593 = true;
                        if (r593)
                        { // may a594
                            bool a594 = false;
                            {
                                Checkpoint(parser); // r595

                                bool r595 = true;
                                r595 = r595 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r595, parser);
                                a594 = r595;
                            }

                            r593 |= a594;
                        } // end may a594

                        r593 = r593 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r593, parser);
                        a592 = r593;
                    }

                    r583 |= a592;
                } // end may a592

                CommitOrRollback(r583, parser);
                res = r583;
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
                Checkpoint(parser); // r596

                bool r596 = true;
                r596 = r596 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
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

                r596 = r596 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                if (r596)
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

                        r600 = r600 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                        CommitOrRollback(r600, parser);
                        a599 = r600;
                    }

                    r596 |= a599;
                } // end may a599

                if (r596)
                { // may a603
                    bool a603 = false;
                    {
                        Checkpoint(parser); // r604

                        bool r604 = true;
                        r604 = r604 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r604, parser);
                        a603 = r604;
                    }

                    r596 |= a603;
                } // end may a603

                r596 = r596 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r596, parser);
                res = r596;
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
                Checkpoint(parser); // r605

                bool r605 = true;
                if (r605)
                { // alternatives a606 must
                    bool a606 = false;
                    if (!a606)
                    {
                        Checkpoint(parser); // r607

                        bool r607 = true;
                        r607 = r607 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpressionBrackets());
                        CommitOrRollback(r607, parser);
                        a606 = r607;
                    }

                    if (!a606)
                    {
                        Checkpoint(parser); // r608

                        bool r608 = true;
                        r608 = r608 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QuerySpecification());
                        CommitOrRollback(r608, parser);
                        a606 = r608;
                    }

                    r605 &= a606;

                } // end alternatives a606

                if (r605)
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
                                r612 = r612 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r612, parser);
                                a611 = r612;
                            }

                            r610 |= a611;
                        } // end may a611

                        r610 = r610 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryOperator());
                        if (r610)
                        { // may a613
                            bool a613 = false;
                            {
                                Checkpoint(parser); // r614

                                bool r614 = true;
                                r614 = r614 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r614, parser);
                                a613 = r614;
                            }

                            r610 |= a613;
                        } // end may a613

                        r610 = r610 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                        CommitOrRollback(r610, parser);
                        a609 = r610;
                    }

                    r605 |= a609;
                } // end may a609

                CommitOrRollback(r605, parser);
                res = r605;
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
                Checkpoint(parser); // r615

                bool r615 = true;
                r615 = r615 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
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

                r615 = r615 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
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

                r615 = r615 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r615, parser);
                res = r615;
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
                Checkpoint(parser); // r620

                bool r620 = true;
                if (r620)
                { // alternatives a621 must
                    bool a621 = false;
                    if (!a621)
                    {
                        Checkpoint(parser); // r622

                        bool r622 = true;
                        r622 = r622 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNION"));
                        if (r622)
                        { // may a623
                            bool a623 = false;
                            {
                                Checkpoint(parser); // r624

                                bool r624 = true;
                                r624 = r624 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r624 = r624 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r624, parser);
                                a623 = r624;
                            }

                            r622 |= a623;
                        } // end may a623

                        CommitOrRollback(r622, parser);
                        a621 = r622;
                    }

                    if (!a621)
                    {
                        Checkpoint(parser); // r625

                        bool r625 = true;
                        r625 = r625 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"EXCEPT"));
                        CommitOrRollback(r625, parser);
                        a621 = r625;
                    }

                    if (!a621)
                    {
                        Checkpoint(parser); // r626

                        bool r626 = true;
                        r626 = r626 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTERSECT"));
                        CommitOrRollback(r626, parser);
                        a621 = r626;
                    }

                    r620 &= a621;

                } // end alternatives a621

                CommitOrRollback(r620, parser);
                res = r620;
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
                Checkpoint(parser); // r627

                bool r627 = true;
                r627 = r627 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SELECT"));
                if (r627)
                { // may a628
                    bool a628 = false;
                    {
                        Checkpoint(parser); // r629

                        bool r629 = true;
                        r629 = r629 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r629)
                        { // alternatives a630 must
                            bool a630 = false;
                            if (!a630)
                            {
                                Checkpoint(parser); // r631

                                bool r631 = true;
                                r631 = r631 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                                CommitOrRollback(r631, parser);
                                a630 = r631;
                            }

                            if (!a630)
                            {
                                Checkpoint(parser); // r632

                                bool r632 = true;
                                r632 = r632 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DISTINCT"));
                                CommitOrRollback(r632, parser);
                                a630 = r632;
                            }

                            r629 &= a630;

                        } // end alternatives a630

                        CommitOrRollback(r629, parser);
                        a628 = r629;
                    }

                    r627 |= a628;
                } // end may a628

                if (r627)
                { // may a633
                    bool a633 = false;
                    {
                        Checkpoint(parser); // r634

                        bool r634 = true;
                        r634 = r634 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r634 = r634 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TopExpression());
                        CommitOrRollback(r634, parser);
                        a633 = r634;
                    }

                    r627 |= a633;
                } // end may a633

                if (r627)
                { // may a635
                    bool a635 = false;
                    {
                        Checkpoint(parser); // r636

                        bool r636 = true;
                        r636 = r636 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r636, parser);
                        a635 = r636;
                    }

                    r627 |= a635;
                } // end may a635

                r627 = r627 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectList());
                if (r627)
                { // may a637
                    bool a637 = false;
                    {
                        Checkpoint(parser); // r638

                        bool r638 = true;
                        if (r638)
                        { // may a639
                            bool a639 = false;
                            {
                                Checkpoint(parser); // r640

                                bool r640 = true;
                                r640 = r640 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r640, parser);
                                a639 = r640;
                            }

                            r638 |= a639;
                        } // end may a639

                        r638 = r638 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IntoClause());
                        CommitOrRollback(r638, parser);
                        a637 = r638;
                    }

                    r627 |= a637;
                } // end may a637

                if (r627)
                { // may a641
                    bool a641 = false;
                    {
                        Checkpoint(parser); // r642

                        bool r642 = true;
                        if (r642)
                        { // may a643
                            bool a643 = false;
                            {
                                Checkpoint(parser); // r644

                                bool r644 = true;
                                r644 = r644 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r644, parser);
                                a643 = r644;
                            }

                            r642 |= a643;
                        } // end may a643

                        r642 = r642 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r642, parser);
                        a641 = r642;
                    }

                    r627 |= a641;
                } // end may a641

                if (r627)
                { // may a645
                    bool a645 = false;
                    {
                        Checkpoint(parser); // r646

                        bool r646 = true;
                        if (r646)
                        { // may a647
                            bool a647 = false;
                            {
                                Checkpoint(parser); // r648

                                bool r648 = true;
                                r648 = r648 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r648, parser);
                                a647 = r648;
                            }

                            r646 |= a647;
                        } // end may a647

                        r646 = r646 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r646, parser);
                        a645 = r646;
                    }

                    r627 |= a645;
                } // end may a645

                if (r627)
                { // may a649
                    bool a649 = false;
                    {
                        Checkpoint(parser); // r650

                        bool r650 = true;
                        if (r650)
                        { // may a651
                            bool a651 = false;
                            {
                                Checkpoint(parser); // r652

                                bool r652 = true;
                                r652 = r652 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r652, parser);
                                a651 = r652;
                            }

                            r650 |= a651;
                        } // end may a651

                        r650 = r650 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByClause());
                        CommitOrRollback(r650, parser);
                        a649 = r650;
                    }

                    r627 |= a649;
                } // end may a649

                if (r627)
                { // may a653
                    bool a653 = false;
                    {
                        Checkpoint(parser); // r654

                        bool r654 = true;
                        if (r654)
                        { // may a655
                            bool a655 = false;
                            {
                                Checkpoint(parser); // r656

                                bool r656 = true;
                                r656 = r656 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r656, parser);
                                a655 = r656;
                            }

                            r654 |= a655;
                        } // end may a655

                        r654 = r654 && Match(parser, new Jhu.Graywulf.Sql.Parsing.HavingClause());
                        CommitOrRollback(r654, parser);
                        a653 = r654;
                    }

                    r627 |= a653;
                } // end may a653

                CommitOrRollback(r627, parser);
                res = r627;
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
                Checkpoint(parser); // r657

                bool r657 = true;
                r657 = r657 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TOP"));
                r657 = r657 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r657 = r657 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r657)
                { // may a658
                    bool a658 = false;
                    {
                        Checkpoint(parser); // r659

                        bool r659 = true;
                        r659 = r659 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r659 = r659 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PERCENT"));
                        CommitOrRollback(r659, parser);
                        a658 = r659;
                    }

                    r657 |= a658;
                } // end may a658

                if (r657)
                { // may a660
                    bool a660 = false;
                    {
                        Checkpoint(parser); // r661

                        bool r661 = true;
                        r661 = r661 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r661 = r661 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                        r661 = r661 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r661 = r661 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TIES"));
                        CommitOrRollback(r661, parser);
                        a660 = r661;
                    }

                    r657 |= a660;
                } // end may a660

                CommitOrRollback(r657, parser);
                res = r657;
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
                Checkpoint(parser); // r662

                bool r662 = true;
                r662 = r662 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnExpression());
                if (r662)
                { // may a663
                    bool a663 = false;
                    {
                        Checkpoint(parser); // r664

                        bool r664 = true;
                        if (r664)
                        { // may a665
                            bool a665 = false;
                            {
                                Checkpoint(parser); // r666

                                bool r666 = true;
                                r666 = r666 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r666, parser);
                                a665 = r666;
                            }

                            r664 |= a665;
                        } // end may a665

                        r664 = r664 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r664)
                        { // may a667
                            bool a667 = false;
                            {
                                Checkpoint(parser); // r668

                                bool r668 = true;
                                r668 = r668 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r668, parser);
                                a667 = r668;
                            }

                            r664 |= a667;
                        } // end may a667

                        r664 = r664 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SelectList());
                        CommitOrRollback(r664, parser);
                        a663 = r664;
                    }

                    r662 |= a663;
                } // end may a663

                CommitOrRollback(r662, parser);
                res = r662;
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
                Checkpoint(parser); // r669

                bool r669 = true;
                if (r669)
                { // alternatives a670 must
                    bool a670 = false;
                    if (!a670)
                    {
                        Checkpoint(parser); // r671

                        bool r671 = true;
                        r671 = r671 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
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

                        r671 = r671 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
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

                        r671 = r671 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r671, parser);
                        a670 = r671;
                    }

                    if (!a670)
                    {
                        Checkpoint(parser); // r676

                        bool r676 = true;
                        r676 = r676 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
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
                        a670 = r676;
                    }

                    if (!a670)
                    {
                        Checkpoint(parser); // r681

                        bool r681 = true;
                        r681 = r681 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        if (r681)
                        { // may a682
                            bool a682 = false;
                            {
                                Checkpoint(parser); // r683

                                bool r683 = true;
                                if (r683)
                                { // may a684
                                    bool a684 = false;
                                    {
                                        Checkpoint(parser); // r685

                                        bool r685 = true;
                                        r685 = r685 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        r685 = r685 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                        CommitOrRollback(r685, parser);
                                        a684 = r685;
                                    }

                                    r683 |= a684;
                                } // end may a684

                                r683 = r683 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                r683 = r683 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                                CommitOrRollback(r683, parser);
                                a682 = r683;
                            }

                            r681 |= a682;
                        } // end may a682

                        CommitOrRollback(r681, parser);
                        a670 = r681;
                    }

                    r669 &= a670;

                } // end alternatives a670

                CommitOrRollback(r669, parser);
                res = r669;
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
                Checkpoint(parser); // r686

                bool r686 = true;
                r686 = r686 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INTO"));
                if (r686)
                { // may a687
                    bool a687 = false;
                    {
                        Checkpoint(parser); // r688

                        bool r688 = true;
                        r688 = r688 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r688, parser);
                        a687 = r688;
                    }

                    r686 |= a687;
                } // end may a687

                r686 = r686 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                CommitOrRollback(r686, parser);
                res = r686;
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
                Checkpoint(parser); // r689

                bool r689 = true;
                if (r689)
                { // alternatives a690 must
                    bool a690 = false;
                    if (!a690)
                    {
                        Checkpoint(parser); // r691

                        bool r691 = true;
                        r691 = r691 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                        CommitOrRollback(r691, parser);
                        a690 = r691;
                    }

                    if (!a690)
                    {
                        Checkpoint(parser); // r692

                        bool r692 = true;
                        r692 = r692 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                        CommitOrRollback(r692, parser);
                        a690 = r692;
                    }

                    r689 &= a690;

                } // end alternatives a690

                if (r689)
                { // may a693
                    bool a693 = false;
                    {
                        Checkpoint(parser); // r694

                        bool r694 = true;
                        if (r694)
                        { // may a695
                            bool a695 = false;
                            {
                                Checkpoint(parser); // r696

                                bool r696 = true;
                                r696 = r696 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r696, parser);
                                a695 = r696;
                            }

                            r694 |= a695;
                        } // end may a695

                        r694 = r694 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintClause());
                        CommitOrRollback(r694, parser);
                        a693 = r694;
                    }

                    r689 |= a693;
                } // end may a693

                CommitOrRollback(r689, parser);
                res = r689;
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
                Checkpoint(parser); // r697

                bool r697 = true;
                r697 = r697 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                if (r697)
                { // may a698
                    bool a698 = false;
                    {
                        Checkpoint(parser); // r699

                        bool r699 = true;
                        r699 = r699 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r699, parser);
                        a698 = r699;
                    }

                    r697 |= a698;
                } // end may a698

                r697 = r697 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSourceExpression());
                CommitOrRollback(r697, parser);
                res = r697;
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
                Checkpoint(parser); // r700

                bool r700 = true;
                r700 = r700 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                if (r700)
                { // may a701
                    bool a701 = false;
                    {
                        Checkpoint(parser); // r702

                        bool r702 = true;
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

                        r702 = r702 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinedTable());
                        CommitOrRollback(r702, parser);
                        a701 = r702;
                    }

                    r700 |= a701;
                } // end may a701

                CommitOrRollback(r700, parser);
                res = r700;
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
                Checkpoint(parser); // r705

                bool r705 = true;
                if (r705)
                { // alternatives a706 must
                    bool a706 = false;
                    if (!a706)
                    {
                        Checkpoint(parser); // r707

                        bool r707 = true;
                        r707 = r707 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinType());
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

                        r707 = r707 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        if (r707)
                        { // may a710
                            bool a710 = false;
                            {
                                Checkpoint(parser); // r711

                                bool r711 = true;
                                r711 = r711 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r711, parser);
                                a710 = r711;
                            }

                            r707 |= a710;
                        } // end may a710

                        r707 = r707 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                        if (r707)
                        { // may a712
                            bool a712 = false;
                            {
                                Checkpoint(parser); // r713

                                bool r713 = true;
                                r713 = r713 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r713, parser);
                                a712 = r713;
                            }

                            r707 |= a712;
                        } // end may a712

                        r707 = r707 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                        CommitOrRollback(r707, parser);
                        a706 = r707;
                    }

                    if (!a706)
                    {
                        Checkpoint(parser); // r714

                        bool r714 = true;
                        r714 = r714 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CROSS"));
                        r714 = r714 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r714 = r714 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"JOIN"));
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
                        CommitOrRollback(r714, parser);
                        a706 = r714;
                    }

                    if (!a706)
                    {
                        Checkpoint(parser); // r717

                        bool r717 = true;
                        r717 = r717 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r717)
                        { // may a718
                            bool a718 = false;
                            {
                                Checkpoint(parser); // r719

                                bool r719 = true;
                                r719 = r719 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r719, parser);
                                a718 = r719;
                            }

                            r717 |= a718;
                        } // end may a718

                        r717 = r717 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r717, parser);
                        a706 = r717;
                    }

                    if (!a706)
                    {
                        Checkpoint(parser); // r720

                        bool r720 = true;
                        if (r720)
                        { // alternatives a721 must
                            bool a721 = false;
                            if (!a721)
                            {
                                Checkpoint(parser); // r722

                                bool r722 = true;
                                r722 = r722 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CROSS"));
                                CommitOrRollback(r722, parser);
                                a721 = r722;
                            }

                            if (!a721)
                            {
                                Checkpoint(parser); // r723

                                bool r723 = true;
                                r723 = r723 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OUTER"));
                                CommitOrRollback(r723, parser);
                                a721 = r723;
                            }

                            r720 &= a721;

                        } // end alternatives a721

                        r720 = r720 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r720 = r720 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"APPLY"));
                        if (r720)
                        { // may a724
                            bool a724 = false;
                            {
                                Checkpoint(parser); // r725

                                bool r725 = true;
                                r725 = r725 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r725, parser);
                                a724 = r725;
                            }

                            r720 |= a724;
                        } // end may a724

                        r720 = r720 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSource());
                        CommitOrRollback(r720, parser);
                        a706 = r720;
                    }

                    r705 &= a706;

                } // end alternatives a706

                if (r705)
                { // may a726
                    bool a726 = false;
                    {
                        Checkpoint(parser); // r727

                        bool r727 = true;
                        if (r727)
                        { // may a728
                            bool a728 = false;
                            {
                                Checkpoint(parser); // r729

                                bool r729 = true;
                                r729 = r729 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r729, parser);
                                a728 = r729;
                            }

                            r727 |= a728;
                        } // end may a728

                        r727 = r727 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinedTable());
                        CommitOrRollback(r727, parser);
                        a726 = r727;
                    }

                    r705 |= a726;
                } // end may a726

                CommitOrRollback(r705, parser);
                res = r705;
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
                Checkpoint(parser); // r730

                bool r730 = true;
                if (r730)
                { // may a731
                    bool a731 = false;
                    {
                        Checkpoint(parser); // r732

                        bool r732 = true;
                        if (r732)
                        { // alternatives a733 must
                            bool a733 = false;
                            if (!a733)
                            {
                                Checkpoint(parser); // r734

                                bool r734 = true;
                                r734 = r734 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INNER"));
                                CommitOrRollback(r734, parser);
                                a733 = r734;
                            }

                            if (!a733)
                            {
                                Checkpoint(parser); // r735

                                bool r735 = true;
                                if (r735)
                                { // alternatives a736 must
                                    bool a736 = false;
                                    if (!a736)
                                    {
                                        Checkpoint(parser); // r737

                                        bool r737 = true;
                                        r737 = r737 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LEFT"));
                                        CommitOrRollback(r737, parser);
                                        a736 = r737;
                                    }

                                    if (!a736)
                                    {
                                        Checkpoint(parser); // r738

                                        bool r738 = true;
                                        r738 = r738 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"RIGHT"));
                                        CommitOrRollback(r738, parser);
                                        a736 = r738;
                                    }

                                    if (!a736)
                                    {
                                        Checkpoint(parser); // r739

                                        bool r739 = true;
                                        r739 = r739 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FULL"));
                                        CommitOrRollback(r739, parser);
                                        a736 = r739;
                                    }

                                    r735 &= a736;

                                } // end alternatives a736

                                if (r735)
                                { // may a740
                                    bool a740 = false;
                                    {
                                        Checkpoint(parser); // r741

                                        bool r741 = true;
                                        r741 = r741 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        r741 = r741 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OUTER"));
                                        CommitOrRollback(r741, parser);
                                        a740 = r741;
                                    }

                                    r735 |= a740;
                                } // end may a740

                                CommitOrRollback(r735, parser);
                                a733 = r735;
                            }

                            r732 &= a733;

                        } // end alternatives a733

                        if (r732)
                        { // may a742
                            bool a742 = false;
                            {
                                Checkpoint(parser); // r743

                                bool r743 = true;
                                r743 = r743 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r743, parser);
                                a742 = r743;
                            }

                            r732 |= a742;
                        } // end may a742

                        if (r732)
                        { // may a744
                            bool a744 = false;
                            {
                                Checkpoint(parser); // r745

                                bool r745 = true;
                                r745 = r745 && Match(parser, new Jhu.Graywulf.Sql.Parsing.JoinHint());
                                CommitOrRollback(r745, parser);
                                a744 = r745;
                            }

                            r732 |= a744;
                        } // end may a744

                        CommitOrRollback(r732, parser);
                        a731 = r732;
                    }

                    r730 |= a731;
                } // end may a731

                if (r730)
                { // may a746
                    bool a746 = false;
                    {
                        Checkpoint(parser); // r747

                        bool r747 = true;
                        r747 = r747 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r747, parser);
                        a746 = r747;
                    }

                    r730 |= a746;
                } // end may a746

                r730 = r730 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"JOIN"));
                CommitOrRollback(r730, parser);
                res = r730;
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
                Checkpoint(parser); // r748

                bool r748 = true;
                if (r748)
                { // alternatives a749 must
                    bool a749 = false;
                    if (!a749)
                    {
                        Checkpoint(parser); // r750

                        bool r750 = true;
                        r750 = r750 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"LOOP"));
                        CommitOrRollback(r750, parser);
                        a749 = r750;
                    }

                    if (!a749)
                    {
                        Checkpoint(parser); // r751

                        bool r751 = true;
                        r751 = r751 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"HASH"));
                        CommitOrRollback(r751, parser);
                        a749 = r751;
                    }

                    if (!a749)
                    {
                        Checkpoint(parser); // r752

                        bool r752 = true;
                        r752 = r752 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"MERGE"));
                        CommitOrRollback(r752, parser);
                        a749 = r752;
                    }

                    if (!a749)
                    {
                        Checkpoint(parser); // r753

                        bool r753 = true;
                        r753 = r753 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"REMOTE"));
                        CommitOrRollback(r753, parser);
                        a749 = r753;
                    }

                    r748 &= a749;

                } // end alternatives a749

                CommitOrRollback(r748, parser);
                res = r748;
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
                Checkpoint(parser); // r754

                bool r754 = true;
                if (r754)
                { // alternatives a755 must
                    bool a755 = false;
                    if (!a755)
                    {
                        Checkpoint(parser); // r756

                        bool r756 = true;
                        r756 = r756 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionTableSource());
                        CommitOrRollback(r756, parser);
                        a755 = r756;
                    }

                    if (!a755)
                    {
                        Checkpoint(parser); // r757

                        bool r757 = true;
                        r757 = r757 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SimpleTableSource());
                        CommitOrRollback(r757, parser);
                        a755 = r757;
                    }

                    if (!a755)
                    {
                        Checkpoint(parser); // r758

                        bool r758 = true;
                        r758 = r758 && Match(parser, new Jhu.Graywulf.Sql.Parsing.VariableTableSource());
                        CommitOrRollback(r758, parser);
                        a755 = r758;
                    }

                    if (!a755)
                    {
                        Checkpoint(parser); // r759

                        bool r759 = true;
                        r759 = r759 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SubqueryTableSource());
                        CommitOrRollback(r759, parser);
                        a755 = r759;
                    }

                    r754 &= a755;

                } // end alternatives a755

                CommitOrRollback(r754, parser);
                res = r754;
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
                Checkpoint(parser); // r760

                bool r760 = true;
                r760 = r760 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r760)
                { // may a761
                    bool a761 = false;
                    {
                        Checkpoint(parser); // r762

                        bool r762 = true;
                        r762 = r762 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r762)
                        { // may a763
                            bool a763 = false;
                            {
                                Checkpoint(parser); // r764

                                bool r764 = true;
                                r764 = r764 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                r764 = r764 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r764, parser);
                                a763 = r764;
                            }

                            r762 |= a763;
                        } // end may a763

                        r762 = r762 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                        CommitOrRollback(r762, parser);
                        a761 = r762;
                    }

                    r760 |= a761;
                } // end may a761

                if (r760)
                { // may a765
                    bool a765 = false;
                    {
                        Checkpoint(parser); // r766

                        bool r766 = true;
                        r766 = r766 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r766 = r766 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableSampleClause());
                        CommitOrRollback(r766, parser);
                        a765 = r766;
                    }

                    r760 |= a765;
                } // end may a765

                if (r760)
                { // may a767
                    bool a767 = false;
                    {
                        Checkpoint(parser); // r768

                        bool r768 = true;
                        r768 = r768 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r768 = r768 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintClause());
                        CommitOrRollback(r768, parser);
                        a767 = r768;
                    }

                    r760 |= a767;
                } // end may a767

                if (r760)
                { // may a769
                    bool a769 = false;
                    {
                        Checkpoint(parser); // r770

                        bool r770 = true;
                        r770 = r770 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r770 = r770 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TablePartitionClause());
                        CommitOrRollback(r770, parser);
                        a769 = r770;
                    }

                    r760 |= a769;
                } // end may a769

                CommitOrRollback(r760, parser);
                res = r760;
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
                Checkpoint(parser); // r771

                bool r771 = true;
                r771 = r771 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableValuedFunctionCall());
                if (r771)
                { // may a772
                    bool a772 = false;
                    {
                        Checkpoint(parser); // r773

                        bool r773 = true;
                        r773 = r773 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r773, parser);
                        a772 = r773;
                    }

                    r771 |= a772;
                } // end may a772

                if (r771)
                { // may a774
                    bool a774 = false;
                    {
                        Checkpoint(parser); // r775

                        bool r775 = true;
                        r775 = r775 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                        r775 = r775 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r775, parser);
                        a774 = r775;
                    }

                    r771 |= a774;
                } // end may a774

                r771 = r771 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                if (r771)
                { // may a776
                    bool a776 = false;
                    {
                        Checkpoint(parser); // r777

                        bool r777 = true;
                        if (r777)
                        { // may a778
                            bool a778 = false;
                            {
                                Checkpoint(parser); // r779

                                bool r779 = true;
                                r779 = r779 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r779, parser);
                                a778 = r779;
                            }

                            r777 |= a778;
                        } // end may a778

                        r777 = r777 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r777)
                        { // may a780
                            bool a780 = false;
                            {
                                Checkpoint(parser); // r781

                                bool r781 = true;
                                r781 = r781 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r781, parser);
                                a780 = r781;
                            }

                            r777 |= a780;
                        } // end may a780

                        r777 = r777 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAliasList());
                        if (r777)
                        { // may a782
                            bool a782 = false;
                            {
                                Checkpoint(parser); // r783

                                bool r783 = true;
                                r783 = r783 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r783, parser);
                                a782 = r783;
                            }

                            r777 |= a782;
                        } // end may a782

                        r777 = r777 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r777, parser);
                        a776 = r777;
                    }

                    r771 |= a776;
                } // end may a776

                CommitOrRollback(r771, parser);
                res = r771;
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
                Checkpoint(parser); // r784

                bool r784 = true;
                r784 = r784 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                if (r784)
                { // may a785
                    bool a785 = false;
                    {
                        Checkpoint(parser); // r786

                        bool r786 = true;
                        if (r786)
                        { // may a787
                            bool a787 = false;
                            {
                                Checkpoint(parser); // r788

                                bool r788 = true;
                                r788 = r788 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r788, parser);
                                a787 = r788;
                            }

                            r786 |= a787;
                        } // end may a787

                        if (r786)
                        { // may a789
                            bool a789 = false;
                            {
                                Checkpoint(parser); // r790

                                bool r790 = true;
                                r790 = r790 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"AS"));
                                if (r790)
                                { // may a791
                                    bool a791 = false;
                                    {
                                        Checkpoint(parser); // r792

                                        bool r792 = true;
                                        r792 = r792 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r792, parser);
                                        a791 = r792;
                                    }

                                    r790 |= a791;
                                } // end may a791

                                CommitOrRollback(r790, parser);
                                a789 = r790;
                            }

                            r786 |= a789;
                        } // end may a789

                        r786 = r786 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                        CommitOrRollback(r786, parser);
                        a785 = r786;
                    }

                    r784 |= a785;
                } // end may a785

                CommitOrRollback(r784, parser);
                res = r784;
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
                Checkpoint(parser); // r793

                bool r793 = true;
                r793 = r793 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Subquery());
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
                        r797 = r797 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r797, parser);
                        a796 = r797;
                    }

                    r793 |= a796;
                } // end may a796

                r793 = r793 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableAlias());
                CommitOrRollback(r793, parser);
                res = r793;
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
                Checkpoint(parser); // r798

                bool r798 = true;
                r798 = r798 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAlias());
                if (r798)
                { // may a799
                    bool a799 = false;
                    {
                        Checkpoint(parser); // r800

                        bool r800 = true;
                        if (r800)
                        { // may a801
                            bool a801 = false;
                            {
                                Checkpoint(parser); // r802

                                bool r802 = true;
                                r802 = r802 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r802, parser);
                                a801 = r802;
                            }

                            r800 |= a801;
                        } // end may a801

                        r800 = r800 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        r800 = r800 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnAliasList());
                        CommitOrRollback(r800, parser);
                        a799 = r800;
                    }

                    r798 |= a799;
                } // end may a799

                CommitOrRollback(r798, parser);
                res = r798;
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
                Checkpoint(parser); // r803

                bool r803 = true;
                r803 = r803 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLESAMPLE"));
                if (r803)
                { // may a804
                    bool a804 = false;
                    {
                        Checkpoint(parser); // r805

                        bool r805 = true;
                        r805 = r805 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r805 = r805 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SYSTEM"));
                        CommitOrRollback(r805, parser);
                        a804 = r805;
                    }

                    r803 |= a804;
                } // end may a804

                if (r803)
                { // may a806
                    bool a806 = false;
                    {
                        Checkpoint(parser); // r807

                        bool r807 = true;
                        r807 = r807 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r807, parser);
                        a806 = r807;
                    }

                    r803 |= a806;
                } // end may a806

                r803 = r803 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r803)
                { // may a808
                    bool a808 = false;
                    {
                        Checkpoint(parser); // r809

                        bool r809 = true;
                        r809 = r809 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r809, parser);
                        a808 = r809;
                    }

                    r803 |= a808;
                } // end may a808

                r803 = r803 && Match(parser, new Jhu.Graywulf.Sql.Parsing.SampleNumber());
                if (r803)
                { // may a810
                    bool a810 = false;
                    {
                        Checkpoint(parser); // r811

                        bool r811 = true;
                        r811 = r811 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r811)
                        { // may a812
                            bool a812 = false;
                            {
                                Checkpoint(parser); // r813

                                bool r813 = true;
                                if (r813)
                                { // alternatives a814 must
                                    bool a814 = false;
                                    if (!a814)
                                    {
                                        Checkpoint(parser); // r815

                                        bool r815 = true;
                                        r815 = r815 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PERCENT"));
                                        CommitOrRollback(r815, parser);
                                        a814 = r815;
                                    }

                                    if (!a814)
                                    {
                                        Checkpoint(parser); // r816

                                        bool r816 = true;
                                        r816 = r816 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ROWS"));
                                        CommitOrRollback(r816, parser);
                                        a814 = r816;
                                    }

                                    r813 &= a814;

                                } // end alternatives a814

                                CommitOrRollback(r813, parser);
                                a812 = r813;
                            }

                            r811 |= a812;
                        } // end may a812

                        CommitOrRollback(r811, parser);
                        a810 = r811;
                    }

                    r803 |= a810;
                } // end may a810

                if (r803)
                { // may a817
                    bool a817 = false;
                    {
                        Checkpoint(parser); // r818

                        bool r818 = true;
                        r818 = r818 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r818, parser);
                        a817 = r818;
                    }

                    r803 |= a817;
                } // end may a817

                r803 = r803 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                if (r803)
                { // may a819
                    bool a819 = false;
                    {
                        Checkpoint(parser); // r820

                        bool r820 = true;
                        r820 = r820 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r820 = r820 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"REPEATABLE"));
                        if (r820)
                        { // may a821
                            bool a821 = false;
                            {
                                Checkpoint(parser); // r822

                                bool r822 = true;
                                r822 = r822 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r822, parser);
                                a821 = r822;
                            }

                            r820 |= a821;
                        } // end may a821

                        r820 = r820 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r820)
                        { // may a823
                            bool a823 = false;
                            {
                                Checkpoint(parser); // r824

                                bool r824 = true;
                                r824 = r824 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r824, parser);
                                a823 = r824;
                            }

                            r820 |= a823;
                        } // end may a823

                        r820 = r820 && Match(parser, new Jhu.Graywulf.Sql.Parsing.RepeatSeed());
                        if (r820)
                        { // may a825
                            bool a825 = false;
                            {
                                Checkpoint(parser); // r826

                                bool r826 = true;
                                r826 = r826 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r826, parser);
                                a825 = r826;
                            }

                            r820 |= a825;
                        } // end may a825

                        r820 = r820 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r820, parser);
                        a819 = r820;
                    }

                    r803 |= a819;
                } // end may a819

                CommitOrRollback(r803, parser);
                res = r803;
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
                Checkpoint(parser); // r827

                bool r827 = true;
                r827 = r827 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PARTITION"));
                r827 = r827 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r827 = r827 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                r827 = r827 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r827 = r827 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentifier());
                CommitOrRollback(r827, parser);
                res = r827;
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
                Checkpoint(parser); // r828

                bool r828 = true;
                r828 = r828 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WHERE"));
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

                r828 = r828 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                CommitOrRollback(r828, parser);
                res = r828;
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
                Checkpoint(parser); // r831

                bool r831 = true;
                r831 = r831 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"GROUP"));
                r831 = r831 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r831 = r831 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
                if (r831)
                { // alternatives a832 must
                    bool a832 = false;
                    if (!a832)
                    {
                        Checkpoint(parser); // r833

                        bool r833 = true;
                        r833 = r833 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r833 = r833 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ALL"));
                        CommitOrRollback(r833, parser);
                        a832 = r833;
                    }

                    if (!a832)
                    {
                        Checkpoint(parser); // r834

                        bool r834 = true;
                        if (r834)
                        { // may a835
                            bool a835 = false;
                            {
                                Checkpoint(parser); // r836

                                bool r836 = true;
                                r836 = r836 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r836, parser);
                                a835 = r836;
                            }

                            r834 |= a835;
                        } // end may a835

                        r834 = r834 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByList());
                        CommitOrRollback(r834, parser);
                        a832 = r834;
                    }

                    r831 &= a832;

                } // end alternatives a832

                CommitOrRollback(r831, parser);
                res = r831;
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
                Checkpoint(parser); // r837

                bool r837 = true;
                r837 = r837 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r837)
                { // may a838
                    bool a838 = false;
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

                        r839 = r839 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r839)
                        { // may a842
                            bool a842 = false;
                            {
                                Checkpoint(parser); // r843

                                bool r843 = true;
                                r843 = r843 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r843, parser);
                                a842 = r843;
                            }

                            r839 |= a842;
                        } // end may a842

                        r839 = r839 && Match(parser, new Jhu.Graywulf.Sql.Parsing.GroupByList());
                        CommitOrRollback(r839, parser);
                        a838 = r839;
                    }

                    r837 |= a838;
                } // end may a838

                CommitOrRollback(r837, parser);
                res = r837;
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
                Checkpoint(parser); // r844

                bool r844 = true;
                r844 = r844 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"HAVING"));
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

                r844 = r844 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BooleanExpression());
                CommitOrRollback(r844, parser);
                res = r844;
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
                Checkpoint(parser); // r847

                bool r847 = true;
                r847 = r847 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ORDER"));
                r847 = r847 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r847 = r847 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"BY"));
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

                r847 = r847 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByList());
                CommitOrRollback(r847, parser);
                res = r847;
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
                Checkpoint(parser); // r850

                bool r850 = true;
                r850 = r850 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByArgument());
                if (r850)
                { // may a851
                    bool a851 = false;
                    {
                        Checkpoint(parser); // r852

                        bool r852 = true;
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

                        r852 = r852 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r852)
                        { // may a855
                            bool a855 = false;
                            {
                                Checkpoint(parser); // r856

                                bool r856 = true;
                                r856 = r856 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r856, parser);
                                a855 = r856;
                            }

                            r852 |= a855;
                        } // end may a855

                        r852 = r852 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByList());
                        CommitOrRollback(r852, parser);
                        a851 = r852;
                    }

                    r850 |= a851;
                } // end may a851

                CommitOrRollback(r850, parser);
                res = r850;
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
                Checkpoint(parser); // r857

                bool r857 = true;
                r857 = r857 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                if (r857)
                { // may a858
                    bool a858 = false;
                    {
                        Checkpoint(parser); // r859

                        bool r859 = true;
                        if (r859)
                        { // may a860
                            bool a860 = false;
                            {
                                Checkpoint(parser); // r861

                                bool r861 = true;
                                r861 = r861 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r861, parser);
                                a860 = r861;
                            }

                            r859 |= a860;
                        } // end may a860

                        if (r859)
                        { // alternatives a862 must
                            bool a862 = false;
                            if (!a862)
                            {
                                Checkpoint(parser); // r863

                                bool r863 = true;
                                r863 = r863 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ASC"));
                                CommitOrRollback(r863, parser);
                                a862 = r863;
                            }

                            if (!a862)
                            {
                                Checkpoint(parser); // r864

                                bool r864 = true;
                                r864 = r864 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DESC"));
                                CommitOrRollback(r864, parser);
                                a862 = r864;
                            }

                            r859 &= a862;

                        } // end alternatives a862

                        CommitOrRollback(r859, parser);
                        a858 = r859;
                    }

                    r857 |= a858;
                } // end may a858

                CommitOrRollback(r857, parser);
                res = r857;
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
                Checkpoint(parser); // r865

                bool r865 = true;
                r865 = r865 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"WITH"));
                if (r865)
                { // may a866
                    bool a866 = false;
                    {
                        Checkpoint(parser); // r867

                        bool r867 = true;
                        r867 = r867 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r867, parser);
                        a866 = r867;
                    }

                    r865 |= a866;
                } // end may a866

                r865 = r865 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r865)
                { // may a868
                    bool a868 = false;
                    {
                        Checkpoint(parser); // r869

                        bool r869 = true;
                        r869 = r869 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r869, parser);
                        a868 = r869;
                    }

                    r865 |= a868;
                } // end may a868

                r865 = r865 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintList());
                if (r865)
                { // may a870
                    bool a870 = false;
                    {
                        Checkpoint(parser); // r871

                        bool r871 = true;
                        r871 = r871 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r871, parser);
                        a870 = r871;
                    }

                    r865 |= a870;
                } // end may a870

                r865 = r865 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r865, parser);
                res = r865;
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
                Checkpoint(parser); // r872

                bool r872 = true;
                r872 = r872 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHint());
                if (r872)
                { // may a873
                    bool a873 = false;
                    {
                        Checkpoint(parser); // r874

                        bool r874 = true;
                        if (r874)
                        { // may a875
                            bool a875 = false;
                            {
                                Checkpoint(parser); // r876

                                bool r876 = true;
                                r876 = r876 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r876, parser);
                                a875 = r876;
                            }

                            r874 |= a875;
                        } // end may a875

                        r874 = r874 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r874)
                        { // may a877
                            bool a877 = false;
                            {
                                Checkpoint(parser); // r878

                                bool r878 = true;
                                r878 = r878 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r878, parser);
                                a877 = r878;
                            }

                            r874 |= a877;
                        } // end may a877

                        r874 = r874 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableHintList());
                        CommitOrRollback(r874, parser);
                        a873 = r874;
                    }

                    r872 |= a873;
                } // end may a873

                CommitOrRollback(r872, parser);
                res = r872;
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
                Checkpoint(parser); // r879

                bool r879 = true;
                if (r879)
                { // alternatives a880 must
                    bool a880 = false;
                    if (!a880)
                    {
                        Checkpoint(parser); // r881

                        bool r881 = true;
                        r881 = r881 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r881)
                        { // may a882
                            bool a882 = false;
                            {
                                Checkpoint(parser); // r883

                                bool r883 = true;
                                r883 = r883 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r883, parser);
                                a882 = r883;
                            }

                            r881 |= a882;
                        } // end may a882

                        r881 = r881 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r881, parser);
                        a880 = r881;
                    }

                    if (!a880)
                    {
                        Checkpoint(parser); // r884

                        bool r884 = true;
                        r884 = r884 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r884, parser);
                        a880 = r884;
                    }

                    r879 &= a880;

                } // end alternatives a880

                CommitOrRollback(r879, parser);
                res = r879;
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
                Checkpoint(parser); // r885

                bool r885 = true;
                r885 = r885 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"OPTION"));
                if (r885)
                { // may a886
                    bool a886 = false;
                    {
                        Checkpoint(parser); // r887

                        bool r887 = true;
                        r887 = r887 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r887, parser);
                        a886 = r887;
                    }

                    r885 |= a886;
                } // end may a886

                r885 = r885 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r885)
                { // may a888
                    bool a888 = false;
                    {
                        Checkpoint(parser); // r889

                        bool r889 = true;
                        r889 = r889 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r889, parser);
                        a888 = r889;
                    }

                    r885 |= a888;
                } // end may a888

                r885 = r885 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintList());
                if (r885)
                { // may a890
                    bool a890 = false;
                    {
                        Checkpoint(parser); // r891

                        bool r891 = true;
                        r891 = r891 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r891, parser);
                        a890 = r891;
                    }

                    r885 |= a890;
                } // end may a890

                r885 = r885 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r885, parser);
                res = r885;
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
                Checkpoint(parser); // r892

                bool r892 = true;
                r892 = r892 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHint());
                if (r892)
                { // may a893
                    bool a893 = false;
                    {
                        Checkpoint(parser); // r894

                        bool r894 = true;
                        if (r894)
                        { // may a895
                            bool a895 = false;
                            {
                                Checkpoint(parser); // r896

                                bool r896 = true;
                                r896 = r896 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r896, parser);
                                a895 = r896;
                            }

                            r894 |= a895;
                        } // end may a895

                        r894 = r894 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r894)
                        { // may a897
                            bool a897 = false;
                            {
                                Checkpoint(parser); // r898

                                bool r898 = true;
                                r898 = r898 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r898, parser);
                                a897 = r898;
                            }

                            r894 |= a897;
                        } // end may a897

                        r894 = r894 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintList());
                        CommitOrRollback(r894, parser);
                        a893 = r894;
                    }

                    r892 |= a893;
                } // end may a893

                CommitOrRollback(r892, parser);
                res = r892;
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
                Checkpoint(parser); // r899

                bool r899 = true;
                if (r899)
                { // alternatives a900 must
                    bool a900 = false;
                    if (!a900)
                    {
                        Checkpoint(parser); // r901

                        bool r901 = true;
                        r901 = r901 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r901)
                        { // may a902
                            bool a902 = false;
                            {
                                Checkpoint(parser); // r903

                                bool r903 = true;
                                r903 = r903 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r903, parser);
                                a902 = r903;
                            }

                            r901 |= a902;
                        } // end may a902

                        r901 = r901 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r901, parser);
                        a900 = r901;
                    }

                    if (!a900)
                    {
                        Checkpoint(parser); // r904

                        bool r904 = true;
                        r904 = r904 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        if (r904)
                        { // may a905
                            bool a905 = false;
                            {
                                Checkpoint(parser); // r906

                                bool r906 = true;
                                r906 = r906 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r906, parser);
                                a905 = r906;
                            }

                            r904 |= a905;
                        } // end may a905

                        r904 = r904 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r904)
                        { // may a907
                            bool a907 = false;
                            {
                                Checkpoint(parser); // r908

                                bool r908 = true;
                                r908 = r908 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r908, parser);
                                a907 = r908;
                            }

                            r904 |= a907;
                        } // end may a907

                        r904 = r904 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r904, parser);
                        a900 = r904;
                    }

                    if (!a900)
                    {
                        Checkpoint(parser); // r909

                        bool r909 = true;
                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r909 = r909 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Number());
                        CommitOrRollback(r909, parser);
                        a900 = r909;
                    }

                    if (!a900)
                    {
                        Checkpoint(parser); // r910

                        bool r910 = true;
                        r910 = r910 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintIdentifierList());
                        CommitOrRollback(r910, parser);
                        a900 = r910;
                    }

                    if (!a900)
                    {
                        Checkpoint(parser); // r911

                        bool r911 = true;
                        r911 = r911 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r911, parser);
                        a900 = r911;
                    }

                    r899 &= a900;

                } // end alternatives a900

                CommitOrRollback(r899, parser);
                res = r899;
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
                        r914 = r914 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Identifier());
                        CommitOrRollback(r914, parser);
                        a913 = r914;
                    }

                    r912 |= a913;
                } // end may a913

                CommitOrRollback(r912, parser);
                res = r912;
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
                Checkpoint(parser); // r915

                bool r915 = true;
                if (r915)
                { // may a916
                    bool a916 = false;
                    {
                        Checkpoint(parser); // r917

                        bool r917 = true;
                        r917 = r917 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r917)
                        { // may a918
                            bool a918 = false;
                            {
                                Checkpoint(parser); // r919

                                bool r919 = true;
                                r919 = r919 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r919, parser);
                                a918 = r919;
                            }

                            r917 |= a918;
                        } // end may a918

                        CommitOrRollback(r917, parser);
                        a916 = r917;
                    }

                    r915 |= a916;
                } // end may a916

                r915 = r915 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INSERT"));
                if (r915)
                { // may a920
                    bool a920 = false;
                    {
                        Checkpoint(parser); // r921

                        bool r921 = true;
                        r921 = r921 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r921, parser);
                        a920 = r921;
                    }

                    r915 |= a920;
                } // end may a920

                if (r915)
                { // alternatives a922 must
                    bool a922 = false;
                    if (!a922)
                    {
                        Checkpoint(parser); // r923

                        bool r923 = true;
                        r923 = r923 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IntoClause());
                        CommitOrRollback(r923, parser);
                        a922 = r923;
                    }

                    if (!a922)
                    {
                        Checkpoint(parser); // r924

                        bool r924 = true;
                        r924 = r924 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                        CommitOrRollback(r924, parser);
                        a922 = r924;
                    }

                    r915 &= a922;

                } // end alternatives a922

                if (r915)
                { // may a925
                    bool a925 = false;
                    {
                        Checkpoint(parser); // r926

                        bool r926 = true;
                        if (r926)
                        { // may a927
                            bool a927 = false;
                            {
                                Checkpoint(parser); // r928

                                bool r928 = true;
                                r928 = r928 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r928, parser);
                                a927 = r928;
                            }

                            r926 |= a927;
                        } // end may a927

                        r926 = r926 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnListBrackets());
                        CommitOrRollback(r926, parser);
                        a925 = r926;
                    }

                    r915 |= a925;
                } // end may a925

                if (r915)
                { // may a929
                    bool a929 = false;
                    {
                        Checkpoint(parser); // r930

                        bool r930 = true;
                        r930 = r930 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r930, parser);
                        a929 = r930;
                    }

                    r915 |= a929;
                } // end may a929

                if (r915)
                { // alternatives a931 must
                    bool a931 = false;
                    if (!a931)
                    {
                        Checkpoint(parser); // r932

                        bool r932 = true;
                        r932 = r932 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesClause());
                        CommitOrRollback(r932, parser);
                        a931 = r932;
                    }

                    if (!a931)
                    {
                        Checkpoint(parser); // r933

                        bool r933 = true;
                        r933 = r933 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        r933 = r933 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r933 = r933 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"VALUES"));
                        CommitOrRollback(r933, parser);
                        a931 = r933;
                    }

                    if (!a931)
                    {
                        Checkpoint(parser); // r934

                        bool r934 = true;
                        r934 = r934 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryExpression());
                        if (r934)
                        { // may a935
                            bool a935 = false;
                            {
                                Checkpoint(parser); // r936

                                bool r936 = true;
                                if (r936)
                                { // may a937
                                    bool a937 = false;
                                    {
                                        Checkpoint(parser); // r938

                                        bool r938 = true;
                                        r938 = r938 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                        CommitOrRollback(r938, parser);
                                        a937 = r938;
                                    }

                                    r936 |= a937;
                                } // end may a937

                                r936 = r936 && Match(parser, new Jhu.Graywulf.Sql.Parsing.OrderByClause());
                                CommitOrRollback(r936, parser);
                                a935 = r936;
                            }

                            r934 |= a935;
                        } // end may a935

                        CommitOrRollback(r934, parser);
                        a931 = r934;
                    }

                    r915 &= a931;

                } // end alternatives a931

                if (r915)
                { // may a939
                    bool a939 = false;
                    {
                        Checkpoint(parser); // r940

                        bool r940 = true;
                        if (r940)
                        { // may a941
                            bool a941 = false;
                            {
                                Checkpoint(parser); // r942

                                bool r942 = true;
                                r942 = r942 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r942, parser);
                                a941 = r942;
                            }

                            r940 |= a941;
                        } // end may a941

                        r940 = r940 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r940, parser);
                        a939 = r940;
                    }

                    r915 |= a939;
                } // end may a939

                CommitOrRollback(r915, parser);
                res = r915;
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
                Checkpoint(parser); // r943

                bool r943 = true;
                r943 = r943 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r943)
                { // may a944
                    bool a944 = false;
                    {
                        Checkpoint(parser); // r945

                        bool r945 = true;
                        r945 = r945 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r945, parser);
                        a944 = r945;
                    }

                    r943 |= a944;
                } // end may a944

                r943 = r943 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnList());
                if (r943)
                { // may a946
                    bool a946 = false;
                    {
                        Checkpoint(parser); // r947

                        bool r947 = true;
                        r947 = r947 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r947, parser);
                        a946 = r947;
                    }

                    r943 |= a946;
                } // end may a946

                r943 = r943 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r943, parser);
                res = r943;
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
                Checkpoint(parser); // r948

                bool r948 = true;
                r948 = r948 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r948)
                { // may a949
                    bool a949 = false;
                    {
                        Checkpoint(parser); // r950

                        bool r950 = true;
                        if (r950)
                        { // may a951
                            bool a951 = false;
                            {
                                Checkpoint(parser); // r952

                                bool r952 = true;
                                r952 = r952 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r952, parser);
                                a951 = r952;
                            }

                            r950 |= a951;
                        } // end may a951

                        r950 = r950 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r950)
                        { // may a953
                            bool a953 = false;
                            {
                                Checkpoint(parser); // r954

                                bool r954 = true;
                                r954 = r954 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r954, parser);
                                a953 = r954;
                            }

                            r950 |= a953;
                        } // end may a953

                        r950 = r950 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnList());
                        CommitOrRollback(r950, parser);
                        a949 = r950;
                    }

                    r948 |= a949;
                } // end may a949

                CommitOrRollback(r948, parser);
                res = r948;
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
                Checkpoint(parser); // r955

                bool r955 = true;
                r955 = r955 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"VALUES"));
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

                r955 = r955 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroupList());
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

                CommitOrRollback(r955, parser);
                res = r955;
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
                Checkpoint(parser); // r960

                bool r960 = true;
                r960 = r960 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroup());
                if (r960)
                { // may a961
                    bool a961 = false;
                    {
                        Checkpoint(parser); // r962

                        bool r962 = true;
                        if (r962)
                        { // may a963
                            bool a963 = false;
                            {
                                Checkpoint(parser); // r964

                                bool r964 = true;
                                r964 = r964 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r964, parser);
                                a963 = r964;
                            }

                            r962 |= a963;
                        } // end may a963

                        r962 = r962 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r962)
                        { // may a965
                            bool a965 = false;
                            {
                                Checkpoint(parser); // r966

                                bool r966 = true;
                                r966 = r966 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r966, parser);
                                a965 = r966;
                            }

                            r962 |= a965;
                        } // end may a965

                        r962 = r962 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesGroupList());
                        CommitOrRollback(r962, parser);
                        a961 = r962;
                    }

                    r960 |= a961;
                } // end may a961

                CommitOrRollback(r960, parser);
                res = r960;
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
                Checkpoint(parser); // r967

                bool r967 = true;
                r967 = r967 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
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

                r967 = r967 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesList());
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

                r967 = r967 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r967, parser);
                res = r967;
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
                Checkpoint(parser); // r972

                bool r972 = true;
                if (r972)
                { // alternatives a973 must
                    bool a973 = false;
                    if (!a973)
                    {
                        Checkpoint(parser); // r974

                        bool r974 = true;
                        r974 = r974 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        CommitOrRollback(r974, parser);
                        a973 = r974;
                    }

                    if (!a973)
                    {
                        Checkpoint(parser); // r975

                        bool r975 = true;
                        r975 = r975 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r975, parser);
                        a973 = r975;
                    }

                    r972 &= a973;

                } // end alternatives a973

                if (r972)
                { // may a976
                    bool a976 = false;
                    {
                        Checkpoint(parser); // r977

                        bool r977 = true;
                        if (r977)
                        { // may a978
                            bool a978 = false;
                            {
                                Checkpoint(parser); // r979

                                bool r979 = true;
                                r979 = r979 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r979, parser);
                                a978 = r979;
                            }

                            r977 |= a978;
                        } // end may a978

                        r977 = r977 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r977)
                        { // may a980
                            bool a980 = false;
                            {
                                Checkpoint(parser); // r981

                                bool r981 = true;
                                r981 = r981 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r981, parser);
                                a980 = r981;
                            }

                            r977 |= a980;
                        } // end may a980

                        r977 = r977 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValuesList());
                        CommitOrRollback(r977, parser);
                        a976 = r977;
                    }

                    r972 |= a976;
                } // end may a976

                CommitOrRollback(r972, parser);
                res = r972;
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
                Checkpoint(parser); // r982

                bool r982 = true;
                if (r982)
                { // may a983
                    bool a983 = false;
                    {
                        Checkpoint(parser); // r984

                        bool r984 = true;
                        r984 = r984 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r984)
                        { // may a985
                            bool a985 = false;
                            {
                                Checkpoint(parser); // r986

                                bool r986 = true;
                                r986 = r986 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r986, parser);
                                a985 = r986;
                            }

                            r984 |= a985;
                        } // end may a985

                        CommitOrRollback(r984, parser);
                        a983 = r984;
                    }

                    r982 |= a983;
                } // end may a983

                r982 = r982 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UPDATE"));
                if (r982)
                { // may a987
                    bool a987 = false;
                    {
                        Checkpoint(parser); // r988

                        bool r988 = true;
                        r988 = r988 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r988, parser);
                        a987 = r988;
                    }

                    r982 |= a987;
                } // end may a987

                r982 = r982 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                if (r982)
                { // may a989
                    bool a989 = false;
                    {
                        Checkpoint(parser); // r990

                        bool r990 = true;
                        r990 = r990 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r990, parser);
                        a989 = r990;
                    }

                    r982 |= a989;
                } // end may a989

                r982 = r982 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"SET"));
                if (r982)
                { // may a991
                    bool a991 = false;
                    {
                        Checkpoint(parser); // r992

                        bool r992 = true;
                        r992 = r992 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r992, parser);
                        a991 = r992;
                    }

                    r982 |= a991;
                } // end may a991

                r982 = r982 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetList());
                if (r982)
                { // may a993
                    bool a993 = false;
                    {
                        Checkpoint(parser); // r994

                        bool r994 = true;
                        if (r994)
                        { // may a995
                            bool a995 = false;
                            {
                                Checkpoint(parser); // r996

                                bool r996 = true;
                                r996 = r996 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r996, parser);
                                a995 = r996;
                            }

                            r994 |= a995;
                        } // end may a995

                        r994 = r994 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r994, parser);
                        a993 = r994;
                    }

                    r982 |= a993;
                } // end may a993

                if (r982)
                { // may a997
                    bool a997 = false;
                    {
                        Checkpoint(parser); // r998

                        bool r998 = true;
                        if (r998)
                        { // may a999
                            bool a999 = false;
                            {
                                Checkpoint(parser); // r1000

                                bool r1000 = true;
                                r1000 = r1000 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1000, parser);
                                a999 = r1000;
                            }

                            r998 |= a999;
                        } // end may a999

                        r998 = r998 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r998, parser);
                        a997 = r998;
                    }

                    r982 |= a997;
                } // end may a997

                if (r982)
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

                        r1002 = r1002 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r1002, parser);
                        a1001 = r1002;
                    }

                    r982 |= a1001;
                } // end may a1001

                CommitOrRollback(r982, parser);
                res = r982;
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
                Checkpoint(parser); // r1005

                bool r1005 = true;
                r1005 = r1005 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetColumn());
                if (r1005)
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

                        r1007 = r1007 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1007)
                        { // may a1010
                            bool a1010 = false;
                            {
                                Checkpoint(parser); // r1011

                                bool r1011 = true;
                                r1011 = r1011 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1011, parser);
                                a1010 = r1011;
                            }

                            r1007 |= a1010;
                        } // end may a1010

                        r1007 = r1007 && Match(parser, new Jhu.Graywulf.Sql.Parsing.UpdateSetList());
                        CommitOrRollback(r1007, parser);
                        a1006 = r1007;
                    }

                    r1005 |= a1006;
                } // end may a1006

                CommitOrRollback(r1005, parser);
                res = r1005;
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
                Checkpoint(parser); // r1012

                bool r1012 = true;
                if (r1012)
                { // alternatives a1013 must
                    bool a1013 = false;
                    if (!a1013)
                    {
                        Checkpoint(parser); // r1014

                        bool r1014 = true;
                        r1014 = r1014 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                        if (r1014)
                        { // may a1015
                            bool a1015 = false;
                            {
                                Checkpoint(parser); // r1016

                                bool r1016 = true;
                                r1016 = r1016 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1016, parser);
                                a1015 = r1016;
                            }

                            r1014 |= a1015;
                        } // end may a1015

                        r1014 = r1014 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Equals1());
                        if (r1014)
                        { // may a1017
                            bool a1017 = false;
                            {
                                Checkpoint(parser); // r1018

                                bool r1018 = true;
                                r1018 = r1018 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1018, parser);
                                a1017 = r1018;
                            }

                            r1014 |= a1017;
                        } // end may a1017

                        r1014 = r1014 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                        CommitOrRollback(r1014, parser);
                        a1013 = r1014;
                    }

                    if (!a1013)
                    {
                        Checkpoint(parser); // r1019

                        bool r1019 = true;
                        if (r1019)
                        { // alternatives a1020 must
                            bool a1020 = false;
                            if (!a1020)
                            {
                                Checkpoint(parser); // r1021

                                bool r1021 = true;
                                r1021 = r1021 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Variable());
                                CommitOrRollback(r1021, parser);
                                a1020 = r1021;
                            }

                            if (!a1020)
                            {
                                Checkpoint(parser); // r1022

                                bool r1022 = true;
                                r1022 = r1022 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                                CommitOrRollback(r1022, parser);
                                a1020 = r1022;
                            }

                            r1019 &= a1020;

                        } // end alternatives a1020

                        CommitOrRollback(r1019, parser);
                        a1013 = r1019;
                    }

                    r1012 &= a1013;

                } // end alternatives a1013

                if (r1012)
                { // may a1023
                    bool a1023 = false;
                    {
                        Checkpoint(parser); // r1024

                        bool r1024 = true;
                        r1024 = r1024 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1024, parser);
                        a1023 = r1024;
                    }

                    r1012 |= a1023;
                } // end may a1023

                r1012 = r1012 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ValueAssignmentOperator());
                if (r1012)
                { // may a1025
                    bool a1025 = false;
                    {
                        Checkpoint(parser); // r1026

                        bool r1026 = true;
                        r1026 = r1026 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1026, parser);
                        a1025 = r1026;
                    }

                    r1012 |= a1025;
                } // end may a1025

                if (r1012)
                { // alternatives a1027 must
                    bool a1027 = false;
                    if (!a1027)
                    {
                        Checkpoint(parser); // r1028

                        bool r1028 = true;
                        r1028 = r1028 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                        CommitOrRollback(r1028, parser);
                        a1027 = r1028;
                    }

                    if (!a1027)
                    {
                        Checkpoint(parser); // r1029

                        bool r1029 = true;
                        r1029 = r1029 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                        CommitOrRollback(r1029, parser);
                        a1027 = r1029;
                    }

                    r1012 &= a1027;

                } // end alternatives a1027

                CommitOrRollback(r1012, parser);
                res = r1012;
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
                Checkpoint(parser); // r1030

                bool r1030 = true;
                r1030 = r1030 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DeleteSpecification());
                CommitOrRollback(r1030, parser);
                res = r1030;
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
                Checkpoint(parser); // r1031

                bool r1031 = true;
                if (r1031)
                { // may a1032
                    bool a1032 = false;
                    {
                        Checkpoint(parser); // r1033

                        bool r1033 = true;
                        r1033 = r1033 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommonTableExpression());
                        if (r1033)
                        { // may a1034
                            bool a1034 = false;
                            {
                                Checkpoint(parser); // r1035

                                bool r1035 = true;
                                r1035 = r1035 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1035, parser);
                                a1034 = r1035;
                            }

                            r1033 |= a1034;
                        } // end may a1034

                        CommitOrRollback(r1033, parser);
                        a1032 = r1033;
                    }

                    r1031 |= a1032;
                } // end may a1032

                r1031 = r1031 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DELETE"));
                if (r1031)
                { // may a1036
                    bool a1036 = false;
                    {
                        Checkpoint(parser); // r1037

                        bool r1037 = true;
                        r1037 = r1037 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1037, parser);
                        a1036 = r1037;
                    }

                    r1031 |= a1036;
                } // end may a1036

                if (r1031)
                { // may a1038
                    bool a1038 = false;
                    {
                        Checkpoint(parser); // r1039

                        bool r1039 = true;
                        r1039 = r1039 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"FROM"));
                        CommitOrRollback(r1039, parser);
                        a1038 = r1039;
                    }

                    r1031 |= a1038;
                } // end may a1038

                if (r1031)
                { // may a1040
                    bool a1040 = false;
                    {
                        Checkpoint(parser); // r1041

                        bool r1041 = true;
                        r1041 = r1041 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1041, parser);
                        a1040 = r1041;
                    }

                    r1031 |= a1040;
                } // end may a1040

                r1031 = r1031 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TargetTableSpecification());
                if (r1031)
                { // may a1042
                    bool a1042 = false;
                    {
                        Checkpoint(parser); // r1043

                        bool r1043 = true;
                        if (r1043)
                        { // may a1044
                            bool a1044 = false;
                            {
                                Checkpoint(parser); // r1045

                                bool r1045 = true;
                                r1045 = r1045 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1045, parser);
                                a1044 = r1045;
                            }

                            r1043 |= a1044;
                        } // end may a1044

                        r1043 = r1043 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FromClause());
                        CommitOrRollback(r1043, parser);
                        a1042 = r1043;
                    }

                    r1031 |= a1042;
                } // end may a1042

                if (r1031)
                { // may a1046
                    bool a1046 = false;
                    {
                        Checkpoint(parser); // r1047

                        bool r1047 = true;
                        if (r1047)
                        { // may a1048
                            bool a1048 = false;
                            {
                                Checkpoint(parser); // r1049

                                bool r1049 = true;
                                r1049 = r1049 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1049, parser);
                                a1048 = r1049;
                            }

                            r1047 |= a1048;
                        } // end may a1048

                        r1047 = r1047 && Match(parser, new Jhu.Graywulf.Sql.Parsing.WhereClause());
                        CommitOrRollback(r1047, parser);
                        a1046 = r1047;
                    }

                    r1031 |= a1046;
                } // end may a1046

                if (r1031)
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

                        r1051 = r1051 && Match(parser, new Jhu.Graywulf.Sql.Parsing.QueryHintClause());
                        CommitOrRollback(r1051, parser);
                        a1050 = r1051;
                    }

                    r1031 |= a1050;
                } // end may a1050

                CommitOrRollback(r1031, parser);
                res = r1031;
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
                Checkpoint(parser); // r1054

                bool r1054 = true;
                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CREATE"));
                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r1054)
                { // may a1055
                    bool a1055 = false;
                    {
                        Checkpoint(parser); // r1056

                        bool r1056 = true;
                        r1056 = r1056 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1056, parser);
                        a1055 = r1056;
                    }

                    r1054 |= a1055;
                } // end may a1055

                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1054)
                { // may a1057
                    bool a1057 = false;
                    {
                        Checkpoint(parser); // r1058

                        bool r1058 = true;
                        r1058 = r1058 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1058, parser);
                        a1057 = r1058;
                    }

                    r1054 |= a1057;
                } // end may a1057

                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                if (r1054)
                { // may a1059
                    bool a1059 = false;
                    {
                        Checkpoint(parser); // r1060

                        bool r1060 = true;
                        r1060 = r1060 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1060, parser);
                        a1059 = r1060;
                    }

                    r1054 |= a1059;
                } // end may a1059

                r1054 = r1054 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r1054, parser);
                res = r1054;
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
                Checkpoint(parser); // r1061

                bool r1061 = true;
                if (r1061)
                { // alternatives a1062 must
                    bool a1062 = false;
                    if (!a1062)
                    {
                        Checkpoint(parser); // r1063

                        bool r1063 = true;
                        r1063 = r1063 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnDefinition());
                        CommitOrRollback(r1063, parser);
                        a1062 = r1063;
                    }

                    if (!a1062)
                    {
                        Checkpoint(parser); // r1064

                        bool r1064 = true;
                        r1064 = r1064 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableConstraint());
                        CommitOrRollback(r1064, parser);
                        a1062 = r1064;
                    }

                    r1061 &= a1062;

                } // end alternatives a1062

                if (r1061)
                { // may a1065
                    bool a1065 = false;
                    {
                        Checkpoint(parser); // r1066

                        bool r1066 = true;
                        if (r1066)
                        { // may a1067
                            bool a1067 = false;
                            {
                                Checkpoint(parser); // r1068

                                bool r1068 = true;
                                r1068 = r1068 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1068, parser);
                                a1067 = r1068;
                            }

                            r1066 |= a1067;
                        } // end may a1067

                        r1066 = r1066 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1066)
                        { // may a1069
                            bool a1069 = false;
                            {
                                Checkpoint(parser); // r1070

                                bool r1070 = true;
                                r1070 = r1070 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1070, parser);
                                a1069 = r1070;
                            }

                            r1066 |= a1069;
                        } // end may a1069

                        r1066 = r1066 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableDefinitionList());
                        CommitOrRollback(r1066, parser);
                        a1065 = r1066;
                    }

                    r1061 |= a1065;
                } // end may a1065

                CommitOrRollback(r1061, parser);
                res = r1061;
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
                Checkpoint(parser); // r1071

                bool r1071 = true;
                r1071 = r1071 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                r1071 = r1071 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1071 = r1071 && Match(parser, new Jhu.Graywulf.Sql.Parsing.DataType());
                if (r1071)
                { // may a1072
                    bool a1072 = false;
                    {
                        Checkpoint(parser); // r1073

                        bool r1073 = true;
                        if (r1073)
                        { // may a1074
                            bool a1074 = false;
                            {
                                Checkpoint(parser); // r1075

                                bool r1075 = true;
                                r1075 = r1075 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1075, parser);
                                a1074 = r1075;
                            }

                            r1073 |= a1074;
                        } // end may a1074

                        if (r1073)
                        { // alternatives a1076 must
                            bool a1076 = false;
                            if (!a1076)
                            {
                                Checkpoint(parser); // r1077

                                bool r1077 = true;
                                r1077 = r1077 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnDefaultDefinition());
                                CommitOrRollback(r1077, parser);
                                a1076 = r1077;
                            }

                            if (!a1076)
                            {
                                Checkpoint(parser); // r1078

                                bool r1078 = true;
                                r1078 = r1078 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnIdentityDefinition());
                                CommitOrRollback(r1078, parser);
                                a1076 = r1078;
                            }

                            r1073 &= a1076;

                        } // end alternatives a1076

                        CommitOrRollback(r1073, parser);
                        a1072 = r1073;
                    }

                    r1071 |= a1072;
                } // end may a1072

                if (r1071)
                { // may a1079
                    bool a1079 = false;
                    {
                        Checkpoint(parser); // r1080

                        bool r1080 = true;
                        if (r1080)
                        { // may a1081
                            bool a1081 = false;
                            {
                                Checkpoint(parser); // r1082

                                bool r1082 = true;
                                r1082 = r1082 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1082, parser);
                                a1081 = r1082;
                            }

                            r1080 |= a1081;
                        } // end may a1081

                        r1080 = r1080 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnConstraint());
                        CommitOrRollback(r1080, parser);
                        a1079 = r1080;
                    }

                    r1071 |= a1079;
                } // end may a1079

                CommitOrRollback(r1071, parser);
                res = r1071;
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
                Checkpoint(parser); // r1083

                bool r1083 = true;
                if (r1083)
                { // may a1084
                    bool a1084 = false;
                    {
                        Checkpoint(parser); // r1085

                        bool r1085 = true;
                        r1085 = r1085 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1085, parser);
                        a1084 = r1085;
                    }

                    r1083 |= a1084;
                } // end may a1084

                r1083 = r1083 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DEFAULT"));
                if (r1083)
                { // may a1086
                    bool a1086 = false;
                    {
                        Checkpoint(parser); // r1087

                        bool r1087 = true;
                        r1087 = r1087 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1087, parser);
                        a1086 = r1087;
                    }

                    r1083 |= a1086;
                } // end may a1086

                r1083 = r1083 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Expression());
                CommitOrRollback(r1083, parser);
                res = r1083;
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
                Checkpoint(parser); // r1088

                bool r1088 = true;
                r1088 = r1088 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CONSTRAINT"));
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

                r1088 = r1088 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintName());
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

                CommitOrRollback(r1088, parser);
                res = r1088;
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
                Checkpoint(parser); // r1093

                bool r1093 = true;
                r1093 = r1093 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"IDENTITY"));
                if (r1093)
                { // may a1094
                    bool a1094 = false;
                    {
                        Checkpoint(parser); // r1095

                        bool r1095 = true;
                        if (r1095)
                        { // may a1096
                            bool a1096 = false;
                            {
                                Checkpoint(parser); // r1097

                                bool r1097 = true;
                                r1097 = r1097 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1097, parser);
                                a1096 = r1097;
                            }

                            r1095 |= a1096;
                        } // end may a1096

                        r1095 = r1095 && Match(parser, new Jhu.Graywulf.Sql.Parsing.FunctionArguments());
                        CommitOrRollback(r1095, parser);
                        a1094 = r1095;
                    }

                    r1093 |= a1094;
                } // end may a1094

                CommitOrRollback(r1093, parser);
                res = r1093;
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
                Checkpoint(parser); // r1098

                bool r1098 = true;
                if (r1098)
                { // may a1099
                    bool a1099 = false;
                    {
                        Checkpoint(parser); // r1100

                        bool r1100 = true;
                        r1100 = r1100 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1100, parser);
                        a1099 = r1100;
                    }

                    r1098 |= a1099;
                } // end may a1099

                r1098 = r1098 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintSpecification());
                CommitOrRollback(r1098, parser);
                res = r1098;
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
                Checkpoint(parser); // r1101

                bool r1101 = true;
                if (r1101)
                { // may a1102
                    bool a1102 = false;
                    {
                        Checkpoint(parser); // r1103

                        bool r1103 = true;
                        r1103 = r1103 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintNameSpecification());
                        CommitOrRollback(r1103, parser);
                        a1102 = r1103;
                    }

                    r1101 |= a1102;
                } // end may a1102

                r1101 = r1101 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ConstraintSpecification());
                if (r1101)
                { // may a1104
                    bool a1104 = false;
                    {
                        Checkpoint(parser); // r1105

                        bool r1105 = true;
                        r1105 = r1105 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1105, parser);
                        a1104 = r1105;
                    }

                    r1101 |= a1104;
                } // end may a1104

                r1101 = r1101 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1101)
                { // may a1106
                    bool a1106 = false;
                    {
                        Checkpoint(parser); // r1107

                        bool r1107 = true;
                        r1107 = r1107 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1107, parser);
                        a1106 = r1107;
                    }

                    r1101 |= a1106;
                } // end may a1106

                r1101 = r1101 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                if (r1101)
                { // may a1108
                    bool a1108 = false;
                    {
                        Checkpoint(parser); // r1109

                        bool r1109 = true;
                        r1109 = r1109 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1109, parser);
                        a1108 = r1109;
                    }

                    r1101 |= a1108;
                } // end may a1108

                r1101 = r1101 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                CommitOrRollback(r1101, parser);
                res = r1101;
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
                Checkpoint(parser); // r1110

                bool r1110 = true;
                if (r1110)
                { // alternatives a1111 must
                    bool a1111 = false;
                    if (!a1111)
                    {
                        Checkpoint(parser); // r1112

                        bool r1112 = true;
                        r1112 = r1112 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"PRIMARY"));
                        r1112 = r1112 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r1112 = r1112 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"KEY"));
                        CommitOrRollback(r1112, parser);
                        a1111 = r1112;
                    }

                    if (!a1111)
                    {
                        Checkpoint(parser); // r1113

                        bool r1113 = true;
                        r1113 = r1113 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNIQUE"));
                        CommitOrRollback(r1113, parser);
                        a1111 = r1113;
                    }

                    r1110 &= a1111;

                } // end alternatives a1111

                if (r1110)
                { // may a1114
                    bool a1114 = false;
                    {
                        Checkpoint(parser); // r1115

                        bool r1115 = true;
                        r1115 = r1115 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r1115)
                        { // alternatives a1116 must
                            bool a1116 = false;
                            if (!a1116)
                            {
                                Checkpoint(parser); // r1117

                                bool r1117 = true;
                                r1117 = r1117 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLUSTERED"));
                                CommitOrRollback(r1117, parser);
                                a1116 = r1117;
                            }

                            if (!a1116)
                            {
                                Checkpoint(parser); // r1118

                                bool r1118 = true;
                                r1118 = r1118 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NONCLUSTERED"));
                                CommitOrRollback(r1118, parser);
                                a1116 = r1118;
                            }

                            r1115 &= a1116;

                        } // end alternatives a1116

                        CommitOrRollback(r1115, parser);
                        a1114 = r1115;
                    }

                    r1110 |= a1114;
                } // end may a1114

                CommitOrRollback(r1110, parser);
                res = r1110;
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
                Checkpoint(parser); // r1119

                bool r1119 = true;
                r1119 = r1119 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DROP"));
                r1119 = r1119 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1119 = r1119 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r1119)
                { // may a1120
                    bool a1120 = false;
                    {
                        Checkpoint(parser); // r1121

                        bool r1121 = true;
                        r1121 = r1121 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1121, parser);
                        a1120 = r1121;
                    }

                    r1119 |= a1120;
                } // end may a1120

                r1119 = r1119 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1119, parser);
                res = r1119;
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
                Checkpoint(parser); // r1122

                bool r1122 = true;
                r1122 = r1122 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TRUNCATE"));
                r1122 = r1122 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1122 = r1122 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"TABLE"));
                if (r1122)
                { // may a1123
                    bool a1123 = false;
                    {
                        Checkpoint(parser); // r1124

                        bool r1124 = true;
                        r1124 = r1124 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1124, parser);
                        a1123 = r1124;
                    }

                    r1122 |= a1123;
                } // end may a1123

                r1122 = r1122 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1122, parser);
                res = r1122;
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
                Checkpoint(parser); // r1125

                bool r1125 = true;
                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CREATE"));
                if (r1125)
                { // may a1126
                    bool a1126 = false;
                    {
                        Checkpoint(parser); // r1127

                        bool r1127 = true;
                        r1127 = r1127 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        r1127 = r1127 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"UNIQUE"));
                        CommitOrRollback(r1127, parser);
                        a1126 = r1127;
                    }

                    r1125 |= a1126;
                } // end may a1126

                if (r1125)
                { // may a1128
                    bool a1128 = false;
                    {
                        Checkpoint(parser); // r1129

                        bool r1129 = true;
                        r1129 = r1129 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        if (r1129)
                        { // alternatives a1130 must
                            bool a1130 = false;
                            if (!a1130)
                            {
                                Checkpoint(parser); // r1131

                                bool r1131 = true;
                                r1131 = r1131 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"CLUSTERED"));
                                CommitOrRollback(r1131, parser);
                                a1130 = r1131;
                            }

                            if (!a1130)
                            {
                                Checkpoint(parser); // r1132

                                bool r1132 = true;
                                r1132 = r1132 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"NONCLUSTERED"));
                                CommitOrRollback(r1132, parser);
                                a1130 = r1132;
                            }

                            r1129 &= a1130;

                        } // end alternatives a1130

                        CommitOrRollback(r1129, parser);
                        a1128 = r1129;
                    }

                    r1125 |= a1128;
                } // end may a1128

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INDEX"));
                if (r1125)
                { // may a1133
                    bool a1133 = false;
                    {
                        Checkpoint(parser); // r1134

                        bool r1134 = true;
                        r1134 = r1134 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1134, parser);
                        a1133 = r1134;
                    }

                    r1125 |= a1133;
                } // end may a1133

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexName());
                if (r1125)
                { // may a1135
                    bool a1135 = false;
                    {
                        Checkpoint(parser); // r1136

                        bool r1136 = true;
                        r1136 = r1136 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1136, parser);
                        a1135 = r1136;
                    }

                    r1125 |= a1135;
                } // end may a1135

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                if (r1125)
                { // may a1137
                    bool a1137 = false;
                    {
                        Checkpoint(parser); // r1138

                        bool r1138 = true;
                        r1138 = r1138 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1138, parser);
                        a1137 = r1138;
                    }

                    r1125 |= a1137;
                } // end may a1137

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                if (r1125)
                { // may a1139
                    bool a1139 = false;
                    {
                        Checkpoint(parser); // r1140

                        bool r1140 = true;
                        r1140 = r1140 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1140, parser);
                        a1139 = r1140;
                    }

                    r1125 |= a1139;
                } // end may a1139

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                if (r1125)
                { // may a1141
                    bool a1141 = false;
                    {
                        Checkpoint(parser); // r1142

                        bool r1142 = true;
                        r1142 = r1142 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1142, parser);
                        a1141 = r1142;
                    }

                    r1125 |= a1141;
                } // end may a1141

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                if (r1125)
                { // may a1143
                    bool a1143 = false;
                    {
                        Checkpoint(parser); // r1144

                        bool r1144 = true;
                        r1144 = r1144 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1144, parser);
                        a1143 = r1144;
                    }

                    r1125 |= a1143;
                } // end may a1143

                r1125 = r1125 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                if (r1125)
                { // may a1145
                    bool a1145 = false;
                    {
                        Checkpoint(parser); // r1146

                        bool r1146 = true;
                        if (r1146)
                        { // may a1147
                            bool a1147 = false;
                            {
                                Checkpoint(parser); // r1148

                                bool r1148 = true;
                                r1148 = r1148 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1148, parser);
                                a1147 = r1148;
                            }

                            r1146 |= a1147;
                        } // end may a1147

                        r1146 = r1146 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INCLUDE"));
                        if (r1146)
                        { // may a1149
                            bool a1149 = false;
                            {
                                Checkpoint(parser); // r1150

                                bool r1150 = true;
                                r1150 = r1150 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1150, parser);
                                a1149 = r1150;
                            }

                            r1146 |= a1149;
                        } // end may a1149

                        r1146 = r1146 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketOpen());
                        if (r1146)
                        { // may a1151
                            bool a1151 = false;
                            {
                                Checkpoint(parser); // r1152

                                bool r1152 = true;
                                r1152 = r1152 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1152, parser);
                                a1151 = r1152;
                            }

                            r1146 |= a1151;
                        } // end may a1151

                        r1146 = r1146 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IncludedColumnList());
                        if (r1146)
                        { // may a1153
                            bool a1153 = false;
                            {
                                Checkpoint(parser); // r1154

                                bool r1154 = true;
                                r1154 = r1154 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1154, parser);
                                a1153 = r1154;
                            }

                            r1146 |= a1153;
                        } // end may a1153

                        r1146 = r1146 && Match(parser, new Jhu.Graywulf.Sql.Parsing.BracketClose());
                        CommitOrRollback(r1146, parser);
                        a1145 = r1146;
                    }

                    r1125 |= a1145;
                } // end may a1145

                CommitOrRollback(r1125, parser);
                res = r1125;
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
                Checkpoint(parser); // r1155

                bool r1155 = true;
                r1155 = r1155 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumn());
                if (r1155)
                { // may a1156
                    bool a1156 = false;
                    {
                        Checkpoint(parser); // r1157

                        bool r1157 = true;
                        if (r1157)
                        { // may a1158
                            bool a1158 = false;
                            {
                                Checkpoint(parser); // r1159

                                bool r1159 = true;
                                r1159 = r1159 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1159, parser);
                                a1158 = r1159;
                            }

                            r1157 |= a1158;
                        } // end may a1158

                        r1157 = r1157 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1157)
                        { // may a1160
                            bool a1160 = false;
                            {
                                Checkpoint(parser); // r1161

                                bool r1161 = true;
                                r1161 = r1161 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1161, parser);
                                a1160 = r1161;
                            }

                            r1157 |= a1160;
                        } // end may a1160

                        r1157 = r1157 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexColumnList());
                        CommitOrRollback(r1157, parser);
                        a1156 = r1157;
                    }

                    r1155 |= a1156;
                } // end may a1156

                CommitOrRollback(r1155, parser);
                res = r1155;
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
                Checkpoint(parser); // r1162

                bool r1162 = true;
                r1162 = r1162 && Match(parser, new Jhu.Graywulf.Sql.Parsing.ColumnName());
                if (r1162)
                { // may a1163
                    bool a1163 = false;
                    {
                        Checkpoint(parser); // r1164

                        bool r1164 = true;
                        if (r1164)
                        { // may a1165
                            bool a1165 = false;
                            {
                                Checkpoint(parser); // r1166

                                bool r1166 = true;
                                r1166 = r1166 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1166, parser);
                                a1165 = r1166;
                            }

                            r1164 |= a1165;
                        } // end may a1165

                        if (r1164)
                        { // alternatives a1167 must
                            bool a1167 = false;
                            if (!a1167)
                            {
                                Checkpoint(parser); // r1168

                                bool r1168 = true;
                                r1168 = r1168 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ASC"));
                                CommitOrRollback(r1168, parser);
                                a1167 = r1168;
                            }

                            if (!a1167)
                            {
                                Checkpoint(parser); // r1169

                                bool r1169 = true;
                                r1169 = r1169 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DESC"));
                                CommitOrRollback(r1169, parser);
                                a1167 = r1169;
                            }

                            r1164 &= a1167;

                        } // end alternatives a1167

                        CommitOrRollback(r1164, parser);
                        a1163 = r1164;
                    }

                    r1162 |= a1163;
                } // end may a1163

                CommitOrRollback(r1162, parser);
                res = r1162;
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

                        r1172 = r1172 && Match(parser, new Jhu.Graywulf.Sql.Parsing.Comma());
                        if (r1172)
                        { // may a1175
                            bool a1175 = false;
                            {
                                Checkpoint(parser); // r1176

                                bool r1176 = true;
                                r1176 = r1176 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                                CommitOrRollback(r1176, parser);
                                a1175 = r1176;
                            }

                            r1172 |= a1175;
                        } // end may a1175

                        r1172 = r1172 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IncludedColumnList());
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
                Checkpoint(parser); // r1177

                bool r1177 = true;
                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"DROP"));
                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"INDEX"));
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

                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Sql.Parsing.IndexName());
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

                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Parsing.Literal(@"ON"));
                if (r1177)
                { // may a1182
                    bool a1182 = false;
                    {
                        Checkpoint(parser); // r1183

                        bool r1183 = true;
                        r1183 = r1183 && Match(parser, new Jhu.Graywulf.Sql.Parsing.CommentOrWhitespace());
                        CommitOrRollback(r1183, parser);
                        a1182 = r1183;
                    }

                    r1177 |= a1182;
                } // end may a1182

                r1177 = r1177 && Match(parser, new Jhu.Graywulf.Sql.Parsing.TableOrViewName());
                CommitOrRollback(r1177, parser);
                res = r1177;
            }



            return res;
        }
    }


}