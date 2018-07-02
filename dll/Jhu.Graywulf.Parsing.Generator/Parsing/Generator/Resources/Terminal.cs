    public partial class __Name__ : __LibNamespace__.Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"__Pattern__", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public __Name__()
            :base()
        {
            Value = @"__Pattern__";
        }

        public __Name__(__Name__ old)
            :base(old)
        {
        }

        public static __Name__ Create(string value)
        {
            var terminal = new __Name__();
            terminal.Value = value;
            return terminal;
        }

        public override object Clone()
        {
            return new __Name__(this);
        }
    }