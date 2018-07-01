using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionIdentifier : IFunctionReference
    {
        // TODO: create enum that distinguished function types

        private IFunctionReference specificFunctionIdentifier;

        public FunctionReference FunctionReference
        {
            get { return specificFunctionIdentifier.FunctionReference; }
            set { specificFunctionIdentifier.FunctionReference = value; }
        }
        
        public static FunctionIdentifier Create(FunctionReference functionReference)
        {
            // TODO: implement different types based on function type

            throw new NotImplementedException();

            /*
            if (functionReference.IsSystem)
            {
                return Create(functionReference.SystemFunctionName);
            }
            else
            {
                var fid = new FunctionIdentifier();
                fid.functionReference = functionReference;

                var udf = UdfIdentifier.Create();
                fid.Stack.AddLast(udf);

                return fid;
            }
            */
        }

        public override void Interpret()
        {
            base.Interpret();

            specificFunctionIdentifier = FindSpecificFunctionIdentifier();
        }

        protected virtual IFunctionReference FindSpecificFunctionIdentifier()
        {
            IFunctionReference fr;

            fr = FindDescendant<UdtVariableMethodIdentifier>();
            if (fr != null)
            {
                return fr;
            }

            fr = FindDescendant<UdtStaticMethodIdentifier>();
            if (fr != null)
            {
                return fr;
            }

            fr = FindDescendant<UdfIdentifier>();
            if (fr != null)
            {
                return fr;
            }

            throw new NotImplementedException();
        }
    }
}
