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

            // Command events are bubbled up the control heirarchy
            RaiseBubbleEvent(this, e);
        }

        protected string GetIconUrl(object item)
        {
            if (item is DatasetBase)
            {
                return GetIconUrl((DatasetBase)item);
            }
            else if (item is DatabaseObject)
            {
                return GetIconUrl((DatabaseObject)item);
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        protected string GetIconUrl(DatasetBase dataset)
        {
            return String.Format("~/Assets/Schema/Icons/{0}", dataset.Metadata.Icon);
        }

        protected string GetIconUrl(DatabaseObject dbobj)
        {
            //return String.Format("~/Assets/Schema/Icons/{0}", dataset.Metadata.Icon);
            return null;
        }

        protected string GetDetailsPageUrl(DatasetBase dataset)
        {
            return String.Format("~/Assets/Schema/Pages/{0}.html", dataset.Name);
        }
    }
}