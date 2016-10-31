using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.Jobs.CmdLineUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // Initialize logger
            Jhu.Graywulf.Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.SqlLogWriter());

            List<Type> verbs = new List<Type>() { typeof(Mirror), typeof(Test) };
            Parameters par = null;

            try
            {
                PrintHeader();
                par = (Parameters)ArgumentParser.Parse(args, verbs);
            }
            catch (ArgumentParserException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine();

                ArgumentParser.PrintUsage(verbs, Console.Out);
            }

            if (par != null)
            {
                par.Run();
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine("Graywulf Jobs Command Line Utility");
            Console.WriteLine(Copyright.InfoCopyright);
        }
    }
}
