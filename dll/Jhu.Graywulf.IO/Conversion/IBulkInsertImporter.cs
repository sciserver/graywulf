using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(BulkInsertImporter))]
    public interface IBulkInsertImporter : ITableImporter
    {
        BulkInsertParameters Parameters
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }
    }
}
