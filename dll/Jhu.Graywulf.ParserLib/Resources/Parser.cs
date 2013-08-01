using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace [$Namespace]
{
    public partial class [$Name] : [$InheritedType]
    {
        private static HashSet<string> keywords = new HashSet<string>([$Comparer])
        {
[$Keywords]
        };

        public override HashSet<string> Keywords
        {
            get { return keywords; }
        }   

        public static StringComparer ComparerInstance
        {
            get { return [$Comparer]; }
        }

        public override StringComparer Comparer
        {
            get { return [$Comparer]; }
        }

        public override [$LibNamespace].Token Execute(string code)
        {
            return Execute(new [$RootToken](), code);
        }
    }

[$Symbols]
[$Terminals]
[$Whitespaces]
[$Comments]
[$Rules]
}