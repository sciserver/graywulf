using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components.Test
{
    public class GenericParameterHelper : IComparable, IEnumerable, ICloneable
    {
        private static Random random;

        static GenericParameterHelper()
        {
            random = new Random();
        }

        public static int NextRandom()
        {
            lock (random)
            {
                return random.Next();
            }
        }

        // Fields
        private int data;
        private IList ienumerableStore;

        public int Data
        {
            get
            {
                return this.data;
            }
            set
            {
                this.data = value;
            }
        }

        // Methods
        public GenericParameterHelper()
        {
            InitializeMembers();
        }

        public GenericParameterHelper(GenericParameterHelper old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.data = NextRandom();
            this.ienumerableStore = null;
        }

        private void CopyMembers(GenericParameterHelper old)
        {
            this.data = old.data;
            this.ienumerableStore = old.ienumerableStore;
        }

        public object Clone()
        {
            return new GenericParameterHelper(this);
        }

        public int CompareTo(object obj)
        {
            GenericParameterHelper helper = obj as GenericParameterHelper;
            if (helper == null)
            {
                throw new NotSupportedException("GenericParameterHelper object is designed to compare to objects of GenericParameterHelper type only.");
            }
            return this.data.CompareTo(helper.data);
        }

        public override bool Equals(object obj)
        {
            GenericParameterHelper helper = obj as GenericParameterHelper;
            if (helper == null)
            {
                return false;
            }
            return (this.data == helper.data);
        }

        public IEnumerator GetEnumerator()
        {
            int capacity = this.Data % 10;
            if (this.ienumerableStore == null)
            {
                this.ienumerableStore = new List<object>(capacity);
                for (int i = 0; i < capacity; i++)
                {
                    this.ienumerableStore.Add(new object());
                }
            }
            return this.ienumerableStore.GetEnumerator();
        }

        public override int GetHashCode()
        {
            return this.Data.GetHashCode();
        }

        public override string ToString()
        {
            return data.ToString();
        }
    }

 

}
