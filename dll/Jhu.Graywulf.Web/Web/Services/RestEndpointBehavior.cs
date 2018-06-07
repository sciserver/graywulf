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
            Serialization.RawMessageFormatterBase formatter = null;

            foreach (var b in operationDescription.OperationBehaviors)
            {
                if (b is Serialization.RawFormatAttribute)
                {
                    var rf = (Serialization.RawFormatAttribute)b;
                    formatter = rf.CreateFormatter();
                    break;
                }
            }

            if (formatter == null)
            {
                // Add customized json formatter
                formatter = new Serialization.JsonMessageFormatter();
            }
            
            formatter.Initialize(operationDescription, endpoint, fallbackFormatter);
            return formatter;
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
            Serialization.RawMessageFormatterBase formatter = null;

            foreach (var b in operationDescription.OperationBehaviors)
            {
                if (b is Serialization.RawFormatAttribute)
                {
                    var rf = (Serialization.RawFormatAttribute)b;
                    formatter = rf.CreateFormatter();
                    break;
                }
            }

            if (formatter == null)
            {
                // Add customized json formatter
                formatter = new Serialization.JsonMessageFormatter();
            }

            formatter.Initialize(operationDescription, endpoint, fallbackFormatter);
            return formatter;
        }
    }
}
