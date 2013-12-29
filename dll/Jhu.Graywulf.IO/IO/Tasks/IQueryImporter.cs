using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.IO
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteServiceClass(typeof(QueryImporter))]
    [NetDataContract]
    public interface IQueryImporter : ITableImporter
    {
        SourceQueryParameters Source
        {
            [OperationContract]
            get;

            [OperationContract]
            set; 
        }

        [OperationContract]
        void CreateDestinationTable();
    }
}
