using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.Mapping
{
    internal static class DbError
    {
        public static EntityException NoDbTableAttributeFound(Type type)
        {
            return new EntityException(
                String.Format(
                    ErrorMessages.NoDbTableAttributeFound,
                    type.FullName));
        }

        public static EntityException DuplicateKeyColumn(string column, Type t)
        {
            return new EntityException(
                String.Format(
                    ErrorMessages.DuplicateKeyColumn,
                    column, t.FullName));
        }

        public static EntityException InvalidColumnType(string column, Type t)
        {
            return new EntityException(
                String.Format(
                    ErrorMessages.InvalidColumnType,
                    column, t.FullName));
        }
    }
}
