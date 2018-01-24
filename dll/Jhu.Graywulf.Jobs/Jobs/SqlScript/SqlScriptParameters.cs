using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.IO;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Format;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Jobs.SqlScript
{
    /// <summary>
    /// Represents a set of parameters necessary to execute a
    /// pass-through SQL script
    /// </summary>
    [Serializable]
    [DataContract(Name = "SqlScriptParameters", Namespace = "")]
    public class SqlScriptParameters
    {
        #region Private member variables

        private DatasetBase[] datasets;
        private string script;
        private int timeout;
        private IsolationLevel isolationLevel;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the array of datasets on which the query
        /// will be executed
        /// </summary>
        [DataMember]
        public DatasetBase[] Datasets
        {
            get { return datasets; }
            set { datasets = value; }
        }

        /// <summary>
        /// Gets or sets the SQL script
        /// </summary>
        [DataMember]
        public string Script
        {
            get { return script; }
            set { script = value; }
        }

        /// <summary>
        /// Gets or sets the execution time-out.
        /// </summary>
        [DataMember]
        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }

        /// <summary>
        /// Gets or sets the transaction isolation level.
        /// </summary>
        [DataMember]
        public IsolationLevel IsolationLevel
        {
            get { return isolationLevel; }
            set { isolationLevel = value; }
        }

        #endregion
        #region Constructors and initializers

        public SqlScriptParameters()
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.datasets = null;
            this.script = null;
            this.timeout = 1200;      // *** TODO: get from settings  
            this.isolationLevel = IsolationLevel.RepeatableRead;
        }

        #endregion

        public string[] GetScriptParts()
        {
            var parts = Util.SqlScriptSplitter.SplitByGo(script);
            return parts;
        }
    }
}
