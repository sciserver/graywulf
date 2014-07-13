using Jhu.Graywulf.Components;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Components
{
    [TestClass]
    public class ObjectPoolTest
    {
        string CreateObjectHelper()
        {
            return "test object";
        }

        SqlCommand CreateDisposableObjectHelper()
        {
            return new SqlCommand();
        }

        [TestMethod]
        public void CreateAndReturnTest()
        {
            using (var pool = new ObjectPool<string>(CreateObjectHelper))
            {
                using (var item = pool.Take())
                {
                }
            }
        }

        [TestMethod]
        public void CreateAndReturnDisposableTest()
        {
            using (var pool = new ObjectPool<SqlCommand>(CreateDisposableObjectHelper))
            {
                using (var item = pool.Take())
                {
                }
            }
        }

        [TestMethod]
        public void CreateALotTest()
        {
            using (var pool = new ObjectPool<string>(CreateObjectHelper))
            {
                var alot = new ObjectPoolItem<string>[50];

                for (int i = 0; i < alot.Length; i++)
                {
                    alot[i] = pool.Take();
                }
            }
        }

        [TestMethod]
        public void CreateALotDisposable()
        {
            using (var pool = new ObjectPool<SqlCommand>(CreateDisposableObjectHelper))
            {
                var alot = new ObjectPoolItem<SqlCommand>[50];

                for (int i = 0; i < alot.Length; i++)
                {
                    alot[i] = pool.Take();
                }
            }
        }
    }
}
