﻿using System;
using System.Collections.Generic;
using System.Activities;

namespace Jhu.Graywulf.Activities
{
    public class DequeueItem<T> : CodeActivity<T>
    {
        [RequiredArgument]
        public InArgument<Queue<T>> InputQueue { get; set; }

        protected override T Execute(CodeActivityContext activityContext)
        {
            Queue<T> queue = InputQueue.Get(activityContext);

            lock (queue)
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
