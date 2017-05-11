using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.Web.UI.WebControls;
using Jhu.Graywulf.Schema;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public abstract class SchemaView : FederationUserControlBase
    {
        private static readonly object eventCommand = new object();

        private EventHandlerList events = new EventHandlerList();

        public event CommandEventHandler Command
        {
            add { events.AddHandler(eventCommand, value); }
            remove { events.RemoveHandler(eventCommand, value); }
        }

        public new Default Page
        {
            get { return (Default)base.Page; }
        }

        public abstract void UpdateView();

        protected void OnCommand(CommandEventArgs e)
        {
            ((CommandEventHandler)events[eventCommand])?.Invoke(this, e);
        }

        protected string GetIconUrl(DatasetBase dataset)
        {
            return String.Format("~/Assets/Datasets/Icons/{0}", dataset.Metadata.Icon);
        }

        protected string GetDetailsPageUrl(DatasetBase dataset)
        {
            return String.Format("~/Assets/Datasets/Pages/{0}.html", dataset.Name);
        }
    }
}