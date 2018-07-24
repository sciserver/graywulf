using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;

namespace Jhu.Graywulf.Sql.NameResolution
{
    public static class NameResolutionError
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
        
        public static NameResolverException AmbigousTableReference(TableReference tr)
        {
            return CreateException(ExceptionMessages.AmbigousTableReference, null, tr.DatabaseObjectName, tr.Node);
        }

        public static NameResolverException UnresolvableTableReference(TableReference tr)
        {
            return CreateException(ExceptionMessages.UnresolvableTableReference, null, tr.DatabaseObjectName, tr.Node);
        }

        public static NameResolverException UnresolvableColumnReference(ColumnReference cr)
        {
            var name = String.IsNullOrWhiteSpace(cr.ColumnName) ? cr.Node.ToString() : cr.ColumnName;
            return CreateException(ExceptionMessages.UnresolvableColumnReference, null, cr.ColumnName, cr.Node);
        }

        public static NameResolverException AmbigousColumnReference(ColumnReference cr)
        {
            return CreateException(ExceptionMessages.AmbigousColumnReference, null, cr.ColumnName, cr.Node);
        }

        public static NameResolverException UnknownFunctionName(FunctionReference fr)
        {
            return CreateException(ExceptionMessages.UnknownFunctionName, null, fr.FunctionName, fr.Node);
        }

        public static NameResolverException UnresolvableFunctionReference(FunctionReference fr)
        {
            return CreateException(ExceptionMessages.UnresolvableFunctionReference, null, fr.DatabaseObjectName, fr.Node);
        }

        public static NameResolverException UnresolvableVariableReference(VariableReference vr)
        {
            return CreateException(ExceptionMessages.UnresolvableVariableReference, null, vr.VariableName, vr.Node);
        }

        public static NameResolverException DuplicateVariableName(VariableReference vr)
        {
            return CreateException(ExceptionMessages.DuplicateVariableName, null, vr.VariableName, vr.Node);
        }

        public static NameResolverException ScalarVariableExpected(VariableReference vr)
        {
            return CreateException(ExceptionMessages.ScalarVariableExpected, null, vr.VariableName, vr.Node);
        }

        public static NameResolverException TableVariableExpected(VariableReference vr)
        {
            return CreateException(ExceptionMessages.TableVariableExpected, null, vr.VariableName, vr.Node);
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

        public static NameResolverException ColumnNotPartOfTargetTable(ColumnReference cr)
        {
            return CreateException(ExceptionMessages.ColumnNotPartOfTargetTable, null, null, cr.Node);
        }

        public static NameResolverException TableAlreadyExists(Node node)
        {
            return CreateException(ExceptionMessages.TableAlreadyExists, null, null, node);
        }

        public static NameResolverException TableDoesNotExists(Node node)
        {
            return CreateException(ExceptionMessages.TableDoesNotExists, null, null, node);
        }

        public static NameResolverException TableIdentifierTooManyParts(Node node)
        {
            return CreateException(ExceptionMessages.TableIdentifierTooManyParts, null, null, node);
        }

        public static NameResolverException DataTypeIdentifierTooManyParts(Node node)
        {
            return CreateException(ExceptionMessages.DataTypeIdentifierTooManyParts, null, null, node);
        }

        public static NameResolverException FunctionIdentifierTooManyParts(Node node)
        {
            return CreateException(ExceptionMessages.FunctionIdentifierTooManyParts, null, null, node);
        }

        public static NameResolverException DifferentTableReferenceInHintNotAllowed(TableReference tr)
        {
            return CreateException(ExceptionMessages.DifferentTableReferenceInHintNotAllowed, null, null, tr.Node);
        }
    }
}
