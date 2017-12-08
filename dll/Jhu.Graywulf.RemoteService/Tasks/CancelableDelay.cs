using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using Jhu.Graywulf.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;
using System.Threading.Tasks;

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
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
        ConcurrencyMode = ConcurrencyMode.Multiple)]
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

        public CancelableDelay(CancellationContext cancellationContext)
            : base(cancellationContext)
        {
            InitializeMembers();
        }

        public CancelableDelay(CancellationContext cancellationContext, int period)
            : base(cancellationContext)
        {
            InitializeMembers();

            this.period = period;
        }

        private void InitializeMembers()
        {
            this.period = 1000;
            this.throwException = false;
        }

        protected override async Task OnExecuteAsync()
        {
            var start = DateTime.Now;

            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Sleeping..."));

            await Task.Delay(period, CancellationContext.Token);

            if (throwException)
            {
                throw new Exception("Exception thrown from cancelable delay.");
            }

            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Finished."));
        }

        protected override void OnCancel()
        {
            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Cancelled."));
        }
    }
}
