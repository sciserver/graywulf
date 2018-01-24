using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Sql.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class SourceTableForm : FederationUserControlBase
    {
        public event EventHandler SelectionChanged;

        public string DatasetName
        {
            get { return datasetList.DatasetName; }
        }

        public DatasetBase Dataset
        {
            get { return datasetList.Dataset; }
        }

        public Jhu.Graywulf.Sql.Schema.Table Table
        {
            get { return tableList.Table; }
        }

        public Jhu.Graywulf.Sql.Schema.Table LastTable
        {
            get
            {
                var key = (string)ViewState["LastTable"];

                if (key != null)
                {
                    return (Jhu.Graywulf.Sql.Schema.Table)FederationContext.SchemaManager.GetDatabaseObjectByKey(key);
                }
                else
                {
                    return null;
                }
            }
            set
            {
                string key;

                if (value != null)
                {
                    key = value.UniqueKey;
                }
                else
                {
                    key = null;
                }

                ViewState["LastTable"] = key;
            }
        }

        protected void TableList_SelectedTableChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(sender, e);
        }
    }
}