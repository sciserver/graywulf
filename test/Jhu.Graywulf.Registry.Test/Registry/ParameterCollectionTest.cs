using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using Jhu.Graywulf.Components;

namespace Jhu.Graywulf.Registry
{
    [TestClass]
    public class ParameterCollectionTest
    {
        public class ParameterTestClass
        {
            private ParameterCollection parameters;

            public ParameterTestClass()
            {
                this.parameters = new ParameterCollection();
            }

            public string TestProperty { get; set; }

            [XmlIgnore]
            public ParameterCollection Parameters
            {
                get { return parameters; }
                set { parameters = value; }
            }
            
            [XmlArray("Parameters")]
            [XmlArrayItem(typeof(Parameter))]
            [XmlArrayItem(typeof(JobDefinitionParameter))]
            [XmlArrayItem(typeof(JobInstanceParameter))]
            [DefaultValue(null)]
            public Parameter[] Parameters_ForXml
            {
                get { return parameters.Values.ToArray(); }
                set { parameters = new ParameterCollection(value); }
            }
        }

        private ParameterTestClass CreateTestClass()
        {
            var pars = new ParameterTestClass();

            var par1 = new Parameter()
            {
                Name = "param1",
                Value = "test parameter value"
            };
            pars.Parameters.Add(par1);

            var par2 = new JobDefinitionParameter()
            {
                Name = "param2",
                TypeName = typeof(string).AssemblyQualifiedName,
                Direction = JobParameterDirection.In
            };
            pars.Parameters.Add(par2);

            var par3 = new JobInstanceParameter()
            {
                Name = "param3",
                Value = 123L
            };
            pars.Parameters.Add(par3);

            return pars;
        }

        [TestMethod]
        public void XmlSerializationTest()
        {
            var pars = CreateTestClass();

            var sw = new StringWriter();
            var s = new XmlSerializer(typeof(ParameterTestClass));
            s.Serialize(sw, pars);

            var xml = sw.ToString();

            // now deserialize

            var sr = new StringReader(xml);
            var pars2 = (ParameterTestClass)s.Deserialize(sr);
        }

        [TestMethod]
        public void DatabaseXmlSerializationTest()
        {
            var pars = CreateTestClass();

            var xml = pars.Parameters.SaveToXml();

            pars.Parameters.LoadFromXml(xml);
        }

    }
}
