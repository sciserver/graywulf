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
            public bool Array { get; set; }
            public bool Dictionary { get; set; }
            public string Name { get; set; }
            public string Type { get; set; }
            public string Format { get; set; }
            public string[] Enum { get; set; }
            public string In { get; set; }
            public string Ref { get; set; }
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
                new JProperty("schemes", new JArray("http", "https"))

                // Do not specify service-wide defaults because this prevents
                // Content-Type and Accept header overrides in python client
                /*new JProperty("consumes", new JArray("application/json")),
                new JProperty("produces", new JArray("application/json"))*/
            );
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

        protected override void WriteOperationContractHeader(TextWriter writer, RestOperationContract operation)
        {
            var path = (JObject)paths[GetOperationPath(operation)];

            if (path == null)
            {
                path = new JObject();
                paths.Add(new JProperty(GetOperationPath(operation), path));
            }

            var operationId = GetOperationId(operation);

            // TODO: modify this if proper FaultContracts are implemented some day
            var responses =
                new JObject(
                    new JProperty("default",
                    new JObject(
                        new JProperty("description", "unexpected error"),
                        new JProperty("schema",
                            new JObject(
                                new JProperty("$ref", "#/definitions/RestError"))))));

            var method = new JObject(
                new JProperty("summary", operation.Description),
                new JProperty("operationId", operationId),
                new JProperty("tags", new JArray(operation.Service.ServiceName)),
                new JProperty("consumes", new JArray()),
                new JProperty("produces", new JArray()),
                new JProperty("parameters", new JArray()),
                new JProperty("responses", responses));

            path.Add(new JProperty(operation.HttpMethod.ToLowerInvariant(), method));
        }

        protected override void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter)
        {
            var method = paths[GetOperationPath(parameter.Operation)][GetOperationMethod(parameter.Operation)];
            var consumes = (JArray)method["consumes"];
            var produces = (JArray)method["produces"];
            var parameters = (JArray)method["parameters"];
            var responses = (JObject)method["responses"];

            var info = GetParameterInfo(parameter);
            var schema = GetTypeRefSchema(info, false);

            if (parameter.IsBodyParameter && !parameter.IsReturnParameter)
            {
                // Input body parameter
                if (!parameter.IsRawFormat && !parameter.IsStream)
                {
                    foreach (var format in parameter.Formats)
                    {
                        consumes.Add(format.MimeType);
                    }
                }
                else
                {
                    var hpar =
                        new JObject(
                            new JProperty("name", "Content-Type"),
                            new JProperty("in", "header"),
                            new JProperty("description", "File format mime type."),
                            new JProperty("default", parameter.Formats[0].MimeType),
                            new JProperty("required", true),
                            new JProperty("schema", new JObject(
                                new JProperty("type", "string"),
                                new JProperty("format", "string"))));

                    parameters.Add(hpar);
                }


                var par =
                    new JObject(
                        new JProperty("name", parameter.ParameterName),
                        new JProperty("in", info.In),
                        new JProperty("description", parameter.Description),
                        new JProperty("required", true),
                        new JProperty("schema", schema));

                parameters.Add(par);
            }
            else if (parameter.IsReturnParameter)
            {
                // Return value
                // Do not set format if returning raw to allow client specify format
                if (!parameter.IsRawFormat && !parameter.IsStream)
                {
                    foreach (var format in parameter.Formats)
                    {
                        produces.Add(format.MimeType);
                    }
                }
                else
                {
                    var hpar =
                        new JObject(
                            new JProperty("name", "Accept"),
                            new JProperty("in", "header"),
                            new JProperty("description", "File format mime type."),
                            new JProperty("default", parameter.Formats[0].MimeType),
                            new JProperty("required", true),
                            new JProperty("schema", new JObject(
                                new JProperty("type", "string"),
                                new JProperty("format", "string"))));

                    parameters.Add(hpar);
                }

                var response =
                        new JProperty("200",
                            new JObject(
                                new JProperty("description", ""),
                                new JProperty("schema", schema)));

                // TODO: consider adding per mime type schema entries
                responses.Add(response);
            }
            else
            {
                // Path or query parameter
                var par =
                    new JObject(
                        new JProperty("name", parameter.ParameterName),
                        new JProperty("in", info.In),
                        new JProperty("description", parameter.Description),
                        new JProperty("required", parameter.IsPathParameter),
                        new JProperty("schema", schema));

                parameters.Add(par);
            }
        }

        protected override void WriteDataContractHeader(TextWriter writer, RestDataContract contract)
        {
            var info = GetTypeInfo(contract.Type);
            var obj = GetTypeDefSchema(info);
            var definition = new JProperty(info.Name, obj);

            definitions.Add(definition);
        }

        protected override void WriteDataMember(TextWriter writer, RestDataMember member)
        {
            var dcinfo = GetTypeInfo(member.DataContract.Type);
            var properties = (JObject)definitions[dcinfo.Name]["properties"];
            var info = GetTypeInfo(member.Property.PropertyType);
            var schema = GetTypeRefSchema(info, false);

            properties.Add(new JProperty(member.DataMemberName, schema));
        }

        protected override void WriteDataContractsFooter(TextWriter writer, RestApi api)
        {
            jdoc.Add(new JProperty("definitions", definitions));
        }

        private string GetEscapedName(string name)
        {
            return name.Replace('[', '_').Replace(']', '_');
        }

        private string GetOperationPath(RestOperationContract operation)
        {
            return operation.Service.ServiceUrl + operation.UriTemplate.Path;
        }

        private string GetOperationMethod(RestOperationContract operation)
        {
            return operation.HttpMethod.ToLowerInvariant();
        }

        private string GetOperationId(RestOperationContract operation)
        {
            return operation.OperationName;
        }

        private VariableInfo GetTypeInfo(Type type)
        {
            Type elementType;
            bool array = false;
            bool dictionary = false;
            bool nullable = false;
            VariableInfo info = null;

            if (RestServiceReflector.IsNullableType(type, out elementType))
            {
                nullable = true;
                type = elementType;
            }

            if (RestServiceReflector.IsArrayType(type, out elementType))
            {
                array = true;
                type = elementType;
            }

            if (RestServiceReflector.IsDictionaryType(type, out var keyType, out elementType))
            {
                dictionary = true;
            }

            if (base.Api.DataContracts.ContainsKey(type))
            {
                // Reflected complex types

                if (Api.DataContracts[type].IsEnum)
                {
                    info = GetTypeInfo_Enum(type, array, dictionary);
                }
                else if (Api.DataContracts[type].IsDictionary)
                {
                    info = GetTypeInfo_Dictionary(type, array, dictionary);
                }
                else
                {
                    info = GetTypeInfo_DataContract(type, array, dictionary);
                }
            }
            else
            {
                // Swagger primitive types

                info = GetTypeInfo_Swagger(type, nullable, array, dictionary);
            }

            return info;
        }

        private VariableInfo GetTypeInfo_Enum(Type type, bool array, bool dictionary)
        {
            var dc = base.Api.DataContracts[type];

            var info = new VariableInfo()
            {
                Array = array,
                Dictionary = dictionary,
                Name = dc.DataContractName,
                Enum = Enum.GetNames(type),
            };

            for (int i = 0; i < info.Enum.Length; i++)
            {
                info.Enum[i] = info.Enum[i].ToLowerInvariant();
            }

            // TODO: test with arrays of enums

            return info;
        }

        private VariableInfo GetTypeInfo_Dictionary(Type type, bool array, bool dictionary)
        {
            Type keyType;
            Type valueType;

            RestServiceReflector.IsDictionaryType(type, out keyType, out valueType);

            var dc = base.Api.DataContracts[type];
            var einfo = GetTypeInfo(valueType);

            var info = new VariableInfo()
            {
                Array = array,
                Dictionary = dictionary,
                Name = dc.DataContractName,
                Ref = einfo.Name,
                Type = einfo.Type,
                Format = einfo.Format
            };

            return info;
        }

        private VariableInfo GetTypeInfo_DataContract(Type type, bool array, bool dictionary)
        {
            var dc = base.Api.DataContracts[type];

            var info = new VariableInfo()
            {
                Array = array,
                Dictionary = dictionary,
                Name = dc.DataContractName,
            };

            // TODO: test with arrays of complex objects

            return info;
        }

        private VariableInfo GetTypeInfo_Swagger(Type type, bool nullable, bool array, bool dictionary)
        {
            if (Constants.SwaggerTypes.ContainsKey(type))
            {
                var info = new VariableInfo()
                {
                    Array = array,
                    Dictionary = dictionary,
                    Type = Constants.SwaggerTypes[type],
                    Format = Constants.SwaggerFormats[type]
                };

                return info;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private VariableInfo GetParameterInfo(RestMessageParameter parameter)
        {
            if (parameter.IsRawFormat || parameter.IsStream || parameter.IsMessage)
            {
                var res = new VariableInfo()
                {
                    In = "body",
                    Type = "file"
                };

                return res;
            }
            else
            {
                var t = parameter.Parameter.ParameterType;
                var res = GetTypeInfo(t);

                if (parameter.IsQueryParameter)
                {
                    res.In = Constants.SwaggerParameterInQuery;
                }
                else if (parameter.IsPathParameter)
                {
                    res.In = Constants.SwaggerParameterInPath;
                }
                else if (parameter.IsBodyParameter)
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

        private JObject GetTypeRefSchema(VariableInfo info, bool reference)
        {
            JObject schema = new JObject();
            JObject type;

            if (info.Array)
            {
                var items = new JObject();

                schema.Add(new JProperty("type", "array"));
                schema.Add(new JProperty("items", items));

                type = items;
            }
            else if (info.Dictionary)
            {
                type = schema;
            }
            else
            {
                type = schema;
            }

            if (!reference && info.Name != null)
            {
                type.Add(new JProperty("$ref", "#/definitions/" + info.Name));
            }
            else if (reference && info.Ref != null)
            {
                type.Add(new JProperty("$ref", "#/definitions/" + info.Ref));
            }
            else
            {
                if (info.Type != null)
                {
                    type.Add(new JProperty("type", info.Type));
                }

                if (info.Format != null)
                {
                    type.Add(new JProperty("format", info.Format));
                }
            }

            return schema;
        }

        private JObject GetTypeDefSchema(VariableInfo info)
        {
            JObject obj;

            if (info.Array)
            {
                obj = GetTypeRefSchema(info, true);
            }
            else if (info.Dictionary)
            {
                var additionalProperties = GetTypeRefSchema(info, true);

                obj = new JObject(
                        new JProperty("type", "object"),
                        new JProperty("additionalProperties", additionalProperties));
            }
            else if (info.Enum != null)
            {
                obj = new JObject(
                        new JProperty("type", "string"),
                        new JProperty("enum", new JArray(info.Enum)));
            }
            else
            {
                obj = new JObject(
                        //new JProperty("required", new JArray()),
                        new JProperty("properties", new JObject()));
            }

            return obj;
        }
    }
}
