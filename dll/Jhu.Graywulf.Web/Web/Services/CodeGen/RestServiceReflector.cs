﻿using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Web;
using System.Reflection;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    public class RestServiceReflector
    {
        private RestApi api;

        public RestApi Api
        {
            get { return api; }
        }

        public RestServiceReflector()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.api = new RestApi();
        }

        public void ReflectServiceContract(Type type, string serviceUrl)
        {
            // TODO: assume a single WCF endpoint here, which is currently true

            type = GetServiceInterface(type);

            var service = new RestServiceContract(api, type);
            var attr = type.GetCustomAttribute<ServiceNameAttribute>(true);

            if (attr == null)
            {
                throw Error.MissingServiceNameAttribute(service.Type);
            }

            service.ServiceName = attr?.Name ?? type.Name;
            service.ServiceUrl = serviceUrl;
            service.ServiceVersion = type.Assembly.GetName().Version.ToString();
            service.Description = ReflectDescription(type);

            ReflectOperationContracts(service);

            api.ServiceContracts.Add(type, service);

            // Add error type

            if (!api.DataContracts.ContainsKey(typeof(RestError)))
            {
                var dc = ReflectDataContract(typeof(RestError));
                api.DataContracts.Add(typeof(RestError), dc);
            }
        }

        private Type GetServiceInterface(Type type)
        {
            if (type.IsInterface)
            {
                return type;
            }
            else
            {
                // Find interface with the RestServiceBehavior attribute
                var ifs = type.GetInterfaces();

                for (int i = 0; i < ifs.Length; i++)
                {
                    var attr = ifs[i].GetCustomAttribute<System.ServiceModel.ServiceContractAttribute>(true);
                    if (attr != null)
                    {
                        return ifs[i];
                    }
                }
            }

            throw Error.MissingServiceContractAttribute(type);
        }

        private void ReflectOperationContracts(RestServiceContract service)
        {
            var methods = service.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public);

            // Sort by uri template
            for (int i = 0; i < methods.Length; i++)
            {
                var attr = methods[i].GetCustomAttribute<OperationContractAttribute>();

                if (attr != null)
                {
                    var method = ReflectOperationContract(service, methods[i]);

                    if (!service.UriTemplates.ContainsKey(method.UriTemplate.Path))
                    {
                        service.UriTemplates.Add(method.UriTemplate.Path, new Dictionary<string, RestOperationContract>());
                    }

                    if (!Constants.ReservedFunctionNames.Contains(method.HttpMethod))
                    {
                        service.UriTemplates[method.UriTemplate.Path].Add(method.HttpMethod, method);
                    }
                    else
                    {
                        throw Error.DuplicateMethod(method);
                    }
                }
            }
        }

        private RestOperationContract ReflectOperationContract(RestServiceContract service, MethodInfo method)
        {
            var operation = new RestOperationContract(service, method);

            operation.OperationName = method.Name;
            operation.Description = ReflectDescription(method);

            var attr = method.GetCustomAttributes(typeof(WebGetAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                var wbg = (WebGetAttribute)attr[0];
                operation.HttpMethod = "GET";
                operation.UriTemplate = new RestUriTemplate(wbg.UriTemplate);
            }

            attr = method.GetCustomAttributes(typeof(WebInvokeAttribute), true);
            if (attr != null && attr.Length > 0)
            {
                var wbi = (WebInvokeAttribute)attr[0];
                operation.HttpMethod = wbi.Method.ToUpper();
                operation.UriTemplate = new RestUriTemplate(wbi.UriTemplate);
            }

            if (operation.HttpMethod == null || operation.UriTemplate == null)
            {
                throw Error.MissingHttpMethodOrUriTemplate(method);
            }

            ReflectOperationParameters(operation);

            return operation;
        }

        private void ReflectOperationParameters(RestOperationContract operation)
        {
            // See if a formatter attribute is defined on the operation
            Serialization.RawMessageFormatterBase formatter = null;
            var attr = operation.Method.GetCustomAttribute<Serialization.RawFormatAttribute>();

            if (attr != null)
            {
                formatter = attr.CreateFormatter();
                formatter.Initialize();
            }

            // Process parameters
            var pars = operation.Method.GetParameters();

            for (int i = 0; i < pars.Length; i++)
            {
                var parameter = ReflectParameter(operation, pars[i], formatter);

                if (parameter.IsBodyParameter)
                {
                    operation.BodyParameter = parameter;
                }
                else
                {
                    operation.Parameters.Add(parameter);
                }
            }

            // Process return value
            if (operation.Method.ReturnType != typeof(void))
            {
                var parameter = ReflectParameter(operation, operation.Method.ReturnParameter, formatter);
                parameter.IsReturnParameter = true;
                operation.ReturnParameter = parameter;
            }
        }

        private RestMessageParameter ReflectParameter(RestOperationContract operation, ParameterInfo parameter, Serialization.RawMessageFormatterBase formatter)
        {
            // TODO: it now supports body formatters but what about untyped streams?

            var par = new RestMessageParameter(operation, parameter);
            var type = formatter?.FormattedType;
            var israw = formatter != null && type != null &&
                       (type == parameter.ParameterType ||
                        type.IsAssignableFrom(parameter.ParameterType));

            if (israw && (par.IsBodyParameter || par.IsReturnParameter))
            {
                par.IsRawFormat = true;
                par.Formatter = formatter;
            }
            else
            {
                par.DataContract = ReflectType(parameter.ParameterType);
            }

            if (israw && (par.IsBodyParameter || par.IsReturnParameter))
            {
                par.Formats = formatter?.GetSupportedFormats() ?? new List<Serialization.RestBodyFormat>() { Serialization.RestBodyFormats.Json };
            }
            else if (par.IsBodyParameter || par.IsReturnParameter)
            {
                par.Formats = new List<Serialization.RestBodyFormat>()
                {
                    Serialization.RestBodyFormats.Json,
                    Serialization.RestBodyFormats.Xml
                };
            }

            return par;
        }

        private RestDataContract ReflectEnum(Type type)
        {
            var attr = type.GetCustomAttribute<DataContractAttribute>();

            var dataContract = new RestDataContract(api, type)
            {
                DataContractName = attr?.Name ?? type.Name,
                Description = ReflectDescription(type),
                IsClass = false,
                IsDictionary = false,
                IsEnum = true,
            };

            return dataContract;
        }

        private RestDataContract ReflectDictionary(Type type, Type keyType, Type elementType)
        {
            string name;
            var dataContractAttr = type.GetCustomAttribute<DataContractAttribute>(true);
            var collectionAttr = type.GetCustomAttribute<CollectionDataContractAttribute>(true);

            if (type.IsGenericType)
            {
                name = elementType.Name + "Dictionary";
            }
            else
            {
                name = type.Name;
            }

            name = collectionAttr.Name ?? dataContractAttr?.Name ?? name;

            var dataContract = new RestDataContract(api, type)
            {
                DataContractName = name,
                Description = ReflectDescription(type),
                IsClass = true,
                IsDictionary = true,
                IsEnum = false,
            };

            return dataContract;
        }

        private RestDataContract ReflectDataContract(Type type)
        {
            var dataContractAttr = type.GetCustomAttribute<DataContractAttribute>(true);

            var dataContract = new RestDataContract(api, type)
            {
                DataContractName = dataContractAttr?.Name ?? type.Name,
                Description = ReflectDescription(type),
                IsClass = true,
                IsDictionary = false,
                IsEnum = false,
            };

            ReflectDataMembers(dataContract);

            return dataContract;
        }

        private void ReflectDataMembers(RestDataContract dataContract)
        {
            var props = dataContract.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < props.Length; i++)
            {
                var attr = props[i].GetCustomAttribute<IgnoreDataMemberAttribute>();

                if (attr == null)
                {
                    var member = ReflectDataMember(dataContract, props[i]);
                    dataContract.DataMembers.Add(member.DataMemberName, member);
                }
            }
        }

        private RestDataMember ReflectDataMember(RestDataContract dataContract, PropertyInfo property)
        {
            var member = new RestDataMember(dataContract, property);

            var attr = property.GetCustomAttribute<DataMemberAttribute>();

            member.DataMemberName = attr?.Name ?? property.Name;
            dataContract = ReflectType(property.PropertyType);

            return member;
        }


        private string ReflectDescription(MemberInfo item)
        {
            var attr = item.GetCustomAttribute<DescriptionAttribute>();

            if (attr != null)
            {
                return attr.Description;
            }
            else
            {
                return "";
            }
        }

        public static bool IsNullableType(Type type, out Type elementType)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                elementType = type.GetGenericArguments()[0];
            }
            else
            {
                elementType = type;
            }

            return elementType != type;
        }

        private static IEnumerable<Type> GetGenericInterfaces(Type type)
        {
            if (type.IsInterface && type.IsGenericType)
            {
                yield return type;
            }

            foreach (var iface in type.GetInterfaces())
            {
                if (iface.IsGenericType)
                {
                    yield return iface;
                }
            }
        }

        public static bool IsArrayType(Type type, out Type elementType)
        {
            elementType = null;

            if (type == typeof(string))
            {
                return false;
            }

            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            else
            {
                foreach (var iface in GetGenericInterfaces(type))
                {
                    var gt = iface.GetGenericTypeDefinition();

                    if (gt == typeof(IEnumerable<>) ||
                        gt == typeof(IList<>) ||
                        gt == typeof(List<>))
                    {
                        elementType = iface.GetGenericArguments()[0];
                        break;
                    }
                }
            }

            return elementType != null;
        }

        public static bool IsDictionaryType(Type type, out Type keyType, out Type valueType)
        {
            keyType = null;
            valueType = null;

            foreach (var iface in GetGenericInterfaces(type))
            {
                var gt = iface.GetGenericTypeDefinition();

                if (gt == typeof(IDictionary<,>))
                {
                    var args = iface.GetGenericArguments();
                    keyType = args[0];
                    valueType = args[1];
                }
            }

            if (keyType == null && type == typeof(NameValueCollection) || type.IsSubclassOf(typeof(NameValueCollection)))
            {
                keyType = typeof(string);
                valueType = typeof(string);
            }

            return keyType != null;
        }

        private RestDataContract ReflectType(Type type)
        {
            Type t;
            Type keyType = null;
            Type elementType = null;
            bool isNullable;
            bool isArray;
            bool isDictionary;

            if (isNullable = IsNullableType(type, out t))
            {
                elementType = t;
            }

            if (isArray = IsArrayType(type, out t))
            {
                elementType = t;
            }

            if (isDictionary = IsDictionaryType(type, out keyType, out t))
            {
                elementType = t;
            }

            if (type == typeof(Stream) || type.IsSubclassOf(typeof(Stream)))
            {
                // TODO: figure this out
                return null;
            }
            else if (type == typeof(System.ServiceModel.Channels.Message) || type.IsSubclassOf(typeof(System.ServiceModel.Channels.Message)))
            {
                // TODO: figure this out
                return null;
            }
            else if (type.IsPrimitive || Constants.PrimitiveTypes.Contains(type))
            {
                return null;
            }
            else if (keyType != null && elementType != null)
            {
                if (keyType != typeof(string))
                {
                    throw Error.CollectionKeyMustBeString(type);
                }

                // This is a collection, reflect element type as
                // part of the service
            }

            if (elementType != null && !elementType.IsPrimitive && !Constants.PrimitiveTypes.Contains(elementType))
            {
                // This is a valid array, IEnumerable or List
                // Reflect element type as part of the service

                ReflectType(elementType);
            }

            if (!isNullable && !isArray)
            {
                if (!api.DataContracts.ContainsKey(type))
                {
                    RestDataContract dc;

                    if (type.IsEnum)
                    {
                        dc = ReflectEnum(type);
                    }
                    else if (isDictionary)
                    {
                        dc = ReflectDictionary(type, keyType, elementType);
                    }
                    else
                    {
                        dc = ReflectDataContract(type);
                    }

                    api.DataContracts.Add(type, dc);
                }

                return api.DataContracts[type];
            }
            else
            {
                return null;
            }
        }
    }
}
