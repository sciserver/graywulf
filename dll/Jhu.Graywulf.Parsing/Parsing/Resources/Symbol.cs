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

        public static [$Name] Create()
        {
            var s = new [$Name]();
            s.Value = @"[$Pattern]";
            return s;
        }

        public override object Clone()
        {
            return new [$Name](this);
        }
    }