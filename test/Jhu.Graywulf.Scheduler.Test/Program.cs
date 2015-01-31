using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Scheduler.SchedulerTest.Initialize(null);

            var t = new Scheduler.SchedulerTest();
            
            // Call to test function

            //t.ExceptionTest();
            //t.AsyncExceptionTest();
            t.AsyncExceptionWithRetryTest();


            Console.WriteLine("Press any key to stop");
            Console.ReadLine();


            //Scheduler.SchedulerTest.CleanUp();
        }
    }
}
