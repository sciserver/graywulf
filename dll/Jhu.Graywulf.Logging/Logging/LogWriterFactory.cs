using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Jhu.Graywulf.Logging
{
    public class LogWriterFactory
    {
        public LogWriterFactory()
        {
        }

        public LogWriterBase[] GetLogWriters()
        {
            Configuration config;

            if (System.Web.HttpContext.Current != null)
            {
                config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~");
            }
            else
            {
                config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }

            var res = new List<LogWriterBase>();
            var group = config.GetSectionGroup(Constants.ConfigSectionGroupName);

            // When called from powershell scripts, config might be null
            if (group != null)
            {
                foreach (LogWriterConfigurationBase section in group.Sections)
                {
                    if (section.SectionInformation.IsDeclared && section.IsEnabled)
                    {
                        var writer = section.CreateLogWriter();
                        res.Add(writer);
                    }
                }
            }

            return res.ToArray();
        }
    }
}
