    public partial class __Name__ : __LibNamespace__.Comment, ICloneable
    {
        private static Regex pattern = new Regex(@"__Pattern__", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }

        public __Name__()
            :base()
        {
        }

        public __Name__(__Name__ old)
            :base(old)
        {
        }

        public override object Clone()
        {
            return new __Name__(this);
        }
    }