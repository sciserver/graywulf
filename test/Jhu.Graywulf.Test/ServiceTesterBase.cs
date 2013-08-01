using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jhu.Graywulf.Components;


namespace Jhu.Graywulf
{
    [Serializable]
    public abstract class ServiceTesterBase : MarshalByRefObject
    {
        private object syncRoot;
        private bool isRunning;
        private HashSet<ServiceTesterToken> tokens;

        private ManualResetEvent nonexclusive;
        private AutoResetEvent exclusive;

        protected object SyncRoot
        {
            get { return syncRoot; }
        }

        protected bool IsRunning
        {
            get
            {
                lock (syncRoot)
                {
                    return isRunning;
                }
            }
            set
            {
                lock (syncRoot)
                {
                    isRunning = value; 
                }
            }
        }

        public ServiceTesterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.syncRoot = new object();
            this.isRunning = false;
            this.tokens = new HashSet<ServiceTesterToken>();

            this.nonexclusive = new ManualResetEvent(true);
            this.exclusive = new AutoResetEvent(true);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public ServiceTesterToken GetToken()
        {
            lock (syncRoot)
            {
                nonexclusive.WaitOne();

                var token = new ServiceTesterToken(this, false);
                tokens.Add(token);

                exclusive.Reset();
                
                return token;
            }
        }

        public ServiceTesterToken GetExclusiveToken()
        {
            lock (syncRoot)
            {
                exclusive.WaitOne();

                var token = new ServiceTesterToken(this, true);
                tokens.Add(token);

                exclusive.Reset();
                nonexclusive.Reset();

                return token;
            }
        }

        public void ReleaseToken(ServiceTesterToken token)
        {
            lock (syncRoot)
            {
                tokens.Remove(token);

                if (token.IsExclusive)
                {
                    exclusive.Set();
                    nonexclusive.Set();
                }
                else if (tokens.Count == 0)
                {
                    exclusive.Set();
                }
            }
        }

        public void Start()
        {
            lock (syncRoot)
            {
                if (!IsRunning)
                {
                    isRunning = true;
                    OnStart();
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
        }

        public void Stop()
        {
            lock (syncRoot)
            {
                if (!IsRunning)
                {
                    throw new InvalidOperationException();
                }
                else
                {
                    OnStop();
                    isRunning = false;
                }
            }
        }

        public void EnsureRunning()
        {
            lock (syncRoot)
            {
                if (!IsRunning)
                {
                    isRunning = true;
                    OnStart();
                }
            }
        }

        protected abstract void OnStart();

        protected abstract void OnStop();
    }
}
