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
            return new Serialization.RestQueryStringConverter();
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
            var formatter = CreateFormatter(operationDescription, endpoint);
            formatter.Initialize(fallbackFormatter);
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
            var formatter = CreateFormatter(operationDescription, endpoint);
            formatter.Initialize(fallbackFormatter);
            return formatter;
        }

        private Serialization.DynamicMessageFormatter CreateFormatter(OperationDescription operationDescription, ServiceEndpoint endpoint)
        {
            var dynamicFormatter = new Serialization.DynamicMessageFormatter();

            // Add default formatters
            var json = new Serialization.JsonMessageFormatter();
            json.Initialize(operationDescription, endpoint);
            dynamicFormatter.Formatters.Add(json.MimeType, json);
            // TODO: XML

            foreach (var b in operationDescription.OperationBehaviors)
            {
                if (b is Serialization.DynamicFormatAttribute df)
                {
                    foreach (var ft in df.FormatterTypes)
                    {
                        var ff = (Serialization.RestMessageFormatterBase)Activator.CreateInstance(ft);
                        var formats = ff.GetSupportedFormats();

                        foreach (var format in formats)
                        {
                            ff = (Serialization.RestMessageFormatterBase)Activator.CreateInstance(ft);

                            if (ff is Serialization.RawMessageFormatterBase rf)
                            {
                                rf.Initialize(operationDescription, endpoint);
                                rf.MimeType = format.MimeType;
                                dynamicFormatter.Formatters.Add(rf.MimeType, rf);
                            }
                        }
                    }
                }
                else if (b is Serialization.RawFormatAttribute rf)
                {
                    var ff = rf.CreateFormatter();
                    var formats = ff.GetSupportedFormats();

                    foreach (var format in formats)
                    {
                        ff = rf.CreateFormatter();
                        ff.Initialize(operationDescription, endpoint);
                        ff.MimeType = format.MimeType;
                        dynamicFormatter.Formatters.Add(ff.MimeType, ff);
                    };
                }
            }

            return dynamicFormatter;
        }
    }
}
