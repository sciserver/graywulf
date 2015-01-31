using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Activities.Tracking;
using System.ComponentModel;
using System.Windows.Markup;
using System.Runtime.Serialization;
using Jhu.Graywulf.Logging;

namespace Jhu.Graywulf.Activities
{
    [Designer(typeof(RetryDesigner))]
    [ContentProperty("Try")]
    public class Retry : NativeActivity, IGraywulfActivity
    {
        [DataContract]
        class RetryState
        {
            [DataMember(EmitDefaultValue = false)]
            public int Retries
            {
                get;
                set;
            }

            [DataMember(EmitDefaultValue = false)]
            public Exception Exception
            {
                get;
                set;
            }

            [DataMember(EmitDefaultValue = false)]
            public bool SuppressCancel
            {
                get;
                set;
            }
        }


        [RequiredArgument]
        public InArgument<Guid> JobGuid { get; set; }
        [RequiredArgument]
        public InArgument<Guid> UserGuid { get; set; }

        public Activity Try { get; set; }
        public Activity Finally { get; set; }

        [RequiredArgument]
        public InArgument<int> MaxRetries { get; set; }

        private Variable<RetryState> state = new Variable<RetryState>();

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

            metadata.AddImplementationVariable(this.state);
        }

        protected override void Execute(NativeActivityContext context)
        {
            state.Set(context, new RetryState());

            if (Try != null)
            {
                context.ScheduleActivity(Try, new CompletionCallback(OnTryComplete), new FaultCallback(OnTryFaulted));
            }
            else
            {
                OnTryComplete(context, null);
            }
        }

        protected override void Cancel(NativeActivityContext context)
        {
            var state = this.state.Get(context);
            if (!state.SuppressCancel)
            {
                context.CancelChildren();
            }

            context.CancelChildren();
        }

        private void OnTryComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            var state = this.state.Get(context);

            if (state.Exception != null)
            {
                // If retry is possible, 
                if (state.Retries < MaxRetries.Get(context) - 1)
                {
                    state.Retries++;

                    // Reschedule Try block
                    state.Exception = null;
                    context.ScheduleActivity(this.Try, OnTryComplete, OnTryFaulted);

                    return;
                }
            }

            // Only the try block can be canceled.
            state.SuppressCancel = true;

            // Schedule the finally block
            if (this.Finally != null)
            {
                context.ScheduleActivity(this.Finally, new CompletionCallback(OnFinallyComplete), new FaultCallback(OnFinallyFaulted));
            }
            else
            {
                OnFinallyComplete(context, null);
            }
        }

        private void OnTryFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            var state = this.state.Get(faultContext);
            state.Exception = propagatedException;

            if (state.Retries < MaxRetries.Get(faultContext) - 1)
            {
                // Absorb error
                faultContext.CancelChild(propagatedFrom);
                faultContext.HandleFault();
            }
        }

        private void OnFinallyComplete(NativeActivityContext context, ActivityInstance completedInstance)
        {
            if (context.IsCancellationRequested)
            {
                context.MarkCanceled();
            }
        }

        private void OnFinallyFaulted(NativeActivityFaultContext faultContext, Exception propagatedException, ActivityInstance propagatedFrom)
        {
            var state = this.state.Get(faultContext);
            state.Exception = propagatedException;
            state.SuppressCancel = false;
        }
    }
}
