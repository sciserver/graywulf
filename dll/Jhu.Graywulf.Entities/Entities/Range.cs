using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities
{
    public class Range<T> : IRange
        where T : struct
    {
        private T? from;
        private T? to;

        public T? From
        {
            get { return from; }
            set { from = value; }
        }

        public T? To
        {
            get { return to; }
            set { to = value; }
        }

        public Range()
        {
            this.from = null;
            this.to = null;
        }

        public Range(T from, T to)
        {
            this.from = from;
            this.to = to;
        }

        public Range(T? from, T? to)
        {
            this.from = from;
            this.to = to;
        }

        object IRange.From
        {
            get { return from; }
            set { from = (T)value; }
        }

        object IRange.To
        {
            get { return to; }
            set { to = (T)value; }
        }
    }
}
