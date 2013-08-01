using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ServiceModel;
using Jhu.Graywulf.RemoteService;
using Jhu.Graywulf.Tasks;

namespace Jhu.Graywulf.Jobs.Test
{
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

            while (!IsCanceled && (DateTime.Now - start).TotalMilliseconds < period)
            {
                Thread.Sleep(1000);
            }
        }
    }
}
