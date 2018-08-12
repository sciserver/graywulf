using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Parsing.Generator
{
    [Grammar(Namespace = "SecondInheritedNS", ParserName = "SecondInheritedParser",
        Comparer = "StringComparer.InvariantCultureIgnoreCase", RootToken = "TestNS.List")]
    class SecondInheritedGrammar : InheritedGrammar
    {
        // Overriding a rule in a rule of the topmost grammar should force overriding
        // all rules referencing it, specifically BaseRule7

        public static new Expression<Rule> BaseRule8 = () =>
            Override
            (
                Sequence
                (
                    Word, Comma, Word, Comma, Word
                )
            );
    }
}
