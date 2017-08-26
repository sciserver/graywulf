using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jhu.Graywulf.Parsing.Generator
{
    [Grammar(Namespace = "InheritedNS", ParserName = "InheritedParser",
        Comparer = "StringComparer.InvariantCultureIgnoreCase", RootToken = "TestNS.List")]
    class InheritedGrammar : TestGrammar
    {
        // This will force inheriting BaseRule1 and BaseRule5
        public static new Expression<Rule> List = () =>
            Override
            (
                Sequence(List, Comma, List)
            );

        // This overrides single rule
        public static new Expression<Rule> BaseRule1 = () => 
            Override
            (
                Sequence(List, Comma, List)
            );

        // This should force inheriting BaseRule2
        public static new Expression<Rule> BaseRule3 = () =>
            Override
            (
                Sequence
                (
                    Word, Comma, Comma, Word, BaseRule4
                )
            );

        // This is a new rule, it should show up among dependencies
        // but should not force any inheritance into the current
        // namespace
        public static Expression<Rule> NewRule = () =>
            Sequence
            (
                BaseRule4
            );
    }
}
