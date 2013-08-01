using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Jobs.Test
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(CancelableDelay))]
    public interface ICancelableDelay : IRemoteService
    {
        int Period
        { 
            [OperationContract]
            get; 

            [OperationContract]
            set;
        }
    }
}
