﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public abstract class DatabaseObjectReference : ReferenceBase
    {
        #region Property storage variables

        private DatabaseObject databaseObject;

        private string datasetName;
        private string databaseName;
        private string schemaName;
        private string databaseObjectName;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or set the database object (schema object) this reference refers to
        /// </summary>
        public DatabaseObject DatabaseObject
        {
            get { return databaseObject; }
            set { databaseObject = value; }
        }

        /// <summary>
        /// Gets or sets the resolved dataset name
        /// </summary>
        public string DatasetName
        {
            get { return datasetName; }
            set { datasetName = value; }
        }

        /// <summary>
        /// Gets or sets the resolved database name
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
            set { databaseName = value; }
        }

        /// <summary>
        /// Gets or sets the resolved schema name
        /// </summary>
        public string SchemaName
        {
            get { return schemaName; }
            set { schemaName = value; }
        }

        /// <summary>
        /// Gets or sets the resolved object name
        /// </summary>
        public string DatabaseObjectName
        {
            get { return databaseObjectName; }
            set { databaseObjectName = value; }
        }

        /// <summary>
        /// Gets a unique name for the database object reference that can
        /// be used as a key in collections.
        /// </summary>
        /// <remarks>
        /// Never use this in query generation!
        /// </remarks>
        public override string UniqueName
        {
            get
            {
                if (databaseObject != null)
                {
                    return String.Format(
                            "[{0}]:[{1}].[{2}].[{3}]",
                            databaseObject.DatasetName,
                            databaseObject.DatabaseName,
                            databaseObject.SchemaName,
                            databaseObject.ObjectName);
                }
                else
                {
                    return String.Format(
                            "[{0}]:[{1}].[{2}].[{3}]",
                            DatasetName,
                            DatabaseName,
                            SchemaName,
                            DatabaseObjectName);
                }
            }
            set { throw new InvalidOperationException(); }
        }

        public virtual bool IsUndefined
        {
            get { return datasetName == null && databaseName == null && schemaName == null && databaseObjectName == null; }
        }

        #endregion
        #region Constructor and initializers

        protected DatabaseObjectReference()
        {
            InitializeMembers();
        }

        protected DatabaseObjectReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }

        protected DatabaseObjectReference(DatabaseObject databaseObject)
        {
            InitializeMembers();

            this.databaseObject = databaseObject;

            this.datasetName = databaseObject.DatasetName;
            this.databaseName = databaseObject.DatabaseName;
            this.schemaName = databaseObject.SchemaName;
            this.databaseObjectName = databaseObject.ObjectName;
            this.IsUserDefined = databaseObject.IsUserDefined;
        }

        protected DatabaseObjectReference(DatabaseObjectReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.databaseObject = null;

            this.datasetName = null;
            this.databaseName = null;
            this.schemaName = null;
            this.databaseObjectName = null;
        }

        private void CopyMembers(DatabaseObjectReference old)
        {
            this.databaseObject = old.databaseObject;

            this.datasetName = old.datasetName;
            this.databaseName = old.databaseName;
            this.schemaName = old.schemaName;
            this.databaseObjectName = old.databaseObjectName;
        }

        #endregion

        public virtual void LoadDatabaseObject(DatasetBase dataset)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// This is for debugging purposes only, never use it in code generators!
        /// </remarks>
        public override string ToString()
        {
            return UniqueName;
        }
    }
}
