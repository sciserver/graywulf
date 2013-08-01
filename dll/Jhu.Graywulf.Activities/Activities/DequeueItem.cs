using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using System.Threading;

namespace Jhu.Graywulf.Activities
{
    public class DequeueItem<T> : CodeActivity<T>
    {
        [RequiredArgument]
        public InArgument<Queue<T>> InputQueue { get; set; }

        protected override T Execute(CodeActivityContext activityContext)
        {
            Queue<T> queue = InputQueue.Get(activityContext);

            // Try to dequeue value and sleep if empty
            while (true)
            {
                if (queue.Count > 0)
                {
                    return queue.Dequeue();
                }
                else
                {
                    return default(T);
                }
            }
        }
    }
}
