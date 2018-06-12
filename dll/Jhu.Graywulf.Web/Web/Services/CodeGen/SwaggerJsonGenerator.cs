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
        // NOTE: This is not fully compatible with OpenAPI 2.0, namely:
        // - A 'type: file' is added to response to indicate that the function is a file download
        // - no produces or consumes are added to operations because this would break Accept and Content-Type overrides
        // - enums are added with $ref to parameters which is not supported by codegen

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
                        new JProperty("title", api.Title))),
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
                new JProperty("parameters", new JArray()),
                new JProperty("responses", responses));

            path.Add(new JProperty(operation.HttpMethod.ToLowerInvariant(), method));
        }

        protected override void WriteMessageParameter(TextWriter writer, RestMessageParameter parameter)
        {
            var method = (JObject)paths[GetOperationPath(parameter.Operation)][GetOperationMethod(parameter.Operation)];
            var parameters = (JArray)method["parameters"];
            var responses = (JObject)method["responses"];

            var info = GetParameterInfo(parameter);

            if (parameter.IsBodyParameter && !parameter.IsReturnParameter)
            {
                // Input body parameter
                if (!parameter.IsRawFormat && !parameter.IsStream)
                {
                    var consumes = new JArray();

                    foreach (var format in parameter.Formats)
                    {
                        consumes.Add(format.MimeType);
                    }

                    method.Add(new JProperty("consumes", consumes));
                }
                else
                {
                    var hpar =
                        new JObject(
                            new JProperty("in", "header"),
                            new JProperty("name", "Content-Type"),
                            new JProperty("description", "File format mime type."),
                            new JProperty("default", parameter.Formats[0].MimeType),
                            new JProperty("required", true),
                            new JProperty("type", "string"),
                            new JProperty("format", "string"));

                    parameters.Add(hpar);
                }

                var par =
                    new JObject(
                        new JProperty("in", info.In),
                        new JProperty("name", parameter.ParameterName),
                        new JProperty("description", parameter.Description ?? ""),
                        new JProperty("required", true));

                AddTypeRefSchema(par, info, true, false);

                parameters.Add(par);
            }
            else if (parameter.IsReturnParameter)
            {
                // Return value
                // Do not set format if returning raw to allow client specify format
                if (!parameter.IsRawFormat && !parameter.IsStream && !parameter.IsMessage)
                {
                    var produces = new JArray();

                    foreach (var format in parameter.Formats)
                    {
                        produces.Add(format.MimeType);
                    }

                    method.Add(new JProperty("produces", produces));
                }
                else
                {
                    var hpar =
                        new JObject(
                            new JProperty("in", "header"),
                            new JProperty("name", "Accept"),
                            new JProperty("description", "File format mime type."),
                            new JProperty("required", true),
                            new JProperty("type", "string"),
                            new JProperty("format", "string"),
                            new JProperty("default", parameter.Formats[0].MimeType));

                    parameters.Add(hpar);
                }

                var response = new JObject(new JProperty("description", ""));
                responses.Add(new JProperty("200", response));
                AddTypeRefSchema(response, info, true, false);

                // TODO: consider adding per mime type schema entries

            }
            else
            {
                // Path or query parameter
                var par =
                    new JObject(
                        new JProperty("name", parameter.ParameterName),
                        new JProperty("in", info.In),
                        new JProperty("description", parameter.Description ?? ""),
                        new JProperty("required", parameter.IsPathParameter));

                AddTypeRefSchema(par, info, true, false);
                parameters.Add(par);
            }
        }

        protected override void WriteDataContractHeader(TextWriter writer, RestDataContract contract)
        {
            var info = GetTypeInfo(contract.Type);
            var obj = new JObject();
            var definition = new JProperty(info.Name, obj);

            AddTypeDefSchema(obj, info);

            definitions.Add(definition);
        }

        protected override void WriteDataMember(TextWriter writer, RestDataMember member)
        {
            var dcinfo = GetTypeInfo(member.DataContract.Type);
            var properties = (JObject)definitions[dcinfo.Name]["properties"];
            var info = GetTypeInfo(member.Property.PropertyType);
            var type = new JObject();

            properties.Add(new JProperty(member.DataMemberName, type));
            AddTypeRefSchema(type, info, false, false);
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
                    //  Type = "file"       // TODO: will be supported in OpenAPI 3.0
                };

                return res;
            }
            else
            {
                var t = parameter.Parameter.ParameterType;
                var res = GetTypeInfo(t);

                // TODO This is now a workaround to suppoer OpenAPI 2.0 which doesn't allow
                // referencing enums yet
                if (res.Enum != null)
                {
                    res.Enum = null;
                    res.Type = "string";
                    res.Format = "string";
                    res.Name = null;
                    res.Ref = null;
                }

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

        /// <summary>
        /// Adds type info or schema object for a given variable type.
        /// </summary>
        /// <param name="info"></param>
        /// <param name="useReference"></param>
        /// <returns></returns>
        /// <remarks>
        /// By default, the Name field of the class is used to render the
        /// $ref field. In case of arrays of complex objects, however, often
        /// the element type needs to be referenced
        /// </remarks>
        private void AddTypeRefSchema(JObject variable, VariableInfo info, bool wrapInSchema, bool useReference)
        {
            // TODO: wrapInSchema won't be necessary with OpenAPI 3.0

            JObject type;
            JProperty reference = null;

            if (info.Array)
            {
                if (wrapInSchema)
                {
                    var array = new JObject();
                    variable.Add(new JProperty("schema", array));
                    variable = array;
                }

                var items = new JObject();
                variable.Add(new JProperty("type", "array"));
                variable.Add(new JProperty("items", items));

                type = items;
            }
            else if (info.Dictionary)
            {
                type = variable;
            }
            else
            {
                type = variable;
            }

            // Work around array element issue
            if (!useReference && info.Name != null)
            {
                reference = new JProperty("$ref", "#/definitions/" + info.Name);
            }
            else if (useReference && info.Ref != null)
            {
                reference = new JProperty("$ref", "#/definitions/" + info.Ref);
            }

            if (reference != null)
            {
                if (!info.Array && wrapInSchema)
                {
                    type.Add(new JProperty("schema", new JObject(reference)));
                }
                else
                {
                    type.Add(reference);
                }
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
        }

        private void AddTypeDefSchema(JObject obj, VariableInfo info)
        {
            if (info.Array)
            {
                AddTypeRefSchema(obj, info, true, true);
            }
            else if (info.Dictionary)
            {
                var additionalProperties = new JObject();
                AddTypeRefSchema(additionalProperties, info, true, true);

                obj.Add(new JProperty("type", "object"));
                obj.Add(new JProperty("additionalProperties", additionalProperties));
            }
            else if (info.Enum != null)
            {
                obj.Add(new JProperty("type", "string"));
                obj.Add(new JProperty("enum", new JArray(info.Enum)));
            }
            else
            {
                //obj.Add(new JProperty("required", new JArray()));
                obj.Add(new JProperty("properties", new JObject()));
            }
        }
    }
}
