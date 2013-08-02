using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class SelectionField : CheckBoxField
    {
        internal const string SelectionCheckBoxID = "SelectionCheckBox";

        public SelectionField()
        {
        }

        protected override DataControlField CreateField()
        {
            return new SelectionField();
        }

        protected override void InitializeDataCell(DataControlFieldCell cell, DataControlRowState rowState)
        {
            base.InitializeDataCell(cell, rowState);

            if ((cell.Controls.Count == 0))
            {
                CheckBox chk = new CheckBox();
                chk.ID = SelectionField.SelectionCheckBoxID;
                cell.Controls.Add(chk);
            }
        }
    }
}