using System;
using System.Collections.Generic;
using System.IO;
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
            // TODO: assume a single endpoint here, which is currently true

            var service = new RestServiceContract(api, type);
            var attr = type.GetCustomAttribute<ServiceNameAttribute>();

            if (attr == null)
            {
                throw Error.MissingServiceNameAttribute(service.Type);
            }

            service.ServiceName = attr?.Name ?? type.Name;

            service.ServiceVersion = type.Assembly.GetName().Version.ToString();
            service.ServiceDescription = ReflectDescription(type);

            ReflectOperationContracts(service);

            api.ServiceContracts.Add(type, service);
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

                    if (!service.UriTemplates.ContainsKey(method.UriTemplate.Value))
                    {
                        service.UriTemplates.Add(method.UriTemplate.Value, new Dictionary<string, RestOperationContract>());
                    }

                    if (!Constants.ReservedFunctionNames.Contains(method.HttpMethod))
                    {
                        service.UriTemplates[method.UriTemplate.Value].Add(method.HttpMethod, method);
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
            operation.OperationDescription = ReflectDescription(method);

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
            var pars = operation.Method.GetParameters();

            for (int i = 0; i < pars.Length; i++)
            {
                var parameter = ReflectParameter(operation, pars[i]);
                operation.Parameters.Add(parameter);

                if (parameter.IsBodyParameter())
                {
                    operation.BodyParameter = parameter;
                }
            }

            if (operation.Method.ReturnType != typeof(void))
            {
                var parameter = ReflectParameter(operation, operation.Method.ReturnParameter);
                operation.ReturnParameter = parameter;
            }
        }

        private RestMessageParameter ReflectParameter(RestOperationContract operation, ParameterInfo parameter)
        {
            var par = new RestMessageParameter(operation, parameter);

            par.DataContract = ReflectType(parameter.ParameterType);

            par.ParameterName = parameter.Name;

            return par;
        }

        private RestDataContract ReflectDataContract(Type type)
        {
            var attr = type.GetCustomAttribute<DataContractAttribute>();

            var dataContract = new RestDataContract(api, type)
            {
                DataContractName = attr?.Name ?? type.Name,
                DataContractDescription = ReflectDescription(type),
            };

            ReflectDataMembers(dataContract);

            return dataContract;
        }

        private void ReflectDataMembers(RestDataContract dataContract)
        {
            var props = dataContract.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            for (int i = 0; i < props.Length; i++)
            {
                var attr = props[i].GetCustomAttribute<DataMemberAttribute>();

                if (attr != null)
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

        private RestDataContract ReflectType(Type type)
        {
            if (type == typeof(Stream) || type.IsSubclassOf(typeof(Stream)))
            {
                // TODO: figure this out
                return null;
            }
            else if (type.IsPrimitive || type == typeof(String) || type == typeof(Guid))
            {
                return null;
            }
            else
            {
                if (!api.DataContracts.ContainsKey(type))
                {
                    var dc = ReflectDataContract(type);
                    
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    {
                        dc.ElementType = type.GetGenericArguments()[0];
                    }
                    else if (type.IsArray)
                    {
                        dc.ElementType = type.GetElementType();
                    }

                    api.DataContracts.Add(type, dc);

                    if (dc.ElementType != null)
                    {
                        ReflectType(dc.ElementType);
                    }
                }

                return api.DataContracts[type];
            }
        }
    }
}
