using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.Common;
using System.ServiceModel;
using Jhu.Graywulf.Components;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(ExportTableArchive))]
    [NetDataContract]
    public interface IExportTableArchive : ICopyTableArchiveBase
    {
        SourceTableQuery[] Sources
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

    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession,
        IncludeExceptionDetailInFaults = true)]
    public class ExportTableArchive : CopyTableArchiveBase, IExportTableArchive, ICloneable, IDisposable
    {
        private SourceTableQuery[] sources;
        private DataFileBase[] destinations;

        public SourceTableQuery[] Sources
        {
            get { return sources; }
            set { sources = value; }
        }

        public DataFileBase[] Destinations
        {
            get { return destinations; }
            set { destinations = value; }
        }

        public ExportTableArchive()
        {
            InitializeMembers();
        }

        public ExportTableArchive(ExportTableArchive old)
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
            this.sources = Util.DeepCopy.CopyArray(old.sources);
            this.destinations = Util.DeepCopy.CopyArray(old.destinations);
        }

        public override object Clone()
        {
            return new ExportTableArchive(this);
        }

        public override void Open()
        {
            Open(DataFileMode.Write, DataFileArchival.Automatic);
        }

        protected override void OnExecute()
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

            // Write individual tables into the archive


            for (int i = 0; i < sources.Length; i++)
            {
                var aos = (IArchiveOutputStream)BaseStream;

                var entry = aos.CreateFileEntry(destinations[i].Uri.ToString(), 0);
                aos.WriteNextEntry(entry);

                try
                {
                    destinations[i].Open(BaseStream, DataFileMode.Write);
                    WriteTable(sources[i], destinations[i]);
                }
                finally
                {
                    destinations[i].Close();
                }
            }
        }
    }
}
