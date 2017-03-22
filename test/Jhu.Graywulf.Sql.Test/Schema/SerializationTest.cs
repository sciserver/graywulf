using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Schema
{
    [TestClass]
    public class SerializationTest
    {

        private bool TestType(Type type)
        {
            var sc = new Jhu.Graywulf.Activities.SerializableChecker();
            return sc.Execute(type);
        }

        [TestMethod]
        public void DatasetSerializableTest()
        {
            Assert.IsTrue(TestType(typeof(MySql.MySqlDataset)));
            Assert.IsTrue(TestType(typeof(PostgreSql.PostgreSqlDataset)));
            Assert.IsTrue(TestType(typeof(SqlServer.SqlServerDataset)));
        }

        [TestMethod]
        public void ColumnSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(Column)));
        }

        [TestMethod]
        public void DatabaseObjectSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(DatabaseObject)));
        }

        [TestMethod]
        public void DatabaseObjectMetadataSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(DatabaseObjectMetadata)));
        }

        [TestMethod]
        public void DatasetStatisticsSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(DatasetStatistics)));
        }

        [TestMethod]
        public void DataTypeSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(DataType)));
        }

        [TestMethod]
        public void IndexSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(Index)));
        }

        [TestMethod]
        public void IndexColumnSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(IndexColumn)));
        }

        [TestMethod]
        public void ParameterSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(Parameter)));
        }

        [TestMethod]
        public void ScalarFunctionSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(ScalarFunction)));
        }

        [TestMethod]
        public void StoredProcedureSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(StoredProcedure)));
        }

        [TestMethod]
        public void TableSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(Table)));
        }

        [TestMethod]
        public void TableStatisticsSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(TableStatistics)));
        }

        [TestMethod]
        public void TableValuedFunctionSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(TableValuedFunction)));
        }

        [TestMethod]
        public void VariableMetadataSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(VariableMetadata)));
        }

        [TestMethod]
        public void ViewSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(View)));
        }

        [TestMethod]
        public void QuantitySerializationTest()
        {
            Assert.IsTrue(TestType(typeof(Quantity)));
        }

        [TestMethod]
        public void UnitSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(Unit)));
        }

        [TestMethod]
        public void UnitPartSerializationTest()
        {
            Assert.IsTrue(TestType(typeof(UnitEntity)));
        }
    }
}
