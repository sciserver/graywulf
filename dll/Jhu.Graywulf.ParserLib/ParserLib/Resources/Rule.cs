    public partial class [$Name] : [$InheritedType], ICloneable
    {
        public [$Name]()
            :base()
        {
        }

        public [$Name]([$Namespace].[$Name] old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new [$Namespace].[$Name](this);
        }

        public override bool Match([$LibNamespace].Parser parser)
        {
[$Code]
        }
    }