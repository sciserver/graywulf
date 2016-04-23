using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Entities
{
    public static class Error
    {
        public static InvalidOperationException ContextInvalid()
        {
            return new InvalidOperationException(ErrorMessages.ContextInvalid);
        }

        public static InvalidOperationException ConnectionAlreadyOpen()
        {
            return new InvalidOperationException(ErrorMessages.ConnectionAlreadyOpen);
        }

        public static InvalidOperationException ConnectionNotOpen()
        {
            return new InvalidOperationException(ErrorMessages.ConnectionNotOpen);
        }

        public static InvalidOperationException TransactionAlreadyOpen()
        {
            return new InvalidOperationException(ErrorMessages.TransactionAlreadyOpen);
        }

        public static InvalidOperationException TransactionNotOpen()
        {
            return new InvalidOperationException(ErrorMessages.TransactionNotOpen);
        }

        public static NoResultsException NoResults(int expected)
        {
            return new NoResultsException(String.Format(ErrorMessages.NoResults, expected));
        }

        public static EntityException ErrorCreateEntity()
        {
            return new EntityException(ErrorMessages.ErrorCreateEntity);
        }

        public static EntityException ErrorModifyEntity()
        {
            return new EntityException(ErrorMessages.ErrorModifyEntity);
        }

        public static EntityException ErrorDeleteEntity()
        {
            return new EntityException(ErrorMessages.ErrorDeleteEntity);
        }
    }
}
