using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jhu.Graywulf.Entities.Mapping;
using Jhu.Graywulf.Entities.AccessControl;

namespace Jhu.Graywulf.Entities
{
    [TestClass]
    public class SecurableEntityTest : TestClassBase
    {
        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            InitializeDatabase();
        }

        public SecuredEntity CreateEntity(Context context)
        {
            var e = new SecuredEntity(context)
            {
                Name = "test"
            };

            return e;
        }

        #region Owner operations

        [TestMethod]
        public void CreateTest()
        {
            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.IsTrue(e.ID > 0);
                Assert.AreEqual(context.Principal.Name, e.Permissions.Owner);
            }
        }

        [TestMethod]
        public void LoadTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
                Assert.AreEqual("test", e.Name);
                Assert.AreEqual(context.Principal.Name, e.Permissions.Owner);
            }
        }

        [TestMethod]
        public void ModifyTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                e.Name = "modified";
                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
            }
        }

        [TestMethod]
        public void DeleteTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();
                e.Delete();
            }
        }

        #endregion
        #region Granted access operations

        [TestMethod]
        public void LoadGrantedTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Permissions.Grant(OtherUser, DefaultAccess.Read);
                e.Save();

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                context.Principal = CreateOtherIdentity();

                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                Assert.IsTrue(e.IsLoaded);
                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
                Assert.AreEqual("test", e.Name);
            }
        }

        [TestMethod]
        public void ModifyGratedTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Permissions.Grant(OtherUser, DefaultAccess.Read);
                e.Permissions.Grant(OtherUser, DefaultAccess.Update);
                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                context.Principal = CreateOtherIdentity();

                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                e.Name = "modified";
                e.Save();

                Assert.IsFalse(e.IsDirty);
                Assert.AreEqual(id, e.ID);
            }
        }

        [TestMethod]
        public void DeleteGrantedTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Permissions.Grant(OtherUser, DefaultAccess.Read);
                e.Permissions.Grant(OtherUser, DefaultAccess.Delete);

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                context.Principal = CreateOtherIdentity();

                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();
                e.Delete();
            }
        }

        #endregion
        #region Denied access operations

        [TestMethod]
        public void LoadDeniedest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Save();

                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                context.Principal = CreateOtherIdentity();

                var e = new SecuredEntity(context);
                e.ID = id;

                try
                {
                    e.Load();
                    Assert.Fail();
                }
                catch (SecurityException)
                {
                }
            }
        }

        [TestMethod]
        public void ModifyDeniedTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Permissions.Grant(OtherUser, DefaultAccess.Read);
                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                context.Principal = CreateOtherIdentity();

                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                e.Name = "modified";

                try
                {
                    e.Save();
                    Assert.Fail();
                }
                catch (SecurityException)
                {
                }
            }
        }

        [TestMethod]
        public void DeleteDeniedTest()
        {
            int id;

            using (var context = CreateContext())
            {
                var e = CreateEntity(context);
                e.Permissions.Grant(OtherUser, DefaultAccess.Read);
                e.Save();

                id = e.ID;
            }

            using (var context = CreateContext())
            {
                context.Principal = CreateOtherIdentity();

                var e = new SecuredEntity(context);
                e.ID = id;
                e.Load();

                try
                {
                    e.Delete();
                    Assert.Fail();
                }
                catch (SecurityException)
                {
                }
            }
        }

        #endregion
    }
}
