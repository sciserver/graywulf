using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.ServiceProcess;

namespace Jhu.Graywulf.Util
{
    public static class ServiceHelper
    {
        public static bool IsServiceInstalled(string service)
        {
            return System.ServiceProcess.ServiceController.GetServices()
                .Any(serviceController => serviceController.ServiceName.Equals(service));
        }

        public static void WaitForService(string service, int timeout, int retry)
        {
            int q = 0;
            while (true)
            {
                try
                {
                    var sc = new System.ServiceProcess.ServiceController(service);
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(timeout));
                    return;
                }
                catch (System.ServiceProcess.TimeoutException)
                {
                    q++;
                    if (q == retry)
                    {
                        throw;
                    }
                }
            }
        }

        public static void WriteErrorDump(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = (Exception)e.ExceptionObject;
            var dump = Assembly.GetEntryAssembly().Location;
            dump += ".dump";

            using (var outfile = File.AppendText(dump))
            {
                outfile.WriteLine();
                outfile.WriteLine();
                outfile.WriteLine("{0:s}", DateTime.Now);
                outfile.WriteLine("Unhandled Exception: {0}: {1}", ex.GetType().FullName, ex.Message);
                outfile.WriteLine(ex.StackTrace);
            }
        }
    }
}
