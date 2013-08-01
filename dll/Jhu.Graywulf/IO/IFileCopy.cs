using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(FileCopy))]
    public interface IFileCopy : IRemoteService
    {
        string Source
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }

        string Destination
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }

        bool Overwrite
        {
            [OperationContract]
            get;

            [OperationContract]
            set;
        }
    }
}
