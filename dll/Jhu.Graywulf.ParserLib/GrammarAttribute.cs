using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.ParserLib
{
    /// <summary>
    /// Specifies that the marked class is a grammar.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class GrammarAttribute : Attribute
    {
        public string Namespace;
        public string ParserName;
        public string RootToken;
        public string Comparer;
    }
}
