using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public abstract class RestProxyGeneratorBase
    {
        private TextWriter writer;
        private RestApi api;

        public abstract string MimeType { get; }

        public abstract string Filename { get; }

        protected RestProxyGeneratorBase(RestApi api)
        {
            this.api = api;
            this.writer = null;
        }
        
        public string Execute()
        {
            using (this.writer = new StringWriter())
            {
                Execute(writer);
                return writer.ToString();
            }
        }

        public void Execute(Stream stream)
        {
            using (this.writer = new StreamWriter(stream))
            {
                Execute(writer);
            }
        }

        public void Execute(TextWriter writer)
        {
            this.writer = writer;

            WriteScriptHeader(writer, api);

            foreach (var type in api.ServiceContracts.Keys)
            {
                ProcessServiceContract(api.ServiceContracts[type]);
            }

            foreach (var type in api.DataContracts.Keys)
            {
                ProcessDataContract(api.DataContracts[type]);
            }

            WriteScriptFooter(writer, api);
        }

        private void ProcessServiceContract(RestServiceContract service)
        {
            WriteServiceContractHeader(writer, service);

            foreach (var uriTemplate in service.UriTemplates.Keys)
            {
                ProcessServiceEndpoint(service, uriTemplate);
            }

            WriteServiceContractFooter(writer, service);
        }

        private void ProcessServiceEndpoint(RestServiceContract service, string uriTemplate)
        {
            if (uriTemplate != "*" && !uriTemplate.StartsWith("/proxy"))
            {
                WriteServiceEndpointHeader(writer, uriTemplate);

                foreach (var method in service.UriTemplates[uriTemplate].Keys)
                {
                    ProcessOperationContract(service.UriTemplates[uriTemplate][method]);
                }

                WriteServiceEndpointFooter(writer, uriTemplate);
            }
        }

        private void ProcessOperationContract(RestOperationContract operation)
        {
            WriteOperationContractHeader(writer, operation);

            foreach (var parameter in operation.Parameters)
            {
                WriteMessageParameter(writer, parameter);
            }

            WriteOperationContractFooter(writer, operation);
        }

        private void ProcessDataContract(RestDataContract dataContract)
        {
        }
        
        protected abstract void WriteScriptHeader(TextWriter writer, RestApi api);

        protected abstract void WriteScriptFooter(TextWriter writer, RestApi api);

        protected abstract void WriteServiceContractHeader(TextWriter writer, RestServiceContract service);

        protected abstract void WriteServiceContractFooter(TextWriter writer, RestServiceContract service);

        protected abstract void WriteServiceEndpointHeader(TextWriter writer, string uriTemplate);

        protected abstract void WriteServiceEndpointFooter(TextWriter writer, string uriTemplate);

        protected abstract void WriteOperationContractHeader(TextWriter writer, RestOperationContract operation);

        protected abstract void WriteOperationContractFooter(TextWriter writer, RestOperationContract operation);

        protected abstract void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter);

    }
}
