using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Jhu.Graywulf.Keystone
{
    [TestClass]
    public class ProjectTest : KeystoneTestBase
    {
        [TestMethod]
        public void ManipulateProjectTest()
        {
            PurgeTestEntities();

            // Create a project within the default domain
            var project = CreateTestProject();

            // Get the project by id
            project = Client.GetProjectByID(project.ID);

            // Rename it to something else
            project.Name = "test_project2";
            project = Client.UpdateProject(project);

            // Disable it
            project.Enabled = false;
            project = Client.UpdateProject(project);
            Assert.IsFalse(project.Enabled.Value);

            // Enable it again
            project.Enabled = true;
            project = Client.UpdateProject(project);
            Assert.IsTrue(project.Enabled.Value);

            // Delete project
            Client.DeleteProject(project.ID);

            PurgeTestEntities();
        }
    }
}
