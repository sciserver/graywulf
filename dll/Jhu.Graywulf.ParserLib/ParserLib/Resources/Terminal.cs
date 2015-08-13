    public partial class [$Name] : [$LibNamespace].Terminal, ICloneable
    {
        private static Regex pattern = new Regex(@"[$Pattern]", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public [$Name]()
            :base()
        {
            Value = @"[$Pattern]";
        }

        public [$Name]([$Name] old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new [$Name](this);
        }
    }