using System;
using System.Xml;
using System.Runtime.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Jobs.CopyTables;

namespace Jhu.Graywulf.Web.Api.V1
{
    [DataContract]
    [Description("Represents a table copy job.")]
    public class CopyJob : Job
    {
        #region Private member variables

        private SourceTable source;
        private DestinationTable destination;
        private bool move;

        #endregion
        #region Properties

        [DataMember(Name = "source", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Source table")]
        public SourceTable Source
        {
            get { return source; }
            set { source = value; }
        }

        [DataMember(Name = "destination", EmitDefaultValue = false)]
        [DefaultValue(null)]
        [Description("Destination table")]
        public DestinationTable Destination
        {
            get { return destination; }
            set { destination = value; }
        }

        [DataMember(Name = "move", EmitDefaultValue = false)]
        [DefaultValue(false)]
        [Description("Move table (drop original).")]
        public bool Move
        {
            get { return move; }
            set { move = value; }
        }
        
        #endregion
        #region Constructors and initializers

        public CopyJob()
        {
            InitializeMembers();
        }

        public static new CopyJob FromJobInstance(JobInstance jobInstance)
        {
            var job = new CopyJob();
            job.LoadFromRegistryObject(jobInstance);

            return job;
        }

        private void InitializeMembers()
        {
            base.Type = JobType.Copy;

            this.source = null;
            this.destination = null;
            this.move = false;
        }

        #endregion

        protected override void LoadFromRegistryObject(JobInstance jobInstance)
        {
            base.LoadFromRegistryObject(jobInstance);

            // Because job parameter type might come from an unknown 
            // assembly, instead of deserializing, read xml directly here

            if (jobInstance.Parameters.ContainsKey(Jhu.Graywulf.Jobs.Constants.JobParameterCopyTables))
            {
                var xml = new XmlDocument();
                xml.LoadXml(jobInstance.Parameters[Jhu.Graywulf.Jobs.Constants.JobParameterCopyTables].XmlValue);

                var xr = new Util.XmlReader(xml);

                source = new SourceTable()
                {
                    Dataset = xr.GetXmlInnerText("CopyTablesParameters/Items/CopyTablesItem/Source/Dataset/Name"),
                    Table = Util.SqlParser.CombineTableName(
                        xr.GetXmlInnerText("CopyTablesParameters/Items/CopyTablesItem/Dataset/Source/SourceSchemaName"),
                        xr.GetXmlInnerText("CopyTablesParameters/Items/CopyTablesItem/Dataset/Source/SourceObjectName")),
                };

                destination = new DestinationTable()
                {
                    Dataset = xr.GetXmlInnerText("CopyTablesParameters/Items/CopyTablesItem/Destination/Dataset/Name"),
                    Table = Util.SqlParser.CombineTableName(
                        xr.GetXmlInnerText("CopyTablesParameters/Items/CopyTablesItem/Dataset/Destination/SchemaName"),
                        xr.GetXmlInnerText("CopyTablesParameters/Items/CopyTablesItem/Dataset/Destination/TableNamePattern")),
                };

                move = !xr.GetXmlBoolean("CopyTablesParameters/Items/CopyTablesItem/dropSourceTable");
            }
        }

        public CopyTablesParameters CreateParameters(FederationContext context)
        {
            if (destination == null ||
                String.IsNullOrWhiteSpace(destination.Table) ||
                String.IsNullOrWhiteSpace(destination.Dataset))
            {
                throw new ArgumentException("Destination must be specified"); // TODO ***
            }

            // Source
            if (source == null)
            {
                throw new InvalidOperationException("Source must be specified"); // TODO ***
            }

            var sourcequery = source.GetSourceTableQuery(context);

            // Destination
            if (destination == null)
            {
                throw new InvalidOperationException("Destination must be specified"); // TODO ***
            }

            var destinationtable = destination.GetDestinationTable(context);

            
            var ff = CopyTablesJobFactory.Create(context.RegistryContext);
            var par = ff.CreateParameters();

            par.Items = new CopyTablesItem[]
            {
                new CopyTablesItem()
                {
                    Source = sourcequery,
                    Destination = destinationtable,
                    DropSourceTable = move
                },
            };

            return par;
        }

        public override void Schedule(FederationContext context)
        {
            var p = CreateParameters(context);

            var ef = CopyTablesJobFactory.Create(context.RegistryContext);
            var job = ef.ScheduleAsJob(p, GetQueueName(context), TimeSpan.Zero, Comments);

            job.Save();

            LoadFromRegistryObject(job);
        }
    }
}
