using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Controls
{
    public class BoundEntityField : System.Web.UI.WebControls.BoundField
    {
        private string expression;

        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        public BoundEntityField()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.expression = null;
        }

        protected override object GetValue(Control controlContainer)
        {
            var entity = (Entity)DataBinder.Eval(DataBinder.GetDataItem(controlContainer), DataField);

            if (entity == null)
            {
                return "[null]";
            }
            if (expression == null)
            {
                return "[null]";
            }
            else if (expression == null)
            {
                return entity.Name;
            }
            else
            {
                var ep = new ExpressionProperty(entity, expression);
                return ep.ResolvedValue;
            }
        }
    }
}