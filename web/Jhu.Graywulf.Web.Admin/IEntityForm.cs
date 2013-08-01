using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin
{
    /// <summary>
    /// Summary description for IEntityDetails
    /// </summary>
    public interface IEntityForm
    {
        Entity Item { get; }

        void OnButtonCommand(object sender, CommandEventArgs e);
    }
}