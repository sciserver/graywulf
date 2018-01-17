using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class FunctionIdentifier : IFunctionReference
    {
        private FunctionReference functionReference;

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return functionReference; }
        }
        public FunctionReference FunctionReference
        {
            get { return functionReference; }
            set { functionReference = value; }
        }

        public UdfIdentifier UdfIdentifier
        {
            get { return FindDescendant<UdfIdentifier>(); }
        }

        public FunctionName FunctionName
        {
            get { return FindDescendant<FunctionName>(); }
        }

        protected override void OnInitializeMembers()
        {
            base.OnInitializeMembers();
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (FunctionIdentifier)other;
        }

        public static FunctionIdentifier Create(string functionName)
        {
            var fid = new FunctionIdentifier();
            fid.functionReference = new FunctionReference(functionName);

            var fn = FunctionName.Create(functionName);
            fid.Stack.AddLast(fn);

            return fid;
        }

        public static FunctionIdentifier Create(FunctionReference functionReference)
        {
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
        }

        public override void Interpret()
        {
            base.Interpret();

            var udf = UdfIdentifier;
            var fn = FunctionName;

            if (udf != null)
            {
                this.functionReference = new FunctionReference(udf);
            }
            else if (fn != null)
            {
                this.functionReference = new FunctionReference(fn);
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
