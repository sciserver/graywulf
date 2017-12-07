using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;
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

        bool ThrowException
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
        private bool throwException;

        public int Period
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return period; }

            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            set { period = value; }
        }

        public bool ThrowException
        {
            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            get { return throwException; }

            [OperationBehavior(Impersonation = ServiceHelper.DefaultImpersonation)]
            set { throwException = value; }
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
            this.throwException = false;
        }

        protected override void OnExecute()
        {
            var start = DateTime.Now;

            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Sleeping..."));

            if (throwException)
            {
                Thread.Sleep(1000);
                throw new Exception("Exception thrown from cancelable delay.");
            }
            else
            {
                while (!IsCanceled && (DateTime.Now - start).TotalMilliseconds < period)
                {
                    Thread.Sleep(1000);
                }
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
