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

        public static new Expression<Rule> BaseRule2 = () => Inherit();

        public static Expression<Rule> NewRule1 = () => Inherit(BaseRule2);

        public static Expression<Rule> NewRule2 = () =>
            Inherit
            (
                BaseRule2,
                Sequence
                (
                    Word, Comma, Comma, Word
                )
            );

        public static Expression<Rule> VeryNewRule = () =>
            Sequence(Word, CommentOrWhitespace, Word);

        public static Expression<Rule> InheritedNewRule1 = () => Inherit(VeryNewRule);

        public static Expression<Rule> InheritedNewRule2 = () => 
            Inherit
            (
                VeryNewRule,
                Sequence(Word, CommentOrWhitespace, Word, CommentOrWhitespace, Word)
            );
    }
}
