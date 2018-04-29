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

        protected RestApi Api
        {
            get { return api; }
        }

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
            using (this.writer = new StreamWriter(stream, Encoding.UTF8, 0x10000, true))
            {
                Execute(writer);
            }
        }

        public void Execute(TextWriter writer)
        {
            this.writer = writer;

            WriteScriptHeader(writer);
            WriteServiceContractsHeader(writer, api);

            foreach (var type in api.ServiceContracts.Keys)
            {
                ProcessServiceContract(api.ServiceContracts[type]);
            }

            WriteServiceContractsFooter(writer, api);
            WriteDataContractsHeader(writer, api);

            foreach (var type in api.DataContracts.Keys)
            {
                ProcessDataContract(api.DataContracts[type]);
            }

            WriteDataContractsFooter(writer, api);
            WriteScriptFooter(writer);
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

        private void ProcessDataContract(RestDataContract contract)
        {
            WriteDataContractHeader(writer, contract);

            foreach (var member in contract.DataMembers.Keys)
            {
                WriteDataMember(writer, contract.DataMembers[member]);
            }

            WriteDataContractFooter(writer, contract);
        }

        protected virtual void WriteScriptHeader(TextWriter writer) { }

        protected virtual void WriteScriptFooter(TextWriter writer) { }

        protected virtual void WriteServiceContractsHeader(TextWriter writer, RestApi api) { }

        protected virtual void WriteServiceContractsFooter(TextWriter writer, RestApi api) { }

        protected virtual void WriteServiceContractHeader(TextWriter writer, RestServiceContract service) { }

        protected virtual void WriteServiceContractFooter(TextWriter writer, RestServiceContract service) { }

        protected virtual void WriteServiceEndpointHeader(TextWriter writer, string uriTemplate) { }

        protected virtual void WriteServiceEndpointFooter(TextWriter writer, string uriTemplate) { }

        protected virtual void WriteOperationContractHeader(TextWriter writer, RestOperationContract operation) { }

        protected virtual void WriteOperationContractFooter(TextWriter writer, RestOperationContract operation) { }

        protected virtual void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter) { }

        protected virtual void WriteDataContractsHeader(TextWriter writer, RestApi api) { }

        protected virtual void WriteDataContractsFooter(TextWriter writer, RestApi api) { }

        protected virtual void WriteDataContractHeader(TextWriter writer, RestDataContract contract) { }

        protected virtual void WriteDataContractFooter(TextWriter writer, RestDataContract contract) { }

        protected virtual void WriteDataMember(TextWriter writer, RestDataMember member) { }
    }
}
