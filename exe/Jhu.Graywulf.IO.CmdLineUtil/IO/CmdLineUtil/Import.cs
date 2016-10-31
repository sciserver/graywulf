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
        private bool createTable;
        private bool generateIdentity;
        private string order;
        private string filename;
        private int timeout;

        [Parameter(Name = "Table", Description = "Destination table", Required = true)]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        [Option(Name = "CreateTable", Description = "Create destination table", Required = false)]
        public bool CreateTable
        {
            get { return createTable; }
            set { createTable = value; }
        }

        [Option(Name = "Identity", Description = "Generate identity column", Required = false)]
        public bool GenerateIdentity
        {
            get { return generateIdentity; }
            set { generateIdentity = value; }
        }

        [Parameter(Name = "Order", Description = "Specify ordering of input", Required = false)]
        public string Order
        {
            get { return order; }
            set { order = value; }
        }

        [Parameter(Name = "Filename", Description = "Input file name", Required = true)]
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        [Parameter(Name = "Timeout", Description = "Bulk insert timeout", Required = false)]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        public Import()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.table = null;
            this.createTable = false;
            this.generateIdentity = false;
            this.order = null;
            this.filename = null;
            this.timeout = 0;
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
            source.GenerateIdentityColumn = generateIdentity;

            var destination = new DestinationTable()
            {
                Dataset = dataset,
                SchemaName = Schema.SqlServer.Constants.DefaultSchemaName,
                TableNamePattern = table,
                Options = TableInitializationOptions.Clear,      
            };

            if (createTable)
            {
                destination.Options |= TableInitializationOptions.Create;
            }

            // Create task
            var import = new ImportTable()
            {
                Source = source,
                Destination = destination,
                Timeout = timeout,
            };

            Console.WriteLine("Importing table...");

            import.Open();
            import.Execute();
            import.Close();

            Console.WriteLine("     done.");
        }
    }
}
