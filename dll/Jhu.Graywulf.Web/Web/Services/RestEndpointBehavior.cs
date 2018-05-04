using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Jhu.Graywulf.Web.Services
{
    public class RestEndpointBehavior : WebHttpBehavior, IEndpointBehavior
    {
        protected override QueryStringConverter GetQueryStringConverter(OperationDescription operationDescription)
        {
            return new RestQueryStringConverter();
        }

        protected override IDispatchMessageFormatter GetRequestDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            var formatter = base.GetRequestDispatchFormatter(operationDescription, endpoint);
            formatter = CreateDispatchFormatter(operationDescription, endpoint, formatter);
            return formatter;
        }

        protected override IDispatchMessageFormatter GetReplyDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            var formatter = base.GetReplyDispatchFormatter(operationDescription, endpoint);
            formatter = CreateDispatchFormatter(operationDescription, endpoint, formatter);
            return formatter;
        }

        private IDispatchMessageFormatter CreateDispatchFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint, IDispatchMessageFormatter fallbackFormatter)
        {
            foreach (var b in operationDescription.OperationBehaviors)
            {
                if (b is StreamingRawFormatAttribute)
                {
                    var rf = (StreamingRawFormatAttribute)b;
                    var formatter = rf.CreateDispatchFormatter(operationDescription, endpoint, fallbackFormatter);
                    return formatter;
                }
            }

            return fallbackFormatter;
        }

        protected override IClientMessageFormatter GetRequestClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            var formatter = base.GetRequestClientFormatter(operationDescription, endpoint);
            formatter = CreateClientFormatter(operationDescription, endpoint, formatter);
            return formatter;
        }

        protected override IClientMessageFormatter GetReplyClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            var formatter = base.GetReplyClientFormatter(operationDescription, endpoint);
            formatter = CreateClientFormatter(operationDescription, endpoint, formatter);
            return formatter;
        }

        private IClientMessageFormatter CreateClientFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint, IClientMessageFormatter fallbackFormatter)
        {
            foreach (var b in operationDescription.OperationBehaviors)
            {
                if (b is StreamingRawFormatAttribute)
                {
                    var rf = (StreamingRawFormatAttribute)b;
                    var formatter = rf.CreateClientFormatter(operationDescription, endpoint, fallbackFormatter);
                    return formatter;
                }
            }

            return fallbackFormatter;
        }
    }
}
