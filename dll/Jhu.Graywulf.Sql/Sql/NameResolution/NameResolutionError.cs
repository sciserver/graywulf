using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    static class NameResolutionError
    {
        /// <summary>
        /// Creates and parameterizes and exception to be thrown by the name resolver.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        /// <param name="objectName"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static NameResolverException CreateException(string message, Exception innerException, string objectName, Node node)
        {
            string msg;
            var id = node.FindDescendantRecursive<Identifier>();

            if (id != null)
            {
                msg = String.Format(message, objectName, id.Line + 1, id.Col + 1);
            }
            else
            {
                msg = String.Format(message, objectName, "?", "?");
            }

            NameResolverException ex = new NameResolverException(msg, innerException);
            ex.Token = id;

            return ex;
        }

        public static NameResolverException DuplicateTableAlias(string alias, Node node)
        {
            return CreateException(ExceptionMessages.DuplicateTableAlias, null, alias, node);
        }

        public static NameResolverException DuplicateColumnAlias(string alias, Node node)
        {
            return CreateException(ExceptionMessages.DuplicateColumnAlias, null, alias, node);
        }

        public static NameResolverException MissingColumnAlias(Node node)
        {
            return CreateException(ExceptionMessages.MissingColumnAlias, null, null, node);
        }

        public static NameResolverException UnresolvableDatasetReference(DatabaseObjectReference dr)
        {
            return UnresolvableDatasetReference(null, dr);
        }

        public static NameResolverException UnresolvableDatasetReference(Exception innerException, DatabaseObjectReference dr)
        {
            return CreateException(ExceptionMessages.UnresolvableDatasetReference, innerException, dr.DatasetName, dr.Node);
        }
        
        public static NameResolverException AmbigousTableReference(ITableReference node)
        {
            return CreateException(ExceptionMessages.AmbigousTableReference, null, node.TableReference.DatabaseObjectName, (Node)node);
        }

        public static NameResolverException UnresolvableTableReference(ITableReference node)
        {
            return CreateException(ExceptionMessages.UnresolvableTableReference, null, node.TableReference.DatabaseObjectName, (Node)node);
        }

        public static NameResolverException UnresolvableColumnReference(IColumnReference node)
        {
            return CreateException(ExceptionMessages.UnresolvableColumnReference, null, node.ColumnReference.ColumnName, (Node)node);
        }

        public static NameResolverException AmbigousColumnReference(IColumnReference node)
        {
            return CreateException(ExceptionMessages.AmbigousColumnReference, null, node.ColumnReference.ColumnName, (Node)node);
        }

        public static NameResolverException UnknownFunctionName(IFunctionReference node)
        {
            return CreateException(ExceptionMessages.UnknownFunctionName, null, node.FunctionReference.FunctionName, (Node)node);
        }

        public static NameResolverException UnresolvableFunctionReference(IFunctionReference node)
        {
            return CreateException(ExceptionMessages.UnresolvableFunctionReference, null, node.FunctionReference.DatabaseObjectName, (Node)node);
        }

        public static NameResolverException UnresolvableVariableReference(IVariableReference node)
        {
            return CreateException(ExceptionMessages.UnresolvableVariableReference, null, node.VariableReference.VariableName, (Node)node);
        }

        public static NameResolverException DuplicateVariableName(IVariableReference node)
        {
            return CreateException(ExceptionMessages.DuplicateVariableName, null, node.VariableReference.VariableName, (Node)node);
        }

        public static NameResolverException ScalarVariableExpected(IVariableReference node)
        {
            return CreateException(ExceptionMessages.ScalarVariableExpected, null, node.VariableReference.VariableName, (Node)node);
        }

        public static NameResolverException TargetDatasetReadOnly(ITableReference node)
        {
            return CreateException(ExceptionMessages.TargetDatasetReadOnly, null, node.TableReference.DatasetName, (Node)node);
        }

        public static NameResolverException DuplicateOutputTable(TableReference tr)
        {
            return CreateException(ExceptionMessages.DuplicateOutputTable, null, tr.DatabaseObjectName, tr.Node);
        }

        public static NameResolverException SingleColumnSubqueryRequired(Node node)
        {
            return CreateException(ExceptionMessages.SingleColumnSubqueryRequired, null, null, node);
        }

        public static NameResolverException ColumnNotPartOfTargetTable(Node node)
        {
            return CreateException(ExceptionMessages.ColumnNotPartOfTargetTable, null, null, node);
        }

        public static NameResolverException TableAlreadyExists(Node node)
        {
            return CreateException(ExceptionMessages.TableAlreadyExists, null, null, node);
        }

        public static NameResolverException TableDoesNotExists(Node node)
        {
            return CreateException(ExceptionMessages.TableDoesNotExists, null, null, node);
        }
    }
}
