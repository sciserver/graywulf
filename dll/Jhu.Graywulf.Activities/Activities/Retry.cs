using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Tracking;
using System.ComponentModel;
using System.Windows.Markup;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Activities
{
    [Designer(typeof(RetryDesigner))]
    [ContentProperty("Try")]
    public class Retry : NativeActivity, IGraywulfActivity
    {
        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public Activity Try { get; set; }
        public Activity Finally { get; set; }

        [RequiredArgument]
        public InArgument<int> MaxRetries { get; set; }

        protected Variable<int> retries = new Variable<int>();

        public Retry()
        {
        }

        protected override void CacheMetadata(NativeActivityMetadata metadata)
        {
            RuntimeArgument argument;
            
            argument = new RuntimeArgument("JobGuid", typeof(Guid), ArgumentDirection.In);
            metadata.Bind(this.JobGuid, argument);
            metadata.AddArgument(argument);

            argument = new RuntimeArgument("UserGuid", typeof(Guid), ArgumentDirection.In);
            metadata.Bind(this.UserGuid, argument);
            metadata.AddArgument(argument);

            if (this.Try != null)
            {
                metadata.AddChild(this.Try);
            }
            if (this.Finally != null)
            {
                metadata.AddChild(this.Finally);
            }

            argument = new RuntimeArgument("MaxRetries", typeof(int), ArgumentDirection.In);
            metadata.Bind(this.MaxRetries, argument);
            metadata.AddArgument(argument);

            metadata.AddImplementationVariable(this.retries);
        }

        protected override void Execute(NativeActivityContext context)
        {
            retries.Set(context, 0);

            if (Try != null)
            {
                context.ScheduleActivity(Try, OnTryComplete, OnTryFaulted);
            }
            else
            {
                OnTryComplete(context, null);
            }
        }

        private void OnTryComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            if (Finally != null)
            {
                context.ScheduleActivity(this.Finally, OnFinallyComplete, OnFinallyFaulted);
            }
            else
            {
                OnFinallyComplete(context, null);
            }
        }

        private void OnTryFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {

#if DEBUG
            System.Diagnostics.Debugger.Break();
#endif

            // Handle exception
            int r = retries.Get(faultContext);
            retries.Set(faultContext, ++r);

            faultContext.CancelChild(propagatedFrom);
            faultContext.HandleFault();


            // Run the finally block before doing anything else
            if (Finally != null)
            {
                faultContext.ScheduleActivity(this.Finally, OnFinallyComplete, OnFinallyFaulted);
            }
            else
            {
                OnFinallyComplete(faultContext, null);
            }

            // If retry is possible, 
            if (r < MaxRetries.Get(faultContext))
            {
                // Absorb error
                faultContext.HandleFault();

                faultContext.ScheduleActivity(this.Try, OnTryComplete, OnTryFaulted);
            }
            else
            {
                // Fault
                if (propagatedException is AggregateException)
                {
                    throw ((AggregateException)propagatedException).InnerException;
                }
                else
                {
                    throw propagatedException;
                }
            }
        }

        private void OnFinallyComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {

        }

        private void OnFinallyFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {

        }

        protected override void Cancel(NativeActivityContext context)
        {
            context.CancelChildren();
        }
    }
}
