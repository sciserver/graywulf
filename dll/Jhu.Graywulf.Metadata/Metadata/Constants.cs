using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using smo = Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Metadata
{
    class Constants
    {
        public static readonly Dictionary<ObjectType, List<ParameterType>> TagPairs = new Dictionary<ObjectType, List<ParameterType>>() 
            {
                { ObjectType.Table, new List<ParameterType> {ParameterType.Column} },
                { ObjectType.View, new List<ParameterType> {ParameterType.Column }},
                { ObjectType.Procedure, new List<ParameterType> {ParameterType.Param }},
                { ObjectType.Function, new List<ParameterType> {ParameterType.Param, ParameterType.Column }},
            };

        public static readonly Dictionary<Type, ObjectType> ObjectTypeMap = new Dictionary<Type, ObjectType>()
        { 
            { typeof(smo::Table), ObjectType.Table },
            { typeof(smo::View), ObjectType.View },
            { typeof(smo::StoredProcedure), ObjectType.Procedure },
            { typeof(smo::UserDefinedFunction), ObjectType.Function },
        };

        public static readonly Dictionary<Type, ParameterType> ParameterTypeMap = new Dictionary<Type, ParameterType>()
        {
            { typeof(smo::Column), ParameterType.Column },
            { typeof(smo::Parameter), ParameterType.Param },
            { typeof(smo::StoredProcedureParameter), ParameterType.Param },
            { typeof(smo::UserDefinedFunctionParameter), ParameterType.Param },
        };

        public static readonly List<string> ObjectElements = new List<string>() { "summary", "remarks", "type", "returns" };
        public static readonly List<string> ParameterAttributes = new List<string>() { "unit", "content", "class", "enum" };

        // TODO: remove there
        public const string SchemaMeta = "meta";
        public const string SchemaSys = "sys";
        public const string SchemaInfoSch = "INFORMATION_SCHEMA";
        public const string TableEnum = "Enum";
        public const string TableIndexMap = "Enum";
        // -------------------

        public const string MetaExtendedPropertyName = "meta.{0}";

        public const string TagMetadata = "metadata";
        public const string TagDataset = "dataset";
        public const string TagEnum = "enum";
        public const string TagKey = "key";
        public const string AttributeName = "name";
        public const string AttributeValue = "value";
    }
}
