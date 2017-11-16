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

        public ServiceLoggingContext(AmbientContextSupport support)
            : base(support)
        {
            if (OuterContext is ServiceLoggingContext)
            {
                CopyMembers((ServiceLoggingContext)OuterContext);
            }
            else
            {
                InitializeMembers(new StreamingContext());
            }
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
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
