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
            new SqlParser().Execute<DeclareCursorStatement>(sql);

            sql = @"DECLARE @test_cursor CURSOR FOR SELECT 1";
            new SqlParser().Execute<DeclareCursorStatement>(sql);
        }

        [TestMethod]
        public void CursorOperationTest()
        {
            var sql = "OPEN test_cursor";
            new SqlParser().Execute<CursorOperationStatement>(sql);

            sql = "OPEN @test_cursor";
            new SqlParser().Execute<CursorOperationStatement>(sql);

            sql = "CLOSE test_cursor";
            new SqlParser().Execute<CursorOperationStatement>(sql);

            sql = "CLOSE @test_cursor";
            new SqlParser().Execute<CursorOperationStatement>(sql);

            sql = "DEALLOCATE test_cursor";
            new SqlParser().Execute<CursorOperationStatement>(sql);

            sql = "DEALLOCATE @test_cursor";
            new SqlParser().Execute<CursorOperationStatement>(sql);
        }

        [TestMethod]
        public void SetCursorTest()
        {
            var sql = "SET @test_cursor = CURSOR FOR SELECT 1";
            new SqlParser().Execute<SetCursorStatement>(sql);

            sql = "SET @test_cursor=CURSOR FOR SELECT 1";
            new SqlParser().Execute<SetCursorStatement>(sql);
        }

        [TestMethod]
        public void FetchTest()
        {
            var sql = "FETCH NEXT FROM test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM @test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH PRIOR FROM @test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH FIRST FROM @test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH LAST FROM @test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);
            
            sql = "FETCH ABSOLUTE 5 FROM @test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH ABSOLUTE @num FROM @test_cursor";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM test_cursor INTO @test";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM test_cursor INTO @test1,@test2";
            new SqlParser().Execute<FetchStatement>(sql);

            sql = "FETCH NEXT FROM test_cursor INTO @test1 , @test2";
            new SqlParser().Execute<FetchStatement>(sql);
        }
    }
}
