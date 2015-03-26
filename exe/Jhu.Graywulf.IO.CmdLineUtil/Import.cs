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
    [Verb(Name = "Import", Description = "Import table from file.")]
    class Import : DatabaseVerbBase
    {
        private string table;
        private string filename;

        [Parameter(Name = "Table", Description = "Destination table", Required = true)]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        [Parameter(Name = "Filename", Description = "Input file name", Required = true)]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        public Import()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.table = null;
            this.filename = null;
        }

        public override void Run()
        {
            var sf = StreamFactory.Create(null);
            var ff = FileFormatFactory.Create(null);

            var uri = Util.UriConverter.FromFilePath(filename);

            var dataset = new SqlServerDataset()
            {
                ConnectionString = GetConnectionString().ConnectionString,
                IsMutable = true,
            };

            var source = ff.CreateFile(filename);

            var destination = new DestinationTable()
            {
                Dataset = dataset,
                SchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                TableNamePattern = table,
                Options = TableInitializationOptions.Clear,      
            };

            // Create task
            var import = new ImportTable()
            {
                Source = source,
                Destination = destination,
            };

            Console.WriteLine("Importing table...");

            import.Open();
            import.Execute();
            import.Close();

            Console.WriteLine("     done.");
        }
    }
}
