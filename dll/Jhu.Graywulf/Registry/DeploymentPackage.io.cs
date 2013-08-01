using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Registry
{
    public partial class DeploymentPackage : Entity
    {
        #region Database IO Functions

        #endregion
        #region Binary Blob Manipulation Functions

        /// <summary>
        /// Sets the binary blob stored in the deployment package.
        /// </summary>
        /// <param name="data">The blob data.</param>
        public void SetData(byte[] data)
        {
            // TODO to be implemented
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the binary blob stored in the deployment package.
        /// </summary>
        /// <returns>The blob data.</returns>
        public byte[] GetData()
        {
            // TODO to be implemented
            throw new NotImplementedException();
        }

        #endregion
    }
}
