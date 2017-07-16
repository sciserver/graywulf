﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Configuration;

namespace Jhu.Graywulf.Scheduler
{
    public class SchedulerService : ServiceBase
    {
        public SchedulerService()
        {
            this.ServiceName = "SchedulerService";
        }

        protected override void OnStart(string[] args)
        {
            QueueManager.Instance.Start(Registry.ContextManager.Configuration.ClusterName, false);
        }

        protected override void OnStop()
        {
            QueueManager.Instance.Stop(TimeSpan.FromHours(1.5));
        }

        protected override void OnPause()
        {
            QueueManager.Instance.StopPoller();
        }

        protected override void OnContinue()
        {
            QueueManager.Instance.StartPoller();
        }
    }
}