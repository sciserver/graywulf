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

namespace Jhu.Graywulf.Jobs.ImportTables
{
    /// <summary>
    /// Represents a set of parameters that describe a table import job.
    /// </summary>
    [Serializable]
    [DataContract(Name = "ImportTablesParameters", Namespace = "")]
    public class ImportTablesParameters
    {
        #region Private member variables

        private DataFileBase[] sources;
        private DestinationTable[] destinations;
        private Uri uri;
        private Credentials credentials;
        private string fileFormatFactoryType;
        private string streamFactoryType;
        private DataFileArchival archival;
        private int timeout;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets an array of data files that carry specific
        /// format settings for the individual input files.
        /// </summary>
        /// <remarks>
        /// This field can be left null. In this case data files in
        /// the archive are identified and imported automatically.
        /// </remarks>
        [DataMember]
        public DataFileBase[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        /// <summary>
        /// Gets or sets the destination table (or table naming pattern)
        /// into which file(s) are to be read.
        /// </summary>
        [DataMember]
        public DestinationTable[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        /// <summary>
        /// Gets or sets the source URI of the import.
        /// </summary>
        /// <remarks>
        /// This can point to an archive or a directory. In these cases, multiple files
        /// are imported into a set of tables.
        /// </remarks>
        [DataMember]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// Gets or sets the credentials to access the source file.
        /// </summary>
        [DataMember]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        /// <summary>
        /// Gets or sets the name of the file format factory to be used during import.
        /// </summary>
        /// <remarks>
        /// This factory class is responsible for opening the files within the
        /// archive.
        /// </remarks>
        [DataMember]
        public string FileFormatFactoryType
        {
            get { return fileFormatFactoryType; }
            set { fileFormatFactoryType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the stream format factory that is
        /// used to open a stream to the source URI.
        /// </summary>
        /// <remarks>
        /// This factory is responsible for opening the right stream
        /// </remarks>
        [DataMember]
        public string StreamFactoryType
        {
            get { return streamFactoryType; }
            set { streamFactoryType = value; }
        }

        /// <summary>
        /// Gets or sets whether the destination is an archive.
        /// </summary>
        [DataMember]
        public DataFileArchival Archival
        {
            get { return archival; }
            set { archival = value; }
        }

        /// <summary>
        /// Gets or sets the execution time-out.
        /// </summary>
        [DataMember]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        #endregion
        #region Constructors and initializers

        public ImportTablesParameters()
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
            this.archival = DataFileArchival.Automatic;
            this.timeout = 1200;    // *** TODO: get from settings
        }

        #endregion

        /// <summary>
        /// Returns an initialized table export task based on the job parameters
        /// </summary>
        /// <returns></returns>
        public IExportTableArchive GetInitializedTableImportTask()
        {
            /*
            // Determine server name from connection string
            // This is required, because bulk copy can go into databases that are only known
            // by their connection string

            // Get server name from the very first data source
            // (requires trimming the sql server instance name)
            // This will be the database server responsible for executing the export
            // What
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
            var task = RemoteServiceHelper.CreateObject<IExportTableArchive>(host);

            task.Sources = ss;
            task.Destinations = destination;
            task.Uri = uri;
            task.FileFormatFactoryType = fileFormatFactoryType;
            task.StreamFactoryType = streamFactoryType;
            task.Timeout = timeout;

            return task;
             * */

            throw new NotImplementedException();
        }
    }
}
