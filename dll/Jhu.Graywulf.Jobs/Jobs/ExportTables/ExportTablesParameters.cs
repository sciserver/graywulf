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
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    [Serializable]
    [DataContract(Name = "ExportTablesParameters", Namespace = "")]
    public class ExportTablesParameters
    {
        #region Private member variables

        private TableOrView[] sources;
        private DataFileBase[] destinations;
        private Uri uri;
        private Credentials credentials;
        private string fileFormatFactoryType;
        private string streamFactoryType;
        private DataFileArchival archival;
        private int timeout;

        #endregion
        #region Properties

        [DataMember]
        public TableOrView[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        [DataMember]
        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        [DataMember]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        [DataMember]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        [DataMember]
        public string FileFormatFactoryType
        {
            get { return fileFormatFactoryType; }
            set { fileFormatFactoryType = value; }
        }

        public string StreamFactoryType
        {
            get { return streamFactoryType; }
            set { streamFactoryType = value; }
        }

        public DataFileArchival Archival
        {
            get { return archival; }
            set { archival = value; }
        }

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        #endregion
        #region Constructors and initializers

        public ExportTablesParameters()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.sources = null;
            this.destinations = null;
            this.uri = null;
            this.credentials = null;
            this.fileFormatFactoryType = null;
            this.streamFactoryType = null;
            this.archival = DataFileArchival.Zip;
            this.timeout = 1200;    // *** TODO: get from settings
        }

        #endregion

        public IExportTableArchive GetInitializedTableExportTask()
        {
            // Determine server name from connection string
            // This is required, because bulk copy can go into databases that are only known
            // by their connection string
            // Get server name from data source name (requires trimming the sql server instance name)
            string host = ((Jhu.Graywulf.Schema.SqlServer.SqlServerDataset)sources[0].Dataset).Host;

            var ss = new SourceTableQuery[sources.Length];
            for (int i = 0; i < sources.Length; i++)
            {
                ss[i] = new SourceTableQuery()
                {
                    Dataset = sources[i].Dataset,
                    Query = String.Format("SELECT t.* FROM [{0}].[{1}] AS t", sources[i].SchemaName, sources[i].ObjectName)
                };
            }

            // Create bulk operation
            var te = RemoteServiceHelper.CreateObject<IExportTableArchive>(host);

            te.Sources = ss;
            te.Destinations = destinations;
            te.Uri = uri;
            te.FileFormatFactoryType = fileFormatFactoryType;
            te.StreamFactoryType = streamFactoryType;
            te.Timeout = timeout;

            return te;
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
