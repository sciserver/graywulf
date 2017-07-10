using System;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Logging
{
    public class LoggingContext : MarshalByRefObject, ICloneable, IDisposable
    {
        private Guid contextGuid;
        private Guid sessionGuid;
        private bool isValid;
        internal int eventOrder;

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

        /// <summary>
        /// Gets the validity of the context.
        /// </summary>
        public bool IsValid
        {
            get { return isValid; }
        }

        public LoggingContext()
        {
            InitializeMembers(new StreamingContext());
        }

        public LoggingContext(LoggingContext old)
        {
            CopyMembers(old);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.contextGuid = Guid.NewGuid();
            this.sessionGuid = Guid.Empty;
            this.isValid = true;
            this.eventOrder = 0;
        }

        private void CopyMembers(LoggingContext old)
        {
            this.contextGuid = Guid.NewGuid();
            this.sessionGuid = old.sessionGuid;
            this.isValid = true;
            this.eventOrder = 0;
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }

        public LoggingContext Clone()
        {
            return new LoggingContext(this);
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public virtual void Dispose()
        {
            isValid = false;
        }
    }
}
