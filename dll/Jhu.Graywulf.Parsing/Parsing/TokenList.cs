using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Parsing
{
    public class TokenList : IEnumerable<Token>
    {
        public enum Direction
        {
            Forward,
            Backward,
        }

        public class TokenListEnumerator : IEnumerable<Token>, IEnumerator<Token>
        {
            private Direction direction;
            private int current;
            private TokenList tokenList;

            public TokenListEnumerator(TokenList tokenList, Direction direction)
            {
                this.direction = direction;
                this.tokenList = tokenList;

                this.current = -1;
            }

            public Token Current
            {
                get
                {
                    if (current == -1)
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        return tokenList.tokens[current];
                    }
                }
            }

            object IEnumerator.Current
            {
                get { return Current; }
            }

            public void Dispose()
            {
            }

            public IEnumerator<Token> GetEnumerator()
            {
                return this;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this;
            }

            public bool MoveNext()
            {
                if (current == -1)
                {
                    switch (direction)
                    {
                        case Direction.Forward:
                            current = 0;
                            return current < tokenList.tokens.Count;
                        case Direction.Backward:
                            current = tokenList.tokens.Count - 1;
                            return 0 <= current;
                        default:
                            throw new NotImplementedException();
                    }
                }
                else
                {
                    switch (direction)
                    {
                        case Direction.Forward:
                            current++;
                            return current < tokenList.tokens.Count;
                        case Direction.Backward:
                            current--;
                            return 0 <= current;
                        default:
                            throw new NotImplementedException();
                    }
                }
            }

            public void Reset()
            {
                throw new NotImplementedException();
            }
        }

        private List<Token> tokens;

        public TokenListEnumerator Forward
        {
            get { return new TokenListEnumerator(this, Direction.Forward); }
        }

        public TokenListEnumerator Backward
        {
            get { return new TokenListEnumerator(this, Direction.Backward); }
        }

        public int Count
        {
            get { return tokens.Count; }
        }

        public Token First
        {
            get { return tokens[0]; }
        }

        public Token Last
        {
            get { return tokens[tokens.Count - 1]; }
        }

        public Token this[int index]
        {
            get { return tokens[index]; }
            set { tokens[index] = value; }
        }

        public TokenList()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.tokens = new List<Token>();
        }

        public void AddLast(Token token)
        {
            this.tokens.Add(token);
        }

        public void AddLast(IEnumerable<Token> tokens)
        {
            this.tokens.AddRange(tokens);
        }
        
        public void AddFirst(Token token)
        {
            tokens.Insert(0, token);
        }

        public int Insert(int index, Token token)
        {
            tokens.Insert(index, token);
            return index;
        }

        public int IndexOf(Token token)
        {
            return tokens.IndexOf(token);
        }

        public Token RemoveLast()
        {
            if (this.tokens.Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            else
            {
                var idx = this.tokens.Count - 1;
                var t = this.tokens[idx];
                this.tokens.RemoveAt(idx);
                return t;
            }
        }

        public void Remove(Token token)
        {
            tokens.Remove(token);
        }

        public void Replace(Token old, Token other)
        {
            if (old != other)
            {
                var idx = tokens.IndexOf(old);
                tokens[idx] = other;
            }
        }

        public void Replace<T>(Token other)
        {
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i] is T)
                {
                    tokens[i] = other;
                    break;
                }
            }
        }

        public IEnumerator<Token> GetEnumerator()
        {
            return new TokenListEnumerator(this, Direction.Forward);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new TokenListEnumerator(this, Direction.Forward);
        }
    }
}
