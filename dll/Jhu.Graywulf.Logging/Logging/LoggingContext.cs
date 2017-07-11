using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Activities.Tracking;


namespace Jhu.Graywulf.Logging
{
    public class LoggingContext : IDisposable
    {
        #region Singleton

        [ThreadStatic]
        private static LoggingContext context;

        public static LoggingContext Current
        {
            get
            {
                if (context == null)
                {
                    context = new LoggingContext();
                }

                return context;
            }
            set
            {
                context = value;
            }
        }

        #endregion

        private LoggingContext outerContext;
        private bool isValid;
        private bool isAsync;
        private Guid contextGuid;
        private Guid sessionGuid;
        private EventSource eventSource;
        private List<Event> asyncEvents;

        /// <summary>
        /// Gets the validity of the context.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
        }

        /// <summary>
        /// Gets if the logging context is inside an async activity
        /// </summary>
        public bool IsAsync
        {
            get { return isAsync; }
        }

        /// <summary>
        /// Gets or sets the guid of this context.
        /// </summary>
        public Guid ContextGuid
        {
            get { return contextGuid; }
            set { contextGuid = value; }
        }

        public Guid SessionGuid
        {
            get { return sessionGuid; }
            set { sessionGuid = value; }
        }

        protected List<Event> AsyncEvents
        {
            get { return asyncEvents; }
        }

        #region Constructors and initializers

        protected LoggingContext()
            : this(false)
        {
        }

        protected LoggingContext(bool isAsync)
        {
            InitializeMembers();

            if (isAsync)
            {
                InitializeAsyncMode();
            }
        }

        protected LoggingContext(LoggingContext outerContext)
        {
            if (outerContext != null)
            {
                CopyMembers(outerContext);
            }
            else
            {
                InitializeMembers();
            }

            if (isAsync)
            {
                InitializeAsyncMode();
            }
        }

        private void InitializeMembers()
        {
            this.outerContext = null;
            this.isValid = true;
            this.isAsync = false;
            this.contextGuid = Guid.NewGuid();
            this.sessionGuid = Guid.Empty;
            this.eventSource = EventSource.None;
            this.asyncEvents = null;
        }

        private void InitializeAsyncMode()
        {
            this.isAsync = true;
            this.asyncEvents = new List<Event>();
        }

        private void CopyMembers(LoggingContext outerContext)
        {
            this.outerContext = outerContext;
            this.isValid = true;
            this.isAsync = outerContext.isAsync;
            this.contextGuid = Guid.NewGuid();
            this.sessionGuid = outerContext.sessionGuid;
            this.eventSource = outerContext.eventSource;
            this.asyncEvents = null;
        }

        public virtual void Dispose()
        {
            isValid = false;
        }

        #endregion

        public void Push()
        {
            LoggingContext.Current = this;
        }

        public void Pop()
        {
            LoggingContext.Current = this.outerContext;
        }

        public virtual void UpdateEvent(Event e)
        {
            e.ContextGuid = contextGuid;
            e.SessionGuid = sessionGuid;
            e.Source = eventSource;
        }

        /// <summary>
        /// Route the event through the pipeline, eventually generating a custom tracking record
        /// </summary>
        /// <param name="e"></param>
        public virtual void RecordEvent(Event e)
        {
            if (isAsync)
            {
                asyncEvents.Add(e);
            }
            else
            {
                Logger.Instance.WriteEvent(e);
            }
        }

        public virtual void FlushEvents()
        {
            foreach (var e in asyncEvents)
            {
                Logger.Instance.WriteEvent(e);
            }

            asyncEvents.Clear();
        }
    }
}
