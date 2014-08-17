using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jhu.Graywulf.Web.Controls
{
    public class Tab : ToolbarButton
    {
        //[Themeable(false), DefaultValue(false)]
        //public bool Selected
        //{
        //    get { return (bool)(ViewState["Selected"] ?? false); }
        //    set { ViewState["Selected"] = value; }
        //}

#if false
        private static readonly object EventClick;

        static Tab()
        {
            EventClick = new object();
        }

        #region Properties

        [Bindable(true), DefaultValue(""), Localizable(true)]
        public string Text
        {
            get { return (string)ViewState["Text"] ?? String.Empty; }
            set { ViewState["Text"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassDisabled
        {
            get { return (string)(ViewState["CssClassDisabled"] ?? String.Empty); }
            set { ViewState["CssClassDisabled"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassHover
        {
            get { return (string)(ViewState["CssClassHover"] ?? String.Empty); }
            set { ViewState["CssClassHover"] = value; }
        }

        [DefaultValue(""), Bindable(true), Themeable(true)]
        public string CssClassSelected
        {
            get { return (string)(ViewState["CssClassSelected"] ?? String.Empty); }
            set { ViewState["CssClassSelected"] = value; }
        }

        #endregion
        #region Events

        public event EventHandler Click
        {
            add { base.Events.AddHandler(EventClick, value); }
            remove { base.Events.RemoveHandler(EventClick, value); }
        }

        #endregion
        #region Contructors and initializers

        public Tab()
        {
            
        }

        #endregion

        protected override void Render(HtmlTextWriter writer)
        {
            string css, csshover;
            if (Enabled)
            {
                if (Selected)
                {
                    css = !String.IsNullOrEmpty(CssClassSelected) ? CssClassSelected : ((TabHeader)Parent).CssClassSelected;
                }
                else
                {
                    css = !String.IsNullOrEmpty(CssClass) ? CssClass : ((TabHeader)Parent).CssClass;
                }
            }
            else
            {
                css = !String.IsNullOrEmpty(CssClassDisabled) ? CssClassDisabled : ((TabHeader)Parent).CssClassDisabled;
            }
            csshover = !String.IsNullOrEmpty(CssClassHover) ? CssClassHover : ((TabHeader)Parent).CssClassHover;

            // render style attribute
            ControlStyle.AddAttributesToRender(writer);

            writer.AddAttribute("id", ClientID);
            writer.AddAttribute("class", css);

            if (!Selected && Enabled)
            {
                writer.AddAttribute("onClick", Page.ClientScript.GetPostBackEventReference(this, null));
            }

            if (!Selected && Enabled && !String.IsNullOrEmpty(csshover))
            {
                writer.AddAttribute("onMouseOver", String.Format("Graywulf_Button_Over('{0}', '{1}');", ClientID, csshover));
                writer.AddAttribute("onMouseOut", String.Format("Graywulf_Button_Out('{0}', '{1}');", ClientID, css));
            }

            writer.RenderBeginTag("td");

            writer.WriteEncodedText(Text);

            writer.RenderEndTag();
        }

        protected virtual void OnClick(EventArgs e)
        {
            var handler = (EventHandler)base.Events[EventClick];
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            OnClick(new EventArgs());
        }
#endif
    }
}