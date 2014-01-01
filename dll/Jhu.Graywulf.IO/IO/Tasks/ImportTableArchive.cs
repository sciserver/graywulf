using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(ImportTableArchive))]
    public interface IImportTableArchive : ICopyTableArchiveBase
    {
        DestinationTable Destination
        {
            [OperationContract]
            get;
            [OperationContract]
            set;
        }
    }

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ImportTableArchive : CopyTableArchiveBase, IImportTableArchive, ICloneable, IDisposable
    {
        private DestinationTable destination;

        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        public ImportTableArchive()
        {
            InitializeMembers();
        }

        public ImportTableArchive(ImportTableArchive old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.destination = null;
        }

        private void CopyMembers(ImportTableArchive old)
        {
            this.destination = old.destination;
        }

        public override object Clone()
        {
            return new ImportTableArchive(this);
        }

        public override void Open()
        {
            Open(DataFileMode.Read, DataFileArchival.Automatic);
        }

        protected override void OnExecute()
        {
            // Make sure stream is open
            if (BaseStream == null)
            {
                throw new InvalidOperationException();
            }

            // Make sure it's an archive stream
            if (!(BaseStream is IArchiveInputStream))
            {
                throw new InvalidOperationException();
            }

            // Create the file format factory
            var ff = GetFileFormatFactory();

            // Read the archive file by file and import tables
            var ais = (IArchiveInputStream)BaseStream;
            IArchiveEntry entry;

            while ((entry = ais.ReadNextFileEntry()) != null)
            {
                if (!entry.IsDirectory)
                {
                    // Create source file
                    using (var format = ff.CreateFile(entry.Filename))
                    {
                        format.Open(BaseStream, DataFileMode.Read);

                        using (var cmd = new FileCommand(format))
                        {
                            // TODO: Pass table name here
                            ImportTable(cmd, destination);
                        }
                    }
                }
            }
        }
    }
}
