using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Jhu.Graywulf.ParserLib.Test
{
    [Grammar(RootToken = "List")]
    class TestGrammar : Grammar
    {
        public static Expression<Symbol> Comma = () => @",";

        public static Expression<Whitespace> Whitespace = () => @"\G\s+";
        public static Expression<Comment> SingleLineComment = () => @"\G//.*";
        public static Expression<Comment> MultiLineComment = () => @"\G(?sm)/\*.*?\*/!";
        public static Expression<Terminal> Word = () => @"\G([a-zA-Z_]+[a-zA-Z0-9_]*)";

        public static Expression<Rule> CommentOrWhitespace = () =>
            Sequence
            (
                Must(MultiLineComment, SingleLineComment, Whitespace),
                May(CommentOrWhitespace)
            );

        public static Expression<Rule> List = () =>
            Sequence
            (
                May(CommentOrWhitespace),
                Word,
                May
                (
                    Sequence
                    (
                        May(CommentOrWhitespace),
                        Comma,
                        List
                    )
                )
            );
    }
}
