using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.CommandLineParser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            string pars = "run -que xx -quota 11 -b -test InvariantCulture"; 

            object pp = ArgumentParser.Parse(pars.Split(' '),
                new List<Type>() { typeof(GetParameters), typeof(RunParameters) });

            ArgumentParser.PrintUsage(new List<Type>() { typeof(GetParameters), typeof(RunParameters) }, Console.Out);

            Console.ReadLine();
        }
    }
}
