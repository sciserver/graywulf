using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.ExportTable
{
    [DataContract]
    [Serializable]
    public class ExportTable
    {
        /*private string databaseInstanceName;
        private string connectionString;
        private string datasetName;
        private string databaseName;
        private string schemaName;
        private string tableName;*/
        private TableOrView source;
        private DataFileBase destination;

        /*[DataMember]
        public string DatabaseInstanceName
        {
            get { return databaseInstanceName; }
            set { databaseInstanceName = value; }
        }

        [DataMember]
        public string ConnectionString
        {
            get { return connectionString; }
            set { connectionString = value; }
        }

        [DataMember]
        public string DatasetName
        {
            get { return datasetName; }
            set { datasetName = value; }
        }

        [DataMember]
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        [DataMember]
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        [DataMember]
        public string TableName
        {
            get { return tableName; }
            set { tableName = value; }
        }*/

        [DataMember]
        public TableOrView Source
        {
            get { return source; }
            set { source = value; }
        }

        [DataMember]
        public DataFileBase Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public ExportTable()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            //this.databaseInstanceName = String.Empty;
            //this.schemaName = null;
            //this.datasetName = null;
            //this.databaseName = null;
            //this.tableName = null;
            this.source = null;
            this.destination = null;
        }

        public IDataFileExporter GetInitializedExporter()
        {
            // Determine server name from connection string
            // This is required, because bulk copy can go into databases that are only known
            // by their connection string
            // Get server name from data source name (requires trimming the sql server instance name)
            string host;

            var csb = new SqlConnectionStringBuilder(source.Dataset.ConnectionString);
            int i = csb.DataSource.IndexOf('\\');
            if (i > -1)
            {
                host = csb.DataSource.Substring(i);
            }
            else
            {
                host = csb.DataSource;
            }

            // Create bulk operation
            var sq = new SourceQueryParameters();
            sq.Dataset = source.Dataset;
            //sq.ProviderInvariantName = Jhu.Graywulf.Schema.Constants.SqlServerProviderName;
            //sq.ConnectionString = connectionString;
            sq.Query = String.Format("SELECT t.* FROM [{0}].[{1}] AS t", source.SchemaName, source.ObjectName);

            var dfe = RemoteServiceHelper.CreateObject<IDataFileExporter>(host);
            dfe.Source = sq;
            dfe.Destination = destination;

            return dfe;
        }

        /*
        public void DeleteOutput()
        {
            // Find all files starting with destinationFile
            string[] files = Directory.GetFiles(
                Path.GetDirectoryName(destination.Path),
                String.Format("{0}.*", Path.GetFileNameWithoutExtension(destination.Path)));

            for (int i = 0; i < files.Length; i++)
            {
                File.Delete(files[i]);
            }
        }
         * */
    }
}
