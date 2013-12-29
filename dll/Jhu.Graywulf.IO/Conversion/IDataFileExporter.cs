using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.IO
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    [RemoteServiceClass(typeof(DataFileExporter))]
    public interface IDataFileExporter : IRemoteService
    {
        SourceQueryParameters Source
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }

        DataFileBase Destination
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }
    }
}
