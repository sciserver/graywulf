using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Tasks
{
    [ServiceContract(SessionMode = SessionMode.Required)]
    [RemoteService(typeof(CancelableDelay))]
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

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// This class is used for testing purposes.
    /// </remarks>
    [ServiceBehavior(
        InstanceContextMode = InstanceContextMode.PerSession)]
    public class CancelableDelay : RemoteServiceBase, ICancelableDelay
    {
        private int period;

        public int Period
        {
            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            get { return period; }

            [OperationBehavior(Impersonation = RemoteServiceHelper.DefaultImpersonation)]
            set { period = value; }
        }

        public CancelableDelay()
        {
            InitializeMembers();
        }

        public CancelableDelay(int period)
        {
            InitializeMembers();

            this.period = period;
        }

        private void InitializeMembers()
        {
            this.period = 1000;
        }

        protected override void OnExecute()
        {
            var start = DateTime.Now;

            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Sleeping..."));

            while (!IsCanceled && (DateTime.Now - start).TotalMilliseconds < period)
            {
                Thread.Sleep(1000);
            }

            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Finished."));
        }

        public override void Cancel()
        {
            base.Cancel();

            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Cancelled."));
        }
    }
}
