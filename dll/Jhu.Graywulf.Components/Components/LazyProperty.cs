using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Components
{
    [Serializable]
    public class LazyProperty<T>
    {
        [NonSerialized]
        private Func<T> initializer;

        private bool isValueLoaded;
        private T value;

        public T Value
        {
            get
            {
                if (!isValueLoaded)
                {
                    LoadValue();
                }

                return this.value;
            }
            set
            {
                this.value = value;
                this.isValueLoaded = true;
            }
        }

        public LazyProperty(Func<T> initializer)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException("initializer");
            }

            InitializeMembers();

            this.initializer = initializer;
        }

        private void InitializeMembers()
        {
            this.initializer = null;
            this.isValueLoaded = false;
            this.value = default(T);
        }

        private void LoadValue()
        {
            lock (this)
            {
                this.value = initializer();
                this.isValueLoaded = true;
            }
        }

        public void Clear()
        {
            lock (this)
            {
                this.isValueLoaded = false;
                this.value = default(T);
            }
        }
    }
}
