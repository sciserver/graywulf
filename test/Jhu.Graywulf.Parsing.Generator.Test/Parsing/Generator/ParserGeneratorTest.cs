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

namespace Jhu.Graywulf.Parsing.Generator
{
    [TestClass]
    public class ParserGeneratorTest
    {
        private string GenerateParser(Type grammar)
        {
            var g = new ParserGenerator();

            var sw = new StringWriter();
            g.Execute(sw, grammar);

            return sw.ToString();
        }

        private void AddInheritedGrammarReference(CompilerParameters cp, Type grammar)
        {
            var g = new GrammarInfo(grammar);
            var i = g.InheritedGrammar;

            if (i != null)
            {
                var code = GenerateParser(i.GrammarType);
                var a = BuildParser(code, i.GrammarType);
                cp.ReferencedAssemblies.Add(a.Location);

                AddInheritedGrammarReference(cp, i.GrammarType);
            }
        }

        private Assembly BuildParser(string code, Type grammar)
        {
            var g = new GrammarInfo(grammar);
            var t = Type.GetType(g.Namespace + "." + g.ParserClassName + "," + grammar.Name);
            if (t != null)
            {
                return t.Assembly;
            }

            var source = grammar.Name + ".cs";
            var output = grammar.Name + ".dll";
            File.WriteAllText(source, code);

            var cp = new CompilerParameters();
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add("System.Core.dll");
            cp.ReferencedAssemblies.Add(typeof(Parser).Assembly.Location);
            AddInheritedGrammarReference(cp, grammar);

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
                return Assembly.LoadFile(Path.Combine(Environment.CurrentDirectory, output));
            }
        }

        private Parser CreateParser(Type grammar)
        {
            var g = new GrammarInfo(grammar);
            var code = GenerateParser(grammar);
            var a = BuildParser(code, grammar);

            var p = (Parser)Activator.CreateInstance(a.GetType(String.Format("{0}.{1}", g.Namespace, g.ParserClassName)));

            return p;
        }

        [TestMethod]
        public void SimpleGrammarTest()
        {
            var p = CreateParser(typeof(TestGrammar));
            var code = "one, two,three ,four";
            var t = p.Execute(code);
        }

        [TestMethod]
        public void InheritedGrammarTest()
        {
            var p = CreateParser(typeof(InheritedGrammar));
            var code = "one, two,three ,four";
            var t = p.Execute(code);
        }

        [TestMethod]
        public void SecondInheritedGrammarTest()
        {
            var p = CreateParser(typeof(SecondInheritedGrammar));
            var code = "one, two,three ,four";
            var t = p.Execute(code);
        }
    }
}
