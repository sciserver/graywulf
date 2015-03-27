using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO.CmdLineUtil
{
    [Verb(Name = "Export", Description = "Export query results to file.")]
    class Export : DatabaseVerbBase
    {
        private string query;
        private string filename;

        [Parameter(Name = "Query", Description = "Input query", Required = true)]
        public string Query
        {
            get { return query; }
            set { query = value; }
        }

        [Parameter(Name = "Filename", Description = "Output file name", Required = true)]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public Export()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.query = null;
            this.filename = null;
        }

        public override void Run()
        {
            var uri = Util.UriConverter.FromFilePath(filename);

            var sf = StreamFactory.Create(null);
            sf.BufferSize = 0x40000;   // 256k
            sf.Options = System.IO.FileOptions.SequentialScan;

            var ff = FileFormatFactory.Create(null);

            var dataset = new SqlServerDataset()
            {
                ConnectionString = GetConnectionString().ConnectionString,
            };

            if (this.query.EndsWith(".sql", StringComparison.InvariantCultureIgnoreCase))
            {
                this.query = System.IO.File.ReadAllText(this.query);
            }

            var source = new SourceTableQuery()
            {
                Dataset = dataset,
                Query = this.query,
            };

            var destination = ff.CreateFile(filename);

            // Create task
            var export = new ExportTable()
            {
                Source = source,
                FileFormatFactoryType = "",
                StreamFactoryType = "",
                Destination = destination,
            };

            Console.WriteLine("Exporting table...");

            export.Open();
            export.Execute();
            export.Close();

            Console.WriteLine("     done.");
        }
    }
}
