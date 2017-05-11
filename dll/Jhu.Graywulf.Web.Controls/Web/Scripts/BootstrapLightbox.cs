using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace Jhu.Graywulf.Web.Scripts
{
    public class BootstrapLighbox : ScriptLibrary
    {
        public override Script[] Scripts
        {
            get
            {
                return new Script[] {
                    new ScriptMapping()
                    {
                        Name = "bootstrap-lightbox",
                        Mapping = new ScriptResourceDefinition()
                        {
                            Path = "~/Scripts/Bootstrap-Lightbox/ekko-lightbox.min.js",
                            DebugPath = "~/Scripts/Bootstrap-Lightbox/ekko-lightbox.js",
                            CdnPath = "https://cdnjs.cloudflare.com/ajax/libs/ekko-lightbox/5.1.1/ekko-lightbox.min.js",
                            CdnDebugPath = "https://cdnjs.cloudflare.com/ajax/libs/ekko-lightbox/5.1.1/ekko-lightbox.js",
                        },
                    },
                    new ScriptBlock()
                    {
                        Name = "bootstrap-lightbox-init",
                        Code = @"
$(document).on('click', '[data-toggle=""lightbox""]', function(event) {
    event.preventDefault();
    $(this).ekkoLightbox();
        });
"
                    }
                };
            }
        }

        public override string[] StyleSheets
        {
            get
            {
                return new[] {
                    "~/Scripts/Bootstrap-Lightbox/ekko-lightbox.min.css"
                };
            }
        }
    }
}
