﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Sql.Parsing
{
    [TestClass]
    public class UdtMemberListTest
    {
        [TestMethod]
        public void SingleMethodCallTest()
        {
            var sql = ".method(a)";
            var exp = new SqlParser().Execute<UdtMemberList>(sql);

            sql = ". method ( a )";
            exp = new SqlParser().Execute<UdtMemberList>(sql);
        }

        [TestMethod]
        public void SinglePropertyAccessTest()
        {
            var sql = ".property";
            var exp = new SqlParser().Execute<UdtMemberList>(sql);

            sql = ". property";
            exp = new SqlParser().Execute<UdtMemberList>(sql);
        }

        [TestMethod]
        public void MultipleMemberList()
        {
            var sql = ".property.method().method2().property2.property3";
            var exp = new SqlParser().Execute<UdtMemberList>(sql);

            sql = ". property . method () . method2 () . property2 . property3";
            exp = new SqlParser().Execute<UdtMemberList>(sql);
        }
    }
}
