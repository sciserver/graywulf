using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Activities;
using System.ComponentModel;
using System.Windows.Markup;

namespace Jhu.Graywulf.Activities
{
    [ContentProperty("Child")]
    [Designer(typeof(CheckpointDesigner))]
    public class Checkpoint : NativeActivity, ICheckpoint
    {

        public Activity Child { get; set; }
        
        [RequiredArgument]
        public InArgument<string> CheckpointName { get; set; }

        private CompletionCallback onChildComplete;

        // Methods
        public Checkpoint()
        {
            this.onChildComplete = new CompletionCallback(this.InternalExecute);
        }

        protected override void Execute(NativeActivityContext context)
        {
            if (this.Child != null)
            {
                context.ScheduleActivity(Child, this.onChildComplete);
            }
        }

        private void InternalExecute(NativeActivityContext context, ActivityInstance completedInstance)
        {

        }

    }
}
