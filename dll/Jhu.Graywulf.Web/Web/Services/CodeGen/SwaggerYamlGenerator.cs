using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Net;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class SwaggerYamlGenerator : RestProxyGeneratorBase
    {

        public override string MimeType
        {
            get { return "application/json"; }
        }

        public override string Filename
        {
            get { return "Service.yaml"; }
        }

        public SwaggerYamlGenerator(RestApi api)
            : base(api)
        {
        }

        protected override void WriteScriptHeader(TextWriter writer, RestApi api)
        {
            var script = new StringBuilder(Swagger.Service);
            api.SubstituteTokens(script);
            writer.Write(script);
        }

        protected override void WriteScriptFooter(TextWriter writer, RestApi api)
        {
            // no op
        }

        protected override void WriteServiceContractHeader(TextWriter writer, RestServiceContract service)
        {
            // no op
        }

        protected override void WriteServiceContractFooter(TextWriter writer, RestServiceContract service)
        {
            // no op
        }

        protected override void WriteServiceEndpointHeader(TextWriter writer, string uriTemplate)
        {
            var script = new StringBuilder(Swagger.Endpoint);
            script.Replace("__path__", uriTemplate);
            writer.Write(script);
        }

        protected override void WriteServiceEndpointFooter(TextWriter writer, string uriTemplate)
        {
            // no op
        }

        protected override void WriteOperationContractHeader(TextWriter writer, RestOperationContract operation)
        {
            var script = new StringBuilder(Swagger.Method);

            operation.Service.Api.SubstituteTokens(script);
            operation.Service.SubstituteTokens(script);
            operation.SubstituteTokens(script);

            writer.Write(script);
        }

        protected override void WriteOperationContractFooter(TextWriter writer, RestOperationContract operation)
        {
            // no op
        }

        protected override void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter)
        {
            var script = new StringBuilder(Swagger.Parameter);

            parameter.Operation.Service.Api.SubstituteTokens(script);
            parameter.Operation.Service.SubstituteTokens(script);
            parameter.Operation.SubstituteTokens(script);
            parameter.SubstituteTokens(script);

            GetParameterInfo(parameter, out var type, out var format, out var @in);

            script.Replace("__parameterType__", type);
            script.Replace("__parameterFormat__", format);
            script.Replace("__parameterIn__", @in);
            
            // TODO: add support for header and cookie parameters

            writer.Write(script);
        }
        
        private void GetParameterInfo(RestMessageParameter parameter, out string type, out string format, out string @in)
        {
            var t = parameter.Parameter.ParameterType;

            if (Constants.SwaggerTypes.ContainsKey(t))
            {
                type = Constants.SwaggerTypes[t];
                format = Constants.SwaggerFormats[t];
            }
            else
            {
                // TODO: this is a data contract
                throw new NotImplementedException();
            }

            if (parameter.IsQueryParameter())
            {
                @in = Constants.SwaggerParameterInQuery;
            }
            else if (parameter.IsPathParameter())
            {
                @in = Constants.SwaggerParameterInPath;
            }
            else if (parameter.IsBodyParameter())
            {
                @in = Constants.SwaggerParameterInBody;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
