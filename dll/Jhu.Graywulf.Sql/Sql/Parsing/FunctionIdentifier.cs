﻿using System;
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

        public static FunctionIdentifier Create(FunctionReference functionReference)
        {
            // TODO: create token explicitly if necessary,
            // current version works with fully resolved code generation only

            var udfi = new FunctionIdentifier();
            udfi.functionReference = functionReference;
            return udfi;
        }

        public override void Interpret()
        {
            base.Interpret();
            this.functionReference = FunctionReference.Interpret(this);
        }
    }
}
