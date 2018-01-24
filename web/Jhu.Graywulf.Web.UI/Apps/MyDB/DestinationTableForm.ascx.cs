using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Sql.Schema;
using Jhu.Graywulf.IO.Tasks;
using Jhu.Graywulf.Sql.Jobs;
using Jhu.Graywulf.Web.Api.V1;

namespace Jhu.Graywulf.Web.UI.Apps.MyDB
{
    public partial class DestinationTableForm : FederationUserControlBase
    {
        public bool AllowEmptyTableName
        {
            get { return (bool)(ViewState["AllowEmptyTableName"] ?? true); }
            set
            {
                ViewState["AllowEmptyTableName"] = value;
                tableNameRequiredValidator.Enabled = !value;
            }
        }

        public string DatasetName
        {
            get { return datasetList.DatasetName; }
        }

        public DatasetBase Dataset
        {
            get { return datasetList.Dataset; }
        }

        public string TableName
        {
            get
            {
                if (this.Visible && !String.IsNullOrWhiteSpace(tableName.Text))
                {
                    return tableName.Text;
                }
                else
                {
                    return null;
                }
            }
        }

        protected void TableNameValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            string sn, tn;
            args.IsValid = Jhu.Graywulf.Web.Api.Util.SqlParser.TryParseTableName(FederationContext, DatasetName, tableName.Text, out sn, out tn);
        }
    }
}