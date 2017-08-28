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

            this.functionReference = null;
        }

        protected override void OnCopyMembers(object other)
        {
            base.OnCopyMembers(other);

            var old = (FunctionIdentifier)other;

            this.functionReference = old.functionReference;
        }

        public static FunctionIdentifier Create(string functionName)
        {
            var fid = new FunctionIdentifier();
            fid.functionReference = new FunctionReference(functionName);
            return fid;
        }

        public static FunctionIdentifier Create(FunctionReference functionReference)
        {
            var fid = new FunctionIdentifier();
            fid.Stack.AddLast(UdfIdentifier.Create());
            fid.functionReference = functionReference;
            return fid;
        }

        public override void Interpret()
        {
            base.Interpret();

            this.functionReference = new FunctionReference(this);
        }
    }
}
