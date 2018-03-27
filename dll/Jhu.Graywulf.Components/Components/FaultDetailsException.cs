using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;

namespace Jhu.Graywulf.Components
{
    /// <summary>
    /// This exception is used to proxy WCF ExceptionDetail information
    /// across AppDomains.
    /// </summary>
    /// <remarks>
    /// The class is used because ExceptionDetail is not serializable.
    /// </remarks>
    [Serializable]
    public class FaultDetailsException : Exception
    {
        private string action;
        private FaultCode code;
        private FaultReason reason;
        private string source;
        private string stackTrace;
        private string type;

        public string Action
        {
            get { return action; }
            set { action = value; }
        }

        public FaultCode Code
        {
            get { return code; }
            set { code = value; }
        }

        public FaultReason Reason
        {
            get { return reason; }
            set { reason = value; }
        }

        public override string Source
        {
            get { return source; }
            set { source = value; }
        }

        public override string StackTrace
        {
            get { return stackTrace; }
        }

        public FaultDetailsException()
        {
            InitializeMembers();
        }

        protected FaultDetailsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            InitializeMembers();
        }

        public FaultDetailsException(FaultException<ExceptionDetail> ex)
            : base(ex.Detail.Message)
        {
            this.action = ex.Action;
            this.code = ex.Code;
            this.reason = ex.Reason;
            this.source = ex.Source;
            this.stackTrace = ex.Detail.StackTrace;
            this.type = ex.Detail.Type;

            foreach (var key in ex.Data.Keys)
            {
                this.Data.Add(key, ex.Data[key]);
            }
        }

        private void InitializeMembers()
        {
            this.action = null;
            this.code = null;
            this.reason = null;
            this.source = null;
            this.stackTrace = null;
            this.type = null;
        }
    }
}
