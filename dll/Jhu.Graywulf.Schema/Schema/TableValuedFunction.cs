using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a table-valued function
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class TableValuedFunction : DatabaseObject, IColumns, IParameters, ICloneable
    {
        private bool isColumnsLoaded;
        private ConcurrentDictionary<string, Column> columns;

        private bool isParametersLoaded;
        private ConcurrentDictionary<string, Parameter> parameters;

        /// <summary>
        /// Gets or sets the name of the table-valued function
        /// </summary>
        public string FunctionName
        {
            get { return ObjectName; }
            set { ObjectName = value; }
        }

        /// <summary>
        /// Gets the column collection
        /// </summary>
        public ConcurrentDictionary<string, Column> Columns
        {
            get
            {
                if (!isColumnsLoaded)
                {
                    LoadColumns();
                }

                return columns;
            }
        }

        /// <summary>
        /// Gets the parameter collection
        /// </summary>
        public ConcurrentDictionary<string, Parameter> Parameters
        {
            get
            {
                if (!isParametersLoaded)
                {
                    LoadParameters();
                }

                return parameters;
            }
        }

        #region Constructors and initializers

        /// <summary>
        /// Default constructor
        /// </summary>
        public TableValuedFunction()
            : base()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Creates a table-valued function and initializes its dataset
        /// </summary>
        /// <param name="dataset"></param>
        public TableValuedFunction(DatasetBase dataset)
            : base(dataset)
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public TableValuedFunction(TableValuedFunction old)
            : base(old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variables to their default values
        /// </summary>
        private void InitializeMembers()
        {
            this.ObjectType = DatabaseObjectType.TableValuedFunction;

            this.isColumnsLoaded = false;
            this.columns = null;

            this.isParametersLoaded = false;
            this.parameters = null;
        }

        /// <summary>
        /// Copies member variables
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(TableValuedFunction old)
        {
            this.ObjectType = old.ObjectType;

            this.isColumnsLoaded = false;
            this.columns = null;

            this.isParametersLoaded = false;
            this.parameters = null;
        }

        /// <summary>
        /// Returns a copy of this table
        /// </summary>
        /// <returns></returns>
        public override object Clone()
        {
            return new TableValuedFunction(this);
        }

        #endregion

        /// <summary>
        /// Loads all columns belonging to a table or view
        /// </summary>
        /// <returns></returns>
        private void LoadColumns()
        {
            if (Dataset != null)
            {
                this.columns = new ConcurrentDictionary<string, Column>(Dataset.LoadColumns(this), SchemaManager.Comparer);
            }
            else
            {
                this.columns = new ConcurrentDictionary<string, Column>(SchemaManager.Comparer);
            }

            this.isColumnsLoaded = true;
        }

        /// <summary>
        /// Loads all parameters belonging to a function or stored procedure
        /// </summary>
        /// <returns></returns>
        private void LoadParameters()
        {
            if (Dataset != null)
            {
                this.parameters = new ConcurrentDictionary<string, Parameter>(Dataset.LoadParameters(this), SchemaManager.Comparer);
            }
            else
            {
                this.parameters = new ConcurrentDictionary<string, Parameter>(SchemaManager.Comparer);
            }
            
            this.isParametersLoaded = true;
        }
    }
}
