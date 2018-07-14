using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public class MethodReference : ReferenceBase
    {
        #region Property storage variables

        private string methodName;

        #endregion
        #region Properties

        public string MethodName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        public override string UniqueName
        {
            get { return methodName; }
            set { methodName = value; }
        }

        #endregion
        #region Constructors and initializers

        public MethodReference()
        {
            InitializeMembers();
        }

        public MethodReference(Node node)
            : base(node)
        {
            InitializeMembers();
        }

        public MethodReference(MethodReference old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.methodName = null;
        }

        private void CopyMembers(MethodReference old)
        {
            this.methodName = old.methodName;
        }

        public override object Clone()
        {
            return new MethodReference(this);
        }

        #endregion

        public static MethodReference Interpret(UdtStaticMethodCall mc)
        {
            var mi = mc.FindDescendant<UdtStaticMethodIdentifier>();
            var di = mi.FindDescendant<DataTypeIdentifier>();
            var mn = mi.FindDescendant<MethodName>();

            var mr = new MethodReference(mc)
            {
                MethodName = RemoveIdentifierQuotes(mn.Value)
            };

            return mr;
        }

        public static MethodReference Interpret(UdtMethodCall mc)
        {
            var mn = mc.FindDescendant<MethodName>();

            var mr = new MethodReference(mc)
            {
                MethodName = RemoveIdentifierQuotes(mn.Value)
            };

            return mr;
        }
    }
}
