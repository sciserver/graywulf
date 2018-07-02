using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace __Namespace__
{
    public partial class __Name__ : __InheritedType__
    {
        private static HashSet<string> keywords = new HashSet<string>(__Comparer__)
        {
__Keywords__
        };

        public override HashSet<string> Keywords
        {
            get { return keywords; }
        }   

        public static StringComparer ComparerInstance
        {
            get { return __Comparer__; }
        }

        public override StringComparer Comparer
        {
            get { return __Comparer__; }
        }

        public override __LibNamespace__.Token Execute(string code)
        {
            return Execute(new __RootToken__(), code);
        }
    }

__Symbols__
__Terminals__
__Whitespaces__
__Comments__
__Rules__
}