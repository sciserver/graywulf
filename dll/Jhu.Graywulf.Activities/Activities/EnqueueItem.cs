using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;

namespace Jhu.Graywulf.Activities
{
    public class EnqueueItem<T> : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Queue<T>> InputQueue { get; set; }
        [RequiredArgument]
        public InArgument<T> Item { get; set; }

        protected override void Execute(CodeActivityContext activityContext)
        {
            Queue<T> queue = InputQueue.Get(activityContext);
            T item = Item.Get(activityContext);

            queue.Enqueue(item);
        }
    }
}
