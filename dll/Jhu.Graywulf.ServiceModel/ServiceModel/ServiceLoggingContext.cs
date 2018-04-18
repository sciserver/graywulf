using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.ServiceModel
{
    public class ServiceLoggingContext : LoggingContext
    {
        #region Singleton access

        public static new ServiceLoggingContext Current
        {
            get
            {
                return LoggingContext.Current as ServiceLoggingContext;
            }
        }

        #endregion

        public ServiceLoggingContext()
            :base(false, null, AmbientContextStoreLocation.WcfOperationContext)
        {
            InitializeMembers();
        }

        public ServiceLoggingContext(ServiceLoggingContext parent)
            : base(parent)
        {
            CopyMembers(parent);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(ServiceLoggingContext outerContext)
        {
        }

        public override void UpdateEvent(Event e)
        {
            base.UpdateEvent(e);

            e.Source |= EventSource.RemoteService;

            var context = OperationContext.Current;
            string operation = null;
            string client = null;

            if (context != null)
            {
                var action = OperationContext.Current.IncomingMessageHeaders.Action;
                var operationName = action.Substring(action.LastIndexOf("/", StringComparison.OrdinalIgnoreCase) + 1);

                operation = context.Host.Description.ServiceType.FullName + "." + operationName;

                var ep = (RemoteEndpointMessageProperty)context.IncomingMessageProperties[RemoteEndpointMessageProperty.Name];

                if (ep != null)
                {
                    client = ep.Address;
                }
            }

            if (client != null) e.Client = client;
            if (operation != null) e.Operation = operation;
        }
    }
}
