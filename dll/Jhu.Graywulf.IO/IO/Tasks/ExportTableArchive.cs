using System;
using System.Threading.Tasks;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(ExportTableArchive))]
    [NetDataContract]
    public interface IExportTableArchive : ICopyTableArchiveBase
    {
        SourceQuery[] Sources
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }

        DataFileBase[] Destinations
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    /// <summary>
    /// Implements functions to export a set of tables into a set of files, all wrapped
    /// into a single archive.
    /// </summary>
    [Serializable]
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ExportTableArchive : CopyTableArchiveBase, IExportTableArchive, ICloneable, IDisposable
    {
        #region Private member variables

        private SourceQuery[] sources;
        private DataFileBase[] destinations;

        private int current;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the sources of the export operation.
        /// </summary>
        public SourceQuery[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        /// <summary>
        /// Gets or sets the destination files of the export operation.
        /// </summary>
        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        #endregion
        #region Constructors and initializers

        public ExportTableArchive()
        {
            InitializeMembers();
        }

        public ExportTableArchive(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public ExportTableArchive(ExportTableArchive old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.sources = null;
            this.destinations = null;
        }

        private void CopyMembers(ExportTableArchive old)
        {
            this.sources = Util.DeepCloner.CloneArray(old.sources);
            this.destinations = Util.DeepCloner.CloneArray(old.destinations);
        }

        public override object Clone()
        {
            return new ExportTableArchive(this);
        }

        #endregion

        protected override TableCopyResult CreateResult()
        {
            return sources[current].CreateResult();
        }

        /// <summary>
        /// Opens the archive file for writing.
        /// </summary>
        public override async Task OpenAsync()
        {
            await OpenAsync(DataFileMode.Write, DataFileArchival.Automatic);
        }
        
        /// <summary>
        /// Executes the table export operation
        /// </summary>
        protected override async Task OnExecuteAsync()
        {
            if (BaseStream == null)
            {
                throw new InvalidOperationException();
            }

            // Make sure it's an archive stream
            if (!(BaseStream is IArchiveOutputStream))
            {
                throw new InvalidOperationException();
            }

            if (sources == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (destinations == null)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            if (sources.Length != destinations.Length)
            {
                throw new InvalidOperationException();  // *** TODO
            }

            var stream = (IArchiveOutputStream)BaseStream;

            // Write individual tables into the archive

            // TODO: add logic to handle multiple resultsets
            // certain types of files can hold multiple resultsets but many
            // of them can contain only one, so iterating through the resultsets
            // has to happen here and conditionally, inside the file writer
            // function: DataFileBase.WriteFromDataReader

            // Iterate through all source queries
            for (int i = 0; i < sources.Length; i++)
            {
                current = i;
                
                try
                {
                    // Open the destination file that will be written into the archive
                    // and copy the table into the file

                    // TODO: figure out whether destination file supports multiple resultsets
                    // or new files need to be created for every single resultset

                    destinations[i].Open(BaseStream, DataFileMode.Write);
                    await CopyToFileAsync(sources[i], destinations[i]);
                }
                catch (Exception ex)
                {
                    var result = CreateResult();
                    HandleException(ex, result);
                    Results.Add(result);
                    continue;
                }
                finally
                {
                    destinations[i].Dispose();
                }
            }

            stream.Finish();
        }
    }
}
