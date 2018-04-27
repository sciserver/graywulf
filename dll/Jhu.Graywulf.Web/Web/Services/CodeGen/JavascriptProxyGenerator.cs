using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Description;
using System.IO;
using System.Reflection;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class JavascriptProxyGenerator : RestProxyGeneratorBase
    {
        public override string MimeType
        {
            get { return "application/javascript"; }
        }

        public override string Filename
        {
            get { return "Proxy.js"; }
        }

        public JavascriptProxyGenerator(RestApi api)
            : base(api)
        {
        }

        protected override void WriteScriptHeader(TextWriter writer, RestApi api)
        {
            // no op
        }

        protected override void WriteScriptFooter(TextWriter writer, RestApi api)
        {
            // no op
        }

        protected override void WriteServiceContractHeader(TextWriter writer, RestServiceContract service)
        {
            var script = new StringBuilder(Javascript.ServiceClass);
            service.SubstituteTokens(script);
            writer.Write(script);
        }

        protected override void WriteServiceContractFooter(TextWriter writer, RestServiceContract service)
        {
            // no op
        }

        protected override void WriteServiceEndpointHeader(TextWriter writer, string uriTemplate)
        {
            // no op
        }

        protected override void WriteServiceEndpointFooter(TextWriter writer, string uriTemplate)
        {
            // no op
        }

        protected override void WriteOperationContractHeader(TextWriter writer, RestOperationContract operation)
        {
            var script = new StringBuilder(Javascript.ServiceMethod);

            operation.Service.SubstituteTokens(script);
            operation.SubstituteTokens(script);
            
            var parameterList = GetParameterList(operation);
            var pathParts = GetPathPartsList(operation.UriTemplate);
            var queryParts = GetQueryPartsList(operation.UriTemplate);

            script.Replace("__parameterList__", parameterList);
            script.Replace("__bodyParameter__", operation.BodyParameter?.ParameterName ?? "null");
            script.Replace("__returnType__", operation.ReturnParameter == null ? "\"text\"" : "\"json\"");
            script.Replace("__pathParts__", pathParts);
            script.Replace("__queryParts__", queryParts);

            writer.Write(script);
        }

        protected override void WriteOperationContractFooter(TextWriter writer, RestOperationContract operation)
        {
            // no op
        }

        protected override void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter)
        {
            // no op
        }

        private string GetParameterList(RestOperationContract operation)
        {
            var res = "";

            foreach (var par in operation.Parameters)
            {
                res += par.ParameterName + ", ";
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
    }
}
