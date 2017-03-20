using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Metadata.CmdLineUtil
{
    [Verb(Name = "Drop", Description = "Deletes metadata from the database.")]
    class Drop : DatabaseVerbBase
    {
        public Drop()
        {
        }

        public override void Run()
        {
            var g = new Generator(GetConnectionString());
            g.DropMetadata();
        }
    }
}
