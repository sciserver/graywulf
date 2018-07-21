using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class SqlNameResolverOptions
    {
        private string defaultTableDatasetName;
        private string defaultFunctionDatasetName;
        private string defaultDataTypeDatasetName;
        private string defaultOutputDatasetName;

        /// <summary>
        /// Gets or sets the default dataset name to be assumed when no
        /// dataset is specified
        /// </summary>
        public string DefaultTableDatasetName
        {
            get { return defaultTableDatasetName; }
            set { defaultTableDatasetName = value; }
        }

        public string DefaultFunctionDatasetName
        {
            get { return defaultFunctionDatasetName; }
            set { defaultFunctionDatasetName = value; }
        }

        public string DefaultDataTypeDatasetName
        {
            get { return defaultDataTypeDatasetName; }
            set { defaultDataTypeDatasetName = value; }
        }

        public string DefaultOutputDatasetName
        {
            get { return defaultOutputDatasetName; }
            set { defaultOutputDatasetName = value; }
        }

        public SqlNameResolverOptions()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.defaultTableDatasetName = String.Empty;
            this.defaultFunctionDatasetName = String.Empty;
            this.defaultDataTypeDatasetName = String.Empty;
            this.defaultOutputDatasetName = String.Empty;
        }
    }
}
