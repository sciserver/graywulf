﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Tasks;
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

            var source = new SourceQuery()
            {
                Dataset = dataset,
                Query = this.query,
            };

            var destination = ff.CreateFile(filename);

            using (var cancellationContext = new CancellationContext())
            {
                var export = new ExportTable(cancellationContext)
                {
                    Source = source,
                    Destination = destination,
                    Settings = new TableCopySettings()
                    {
                        FileFormatFactoryType = "",
                        StreamFactoryType = "",
                    }
                };

                Console.WriteLine("Exporting table...");

                Util.TaskHelper.Wait(export.OpenAsync());
                Util.TaskHelper.Wait(export.ExecuteAsync());
                export.Close();

                Console.WriteLine("     done.");
            }
        }
    }
}
