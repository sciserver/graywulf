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

        private static ThreadLocal<AmbientContextStore> threadLocalContexts = new ThreadLocal<AmbientContextStore>();
        private static AsyncLocal<AmbientContextStore> asyncLocalContexts = new AsyncLocal<AmbientContextStore>();

        private AmbientContextSupport support;
        private AmbientContextBase outerContext;
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

        protected AmbientContextBase(AmbientContextSupport support)
        {
            InitializeMembers();

            this.support = support;

            Push();
        }

        [OnDeserializing]
        private void InitializeMembers()
        {
            this.support = AmbientContextSupport.None;
            this.outerContext = null;
            this.contextGuid = Guid.NewGuid();
            this.isValid = true;
        }

        private void CopyMembers(AmbientContextBase old)
        {
            this.support = old.support;
            this.outerContext = old.outerContext;
            this.contextGuid = Guid.NewGuid();
            this.isValid = old.isValid;
        }

        public virtual void Dispose()
        {
            Pop();

            isValid = false;
        }

        private static bool IsThreadLocalSupported(AmbientContextSupport support)
        {
            return support.HasFlag(AmbientContextSupport.ThreadLocal);
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
                store = new AmbientContextStore();
                threadLocalContexts.Value = store;
            }

            return store;
        }

        private static bool IsAsyncLocalSupported(AmbientContextSupport support)
        {
            return support.HasFlag(AmbientContextSupport.AsyncLocal);
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
                store = new AmbientContextStore();
                asyncLocalContexts.Value = store;
            }

            return store;
        }

        private static bool IsHttpContextSupported(AmbientContextSupport support)
        {
            return support.HasFlag(AmbientContextSupport.WebHttpContext) &&
                System.Web.HttpContext.Current != null;
        }

        private static AmbientContextStore GetHttpContextStore(bool create)
        {
            AmbientContextStore store = null;

            store = (AmbientContextStore)System.Web.HttpContext.Current.Items[AmbientContextStoreKey];

            if (create && store == null)
            {
                store = new AmbientContextStore();
                System.Web.HttpContext.Current.Items[AmbientContextStoreKey] = store;
            }

            return store;
        }

        private static bool IsOperationContextSupported(AmbientContextSupport support)
        {
            return support.HasFlag(AmbientContextSupport.WcfOperationContext) &&
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

        private static IEnumerable<AmbientContextStore> EnumerateStores(AmbientContextSupport support, bool create)
        {
            AmbientContextStore store;

            if (IsOperationContextSupported(support))
            {
                store = GetOperationContextStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }

            if (IsHttpContextSupported(support))
            {
                store = GetHttpContextStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }

            if (IsAsyncLocalSupported(support))
            {
                store = GetAsyncLocalStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }

            if (IsThreadLocalSupported(support))
            {
                store = GetThreadLocalStore(create);

                if (store != null)
                {
                    yield return store;
                }
            }
        }

        /// <summary>
        /// Puts the context into all supported stores
        /// </summary>
        private void Push()
        {
            var type = this.GetType();

            foreach (var store in EnumerateStores(support, true))
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
        private void Pop()
        {
            var type = this.GetType();
            
            foreach (var store in EnumerateStores(support, false))
            {
                var key = store.Find(type);

                if (key == null)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    store.Remove(key);
                }

                if (outerContext != null)
                {
                    store.Add(outerContext.GetType(), outerContext);
                }
            }
        }

        protected static T Get<T>(AmbientContextSupport support)
            where T : AmbientContextBase
        {
            var type = typeof(T);

            foreach (var store in EnumerateStores(support, false))
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
