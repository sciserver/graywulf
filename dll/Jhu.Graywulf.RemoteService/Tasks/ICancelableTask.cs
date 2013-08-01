using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ICancelableTask
    {

        bool IsCanceled
        {
            [OperationContract]
            get;
        }

        [OperationContract]
        void Cancel();
    }
}
