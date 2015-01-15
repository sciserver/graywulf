using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Jobs.ImportTables;
using Jhu.Graywulf.SqlParser;

namespace Jhu.Graywulf.Web.Api.V1
{
    [Description("Represents a data table import job.")]
    public class ImportJob : Job
    {
        #region Private member variables

        private string table;
        private Uri uri;
        private Credentials credentials;

        #endregion
        #region Properties

        [DataMember(Name = "destination")]
        [Description("Destination of the import.")]
        public string Table
        {
            get { return table; }
            set { table = value; }
        }

        [DataMember(Name = "uri")]
        [Description("URI of the target file.")]
        public Uri Uri
        {
            get { return uri; }
            set { uri = value; }
        }

        [DataMember(Name = "credentials", EmitDefaultValue = false)]
        [Description("Credentials to access the source URI.")]
        [DefaultValue(null)]
        public Credentials Credentials
        {
            get { return credentials; }
            set { credentials = value; }
        }

        #endregion
        #region Constructors and initializers

        public ImportJob()
        {
            InitializeMembers();
        }

        public static ImportJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new ImportJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Import;

            this.uri = null;
            this.credentials = null;
        }

        #endregion

        public ImportTablesParameters CreateParameters(FederationContext context)
        {
            var ff = ImportTablesJobFactory.Create(context.Federation);

            // Table names are specified as string, so we need to parse them
            var parser = new SqlParser.SqlParser();
            var nr = new SqlNameResolver()
            {
                SchemaManager = context.SchemaManager,
            };

            var tn = (SqlParser.TableOrViewName)parser.Execute(new SqlParser.TableOrViewName(), table);
            var tr = tn.TableReference;
            tr.SubstituteDefaults(context.SchemaManager, context.MyDBDataset.Name);

            var destination = new Jhu.Graywulf.IO.Tasks.DestinationTable()
            {
                Dataset = context.MyDBDataset,  // TODO
                DatabaseName = context.MyDBDataset.DatabaseName,
                SchemaName = tr.SchemaName,
                TableNamePattern = tr.DatabaseObjectName,
            };

            return ff.CreateParameters(context.Federation, uri, destination);
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
