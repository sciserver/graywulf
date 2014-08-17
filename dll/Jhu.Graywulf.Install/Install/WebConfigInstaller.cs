using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install
{
    public class WebConfigInstaller
    {
        public WebConfigInstaller()
        {

        }

        public void MergeSettings()
        {
            using (var context = ContextManager.Instance.CreateContext(ConnectionMode.AutoOpen, TransactionMode.ManualCommit))
            {
                var xml = context.Domain.Settings["web.config"].XmlValue;
            }
        }
    }
}
