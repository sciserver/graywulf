using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jhu.Graywulf.Util
{
    /// <summary>
    /// Implements a safe awaiter to synchonize async calls.
    /// </summary>
    public static class TaskHelper
    {
        public static void Wait(Task task)
        {
            task.ConfigureAwait(false);     // TODO: this seems risky here, test

            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }

        public static T Wait<T>(Task<T> task)
        {
            task.ConfigureAwait(false);

            try
            {
                return task.Result;
            }
            catch (AggregateException ex)
            {
                if (ex.InnerExceptions.Count == 1)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
