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
            Console.WriteLine(
@"JHU Graywulf Jobs Command Line Utility
(c) 2008-2011 László Dobos dobos@pha.jhu.edu
Department of Physics and Astronomy, The Johns Hopkins University

");
        }
    }
}
