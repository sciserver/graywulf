using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.UI.Controls
{
    public class ExpressionPropertyField : DataControlField
    {
        public string DataField
        {
            get { return (string)ViewState["DataField"] ?? ""; }
            set { base.ViewState["DataField"] = value; }
        }

        public ExpressionPropertyField()
        {
        }

        protected override DataControlField CreateField()
        {
            return new ExpressionPropertyField();
        }

        public override void InitializeCell(DataControlFieldCell cell, DataControlCellType cellType, DataControlRowState rowState, int rowIndex)
        {
            base.InitializeCell(cell, cellType, rowState, rowIndex);

            switch (cellType)
            {
                case DataControlCellType.DataCell:
                    InitializeDataCell(cell, rowState);
                    break;
                case DataControlCellType.Header:
                    break;
                case DataControlCellType.Footer:
                    break;
            }
        }

        protected void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            var label = new Label();
            label.DataBinding += new EventHandler(Label_DataBinding);
            cell.Controls.Add(label);
        }

        void Label_DataBinding(object sender, EventArgs e)
        {
            var label = (Label)sender;
            var item = DataBinder.GetDataItem(label.NamingContainer);

            var value = (ExpressionProperty)DataBinder.GetPropertyValue(item, this.DataField);
            
            label.Text = value.ResolvedValue;
        }

    }
}
