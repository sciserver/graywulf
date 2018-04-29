using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.ServiceModel.Web;
using System.IO;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class StreamingRawFormatterBase : RestMessageFormatter
    {
        private OperationDescription operation;
        private Uri operationUri;
        private ServiceEndpoint endpoint;
        private StreamingRawFormatterDirection direction;

        internal OperationDescription Operation
        {
            get { return operation; }
        }

        internal Uri OperationUri
        {
            get
            {
                if (operationUri == null)
                {
                    CreateOperationUri();
                }

                return operationUri;
            }
        }

        internal ServiceEndpoint Endpoint
        {
            get { return endpoint; }
        }

        internal StreamingRawFormatterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        internal abstract Type FormattedType
        {
            get;
        }

        protected StreamingRawFormatterBase()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.operation = null;
            this.endpoint = null;
            this.operationUri = null;
            this.direction = StreamingRawFormatterDirection.None;
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint, IDispatchMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);
            this.operation = operationDescription;
            this.endpoint = endpoint;
        }

        public void Initialize(OperationDescription operationDescription, ServiceEndpoint endpoint, IClientMessageFormatter fallbackFormatter)
        {
            base.Initialize(fallbackFormatter);
            this.operation = operationDescription;
            this.endpoint = endpoint;
        }

        private void CreateOperationUri()
        {
            var endpointAddress = endpoint.Address.Uri.ToString();

            if (!endpointAddress.EndsWith("/"))
            {
                endpointAddress = endpointAddress + "/";
            }

            this.operationUri = new Uri(endpointAddress + operation.Name);
        }
    }
}
