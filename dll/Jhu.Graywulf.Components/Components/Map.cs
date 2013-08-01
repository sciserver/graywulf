using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    public class Map<T1, T2> : IEnumerable<KeyValuePair<T1, T2>>
    {
        private Dictionary<T1, T2> dict1 = new Dictionary<T1, T2>();
        private Dictionary<T2, T1> dict2 = new Dictionary<T2, T1>();

        public T2 this[T1 index]
        {
            get { return dict1[index]; }
        }

        public T1 this[T2 index]
        {
            get { return dict2[index]; }
        }

        public Map()
        {
        }

        private void InitializeMembers()
        {
            this.dict1 = new Dictionary<T1, T2>();
            this.dict2 = new Dictionary<T2, T1>();
        }

        public void Add(T1 v1, T2 v2)
        {
            dict1.Add(v1, v2);
            dict2.Add(v2, v1);
        }

        #region IEnumerable<KeyValuePair<T1,T2>> Members

        public IEnumerator<KeyValuePair<T1, T2>> GetEnumerator()
        {
            return dict1.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion
    } 

}
