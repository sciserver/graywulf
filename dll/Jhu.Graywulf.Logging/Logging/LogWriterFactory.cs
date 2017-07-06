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
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var group = config.GetSectionGroup(Constants.ConfigSectionGroupName);
            var res = new List<LogWriterBase>(group.Sections.Keys.Count);

            foreach (LogWriterConfigurationBase section in group.Sections)
            {
                if (section.SectionInformation.IsDeclared && section.IsEnabled)
                {
                    var writer = section.CreateLogWriter();
                    res.Add(writer);
                }
            }

            return res.ToArray();
        }
    }
}
