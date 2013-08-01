using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DistributedPartitionedView : Entity
    {
        #region Database IO Functions

        /// <summary>
        /// Loads the entity from a <b>SqlDataReader</b> object.
        /// </summary>
        /// <param name="dr">The data reader to read from.</param>
        /// <returns>Returns the number of columns read.</returns>
        /// <remarks>
        /// Always reads at the current cursor position, doesn't calls the <b>Read</b> function
        /// on the <b>SqlDataReader</b> object. Reads data columns by their ordinal position in
        /// the query and not by their names.
        /// </remarks>
        internal override int LoadFromDataReader(SqlDataReader dr)
        {
            int o = base.LoadFromDataReader(dr);

            ++o;    // skip guid
            this.viewSchema = dr.GetString(++o);
            this.viewName = dr.GetString(++o);
            this.databaseVersionReference.Guid = dr.GetGuid(++o);
            this.referencedDatabaseDefinitionReference.Guid = dr.GetGuid(++o);
            this.referencedTableSchema = dr.GetString(++o);
            this.referencedTableName = dr.GetString(++o);
            this.referencedDatabaseVersionReference.Guid = dr.GetGuid(++o);

            return o;
        }

        /// <summary>
        /// Appends required parameters to create/modify stored procedure commands
        /// </summary>
        /// <param name="cmd">The <b>SqlCommand</b> to append the parameters to.</param>
        protected override void AppendCreateModifyParameters(SqlCommand cmd)
        {
            cmd.Parameters.Add("@ViewSchema", SqlDbType.NVarChar, 50).Value = viewSchema;
            cmd.Parameters.Add("@ViewName", SqlDbType.NVarChar, 128).Value = viewName;
            cmd.Parameters.Add("@DatabaseVersionGuid", SqlDbType.UniqueIdentifier).Value = databaseVersionReference.Guid;
            cmd.Parameters.Add("@ReferencedDatabaseDefinitionGuid", SqlDbType.UniqueIdentifier).Value = referencedDatabaseDefinitionReference.Guid;
            cmd.Parameters.Add("@ReferencedTableSchema", SqlDbType.NVarChar, 50).Value = referencedTableSchema;
            cmd.Parameters.Add("@ReferencedTableName", SqlDbType.NVarChar, 128).Value = referencedTableName;
            cmd.Parameters.Add("@ReferencedDatabaseVersionGuid", SqlDbType.UniqueIdentifier).Value = referencedDatabaseVersionReference.Guid;
        }

        #endregion
    }
}
