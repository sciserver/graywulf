    public partial class __Name__ : __LibNamespace__.Symbol, ICloneable
    {
        protected override string Pattern
        {
            get { return @"__Pattern__"; }
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

        public static __Name__ Create()
        {
            var s = new __Name__();
            s.Value = @"__Pattern__";
            return s;
        }

        public override object Clone()
        {
            return new __Name__(this);
        }
    }