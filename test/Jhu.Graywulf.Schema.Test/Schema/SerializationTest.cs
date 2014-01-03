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
        public void SqlServerNativeDataFileSerializableTest()
        {
            Assert.IsTrue(TestType(typeof(Column)));
            Assert.IsTrue(TestType(typeof(DatabaseObject)));
            Assert.IsTrue(TestType(typeof(DatabaseObjectMetadata)));
            Assert.IsTrue(TestType(typeof(DatasetStatistics)));
            Assert.IsTrue(TestType(typeof(DataType)));
            Assert.IsTrue(TestType(typeof(Index)));
            Assert.IsTrue(TestType(typeof(IndexColumn)));
            Assert.IsTrue(TestType(typeof(Parameter)));
            Assert.IsTrue(TestType(typeof(ScalarFunction)));
            Assert.IsTrue(TestType(typeof(StoredProcedure)));
            Assert.IsTrue(TestType(typeof(Table)));
            Assert.IsTrue(TestType(typeof(TableStatistics)));
            Assert.IsTrue(TestType(typeof(TableValuedFunction)));
            Assert.IsTrue(TestType(typeof(VariableMetadata)));
            Assert.IsTrue(TestType(typeof(View)));
        }
    }
}
