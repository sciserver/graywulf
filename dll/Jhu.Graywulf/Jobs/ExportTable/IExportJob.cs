using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace Jhu.Graywulf.Jobs.ExportTable
{
    public interface IExportJob
    {
        InArgument<ExportTable> Parameters { get; set; }
    }
}
