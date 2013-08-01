using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.CSharp;

namespace Jhu.Graywulf.ParserLib.Test
{
    [TestClass]
    public class GeneratorTest
    {
        private string GenerateParser()
        {
            var g = new ParserGenerator();

            var sw = new StringWriter();
            g.Execute(sw, new GrammarInfo(typeof(TestGrammar)));

            return sw.ToString();
        }

        private Assembly BuildParser(string code)
        {
            var source = Path.Combine(Path.GetDirectoryName(typeof(Grammar).Assembly.Location), "TestGrammarParser.cs");
            var output = Path.Combine(Path.GetDirectoryName(typeof(Grammar).Assembly.Location), "TestGrammarParser.dll");
            File.WriteAllText(source, code);
            File.Delete(output);

            var cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add(typeof(Grammar).Assembly.Location);

            cp.OutputAssembly = output;

            cp.IncludeDebugInformation = true;
            cp.GenerateInMemory = false;
            cp.TreatWarningsAsErrors = false;

            var csp = new CSharpCodeProvider();
            var cr = csp.CompileAssemblyFromFile(cp, source);

            if (cr.Errors.Count > 0)
            {
                throw new Exception(cr.Errors[0].ErrorText);
            }
            else
            {
                return Assembly.LoadFile(output);
            }
        }

        private Parser CreateParser()
        {
            var code = GenerateParser();
            var a = BuildParser(code);

            var p = (Parser)Activator.CreateInstance(a.GetType(String.Format("{0}Parser", typeof(TestGrammar).FullName)));

            return p;
        }

        [TestMethod]
        public void ParserExecuteTest()
        {
            var p = CreateParser();
            var code = "one, two,three ,four";
            var t = p.Execute(code);
        }

    }
}
