using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.Sql.Schema.SqlServer;
using Jhu.Graywulf.Sql.CodeGeneration.SqlServer;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class SqlNameResolverTestBase
    {
        private SchemaManager schemaManager;
        private SqlNameResolver nameResolver;

        protected SchemaManager SchemaManager
        {
            get
            {
                if (schemaManager == null)
                {
                    CreateSchemaManager();
                }

                return schemaManager;
            }
        }

        protected SqlNameResolver NameResolver
        {
            get
            {
                if (nameResolver == null)
                {
                    CreateNameResolver();
                }

                return nameResolver;
            }
        }

        protected SchemaManager CreateSchemaManager()
        {
            schemaManager = new SqlServerSchemaManager();

            var ds = new SqlServerDataset(Jhu.Graywulf.Test.Constants.TestDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);
            schemaManager.Datasets[ds.Name] = ds;

            var mydb = new SqlServerDataset(Jhu.Graywulf.Test.Constants.MyDBDatasetName, Jhu.Graywulf.Test.AppSettings.SqlServerSchemaTestConnectionString);
            mydb.IsMutable = true;
            schemaManager.Datasets[mydb.Name] = mydb;

            return schemaManager;
        }

        protected SqlNameResolver CreateNameResolver()
        {
            nameResolver = new SqlNameResolver();
            nameResolver.SchemaManager = CreateSchemaManager();
            nameResolver.DefaultTableDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nameResolver.DefaultFunctionDatasetName = Jhu.Graywulf.Test.Constants.TestDatasetName;
            nameResolver.DefaultOutputDatasetName = Jhu.Graywulf.Test.Constants.MyDBDatasetName;

            return nameResolver;
        }

        protected QueryDetails ResolveNames(StatementBlock script)
        {
            CreateNameResolver();
            return nameResolver.Execute(script);
        }

        protected QueryDetails Parse(string query)
        {
            var p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            return ResolveNames(script);
        }

        protected T Parse<T>(string query)
            where T : Jhu.Graywulf.Parsing.Node
        {
            var p = new SqlParser();
            var script = p.Execute<StatementBlock>(query);

            ResolveNames(script);

            return script.FindDescendantRecursive<T>();
        }

        protected string GenerateCode(Jhu.Graywulf.Parsing.Node node)
        {
            var cg = new SqlServerCodeGenerator();

            cg.TableNameRendering = CodeGeneration.NameRendering.FullyQualified;
            cg.ColumnNameRendering = CodeGeneration.NameRendering.FullyQualified;
            cg.ColumnAliasRendering = CodeGeneration.AliasRendering.Always;
            cg.DataTypeNameRendering = CodeGeneration.NameRendering.FullyQualified;
            cg.FunctionNameRendering = CodeGeneration.NameRendering.FullyQualified;

            var sw = new StringWriter();
            cg.Execute(sw, node);

            return sw.ToString();
        }
    }
}
