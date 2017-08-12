using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Check
{
    public class FileAccessCheck : CheckRoutineBase
    {
        public override CheckCategory Category
        {
            get
            {
                return CheckCategory.Database;
            }
        }

        public string Path { get; set; }

        public FileMode FileMode { get; set; }
        public FileAccess FileAccess { get; set; }
        public FileShare FileShare { get; set; }

        public FileAccessCheck(string path)
        {
            this.Path = path;
        }

        protected override IEnumerable<CheckRoutineStatus> OnExecute()
        {
            yield return ReportInfo(
                "Testing file access to {0} with modes {1}, {2}, {3}.",
                Path, FileMode, FileAccess, FileShare);

            var f = new FileStream(Path, FileMode, FileAccess, FileShare);
            f.Dispose();

            yield return ReportSuccess("File {0} opened successfully.", Path);
        }
    }
}
