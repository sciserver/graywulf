using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class UdtStaticMethodCall : IDataTypeReference, IMethodReference
    {
        public DataTypeIdentifier DataTypeIdentifier
        {
            get { return FindDescendant<DataTypeIdentifier>(); }
        }
        
        public DataTypeReference DataTypeReference
        {
            get { return DataTypeIdentifier.DataTypeReference; }
            set { DataTypeIdentifier.DataTypeReference = value; }
        }

        public static UdtStaticMethodCall Create(DataTypeReference dr, MethodReference mr, Expression[] args)
        {
            var mc = new UdtStaticMethodCall();
            mc.Stack.AddLast(DataTypeIdentifier.Create(dr));
            mc.Stack.AddLast(DoubleColon.Create());
            mc.Stack.AddLast(MethodName.Create(mr));
            mc.Stack.AddLast(FunctionArguments.Create(args));

            return mc;
        }
    }
}
