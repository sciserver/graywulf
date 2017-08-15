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
            Scheduler.FullSchedulerTest.Initialize(null);

            var t = new Scheduler.SimpleSchedulerTest();

            // Call to test function
            t.TestInitialize();
            t.SimpleJobTest();
            t.TestCleanup();

            Console.WriteLine("Press any key to stop");
            Console.ReadLine();
            
            Scheduler.FullSchedulerTest.CleanUp();
        }
    }
}
