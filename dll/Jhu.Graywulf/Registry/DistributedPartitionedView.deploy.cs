using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using smo = Microsoft.SqlServer.Management.Smo;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements extension methods for the <b>DistributedPartitionedView</b> class
    /// to provide SMO based deployment of databases.
    /// </summary>
    public partial class DistributedPartitionedView
    {
        /// <summary>
        /// Deploys a distributed partitioned view to the target database.
        /// </summary>
        /// <param name="dpv">The distributed partitioned view object.</param>
        /// <param name="db">The SMO object pointing to the target database that will contain the distributed partitioned view.</param>
        /// <param name="databaseInstance"></param>
        /// <param name="recreateLinkedServers">If true linked server connections will be recreated.</param>
        public void Deploy(smo::Database db, DatabaseInstance databaseInstance, bool recreateLinkedServers)
        {
            // Ensure that database versions are matching
            if (this.databaseVersionReference.Guid != databaseInstance.DatabaseVersion.Guid)
            {
                throw new DatabaseVersionMismatchException(
                    String.Format(
                        ExceptionMessages.DatabaseVersionMismatch,
                        this.referencedDatabaseDefinitionReference.Name,
                        databaseInstance.DatabaseVersion.Name));
            }

            this.DeploymentState = DeploymentState.Deploying; this.Save();

            // Look for all database instances referenced by this distributed partitioned view
            // and make sure that the linked server connections to those servers exist
            this.ReferencedDatabaseDefinition.LoadDatabaseInstances(false);
            foreach (DatabaseInstance di in this.ReferencedDatabaseDefinition.DatabaseInstances.Values)
            {
                // Make sure that database version and requirement for creating a link (different servers) is required
                // The constraint on the composite names is required for testing
                if (di.DatabaseVersion.Guid == this.referencedDatabaseVersionReference.Guid && di.ServerInstance.Guid != databaseInstance.ServerInstance.Guid
                    && di.ServerInstance.GetCompositeName() != databaseInstance.ServerInstance.GetCompositeName())  // TODO: remove this line

                {
                    bool exists = databaseInstance.ServerInstance.CheckLinkedServerExist(di.ServerInstance);
                    if (recreateLinkedServers || !exists)
                    {
                        /*
                        if (exists)
                        {
                            databaseInstance.ServerInstance.DropLinkedServer(di.ServerInstance);
                        }*/

                        databaseInstance.ServerInstance.CreateLinkedServer(di.ServerInstance);
                    }
                }
            }

            // Look for the dummy view already generated and delete that one to make space
            // for the DPV
            if (db.Views.Contains(this.ViewName, this.ViewSchema))
            {
                db.Views[this.ViewName].Drop();
            }

            // Build query for the new view
            StringBuilder sql = new StringBuilder();

            // TODO add generation of comments here

            int q = 0;
            foreach (DatabaseInstance di in this.ReferencedDatabaseDefinition.DatabaseInstances.Values)
            {
                if (di.DatabaseVersion.Guid == this.referencedDatabaseVersionReference.Guid)
                {
                    if (q > 0)
                    {
                        sql.AppendLine("UNION ALL");
                    }

                    string query = string.Empty;

                    if (di.ServerInstance.Guid != databaseInstance.ServerInstance.Guid
                        && di.ServerInstance.GetCompositeName() != databaseInstance.ServerInstance.GetCompositeName())
                    {
                        // Data from linked server
                        query = "SELECT * FROM [{0}].[{1}].[{2}].[{3}]";
                        query = String.Format(query, di.ServerInstance.GetCompositeName(), di.DatabaseName, this.ReferencedTableSchema, this.ReferencedTableName);
                    }
                    else
                    {
                        // Data from the same server
                        query = "SELECT * FROM [{0}].[{1}].[{2}]";
                        query = String.Format(query, di.DatabaseName, this.ReferencedTableSchema, this.ReferencedTableName);
                    }

                    sql.AppendLine(query);

                    q++;
                }
            }

            // Create view
            smo::View v = new smo::View(db, this.ViewName);
            v.TextHeader = String.Format("CREATE VIEW [{0}].[{1}] AS", this.ViewSchema, this.ViewName);
            v.TextBody = sql.ToString();
            v.Create();

            // Copy extended properties
            // TODO write this

            this.DeploymentState = DeploymentState.Deployed; this.Save();

            this.Context.LogEvent(new Event("Jhu.Graywulf.Registry.DistributedPartitionedView.Deploy", this.Guid));
        }
    }
}
