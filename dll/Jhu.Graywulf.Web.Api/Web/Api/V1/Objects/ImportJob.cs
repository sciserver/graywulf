using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Web.Api.V1
{
    [Description("Represents a data table import job.")]
    public class ImportJob : Job
    {
        #region Private member variables

        private string destination;
        private Uri uri;
        private FileFormat fileFormat;
        private Credentials credentials;

        #endregion
        #region Properties

        [DataMember(Name = "uri")]
        [Description("URI of the target file.")]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        [DataMember(Name = "fileFormat", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Format of the file. Overrides format infered from extension.")]
        public FileFormat FileFormat
        {
            get { return fileFormat; }
            set { fileFormat = value; }
        }

        [DataMember(Name = "credentials", EmitDefaultValue = false)]
        [Description("Credentials to access the source URI.")]
        [DefaultValue(null)]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        [DataMember(Name = "destination")]
        [Description("Destination of the import.")]
        public string Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        #endregion
        #region Constructors and initializers

        public ImportJob()
        {
            InitializeMembers();
        }

        public static new ImportJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new ImportJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Import;

            this.uri = null;
            this.fileFormat = null;
            this.credentials = null;
            this.destination = null;
        }

        #endregion

        public static DestinationTable GetDestinationTable(FederationContext context, string schemaName, string tableName)
        {
            var destination = new DestinationTable(
                    context.MyDBDataset,
                    context.MyDBDataset.DatabaseName,
                    context.MyDBDataset.DefaultSchemaName,
                    IO.Constants.ResultsetNameToken,        // generate table names automatically
                    TableInitializationOptions.Create | TableInitializationOptions.GenerateUniqueName);

            if (schemaName != null)
            {
                destination.SchemaName = schemaName;
            }

            if (tableName != null)
            {
                destination.TableNamePattern = tableName;   // TODO: handle patterns?
            }

            return destination;
        }

        public DestinationTable GetDestinationTable(FederationContext context)
        {
            if (Destination != null)
            {
                // Table names are specified as string, so we need to parse them
                var parser = new SqlParser.SqlParser();
                var nr = new SqlNameResolver()
                {
                    SchemaManager = context.SchemaManager,
                };

                var tn = (SqlParser.TableOrViewName)parser.Execute(new SqlParser.TableOrViewName(), destination);
                var tr = tn.TableReference;
                tr.SubstituteDefaults(context.SchemaManager, context.MyDBDataset.Name);

                return GetDestinationTable(context, tr.SchemaName, tr.DatabaseObjectName);
            }
            else
            {
                return GetDestinationTable(context, null, null);
            }
        }

        

        public ImportTablesParameters CreateParameters(FederationContext context)
        {
            DataFileBase source = null;
            DestinationTable destination = null;
            IO.Credentials credentials = null;

            if (FileFormat != null)
            {
                source = FileFormat.GetDataFile(context, uri);
            }

            destination = GetDestinationTable(context);

            if (Credentials != null)
            {
                credentials = Credentials.GetCredentials(context); 
            }


            var ff = ImportTablesJobFactory.Create(context.Federation);
            return ff.CreateParameters(context.Federation, uri, credentials, source, destination);
        }

        public override void Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ff = ImportTablesJobFactory.Create(context.Federation);
            var job = ff.ScheduleAsJob(p, GetQueueName(context), Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
