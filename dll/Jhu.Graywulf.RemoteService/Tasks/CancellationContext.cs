using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Tasks
{
    public class CancellationContext : IDisposable
    {
        #region Private member variables

        private bool isValid;

        /// <summary>
        /// Server cancellation tokens that can be requested to cause cancellation
        /// by calling the Cancel method.
        /// </summary>
        [NonSerialized]
        private CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// When called with a cancellation token via ExecuteAsync, it registers
        /// a cancellation handler that's fired when cancellation from the outside
        /// is requested.
        /// </summary>
        [NonSerialized]
        private List<CancellationTokenRegistration> cancellationTokenRegistrations;

        /// <summary>
        /// Keeps a list of ICancelableTask objects that don't accept cancellation tokens
        /// (such as proxies to remote tasks) but can be cancelled by calling the
        /// Cancel function
        /// </summary>
        [NonSerialized]
        private List<ICancelableTask> cancellableTasks;

        #endregion
        #region Properties

        public CancellationToken Token
        {
            get { return cancellationTokenSource.Token; }
        }

        public bool IsValid
        {
            get { return isValid; }
        }

        public bool IsRequested
        {
            get { return cancellationTokenSource.IsCancellationRequested; }
        }

        #endregion
        #region Constructors and initializers

        public CancellationContext()
        {
            InitializeMembers();
        }

        private void InitializeMembers()
        {
            this.isValid = true;
            this.cancellationTokenSource = new CancellationTokenSource();
            this.cancellationTokenRegistrations = null;
            this.cancellableTasks = null;
        }

        private void CopyMembers(CancellationContext old)
        {
            // TODO: review
            this.isValid = old.isValid;
            this.cancellationTokenSource = old.cancellationTokenSource;
            this.cancellationTokenRegistrations = old.cancellationTokenRegistrations;
            this.cancellableTasks = old.cancellableTasks;
        }

        public void Dispose()
        {
            isValid = false;

            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null;

            if (this.cancellationTokenRegistrations != null)
            {
                foreach (var registration in cancellationTokenRegistrations)
                {
                    registration.Dispose();
                }
            }
        }

        #endregion

        public void Register(ICancelableTask task)
        {
            if (cancellableTasks == null)
            {
                cancellableTasks = new List<ICancelableTask>();
            }

            cancellableTasks.Add(task);
        }

        public void Register<T>(TaskCompletionSource<T> tcs)
        {
            cancellationTokenSource.Token.Register(tcs.SetCanceled);
        }

        // TODO: review all callers
        /// <summary>
        /// When a cancellation token is received from the outside, this function
        /// registers the entire task for cancellation.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public void Register(CancellationToken cancellationToken)
        {
            if (cancellationToken.CanBeCanceled && !cancellationToken.IsCancellationRequested)
            {
                if (cancellationTokenRegistrations == null)
                {
                    cancellationTokenRegistrations = new List<CancellationTokenRegistration>();
                }

                cancellationTokenRegistrations.Add(cancellationToken.Register(Cancel));
            }
        }

        public void Cancel()
        {
            Util.TaskHelper.Wait(CancelAsync());
        }

        public async Task CancelAsync()
        {
            // The cancellation context can be disposed by ExecuteAsync while
            // this is running here if the task completes too quickly

            if (!cancellationTokenSource.IsCancellationRequested)
            {
                // This causes the disposal of the class instantly as
                // the executing task finishes with a cancelled exception
                cancellationTokenSource.Cancel();

                if (cancellableTasks != null)
                {
                    foreach (var task in cancellableTasks)
                    {
                        if (!await task.IsCancellationRequestedAsync())
                        {
                            await task.CancelAsync();
                        }
                    }
                }

            }
        }
    }
}
