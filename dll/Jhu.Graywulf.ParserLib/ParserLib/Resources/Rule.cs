    public partial class [$Name] : [$InheritedType], ICloneable
    {
        public [$Name]()
            :base()
        {
        }

        public [$Name]([$Name] old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new [$Name](this);
        }

        public override bool Match([$LibNamespace].Parser parser)
        {
[$Code]
        }
    }