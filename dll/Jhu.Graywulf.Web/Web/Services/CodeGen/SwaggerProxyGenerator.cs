using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Net;

namespace Jhu.Graywulf.Web.Services
{
    public class SwaggerProxyGenerator : RestProxyGeneratorBase
    {
        private static readonly Dictionary<Type, string> types = new Dictionary<Type, string>()
        {
            { typeof(Int32), "integer" },
            { typeof(Int64), "integer" },
            { typeof(Single), "number" },
            { typeof(Double), "number" },
            { typeof(String), "string" },
            { typeof(Byte), "string" },
            { typeof(Byte[]), "string" },
            { typeof(Boolean), "boolean" },
            { typeof(DateTime), "string" },
        };

        private static readonly Dictionary<Type, string> formats = new Dictionary<Type, string>()
        {
            { typeof(Int32), "int32" },
            { typeof(Int64), "int64" },
            { typeof(Single), "float" },
            { typeof(Double), "double" },
            { typeof(String), "string" },
            { typeof(Byte), "byte" },
            { typeof(Byte[]), "binary" },
            { typeof(Boolean), "boolean" },
            { typeof(DateTime), "date" },
        };

        public override string MimeType
        {
            get { return "application/json"; }
        }

        public override string Filename
        {
            get { return ServiceName + "Service.json"; }
        }

        public SwaggerProxyGenerator(Type contractType, string serviceUrl)
            : base(contractType, serviceUrl)
        {
        }

        protected override void WriteHeader(TextWriter writer)
        {
            var host = "";

            if (WebOperationContext.Current != null)
            {
                host = WebOperationContext.Current.IncomingRequest.Headers[HttpRequestHeader.Host];
            }

            var script = new StringBuilder(Templates.Swagger.Class);
            script.Replace("__serviceName__", ServiceName);
            script.Replace("__serviceUrl__", ServiceUrl);
            script.Replace("__serviceVersion__", ServiceVersion);
            script.Replace("__serviceHost__", host);
            
            writer.Write(script);
        }

        protected override void WriteEndpoint(TextWriter writer, string uriTemplate)
        {
            var script = new StringBuilder(Templates.Swagger.Endpoint);
            script.Replace("__path__", uriTemplate);
            writer.Write(script);
        }

        protected override void WriteMethod(TextWriter writer, MethodInfo method, ParameterInfo[] parameters, ParameterInfo bodyParameter, string httpMethod, RestUriTemplate uriTemplate)
        {
            var script = new StringBuilder(Templates.Swagger.Method);

            script.Replace("__path__", GetPath(uriTemplate));
            script.Replace("__httpMethod__", httpMethod.ToLowerInvariant());
            script.Replace("__serviceName__", ServiceName);
            script.Replace("__methodName__", GetMethodName_Camel(method));

            writer.Write(script);

            foreach (var parameter in parameters)
            {
                if (parameter != bodyParameter)
                {
                    WriteParameter(writer, uriTemplate, method, parameter);
                }
            }
        }

        private void WriteParameter(TextWriter writer, RestUriTemplate uriTemplate, MethodInfo method, ParameterInfo parameter)
        {
            var script = new StringBuilder(Templates.Swagger.Parameter);

            script.Replace("__parameterName__", parameter.Name);
            script.Replace("__parameterType__", types[parameter.ParameterType]);
            script.Replace("__parameterFormat__", formats[parameter.ParameterType]);
            script.Replace("__parameterDescription__", "");

            if (uriTemplate.IsQueryParameter(parameter.Name))
            {
                script.Replace("__parameterIn__", "query");
            }
            else if (uriTemplate.IsPathParameter(parameter.Name))
            {
                script.Replace("__parameterIn__", "path");
            }
            else
            {
                throw new NotImplementedException();
            }

            // TODO: add support for header and cookie parameters

            writer.Write(script);
        }

        protected override void WriteFooter(TextWriter writer)
        {

        }

        private string GetPath(RestUriTemplate uriTemplate)
        {
            var res = "";

            for (int i = 0; i < uriTemplate.PathParts.Length; i++)
            {
                res += "/" + uriTemplate.PathParts[i];
            }

            return res;
        }
    }
}
