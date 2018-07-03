using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class CursorTest
    {
        [TestMethod]
        public void DeclareCursorTest()
        {
            var sql = "DECLARE test_cursor CURSOR FOR SELECT 1";
            var exp = new SqlParser().Execute<DeclareCursorStatement>(sql);
            Assert.AreEqual("test_cursor", exp.CursorReference.CursorName);

            sql = "DECLARE[test_cursor]CURSOR FOR SELECT 1";
            exp = new SqlParser().Execute<DeclareCursorStatement>(sql);
            Assert.AreEqual("test_cursor", exp.CursorReference.CursorName);
        }

        [TestMethod]
        public void DeclareCursorVaribleTest()
        {
            var sql = "DECLARE @test_cursor CURSOR";
            var exp = new SqlParser().Execute<DeclareCursorStatement>(sql);
            Assert.AreEqual("@test_cursor", exp.CursorReference.VariableName);

            sql = "DECLARE@test_cursor CURSOR";
            exp = new SqlParser().Execute<DeclareCursorStatement>(sql);
            Assert.AreEqual("@test_cursor", exp.CursorReference.VariableName);
        }

        [TestMethod]
        public void CursorOperationTest()
        {
            var sql = "OPEN test_cursor";
            var exp = new SqlParser().Execute<CursorOperationStatement>(sql);
            Assert.AreEqual("test_cursor", exp.CursorReference.CursorName);

            sql = "OPEN @test_cursor";
            exp = new SqlParser().Execute<CursorOperationStatement>(sql);
            Assert.AreEqual("@test_cursor", exp.CursorReference.VariableName);

            sql = "OPEN[test_cursor]";
            exp = new SqlParser().Execute<CursorOperationStatement>(sql);

            sql = "CLOSE test_cursor";
            exp = new SqlParser().Execute<CursorOperationStatement>(sql);
            Assert.AreEqual("test_cursor", exp.CursorReference.CursorName);

            sql = "DEALLOCATE test_cursor";
            exp = new SqlParser().Execute<CursorOperationStatement>(sql);
            Assert.AreEqual("test_cursor", exp.CursorReference.CursorName);
        }

        [TestMethod]
        public void SetCursorTest()
        {
            var sql = "SET @test_cursor = CURSOR FOR SELECT 1";
            var exp = new SqlParser().Execute<SetCursorStatement>(sql);
            Assert.AreEqual("@test_cursor", exp.CursorReference.VariableName);

            sql = "SET@test_cursor=CURSOR FOR SELECT 1";
            exp = new SqlParser().Execute<SetCursorStatement>(sql);
            Assert.AreEqual("@test_cursor", exp.CursorReference.VariableName);
        }

        [TestMethod]
        public void FetchTest()
        {
            var sql = "FETCH NEXT FROM test_cursor";
            var exp = new SqlParser().Execute<FetchStatement>(sql);
            Assert.AreEqual("test_cursor", exp.CursorReference.CursorName);

            sql = "FETCH NEXT FROM @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);
            Assert.AreEqual("@test_cursor", exp.CursorReference.VariableName);

            sql = "FETCH NEXT FROM@test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH@test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH @test_cursor INTO @test_var";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH PRIOR FROM @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH FIRST FROM @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH LAST FROM @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH ABSOLUTE 5 FROM @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH ABSOLUTE @num FROM @test_cursor";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM test_cursor INTO @test";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM test_cursor INTO @test1,@test2";
            exp = new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM test_cursor INTO @test1 , @test2";
            exp = new SqlParser().Execute<FetchStatement>(sql);
        }
    }
}
