using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Components
{
    public abstract class AmbientContextBase : IDisposable
    {
        private const string AmbientContextStoreKey = "Jhu.Graywulf.Components.AmbientContextStore";

        private static AmbientContextStore staticContexts = new AmbientContextStore(AmbientContextStoreLocation.GlobalStatic);
        private static ThreadLocal<AmbientContextStore> threadLocalContexts = new ThreadLocal<AmbientContextStore>();
        private static AsyncLocal<AmbientContextStore> asyncLocalContexts = new AsyncLocal<AmbientContextStore>();

        private AmbientContextBase outerContext;
        private AmbientContextStoreLocation supportedLocation;
        private Guid contextGuid;
        private bool isValid;

        protected abstract string ContextTypeKey { get; }

        protected AmbientContextBase OuterContext
        {
            get { return outerContext; }
        }

        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        /// <summary>
        /// Gets the validity of the context.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
        }

        protected AmbientContextBase()
            : this(AmbientContextStoreLocation.Default)
        {
            // override
        }

        protected AmbientContextBase(AmbientContextStoreLocation supportedLocation)
        {
            InitializeMembers();
            this.supportedLocation = supportedLocation;
            Push();
        }

        [OnDeserializing]
        private void InitializeMembers()
        {
            this.outerContext = null;
            this.contextGuid = Guid.NewGuid();
            this.isValid = true;
        }

        private void CopyMembers(AmbientContextBase old)
        {
            this.outerContext = old.outerContext;
            this.contextGuid = Guid.NewGuid();
            this.isValid = old.isValid;
        }

        public virtual void Dispose()
        {
            Pop();

            isValid = false;
        }

        private static bool IsStaticStoreSupported(AmbientContextStoreLocation support)
        {
            return support.HasFlag(AmbientContextStoreLocation.GlobalStatic);
        }

        private static AmbientContextStore GetStaticStore(bool create)
        {
            // TODO: synchronize?
            return staticContexts;
        }

        private static bool IsThreadLocalStoreSupported(AmbientContextStoreLocation support)
        {
            return support.HasFlag(AmbientContextStoreLocation.ThreadLocal);
        }

        private static AmbientContextStore GetThreadLocalStore(bool create)
        {
            AmbientContextStore store = null;

            if (threadLocalContexts.Value != null)
            {
                store = threadLocalContexts.Value;
            }
            else if (create)
            {
                store = new AmbientContextStore(AmbientContextStoreLocation.ThreadLocal);
                threadLocalContexts.Value = store;
            }

            return store;
        }

        private static bool IsAsyncLocalStoreSupported(AmbientContextStoreLocation support)
        {
            return support.HasFlag(AmbientContextStoreLocation.AsyncLocal);
        }

        private static AmbientContextStore GetAsyncLocalStore(bool create)
        {
            AmbientContextStore store = null;

            if (asyncLocalContexts.Value != null)
            {
                store = asyncLocalContexts.Value;
            }
            else if (create)
            {
                store = new AmbientContextStore(AmbientContextStoreLocation.AsyncLocal);
                asyncLocalContexts.Value = store;
            }

            return store;
        }

        private static bool IsHttpContextStoreSupported(AmbientContextStoreLocation support)
        {
            return
                support.HasFlag(AmbientContextStoreLocation.WebHttpContext) &&
                System.Web.HttpContext.Current != null;
        }

        private static AmbientContextStore GetHttpContextStore(bool create)
        {
            AmbientContextStore store = null;

            store = (AmbientContextStore)System.Web.HttpContext.Current.Items[AmbientContextStoreKey];

            if (create && store == null)
            {
                store = new AmbientContextStore(AmbientContextStoreLocation.WebHttpContext);
                System.Web.HttpContext.Current.Items[AmbientContextStoreKey] = store;
            }

            return store;
        }

        private static bool IsOperationContextStoreSupported(AmbientContextStoreLocation support)
        {
            return
                support.HasFlag(AmbientContextStoreLocation.WcfOperationContext) &&
                System.ServiceModel.OperationContext.Current != null;
        }

        private static AmbientContextStore GetOperationContextStore(bool create)
        {
            var ext = System.ServiceModel.OperationContext.Current.InstanceContext.Extensions.Find<AmbientContextExtension>();

            if (create && ext == null)
            {
                ext = new AmbientContextExtension();
                System.ServiceModel.OperationContext.Current.InstanceContext.Extensions.Add(ext);
            }

            return ext?.Store;
        }

        private static IEnumerable<AmbientContextStore> EnumerateStores(bool create, AmbientContextStoreLocation support)
        {
            AmbientContextStore store;

            if (IsOperationContextStoreSupported(support))
            {
                store = GetOperationContextStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }

            if (IsHttpContextStoreSupported(support))
            {
                store = GetHttpContextStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }

            if (IsAsyncLocalStoreSupported(support))
            {
                store = GetAsyncLocalStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }
            else if (IsThreadLocalStoreSupported(support))
            {
                store = GetThreadLocalStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }

            if (IsStaticStoreSupported(support))
            {
                store = GetStaticStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }
        }

        /// <summary>
        /// Puts the context into all supported stores
        /// </summary>
        public void Push()
        {
            var type = this.GetType();

            foreach (var store in EnumerateStores(true, supportedLocation))
            {
                var key = store.Find(type);

                if (key != null)
                {
                    this.outerContext = store[key];
                    store.Remove(key);
                }

                store.Add(type, this);
            }
        }

        /// <summary>
        /// Takes the context from all supported stores. If the outercontext is set,
        /// puts that back.
        /// </summary>
        public void Pop()
        {
            var type = this.GetType();

            foreach (var store in EnumerateStores(false, supportedLocation))
            {
                var key = store.Find(type);

                if (key == null)
                {
                    throw new InvalidOperationException();
                }

                // Only pop if this is the current one
                if (store[key] == this)
                {
                    store.Remove(key);

                    if (outerContext != null)
                    {
                        store.Add(outerContext.GetType(), outerContext);
                    }
                }
                else
                {
                    // TODO: test this. It could happen if the context is added to
                    // thread local but there's a context switch somewhere
                    throw new InvalidOperationException();
                }
            }
        }

        protected static T Get<T>()
            where T : AmbientContextBase
        {
            var type = typeof(T);

            foreach (var store in EnumerateStores(false, AmbientContextStoreLocation.All))
            {
                var key = store.Find(type);

                if (key != null)
                {
                    return (T)store[key];
                }
            }

            return null;
        }
    }
}
