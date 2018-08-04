using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;
using Jhu.Graywulf.Sql.QueryTraversal;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Extensions.QueryTraversal;

namespace Jhu.Graywulf.Sql.Extensions.NameResolution
{
    public class GraywulfSqlNameResolver : SqlNameResolver
    {
        #region Private member variables

        private SchemaManager schemaManager;

        #endregion
        #region Properties

        public new GraywulfSqlNameResolverOptions Options
        {
            get { return (GraywulfSqlNameResolverOptions)base.Options; }
            set { base.Options = value; }
        }

        /// <summary>
        /// Gets or sets the schema manager to be used by the name resolver
        /// </summary>
        public SchemaManager SchemaManager
        {
            get { return schemaManager; }
            set { schemaManager = value; }
        }

        protected new GraywulfSqlQueryVisitor Visitor
        {
            get { return (GraywulfSqlQueryVisitor)base.Visitor; }
        }

        #endregion
        #region Constructors and initializers

        public GraywulfSqlNameResolver()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.schemaManager = null;
        }

        protected override SqlNameResolverOptions CreateOptions()
        {
            return new GraywulfSqlNameResolverOptions();
        }

        protected override SqlQueryVisitor CreateVisitor()
        {
            return new GraywulfSqlQueryVisitor(this)
            {
                Options = new GraywulfSqlQueryVisitorOptions()
                {
                    LogicalExpressionTraversal = ExpressionTraversalMethod.Infix,
                    ExpressionTraversal = ExpressionTraversalMethod.Infix,
                    VisitExpressionSubqueries = true,
                    VisitPredicateSubqueries = true,
                    VisitSchemaReferences = false,
                }
            };
        }

        #endregion
        #region

        public override void Execute(QueryDetails details)
        {
            // Flush schema cache of already loaded mutable data sets
            foreach (var ds in schemaManager.Datasets.Values)
            {
                if (ds.IsMutable)
                {
                    ds.FlushCache();
                }
            }

            base.Execute(details);
        }

        #endregion
        #region Reference resolution

        protected override DatasetBase LoadDataset(DatabaseObjectReference dr)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(dr.DatasetName))
                {
                    return Dataset;
                }
                else
                {
                    return schemaManager.Datasets[dr.DatasetName];
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
            }
            catch (SchemaException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
            }
        }

        #endregion
        #region Default substitution logic

        protected override void OnSubstituteTableDefaults(TableReference tr)
        {
            SubstituteDefaults(tr, SchemaManager, Options.DefaultTableDatasetName);
        }

        protected override void OnSubstituteFunctionDefaults(TableReference tr)
        {

            SubstituteDefaults(tr, SchemaManager, Options.DefaultFunctionDatasetName);
        }

        /// <summary>
        /// Substitutes table default for tables that are created during query execution 
        /// </summary>
        /// <param name="tr"></param>
        protected override void OnSubstituteOutputTableDefaults(TableReference tr)
        {
            SubstituteDefaults(tr, SchemaManager, Options.DefaultOutputDatasetName);
        }
        
        /// <summary>
        /// Substitutes dataset and schema defaults into function references.
        /// </summary>
        protected override void OnSubstituteFunctionDefaults(FunctionReference fr)
        {
            SubstituteDefaults(fr, SchemaManager, Options.DefaultFunctionDatasetName);
        }

        protected override void OnSubstituteDataTypeDefaults(DataTypeReference dr)
        {
            SubstituteDefaults(dr, SchemaManager, Options.DefaultDataTypeDatasetName);
        }

        /// <summary>
        /// Substitute default dataset and schema names, if necessary
        /// </summary>
        /// <param name="defaultDataSetName"></param>
        /// <param name="defaultSchemaName"></param>
        private void SubstituteDefaults(DatabaseObjectReference dr, SchemaManager schemaManager, string defaultDataSetName)
        {
            // NOTE: This cannot be called for subqueries

            try
            {
                if (dr.DatasetName == null)
                {
                    dr.DatasetName = defaultDataSetName;
                }

                if (dr.DatabaseName == null)
                {
                    dr.DatabaseName = schemaManager.Datasets[dr.DatasetName].DatabaseName;
                }

                if (dr.SchemaName == null)
                {
                    dr.SchemaName = schemaManager.Datasets[dr.DatasetName].DefaultSchemaName;
                }
            }
            catch (KeyNotFoundException ex)
            {
                throw NameResolutionError.UnresolvableDatasetReference(ex, dr);
            }
        }

        #endregion
    }
}
