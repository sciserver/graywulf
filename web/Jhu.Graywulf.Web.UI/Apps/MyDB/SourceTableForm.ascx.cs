using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class SourceTableForm : FederationUserControlBase
    {
        public string DatasetName
        {
            get { return datasetList.DatasetName; }
        }

        public DatasetBase Dataset
        {
            get { return datasetList.Dataset; }
        }

        public string TableID
        {
            get { return tableList.TableID; }
        }

        public Jhu.Graywulf.Schema.Table Table
        {
            get { return tableList.Table; }
        }
    }
}