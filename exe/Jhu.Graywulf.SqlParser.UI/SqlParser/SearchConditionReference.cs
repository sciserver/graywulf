using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Parser.Test
{
    /// <summary>
    /// References a 
    /// </summary>
    public class SearchConditionReference
    {
        public enum ConditionType
        {
            Unknown,
            Where,
            Having,
            JoinOn
        }

        private ConditionType type;
        private List<TableReference> tableReferences;
        private Node node;

        public ConditionType Type
        {
            get { return type; }
            internal set { type = value; }
        }

        public List<TableReference> TableReferences
        {
            get { return tableReferences; }
        }

        public Node Node
        {
            get { return node; }
        }

        public SearchConditionReference()
        {
            InitializeMembers();
        }

        public SearchConditionReference(Sql.Parsing.Predicate node)
        {
            InitializeMembers();
            UpdateFromNode((Node)node);
        }

        public SearchConditionReference(Sql.Parsing.LogicalExpression node)
        {
            InitializeMembers();
            UpdateFromNode((Node)node);
        }

        public SearchConditionReference(Sql.Parsing.LogicalExpressionBrackets node)
        {
            InitializeMembers();
            UpdateFromNode((Node)node);
        }

        private void InitializeMembers()
        {
            this.type = ConditionType.Unknown;
            this.tableReferences = new List<TableReference>();
            this.node = null;
        }

        private void UpdateFromNode(Node node)
        {
            this.node = node;

            this.tableReferences.Clear();
            this.tableReferences.AddRange(node.EnumerateDescendantsRecursive<ITableReference>(typeof(Subquery)).Select(i => i.TableReference));
        }
    }
}
