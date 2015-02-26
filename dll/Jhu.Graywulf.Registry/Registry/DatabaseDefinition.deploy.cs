using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Management.Smo;

namespace Jhu.Graywulf.Registry
{
    /// <summary>
    /// The class implements extension methods for the <b>DatabaseDefinition</b> class
    /// to provide SMO based deployment of databases.
    /// </summary>
    public partial class DatabaseDefinition
    {
        /// <summary>
        /// Returns the SMO object pointing to the template database belonging to the database definition.
        /// </summary>
        /// <param name="databaseDefinition"></param>
        /// <returns></returns>
        public Database GetSmoDatabase()
        {
            Server s;

            if (!this.SchemaSourceServerInstanceReference.IsEmpty)
            {
                s = this.SchemaSourceServerInstance.GetSmoServer();
            }
            else
            {
                s = this.Federation.SchemaSourceServerInstance.GetSmoServer();
            }

            return s.Databases[this.SchemaSourceDatabaseName];
        }

        public Index GetSmoIndex(string tableName, string indexName)
        {
            return this.GetSmoDatabase().Tables[tableName].Indexes[indexName];
        }

        // TODO review this and it's references in activities
        public string[] GetSmoIndexes(string tableName)
        {
            Table t = this.GetSmoDatabase().Tables[tableName];

            List<string> indexlist = new List<string>();
            foreach (Index i in t.Indexes)
            {
                indexlist.Add(i.Name);
            }
            
            return indexlist.ToArray();

            /*
            List<string> foreignkeylist = new List<string>();
            foreach (ForeignKey fk in t.ForeignKeys)
            {
                foreignkeylist.Add(fk.Name);
            }
            foreignKeys = foreignkeylist.ToArray();
             * */
        }
    }
}
