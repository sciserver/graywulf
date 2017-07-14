using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Jhu.Graywulf.Logging
{
    public class WcfLoggingContext : LoggingContext
    {
        #region Singleton access

        public static new WcfLoggingContext Current
        {
            get
            {
                return LoggingContext.Current as WcfLoggingContext;
            }
        }

        #endregion

        public WcfLoggingContext(LoggingContext outerContext)
            : base(outerContext)
        {
            InitializeMembers(new StreamingContext());
        }

        public WcfLoggingContext(WcfLoggingContext outerContext)
            : base(outerContext)
        {
            CopyMembers(outerContext);
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        private void CopyMembers(WcfLoggingContext outerContext)
        {
        }

        public override void UpdateEvent(Event e)
        {
            base.UpdateEvent(e);

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
