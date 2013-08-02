    public partial class [$Name] : [$LibNamespace].Whitespace
    {
        private static Regex pattern = new Regex(@"[$Pattern]", RegexOptions.Compiled);

        protected override Regex Pattern
        {
            get { return pattern; }
        }
    }