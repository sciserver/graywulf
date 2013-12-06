using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.CommandLineParser;

namespace Jhu.Graywulf.ConfigUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            List<Type> verbs = new List<Type>() { typeof(Configure) };

            Verb v = null;

            try
            {
                PrintHeader();
                v = (Verb)ArgumentParser.Parse(args, verbs);
            }
            catch (ArgumentParserException ex)
            {
                Console.WriteLine("Error: {0}", ex.Message);
                Console.WriteLine();

                ArgumentParser.PrintUsage(verbs, Console.Out);
            }

            try
            {
                if (v != null)
                {
                    v.Run();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);

                Environment.Exit(-1);
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine(
@"JHU Graywulf Build Configuration Utility
(c) 2008-2012 László Dobos dobos@pha.jhu.edu
Department of Physics and Astronomy, The Johns Hopkins University

");
        }
    }
}
