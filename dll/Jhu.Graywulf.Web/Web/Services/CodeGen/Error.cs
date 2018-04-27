﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    class Error
    {
        public static RestProxyGeneratorException MissingServiceNameAttribute(Type contractType)
        {
            return new RestProxyGeneratorException(String.Format(ExceptionMessages.MissingServiceNameAttribute, contractType.FullName));
        }

        public static RestProxyGeneratorException MissingHttpMethodOrUriTemplate(MethodInfo method)
        {
            return new RestProxyGeneratorException(String.Format(ExceptionMessages.MissingHttpMethodOrUriTemplate, method.DeclaringType.FullName, method.Name));
        }

        public static RestProxyGeneratorException DuplicateMethod(RestOperationContract method)
        {
            return new RestProxyGeneratorException(String.Format(ExceptionMessages.DuplicateMethod, method.HttpMethod, method.UriTemplate.Value));
        }
    }
}
