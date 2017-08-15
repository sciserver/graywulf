using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using Jhu.Graywulf.CommandLineParser;


namespace Jhu.Graywulf.TrafficGenerator.CmdLineUtil
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            List<Type> verbs = new List<Type>() { typeof(Run) };

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

            if (v != null)
            {
                v.Execute();
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine("Graywulf Traffic Generator Utility");
            Console.WriteLine(Util.AssemblyReflector.GetCopyright());
        }
    }
}
