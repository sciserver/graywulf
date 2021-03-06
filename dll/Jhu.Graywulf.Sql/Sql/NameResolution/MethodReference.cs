﻿using System;
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

        public static MethodReference Interpret(MethodName mn)
        {
            var mr = new MethodReference(mn)
            {
                MethodName = RemoveIdentifierQuotes(mn.Value)
            };

            return mr;
        }
    }
}
