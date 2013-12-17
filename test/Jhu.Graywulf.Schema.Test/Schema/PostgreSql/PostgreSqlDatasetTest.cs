using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.PostgreSql;

namespace Jhu.Graywulf.Schema.PostgreSql.Test
{

    [TestClass]
    public class PostgreSqlDatasetTest
    {
        private PostgreSqlDataset CreateTarget()
        {

            return new PostgreSqlDataset("test", Jhu.Graywulf.Test.AppSettings.PostgreSqlConnectionString);
        }

        [TestMethod]
        public void TableLoadTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "public", "author"];
            Assert.IsTrue(target.Tables.Count == 1);

            // Get another with missing schema name
            Table t2 = target.Tables["GraywulfSchemaTest", "", "author"];
            Assert.IsTrue(target.Tables.Count == 1);
            Assert.AreEqual(t1, t2);

            // Read from DB with missing schema name
            Table t3 = target.Tables["GraywulfSchemaTest", "", "book"];
            Assert.IsTrue(target.Tables.Count == 2);
            Assert.AreNotEqual(t1, t3);
            Assert.AreEqual(t3.SchemaName, "public");

            // Try to load a non-existent table
            try
            {
                Table t4 = target.Tables["GraywulfSchemaTest", "public", "NonExistentTable"];
                Assert.Fail();
            }
            catch (KeyNotFoundException)
            {
            }

            // Try to load object that's not a table

            // Use wrong database name
        }

        [TestMethod]
        public void TableLoadAllTest()
        {
            PostgreSqlDataset target = CreateTarget();
            target.Tables.LoadAll();
            Assert.IsTrue(target.Tables.Count == 5);
            Assert.IsTrue(target.Tables.IsAllLoaded);
        }
        [TestMethod]
        public void ViewLoadAllTest()
        {
            PostgreSqlDataset target = CreateTarget();
            target.Views.LoadAll();
            Assert.IsTrue(target.Views.Count == 4);
            Assert.IsTrue(target.Views.IsAllLoaded);
        }

        [TestMethod]
        public void TableColumnsTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "public", "author"];

            Assert.IsTrue(t1.Columns.Count == 2);
            Assert.IsTrue(t1.Columns["id"].DataType.Name == "bigint");

            // Test cache
            Assert.AreEqual(t1.Columns, target.Tables["GraywulfSchemaTest", "public", "author"].Columns);
            Assert.AreEqual(t1.Columns["id"], target.Tables["GraywulfSchemaTest", "public", "author"].Columns["id"]);
        }

        [TestMethod]
        public void TableIndexesTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "public", "author"];

            Assert.IsTrue(t1.Indexes.Count == 2);
            Assert.IsTrue(t1.Indexes["pk_author"].IsPrimaryKey);

            // Test cache
            Assert.AreEqual(t1.Indexes, target.Tables["GraywulfSchemaTest", "public", "author"].Indexes);
            Assert.AreEqual(t1.Indexes["pk_author"], target.Tables["GraywulfSchemaTest", "public", "author"].Indexes["pk_author"]);
        }

        [TestMethod]
        public void TableIndexeSColumnsTest()
        {
            PostgreSqlDataset target = CreateTarget();

            // Get a single table
            IndexColumn ic = target.Tables["GraywulfSchemaTest", "public", "author"].Indexes["pk_author"].Columns["id"];
            Assert.IsTrue(ic.Ordering == IndexColumnOrdering.Ascending);

            // Test cache
            Assert.AreEqual(ic, target.Tables["GraywulfSchemaTest", "public", "author"].Indexes["pk_author"].Columns["id"]);
        }


        [TestMethod]
        public void StoredProcedureTest()
        {
            PostgreSqlDataset target = CreateTarget();

            target.StoredProcedures.LoadAll();
            Assert.IsTrue(target.StoredProcedures.Count == 1);

            // TODO: test CLR and SQL functions separately
        }

        [TestMethod]
        public void StoredProcedureParametersTest()
        {
            PostgreSqlDataset target = CreateTarget();

            StoredProcedure sp = target.StoredProcedures["GraywulfSchemaTest", "public", "sptest"];
            Assert.IsTrue(target.StoredProcedures.Count == 1);

            Assert.IsTrue(sp.Parameters.Count == 1);
            Assert.IsTrue(sp.Parameters["hello"].DataType.Name == "varchar");
        }

        [TestMethod]
        public void MetaObjectsTest()
        {
            PostgreSqlDataset target = CreateTarget();

            Table t1 = target.Tables["GraywulfSchemaTest", "", "author"];
            Assert.IsTrue(t1.Metadata.Summary == "this is my own table comment");

            Table t2 = target.Tables["GraywulfSchemaTest", "", "book"];
            Assert.IsTrue(t2.Metadata.Summary == "");
        }

        [TestMethod]
        public void MetaColumnsTest()
        {
            PostgreSqlDataset target = CreateTarget();

            Table t1 = target.Tables["GraywulfSchemaTest", "public", "book"];
            Column c1 = t1.Columns["id"];
            Assert.IsTrue(c1.Metadata.Summary == "id of user");
        }

        [TestMethod]
        public void MetaParametersTest()
        {
            PostgreSqlDataset target = CreateTarget();

            StoredProcedure sp = target.StoredProcedures["GraywulfSchemaTest", "public", "sptest"];
            Parameter p = sp.Parameters["hello"];
            Assert.AreEqual(p.Metadata.Summary, "spTestComment");
            Assert.AreNotEqual(sp.Parameters["hello"], "");
        }

        [TestMethod]
        public void TableColumnsDataTypeTest()
        {
            PostgreSqlDataset target = CreateTarget();

            //Get a single table
            Table t1 = target.Tables["GraywulfSchemaTest", "", "sampledata"];

            Assert.IsTrue(t1.Columns.Count == 49);
            Assert.IsTrue(t1.Columns["column_smallint"].DataType.Name == "smallint");
            Assert.IsTrue(t1.Columns["column_integer"].DataType.Name == "int");
            Assert.IsTrue(t1.Columns["column_bigint"].DataType.Name == "bigint");
            Assert.IsTrue(t1.Columns["column_int"].DataType.Name == "int");
            Assert.IsTrue(t1.Columns["column_decimal"].DataType.Name == "numeric");
            Assert.IsTrue(t1.Columns["column_numeric"].DataType.Name == "numeric");
            Assert.IsTrue(t1.Columns["column_real"].DataType.Name == "real");
            Assert.IsTrue(t1.Columns["column_doubleprecision"].DataType.Name == "real");
            Assert.IsTrue(t1.Columns["column_smallserial"].DataType.Name == "smallint");
            Assert.IsTrue(t1.Columns["column_serial"].DataType.Name == "int");
            Assert.IsTrue(t1.Columns["column_bigserial"].DataType.Name == "bigint");
            Assert.IsTrue(t1.Columns["column_money"].DataType.Name == "money");
            Assert.IsTrue(t1.Columns["column_charactervarying"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_varchar"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_character"].DataType.Name == "char");
            Assert.IsTrue(t1.Columns["column_char"].DataType.Name == "char");
            Assert.IsTrue(t1.Columns["column_text"].DataType.Name == "text");
            Assert.IsTrue(t1.Columns["column_bytea"].DataType.Name == "binary");
            Assert.IsTrue(t1.Columns["column_timestamp"].DataType.Name == "timestamp");
            Assert.IsTrue(t1.Columns["column_timestampwithtimezone"].DataType.Name == "timestamp");
            Assert.IsTrue(t1.Columns["column_date"].DataType.Name == "date");
            Assert.IsTrue(t1.Columns["column_time"].DataType.Name == "datetime");
            //Assert.IsTrue(t1.Columns["column_timewithtimezone"].DataType.Name == "datetime");
            Assert.IsTrue(t1.Columns["column_interval"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_bool"].DataType.Name == "bit");
            Assert.IsTrue(t1.Columns["column_point"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_line"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_lseg"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_box"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_path"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_polygon"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_circle"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_cidr"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_inet"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_macaddr"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_bit"].DataType.Name == "bit");
            Assert.IsTrue(t1.Columns["column_bitvarying"].DataType.Name == "bit");
            Assert.IsTrue(t1.Columns["column_tsvector"].DataType.Name == "varchar");
            Assert.IsTrue(t1.Columns["column_xml"].DataType.Name == "xml");
            Assert.IsTrue(t1.Columns["column_json"].DataType.Name == "text");
            Assert.IsTrue(t1.Columns["column_arrayinteger"].DataType.Name == "unknown");
            Assert.IsTrue(t1.Columns["column_int4range"].DataType.Name == "int");
            Assert.IsTrue(t1.Columns["column_int8range"].DataType.Name == "bigint");
            Assert.IsTrue(t1.Columns["column_numrange"].DataType.Name == "numeric");
            Assert.IsTrue(t1.Columns["column_tsrange"].DataType.Name == "datetime");
            Assert.IsTrue(t1.Columns["column_tstzrange"].DataType.Name == "datetime");
            Assert.IsTrue(t1.Columns["column_daterange"].DataType.Name == "date");
            Assert.IsTrue(t1.Columns["column_oid"].DataType.Name == "varchar");

            //Test cache
            Assert.AreEqual(t1.Columns, target.Tables["GraywulfSchemaTest", "", "sampledata"].Columns);
            Assert.AreEqual(t1.Columns["column_smallint"], target.Tables["GraywulfSchemaTest", "", "sampledata"].Columns["column_smallint"]);
        }
    }
}
