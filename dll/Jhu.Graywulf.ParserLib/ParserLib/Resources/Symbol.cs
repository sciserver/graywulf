    public partial class [$Name] : [$LibNamespace].Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"[$Pattern]"; }
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