using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Web.Services.CodeGen
{
    class Constants
    {
        public static readonly HashSet<string> ReservedFunctionNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase)
        {
            "HandleHttpOptionsRequest", "GenerateProxy"
        };

        public static readonly Dictionary<Type, string> SwaggerTypes = new Dictionary<Type, string>()
        {
            { typeof(Int32), "integer" },
            { typeof(Int64), "integer" },
            { typeof(Single), "number" },
            { typeof(Double), "number" },
            { typeof(String), "string" },
            { typeof(Byte), "string" },
            { typeof(Byte[]), "string" },
            { typeof(Boolean), "boolean" },
            { typeof(DateTime), "string" },
            { typeof(Guid), "string" },
            { typeof(Uri), "string" },
        };

        public static readonly Dictionary<Type, string> SwaggerFormats = new Dictionary<Type, string>()
        {
            { typeof(Int32), "int32" },
            { typeof(Int64), "int64" },
            { typeof(Single), "float" },
            { typeof(Double), "double" },
            { typeof(String), "string" },
            { typeof(Byte), "byte" },
            { typeof(Byte[]), "binary" },
            { typeof(Boolean), "boolean" },
            { typeof(DateTime), "date" },
            { typeof(Guid), "string" },
            { typeof(Uri), "string" },
        };

        public const string SwaggerTypeArray = "array";
        public const string SwaggerParameterInQuery = "query";
        public const string SwaggerParameterInPath = "path";
        public const string SwaggerParameterInBody = "body";
    }
}
