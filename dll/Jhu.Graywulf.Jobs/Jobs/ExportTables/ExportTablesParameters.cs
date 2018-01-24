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
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Jobs.ExportTables
{
    /// <summary>
    /// Represents a set of parameters that describe a table export job.
    /// </summary>
    [Serializable]
    [DataContract(Name = "ExportTablesParameters", Namespace = "")]
    public class ExportTablesParameters
    {
        #region Private member variables

        private SourceTableQuery[] sources;
        private DataFileBase[] destinations;
        private Uri uri;
        private Credentials credentials;
        private string fileFormatFactoryType;
        private string streamFactoryType;
        private DataFileArchival archival;
        private int timeout;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets an array of tables that will be exported.
        /// </summary>
        [DataMember]
        public SourceTableQuery[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        /// <summary>
        /// Gets or sets an array of data files that carry specific
        /// format settings for the individual files.
        /// </summary>
        [DataMember]
        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        /// <summary>
        /// Gets or sets the destination URI of the export.
        /// </summary>
        /// <remarks>
        /// This is a single URI, meaning multiple files will be saved into
        /// an archive, or put into a directory pointed by this URI.
        /// </remarks>
        [DataMember]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        /// <summary>
        /// Gets or sets the credentials to access the destination.
        /// </summary>
        [DataMember]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        /// <summary>
        /// Gets or sets the name of the file format factory to be used during export.
        /// </summary>
        [DataMember]
        public string FileFormatFactoryType
        {
            get { return fileFormatFactoryType; }
            set { fileFormatFactoryType = value; }
        }

        /// <summary>
        /// Gets or sets the name of the stream format factory that is
        /// used to open a stream to the destination URI.
        /// </summary>
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
            this.archival = DataFileArchival.None;
            this.timeout = 1200;    // *** TODO: get from settings
        }

        #endregion

        /// <summary>
        /// Returns an initialized table export task based on the job parameters
        /// </summary>
        /// <returns></returns>
        public ServiceModel.IServiceProxy<ICopyDataStream> GetInitializedTableExportTask(CancellationContext cancellationContext)
        {
            var sf = StreamFactory.Create(streamFactoryType);

            // Determine server name from connection string
            // This is required, because bulk copy can go into databases that are only known
            // by their connection string

            // Get server name from the very first data source
            // (requires trimming the sql server instance name)
            // This will be the database server responsible for executing the export
            string host = ((Jhu.Graywulf.Schema.SqlServer.SqlServerDataset)sources[0].Dataset).HostName;

            // Export works in two modes: single file and archive mode.
            // In archive mode, the file is exported using ExportTableArchive task whereas
            // in single file mode a simpler ExportTable task is created

            // If arhival mode is set to automatic, figure out mode from extensions
            if (this.Archival == DataFileArchival.Automatic)
            {
                this.Archival = sf.GetArchivalMethod(this.Uri);
            }

            if (this.Archival == DataFileArchival.None)
            {
                // Single file mode
                // Use only first item from sources and destinations
                // TODO: this could be extended but that would mean multiple tasks

                var task = RemoteServiceHelper.CreateObject<IExportTable>(cancellationContext, host, true);

                task.Value.Source = this.sources[0];
                task.Value.Destination = this.destinations[0];
                task.Value.FileFormatFactoryType = this.fileFormatFactoryType;
                task.Value.StreamFactoryType = this.streamFactoryType;
                task.Value.Timeout = this.timeout;

                return task;
            }
            else
            {
                // Archive mode

                var task = RemoteServiceHelper.CreateObject<IExportTableArchive>(cancellationContext, host, true);

                task.Value.Sources = sources;
                task.Value.Destinations = destinations;
                task.Value.BatchName = Util.UriConverter.GetFileNameWithoutExtension(uri);
                task.Value.Uri = uri;
                task.Value.Credentials = credentials;
                task.Value.FileFormatFactoryType = fileFormatFactoryType;
                task.Value.StreamFactoryType = streamFactoryType;
                task.Value.Timeout = timeout;

                return task;
            }
        }
    }
}
