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

        private bool isInitialized;
        private T value;

        public bool IsInitialized
        {
            get { return isInitialized; }
        }

        public T Value
        {
            get
            {
                if (!isInitialized)
                {
                    InitializeValue();
                }

                return this.value;
            }
            set
            {
                this.value = value;
                this.isInitialized = true;
            }
        }

        public LazyProperty(T initialValue)
        {
            InitializeMembers();

            this.isInitialized = true;
            this.value = initialValue;
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
            this.isInitialized = false;
            this.value = default(T);
        }

        private void InitializeValue()
        {
            lock (this)
            {
                this.value = initializer();
                this.isInitialized = true;
            }
        }

        public void Clear()
        {
            lock (this)
            {
                this.isInitialized = false;
                this.value = default(T);
            }
        }
    }
}
