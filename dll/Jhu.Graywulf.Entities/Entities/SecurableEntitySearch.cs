using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    public abstract class SecurableEntitySearch<T> : EntitySearch<T>
        where T : Entity, new()
    {
        #region Constructors and initializers

        protected SecurableEntitySearch()
        {
        }

        protected SecurableEntitySearch(Context context)
            :base(context)
        {
        }

        #endregion

        protected override void AppendPreambleItems()
        {
            base.AppendPreambleItems();

            var sql = String.Format(
                "DECLARE {0} [entities].[Identity] = [entities].[Identity]::Parse({1});",
                Constants.IdentityVariableName,
                Constants.IdentityParameterName);

            AppendPreambleItem(sql);
        }

        protected override void AppendSearchCriteria()
        {
            base.AppendSearchCriteria();

            if (!(Context.Principal is AccessControl.Principal))
            {
                throw AccessControl.Error.NoSqlPrincipal();
            }

            var p = (AccessControl.Principal)Context.Principal;

            var criterion = String.Format("{0}.CanRead([{1}]) = 1", Constants.IdentityVariableName, Constants.AclColumnName);
            var id = new SqlParameter(Constants.IdentityParameterName, SqlDbType.NVarChar)
            {
                Value = p.ToXml()
            };

            AppendSearchCriterion(criterion);
            AppendSearchParameter(id);
        }
    }
}
