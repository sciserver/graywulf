using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class JQueryValidation : ScriptLibrary
    {
        public override Script[] Scripts
        {
            get
            {
                return new[] {
                    new Script()
                    {
                        Name = "jquery-validation",
                        Mapping = new ScriptResourceDefinition()
                        {
                            Path = "~/Scripts/jQuery-Validation/jquery.validate.min.js",
                            DebugPath = "~/Scripts/jQuery-Validation/jquery.validate.js",
                            CdnPath = "http://ajax.aspnetcdn.com/ajax/jquery.validate/1.15.1/jquery.validate.min.js",
                            CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jquery.validate/1.15.1/jquery.validate.js"
                        }
                    },
                    new Script()
                    {
                        Name = "jquery-validation-additional",
                        Mapping = new ScriptResourceDefinition()
                        {
                            Path = "~/Scripts/jQuery-Validation/additional-methods.min.js",
                            DebugPath = "~/Scripts/jQuery-Validation/additional-methods.js",
                            CdnPath = "http://ajax.aspnetcdn.com/ajax/jquery.validate/1.15.1/additional-methods.min.js",
                            CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/jquery.validate/1.15.1/additional-methods.js"
                        }
                    },
                    new Script()
                    {
                        Name = "jquery-validation-unobtrusive",
                        Mapping = new ScriptResourceDefinition()
                        {
                            Path = "~/Scripts/jQuery-Validation-Unobtrusive/jquery.validate.unobtrusive.min.js",
                            DebugPath = "~/Scripts/jQuery-Validation-Unobtrusive/jquery.validate.unobtrusive.js",
                            CdnPath = "http://ajax.aspnetcdn.com/ajax/mvc/5.2.3/jquery.validate.unobtrusive.min.js",
                            CdnDebugPath = "http://ajax.aspnetcdn.com/ajax/mvc/5.2.3/jquery.validate.unobtrusive.js"
                        }
                    },
                    new Script()
                    {
                        Reference = new ScriptReference("WebUIValidation.js", "System.Web")
                    }
                };
            }
        }
    }
}
