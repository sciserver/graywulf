using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.IO;
using System.Reflection;

namespace Jhu.Graywulf.Web.Services
{
    public class JavascriptProxyGenerator : RestProxyGeneratorBase
    {
        public override string MimeType
        {
            get { return "application/javascript"; }
        }

        public override string Filename
        {
            get { return ServiceName + "Service.js"; }
        }

        public JavascriptProxyGenerator(Type contractType, string serviceUrl)
            : base(contractType, serviceUrl)
        {
        }

        protected override void WriteHeader(TextWriter writer)
        {
            var script = new StringBuilder(Templates.Javascript.Class);
            script.Replace("__serviceName__", ServiceName);
            script.Replace("__serviceUrl__", ServiceUrl);
            writer.Write(script);
        }

        protected override void WriteMethod(TextWriter writer, MethodInfo method, ParameterInfo[] parameters, ParameterInfo bodyParameter, string httpMethod, RestUriTemplate uriTemplate)
        {
            var methodName = GetMethodName(method);
            var script = new StringBuilder(Templates.Javascript.Method);
            var parameterList = GetParameterList(parameters);
            var pathParts = GetPathPartsList(uriTemplate);
            var queryParts = GetQueryPartsList(uriTemplate);

            script.Replace("__serviceName__", ServiceName);
            script.Replace("__methodName__", methodName);
            script.Replace("__parameterList__", parameterList);
            script.Replace("__bodyParameter__", bodyParameter?.Name ?? "null");
            script.Replace("__returnType__", method.ReturnType == typeof(void) ? "\"text\"" : "\"json\"");
            script.Replace("__pathParts__", pathParts);
            script.Replace("__queryParts__", queryParts);
            script.Replace("__httpMethod__", httpMethod);

            writer.Write(script);
        }

        protected override void WriteFooter(TextWriter writer)
        {
        }

        private string GetParameterList(ParameterInfo[] parameters)
        {
            var res = "";

            for (int i = 0; i < parameters.Length; i++)
            {
                res += parameters[i].Name + ", ";
            }

            res += "on_success, on_error";

            return res;
        }

        private string GetPathPartsList(RestUriTemplate uriTemplate)
        {
            var res = "";

            for (int i = 0; i < uriTemplate.PathParts.Length; i++)
            {
                if (res != "")
                {
                    res += ", ";
                }

                if (uriTemplate.IsPathParameter(i))
                {
                    res += uriTemplate.PathParts[i].TrimStart('{').TrimEnd('}');
                }
                else
                {
                    res += "\"" + uriTemplate.PathParts[i] + "\"";
                }
            }

            return "[" + res + "]";
        }

        private string GetQueryPartsList(RestUriTemplate uriTemplate)
        {
            var res = "";

            for (int i = 0; i < uriTemplate.QueryKeys.Length; i++)
            {
                if (res != "")
                {
                    res += ", ";
                }

                if (uriTemplate.IsQueryParameter(i))
                {
                    res += "\"" + uriTemplate.QueryKeys[i] + "\"" +
                        ": " + uriTemplate.QueryValues[i].TrimStart('{').TrimEnd('}');
                }
                else
                {
                    res += "\"" + uriTemplate.QueryKeys[i] + "\"" +
                        ": \"" + uriTemplate.QueryValues[i] + "\"";
                }
            }

            return String.IsNullOrEmpty(res) ? "null" : "{" + res + "}";
        }

        private string GetMethodName(MethodInfo method)
        {
            // c# should use capitalized function names while javascript
            // prefers camel
            var name = method.Name;

            if (name.Length > 1)
            {
                name = Char.ToLowerInvariant(name[0]) + name.Substring(1);
            }
            else
            {
                name = Char.ToLowerInvariant(name[0]).ToString();
            }

            return name;
        }
    }
}
