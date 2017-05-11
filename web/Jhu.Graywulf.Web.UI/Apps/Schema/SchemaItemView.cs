using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Jhu.Graywulf.Web.UI.Apps.Schema
{
    public abstract class SchemaItemView<T> : SchemaView
    {
        private T item;

        public T Item
        {
            get { return item; }
            set
            {
                item = value;
                UpdateView();
            }
        }
    }
}