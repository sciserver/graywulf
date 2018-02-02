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
        [OperationContract(Name = "ExecuteAsyncEx")]
        Task ExecuteAsync(int period, bool throwException);
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
            get { return period; }
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

        public Task ExecuteAsync(int period, bool throwException)
        {
            this.period = period;
            this.throwException = throwException;

            return base.ExecuteAsync();
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

        protected override void OnCancelling()
        {
            Logging.LoggingContext.Current.LogDebug(Logging.EventSource.Test, String.Format("Cancelled."));

            base.OnCancelling();
        }
    }
}
