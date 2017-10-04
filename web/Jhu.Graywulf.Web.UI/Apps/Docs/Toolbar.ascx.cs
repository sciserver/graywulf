using System;
using System.Text.RegularExpressions;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.Hosting;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using Jhu.Graywulf.Web.Controls;

namespace Jhu.Graywulf.Web.UI.Apps.Docs
{
    public partial class Toolbar : System.Web.UI.UserControl, IScriptControl
    {
        class SiteMapVisitor : DocsTreeVisitor
        {
            private Stack<DropDownList> lists;
            private PlaceHolder placeHolder;

            public SiteMapVisitor(PlaceHolder placeHolder)
            {
                this.lists = new Stack<DropDownList>();
                this.placeHolder = placeHolder;
                this.ExpandLevel = 0;
            }

            protected override void OnVisitingDir(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
            {
                base.OnVisitingDir(virtualPathProvider, args);

                if (!args.Skip)
                {
                    CreateListItem(args);
                }

                if (args.Expand)
                {
                    var list = new DropDownList();

                    lists.Push(list);

                    list.Items.Add(new ListItem()
                    {
                        Text = "(select page)",
                        Value = ""
                    });
                }
            }

            protected override void OnVisitedDir(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
            {
                base.OnVisitedDir(virtualPathProvider, args);

                if (args.Expand)
                {
                    var list = lists.Pop();

                    if (list.Items.Count > 1 || placeHolder.Controls.Count > 0)
                    {
                        var panel = new Panel();
                        var label = new Label();

                        panel.Style.Add("min-width", "140px");
                        label.Text = "<br />";
                        panel.Controls.Add(label);
                        panel.Controls.Add(list);
                        placeHolder.Controls.AddAt(0, panel);
                    }
                }
            }

            protected override void OnVisitingFile(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
            {
                base.OnVisitingFile(virtualPathProvider, args);

                if (!args.Skip)
                {
                    CreateListItem(args);
                }
            }

            private void CreateListItem(DocsTreeVisitorEventArgs args)
            {
                var list = lists.Peek();
                var li = new ListItem()
                {
                    Text = args.Title,
                    Value = GetRelativePath(args),
                    Selected = args.IsSelected
                };
                list.Items.Add(li);
            }
        }

        class NavigationMapVisitor : DocsTreeVisitor
        {
            private string prevPage;
            private string selectedPage;
            private string nextPage;
            private string upPage;

            public string PrevPage
            {
                get { return prevPage; }
                set { prevPage = value; }
            }

            public string NextPage
            {
                get { return nextPage; }
                set { nextPage = value; }
            }

            public string UpPage
            {
                get { return upPage; }
                set { upPage = value; }
            }

            public NavigationMapVisitor()
            {
                this.ExpandLevel = 100;
                this.prevPage = null;
                this.selectedPage = null;
                this.nextPage = null;
                this.upPage = null;
            }

            protected override void OnVisitingDir(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
            {
                base.OnVisitingDir(virtualPathProvider, args);

                if (!args.Skip)
                {
                    UpdatePrevNext(args);
                }

                if (args.Expand && args.IsSelected && !args.IsCurrent)
                {
                    upPage = args.Path;
                }
            }

            protected override void OnVisitingFile(VirtualPathProvider virtualPathProvider, DocsTreeVisitorEventArgs args)
            {
                base.OnVisitingFile(virtualPathProvider, args);

                if (!args.Skip)
                {
                    UpdatePrevNext(args);
                }
            }

            protected override void OnComplete()
            {
                base.OnComplete();

                if (selectedPage == null)
                {
                    prevPage = null;
                }
            }

            private void UpdatePrevNext(DocsTreeVisitorEventArgs args)
            {
                if (args.IsCurrent)
                {
                    selectedPage = args.Path;
                }
                else
                {
                    if (selectedPage == null)
                    {
                        prevPage = args.Path;
                    }
                    else if (nextPage == null)
                    {
                        nextPage = args.Path;
                    }
                }
            }
        }

        private string rootPath;

        public string RootPath
        {
            get { return rootPath; }
            set { rootPath = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            var smv = new SiteMapVisitor(buttonPlaceHolder)
            {
                RootPath = rootPath,
                CurrentPath = Page.AppRelativeVirtualPath
            };
            smv.Execute();

            var nmv = new NavigationMapVisitor()
            {
                RootPath = rootPath,
                CurrentPath = Page.AppRelativeVirtualPath
            };
            nmv.Execute();

            prevButton.Enabled = nmv.PrevPage != null;
            prevButton.NavigateUrl = nmv.PrevPage;
            nextButton.Enabled = nmv.NextPage != null;
            nextButton.NavigateUrl = nmv.NextPage;
            upButton.Enabled = nmv.UpPage != null;
            upButton.NavigateUrl = nmv.UpPage;

        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            if (!this.DesignMode)
            {
                var scriptManager = ScriptManager.GetCurrent(this.Page);

                if (scriptManager != null)
                {
                    Scripts.ScriptLibrary.Register(scriptManager, new Scripts.JQuery());
                    scriptManager.RegisterScriptControl(this);
                }
                else
                {
                    throw new InvalidOperationException("You must have a ScriptManager on the Page.");
                }
            }
        }

        public IEnumerable<ScriptDescriptor> GetScriptDescriptors()
        {
            yield break;
        }

        public IEnumerable<ScriptReference> GetScriptReferences()
        {
            yield return new ScriptReference("~/Apps/Docs/Toolbar.js");
        }
    }
}