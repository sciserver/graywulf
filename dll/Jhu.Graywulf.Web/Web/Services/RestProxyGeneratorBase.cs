using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Reflection;

namespace Jhu.Graywulf.Web.Services
{
    public abstract class RestProxyGeneratorBase
    {
        private static readonly HashSet<string> reservedFunctionNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "HandleHttpOptionsRequest", "GenerateProxy"
        };

        private Type contractType;
        private string serviceName;
        private string serviceUrl;

        public abstract string MimeType { get; }

        public abstract string Filename { get; }

        protected string ServiceName
        {
            get { return serviceName; }
        }

        protected string ServiceUrl
        {
            get { return serviceUrl; }
        }

        protected RestProxyGeneratorBase(Type contractType, string serviceUrl)
        {
            // TODO: assume a single endpoint here, which is currently true
            this.contractType = contractType;
            this.serviceUrl = serviceUrl;

            var attrs = contractType.GetCustomAttributes(typeof(ServiceNameAttribute), true);
            if (attrs == null || attrs.Length == 0)
            {
                throw new InvalidOperationException("Missing ServiceNameAttribute");
            }

            var attr = (ServiceNameAttribute)attrs[0];
            this.serviceName = attr.Name;
        }

        public string Execute()
        {
            var writer = new StringWriter();
            Execute(writer);
            return writer.ToString();
        }

        public void Execute(Stream stream)
        {
            var writer = new StreamWriter(stream);
            Execute(writer);
        }

        public void Execute(TextWriter writer)
        {
            WriteHeader(writer);

            var methods = contractType.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < methods.Length; i++)
            {
                if (!reservedFunctionNames.Contains(methods[i].Name))
                {
                    ProcessMethod(writer, methods[i]);
                }
            }

            WriteFooter(writer);

            writer.Flush();
        }

        private void ProcessMethod(TextWriter writer, MethodInfo method)
        {
            // Only methods with OperationContract attribute
            var opc = method.GetCustomAttributes(typeof(OperationContractAttribute), true);
            if (opc != null && opc.Length > 0)
            {
                string httpMethod;
                string uriTemplate;
                var parameters = method.GetParameters();
                GetMethodProperties(method, out httpMethod, out uriTemplate);
                var restUrlTemplate = new RestUriTemplate(uriTemplate);
                var bodyParameter = GetBodyParameter(parameters, restUrlTemplate);
                WriteMethod(writer, method, parameters, bodyParameter, httpMethod, restUrlTemplate);
            }
        }

        private void GetMethodProperties(MethodInfo method, out string httpMethod, out string uriTemplate)
        {
            httpMethod = null;
            uriTemplate = null;

            var attr = method.GetCustomAttributes(typeof(WebGetAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                var wbg = (WebGetAttribute)attr[0];
                httpMethod = "GET";
                uriTemplate = wbg.UriTemplate;
            }

            attr = method.GetCustomAttributes(typeof(WebInvokeAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                var wbi = (WebInvokeAttribute)attr[0];
                httpMethod = wbi.Method.ToUpper();
                uriTemplate = wbi.UriTemplate;
            }
        }

        private ParameterInfo GetBodyParameter(ParameterInfo[] parameters, RestUriTemplate uriTemplate)
        {
            // Find parameter which doesn't correspond to any on the path and query parts
            for (int i = 0; i < parameters.Length; i++)
            {
                if (uriTemplate.IsBodyParameter(parameters[i].Name))
                {
                    return parameters[i];
                }
            }

            return null;
        }

        protected abstract void WriteHeader(TextWriter writer);

        protected abstract void WriteMethod(TextWriter writer, MethodInfo method, ParameterInfo[] parameters, ParameterInfo bodyParameter, string httpMethod, RestUriTemplate uriTemplate);

        protected abstract void WriteFooter(TextWriter writer);
    }
}
