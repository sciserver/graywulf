using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.Schema
{
    public class Error
    {
        public static SchemaException InvalidDataTypeName(DataType dataType)
        {
            string message;
            message = ExceptionMessages.InvalidDataTypeName;
            return new SchemaException(String.Format(message, dataType.ToString()));
        }

        public static SchemaException AmbigousDataTypeName(DataType dataType)
        {
            string message;
            message = ExceptionMessages.AmbigousDataTypeName;
            return new SchemaException(String.Format(message, dataType.ToString()));
        }

        public static SchemaException InvalidObjectName(DatabaseObject databaseObject)
        {
            string message;

            switch (Constants.DatabaseObjectTypes[databaseObject.GetType()])
            {
                case DatabaseObjectType.Table:
                    message = ExceptionMessages.InvalidTableName;
                    break;
                case DatabaseObjectType.View:
                    message = ExceptionMessages.InvalidViewName;
                    break;
                case DatabaseObjectType.TableValuedFunction:
                    message = ExceptionMessages.InvalidTableValuedFunctionName;
                    break;
                case DatabaseObjectType.ScalarFunction:
                    message = ExceptionMessages.InvalidScalarFunctionName;
                    break;
                case DatabaseObjectType.StoredProcedure:
                    message = ExceptionMessages.InvalidStoredProcedureName;
                    break;
                default:
                    throw new NotImplementedException();
            }

            return new SchemaException(String.Format(message, databaseObject.ToString()));
        }

        public static SchemaException AmbigousObjectName(DatabaseObject databaseObject)
        {
            var message = ExceptionMessages.AmbigousObjectName;
            return new SchemaException(String.Format(message, databaseObject.ToString()));
        }
    }
}
