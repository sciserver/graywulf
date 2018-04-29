using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Web;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class SwaggerJsonGenerator : RestProxyGeneratorBase
    {
        class VariableInfo
        {
            public string Type { get; set; }
            public string Format { get; set; }
            public string In { get; set; }
            public string Name { get; set; }
            public string Ref { get; set; }
            public string Items { get; set; }
        }

        private JObject jdoc;
        private JObject paths;
        private JObject definitions;

        public override string MimeType
        {
            get { return "application/json"; }
        }

        public override string Filename
        {
            get { return "Service.json"; }
        }

        public SwaggerJsonGenerator(RestApi api)
            : base(api)
        {
        }

        protected override void WriteScriptFooter(TextWriter writer)
        {
            using (var w = new JsonTextWriter(writer))
            {
                w.Formatting = Formatting.Indented;
                jdoc.WriteTo(w);
            }
        }

        protected override void WriteServiceContractsHeader(TextWriter writer, RestApi api)
        {
            paths = new JObject();
            definitions = new JObject();

            jdoc = new JObject(
                new JProperty("swagger", "2.0"),
                new JProperty("info",
                    new JObject(
                        new JProperty("version", api.Version),
                        new JProperty("title", api.Description))),
                new JProperty("host", api.HostName),
                new JProperty("basePath", api.BasePath),
                new JProperty("schemes", new JArray("http", "https")),
                new JProperty("consumes", new JArray("application/json")),
                new JProperty("produces", new JArray("application/json")));
        }

        protected override void WriteServiceContractsFooter(TextWriter writer, RestApi api)
        {
            jdoc.Add(new JProperty("paths", paths));
        }

        protected override void WriteServiceContractHeader(TextWriter writer, RestServiceContract service)
        {
            // no op

            // TODO: write tags
        }

        protected override void WriteServiceContractFooter(TextWriter writer, RestServiceContract service)
        {
            // no op

            // TODO: write tags
        }

        protected override void WriteServiceEndpointHeader(TextWriter writer, string uriTemplate)
        {
            paths.Add(
                new JProperty(uriTemplate, new JObject()));
        }

        protected override void WriteOperationContractHeader(TextWriter writer, RestOperationContract operation)
        {
            var path = (JObject)paths[operation.UriTemplate.Value];
            JObject responses = new JObject();
            JProperty response;

            path.Add(
                new JProperty(operation.HttpMethod.ToLowerInvariant(),
                    new JObject(
                        new JProperty("summary", operation.OperationDescription),
                        new JProperty("operationId", operation.OperationName),
                        new JProperty("tags", new JArray(operation.Service.ServiceName)),
                        new JProperty("parameters", new JArray()),
                        new JProperty("responses", responses))));

            if (operation.ReturnParameter == null)
            {
                response =
                    new JProperty("200",
                        new JObject(
                            new JProperty("description", "")));
            }
            else
            {
                if (operation.ReturnParameter.DataContract != null)
                {
                    var info = GetParameterInfo(operation.ReturnParameter);

                    response =
                        new JProperty("200",
                            new JObject(
                                new JProperty("description", ""),
                                new JProperty("schema",
                                    new JObject(
                                        new JProperty("$ref", info.Ref)))));
                }
                else
                {
                    response =
                        new JProperty("200",
                            new JObject(
                                new JProperty("description", ""),
                                new JProperty("schema",
                                    new JObject(
                                        new JProperty("type", "file")))));
                }
            }

            responses.Add(response);

            responses.Add(
                new JProperty("default",
                    new JObject(
                        new JProperty("description", "unexpected error"),
                        new JProperty("schema",
                            new JObject(
                                new JProperty("$ref", "#/definitions/RestError"))))));
        }

        protected override void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter)
        {
            var parameters = (JArray)paths[parameter.Operation.UriTemplate.Value][parameter.Operation.HttpMethod.ToLowerInvariant()]["parameters"];

            if (!parameter.IsBodyParameter())
            {
                var info = GetParameterInfo(parameter);

                var par =
                    new JObject(
                        new JProperty("name", parameter.ParameterName),
                        new JProperty("in", info.In),
                        new JProperty("description", parameter.ParameterDescription),
                        new JProperty("required", parameter.IsPathParameter()));

                if (info.Type != null)
                {
                    par.Add(
                        new JProperty("type", info.Type));
                }

                if (info.Format != null)
                {
                    par.Add(
                        new JProperty("format", info.Format));
                }

                parameters.Add(par);
            }
            else
            {
                // TODO: reference if body?
            }
        }

        protected override void WriteDataContractHeader(TextWriter writer, RestDataContract contract)
        {
            var info = GetTypeInfo(contract.Type);
            JObject obj;

            if (info.Items != null)
            {
                obj = new JObject(
                    new JProperty("type", "array"),
                    new JProperty("items",
                        new JObject(
                            new JProperty("$ref", info.Items))));
            }
            else
            {
                obj = new JObject(
                        //new JProperty("required", new JArray()),
                        new JProperty("properties", new JObject()));
            }

            var definition = new JProperty(info.Name, obj);

            definitions.Add(definition);
        }

        protected override void WriteDataMember(TextWriter writer, RestDataMember member)
        {
            var dcinfo = GetTypeInfo(member.DataContract.Type);
            var properties = (JObject)definitions[dcinfo.Name]["properties"];
            var prop = new JObject();
            JObject type;

            var info = GetTypeInfo(member.Property.PropertyType);

            if (info.Ref == null)
            {
                prop = new JObject();

                if (info.Type != null)
                {
                    prop.Add(new JProperty("type", info.Type));
                }

                if (info.Items == null)
                {
                    type = prop;
                }
                else
                {
                    type =
                        new JObject(
                            new JProperty("type", info.Items));

                    prop.Add(new JProperty("items", type));
                }

                if (info.Format != null)
                {
                    type.Add(new JProperty("format", info.Format));
                }
            }
            else
            {
                prop.Add(new JProperty("$ref", info.Ref));
            }

            properties.Add(new JProperty(member.DataMemberName, prop));
        }

        protected override void WriteDataContractsFooter(TextWriter writer, RestApi api)
        {
            jdoc.Add(new JProperty("definitions", definitions));
        }

        private string GetEscapedName(string name)
        {
            return name.Replace('[', '_').Replace(']', '_');
        }

        private VariableInfo GetTypeInfo(Type type)
        {
            if (base.Api.DataContracts.ContainsKey(type))
            {
                return GetTypeInfo_DataContract(type);
            }
            else
            {
                return GetTypeInfo_Swagger(type);
            }
        }

        private VariableInfo GetTypeInfo_DataContract(Type type)
        {
            var info = new VariableInfo();

            var dc = base.Api.DataContracts[type];

            if (dc.ElementType != null)
            {
                var edc = base.Api.DataContracts[dc.ElementType];

                info.Name = edc.DataContractName + "_Array";
                info.Items = "#/definition/" + edc.DataContractName;
            }
            else
            {
                info.Name = dc.DataContractName;
            }

            info.Ref = "#/definition/" + info.Name;

            return info;
        }

        private VariableInfo GetTypeInfo_Swagger(Type type)
        {
            var info = new VariableInfo();
            bool nullable = false;
            bool array = false;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                nullable = true;
                type = type.GetGenericArguments()[0];

                // TODO: we could determine if parameter is required here
            }

            if (type.IsArray)
            {
                // Handle only arrays of primitive types here, all other arrays
                // are already treated together with contracts

                array = true;
                type = type.GetElementType();
            }


            if (Constants.SwaggerTypes.ContainsKey(type))
            {
                if (!array)
                {
                    info.Type = Constants.SwaggerTypes[type];
                    info.Format = Constants.SwaggerFormats[type];
                }
                else
                {
                    info.Type = "array";
                    info.Items = Constants.SwaggerTypes[type];
                    info.Format = Constants.SwaggerFormats[type];
                }

                return info;
            }

            throw new InvalidOperationException();
        }

        private VariableInfo GetParameterInfo(RestMessageParameter parameter)
        {
            var t = parameter.Parameter.ParameterType;
            var res = GetTypeInfo(t);

            if (parameter.IsQueryParameter())
            {
                res.In = Constants.SwaggerParameterInQuery;
            }
            else if (parameter.IsPathParameter())
            {
                res.In = Constants.SwaggerParameterInPath;
            }
            else if (parameter.IsBodyParameter())
            {
                res.In = Constants.SwaggerParameterInBody;
            }
            else
            {
                throw new NotImplementedException();
            }

            return res;
        }
    }
}
