using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Registry
{
    public class DiagnosticMessage : ICloneable
    {
        private string entityName;
        private string networkName;
        private string serviceName;
        private DiagnosticMessageStatus status;
        private string errorMessage;

        public string EntityName
        {
            get { return entityName; }
            set { entityName = value; }
        }

        public string NetworkName
        {
            get { return networkName; }
            set { networkName = value; }
        }

        public string ServiceName
        {
            get { return serviceName; }
            set { serviceName = value; }
        }

        public DiagnosticMessageStatus Status
        {
            get { return status; }
            set { status = value; }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set { errorMessage = value; }
        }

        public DiagnosticMessage()
        {
            InitializeMembers();
        }

        public DiagnosticMessage(DiagnosticMessage old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.entityName = String.Empty;
            this.networkName = String.Empty;
            this.serviceName = String.Empty;
            this.status = DiagnosticMessageStatus.OK;
            this.errorMessage = String.Empty;
        }

        private void CopyMembers(DiagnosticMessage old)
        {
            this.entityName = old.entityName;
            this.networkName = old.networkName;
            this.serviceName = old.serviceName;
            this.status = old.status;
            this.errorMessage = old.errorMessage;
        }

        public object Clone()
        {
            return new DiagnosticMessage(this);
        }
    }
}
