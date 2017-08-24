using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jhu.Graywulf.ParserLib.Test
{
    [Grammar(Namespace = "InheritedNS", ParserName = "InheritedParser",
        Comparer = "StringComparer.InvariantCultureIgnoreCase", RootToken = "TestNS.List")]
    class InheritedGrammar : TestGrammar
    {
        // This should force inheriting BaseRule2 and BaseRule3
        public static new Expression<Rule> List = () => Inherit();

        public static Expression<Rule> List3 = () => Inherit(List);

        public static Expression<Rule> List4 = () => Inherit(List3);

        public static new Expression<Rule> BaseRule1 = () => Inherit();

        // This should force inheriting BaseRule3
        public static new Expression<Rule> BaseRule4 = () =>
            Override
            (
                Sequence(Word, Comma, Comma, Word, List)
            );

        // This should force inheriting BaseRule5
        public static new Expression<Rule> BaseRule6 = () => Inherit();
    }
}
