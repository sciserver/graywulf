using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Parsing
{
    /// <summary>
    /// Represents a parsing tree node.
    /// </summary>
    /// <remarks>
    /// All tokens matching complex rules derive from this class
    /// </remarks>
    public abstract class Node : Token, ICloneable
    {
        #region Private member variables

        private TokenStack stack;

        #endregion
        #region Properties

        public TokenStack Stack
        {
            get { return stack; }
        }

        /// <summary>
        /// Enumerates the child nodes
        /// </summary>
        /// <remarks>
        /// This enumerator performs an automatic roll-up of recursive rules.
        /// </remarks>
        public IEnumerable<Token> Nodes
        {
            get
            {
                foreach (var i in stack)
                {
                    // this part rools up tree if the same node is found as child
                    if (i.GetType() == this.GetType())
                    {
                        foreach (var n in ((Node)i).Nodes)
                        {
                            yield return n;
                        }
                    }
                    else if (!(i is Comment)) // Exclude comments
                    {
                        yield return i;
                    }
                }
            }
        }

        public override string Value
        {
            get
            {
                var sb = new StringBuilder();
                foreach (var n in stack)
                {
                    sb.Append(n.Value);
                }
                return sb.ToString();
            }
            set
            {
                throw new InvalidOperationException();
            }
        }

        #endregion
        #region Constructors and initializers

        protected Node()
            :base()
        {
        }

        protected Node(Node old)
            :base(old)
        {
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();

            this.stack = new TokenStack();
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (Node)other;

            this.stack = new TokenStack();

            foreach (var t in old.stack)
            {
                var nt = (Token)t.Clone();
                nt.Parent = this;
                this.stack.AddLast(nt);
            }
        }
        
        #endregion

        /// <summary>
        /// Matches a token
        /// </summary>
        /// <param name="parser"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected bool Match(Parser parser, Token token)
        {
            if (token.Match(parser))
            {
                token.Parent = this;
                stack.AddLast(token);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Creates a checkpoint.
        /// </summary>
        /// <param name="position"></param>
        protected void Checkpoint(Parser parser)
        {
            parser.Checkpoint(stack.Count);
        }

        protected void CommitOrRollback(bool res, Parser parser)
        {
            if (res)
            {
                Commit(parser);
            }
            else
            {
                Rollback(parser);
            }
        }

        /// <summary>
        /// Commits the last checkpoint
        /// </summary>
        /// <param name="position"></param>
        private void Commit(Parser parser)
        {
            parser.Commit();
        }

        /// <summary>
        /// Rolls the parsing back to the last checkpoint
        /// </summary>
        /// <param name="position"></param>
        private void Rollback(Parser parser)
        {
            var count = parser.Rollback();

            while (stack.Count > count)
            {
                stack.RemoveLast();
            }
        }

        #region Navigation function

        /// <summary>
        /// Returns the first ascendant node of type T in the parsing tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindAscendant<T>()
            where T : Node
        {
            var item = (Node)this.Parent;

            while (item != null)
            {
                //if (item.GetType is T)
                if (typeof(T).IsAssignableFrom(item.GetType()))
                {
                    return (T)item;
                }

                item = (Node)item.Parent;
            }

            return null;
        }

        /// <summary>
        /// Returns the first descendant node of type T in the parsing tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        /// <remarks>
        /// The function takes into accont first children only. Use function
        /// FindDescendantRecursive to traverse the entire subtree.
        /// </remarks>
        public T FindDescendant<T>()
            where T : Token
        {
            return (T)stack.FirstOrDefault(i => i is T);
        }

        /// <summary>
        /// Returns the
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="index"></param>
        /// <returns></returns>
        public T FindDescendant<T>(int index)
            where T:Token
        {
            var args = Nodes.Where(n => n is T).ToArray();

            if (index < args.Length)
            {
                return (T)args[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Enumerates the children of type of T of the given node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public IEnumerable<T> EnumerateDescendants<T>()
            where T : Node
        {
            return EnumerateDescendants<T>(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skipWhitespaces"></param>
        /// <returns></returns>
        /// <remarks>
        /// This function works on the rolled up collection
        /// instead of the stack!
        /// </remarks>
        public IEnumerable<T> EnumerateDescendants<T>(bool skipWhitespaces)
        {
            if (skipWhitespaces)
            {
                return Nodes.Where(i => i is T && !(i is Whitespace)).Cast<T>();
            }
            else
            {
                return Nodes.Where(i => i is T).Cast<T>();
            }
        }

        /// <summary>
        /// Returns the first descendants of type T of the given node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T FindDescendantRecursive<T>()
            where T : Token
        {
            foreach (var item in stack)
            {
                if ((item is T) && !(item is Whitespace))
                {
                    return (T)item;
                }
                if (item is Node)
                {
                    T t = ((Node)item).FindDescendantRecursive<T>();
                    if (t != null)
                    {
                        return t;
                    }
                }
            }

            return default(T);
        }

        public IEnumerable<T> EnumerateDescendantsRecursive<T>()
        {
            return EnumerateDescendantsRecursive<T>(this, null);
        }

        /// <summary>
        /// Enumerates the descendants of type T of the given node recursively by
        /// stopping at the specified type of node.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stopAtType"></param>
        /// <returns></returns>
        public IEnumerable<T> EnumerateDescendantsRecursive<T>(Type stopAtType)
        {
            return EnumerateDescendantsRecursive<T>(this, stopAtType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="node"></param>
        /// <param name="stopAtType"></param>
        /// <returns></returns>
        private IEnumerable<T> EnumerateDescendantsRecursive<T>(Node node, Type stopAtType)
        {
            if (stopAtType == null || !stopAtType.IsAssignableFrom(node.GetType()))
            {
                foreach (object n in node.Stack)
                {
                    // TODO: add parameter to skip whitespaces
                    if ((n is T) && !(n is Whitespace))
                    {
                        yield return (T)n;
                    }

                    //if (n is Node && !((stopAtType != null) && (n.GetType().Equals(stopAtType))))
                    if (n is Node)
                    {
                        foreach (T t in EnumerateDescendantsRecursive<T>((Node)n, stopAtType))
                        {
                            yield return t;
                        }
                    }
                }
            }
        }

        #endregion
        #region Interpreter functions

        internal void ExchangeChildren(Parser parser)
        {
            var item = stack.First;

            while (item != null)
            {
                var o = item.Value;

                if (o is Node)
                {
                    var node = (Node)o;
                    node.ExchangeChildren(parser);
                    item.Value = parser.Exchange(node);
                }

                item = item.Next;
            }
        }
        
        internal void InterpretChildren()
        {
            var item = stack.First;

            while (item != null)
            {
                var o = item.Value;

                if (o is Node)
                {
                    var node = (Node)o;
                    node.InterpretChildren();
                    node.Interpret();
                }

                item = item.Next;
            }
        }

        public virtual void Interpret()
        {
        }

        #endregion

        public void ExchangeWith(Node other)
        {
            this.Parent.Stack.Exchange(this, other);
        }
    }
}
