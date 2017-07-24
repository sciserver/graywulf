using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Registry;

namespace Jhu.Graywulf.Install.CmdLineUtil
{
    class Program
    {
        static int Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // Initialize logger
            Logging.LoggingContext.Current.StartLogger(Logging.EventSource.CommandLineTool, true);

            List<Type> verbs = new List<Type>()
            {
                typeof(CreateRegistry),
                typeof(CreateLog),
                typeof(CreateJobPersistence),
                typeof(DropRegistry),
                typeof(DropLog),
                typeof(DropJobPersistence),
                typeof(AddUser),
                typeof(AddCluster),
                typeof(AddDomain),
                typeof(AddAdmin),
                typeof(Export),
                typeof(Import)
            };

            Verb v = null;

            try
            {
                v = (Verb)ArgumentParser.Parse(args, verbs);

                if (!v.Quiet)
                {
                    PrintHeader();
                }
            }
            catch (ArgumentParserException ex)
            {
                Console.Error.WriteLine("Error: {0}", ex.Message);

                Console.WriteLine();
                ArgumentParser.PrintUsage(verbs, Console.Out);

                return 1;
            }

            if (v != null)
            {
                try
                {
                    v.Run();
                }
                catch (Exception ex)
                {
                    if (!v.Quiet)
                    {
                        Console.WriteLine("failed.");
                    }

                    Console.Error.WriteLine("Error: {0}", ex.Message);
                    Console.Error.WriteLine(ex.StackTrace);

                    return 1;
                }
            }

            Logging.LoggingContext.Current.StopLogger();

            return 0;
        }

        private static void PrintHeader()
        {
            Console.WriteLine("Graywulf Cluster Registry Command-Line Utility");
            Console.WriteLine(Util.AssemblyReflector.GetCopyright());
        }
    }
}
