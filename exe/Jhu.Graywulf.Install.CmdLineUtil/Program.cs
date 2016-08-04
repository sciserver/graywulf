﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Jhu.Graywulf.CommandLineParser;
using Jhu.Graywulf.Install;

namespace Jhu.Graywulf.Registry.CmdLineUtil
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            // Initialize logger
            Jhu.Graywulf.Logging.Logger.Instance.Writers.Add(new Jhu.Graywulf.Logging.SqlLogWriter());

            List<Type> verbs = new List<Type>()
            {
                typeof(CreateRegistry),
                typeof(CreateSchema),
                typeof(CreateCluster),
                typeof(CreateDomain),
                typeof(CreateAdmin),
                typeof(Export),
                typeof(Import)
            };

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
                try
                {
                    v.Run();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("failed.");
                    Console.WriteLine(ex.Message);
                }
            }
        }

        private static void PrintHeader()
        {
            Console.WriteLine("Graywulf Cluster Registry Command-Line Utility");
            Console.WriteLine(Copyright.InfoCopyright);
        }
    }
}
