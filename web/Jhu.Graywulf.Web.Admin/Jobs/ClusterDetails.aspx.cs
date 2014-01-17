﻿using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Web.Admin.Jobs
{
    public partial class ClusterDetails : EntityDetailsPageBase<Registry.Cluster>
    {
        public static string GetUrl(Guid guid)
        {
            return String.Format("~/Jobs/ClusterDetails.aspx?guid={0}", guid);
        }

        protected override void InitLists()
        {
            base.InitLists();

            MachineRoleList.ParentEntity = Item;
            DomainList.ParentEntity = Item;
            QueueDefinitionList.ParentEntity = Item;
        }
    }
}