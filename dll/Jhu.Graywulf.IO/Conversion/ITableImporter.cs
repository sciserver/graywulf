using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [NetDataContract]
    public interface ITableImporter : IRemoteService
    {
        DestinationTableParameters Destination
        {
            [OperationContract]
            get; 
            
            [OperationContract]
            set; 
        }

        long RowsAffected 
        { 
            [OperationContract]
            get; 
        }
    }
}
