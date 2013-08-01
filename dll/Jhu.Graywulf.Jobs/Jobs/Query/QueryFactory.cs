using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.ParserLib;

namespace Jhu.Graywulf.Jobs.Query
{
    /// <summary>
    /// Implements basic functions to create and manipulate jobs
    /// wrapping queries.
    /// </summary>
    /// <remarks>
    /// The main purpose of this class is to support plugin based
    /// query types and dataset types.
    /// </remarks>
    [Serializable]
    public abstract class QueryFactory : JobFactoryBase
    {
        [NonSerialized]
        private static Type[] queryTypes = null;

        public Type[] QueryTypes
        {
            get
            {
                lock (SyncRoot)
                {
                    if (queryTypes == null)
                    {
                        queryTypes = LoadQueryTypes();
                    }
                }

                return queryTypes;
            }
        }

        public QueryFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        public QueryFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        protected abstract Type[] LoadQueryTypes();

        public QueryBase CreateQuery(string queryString)
        {
            return CreateQuery(queryString, ExecutionMode.Graywulf, null);
        }

        public QueryBase CreateQuery(string queryString, ExecutionMode mode)
        {
            return CreateQuery(queryString, mode, null);
        }

        public QueryBase CreateQuery(string queryString, ExecutionMode mode, string outputTable)
        {
            var parser = CreateParser();
            var root = parser.Execute(queryString);

            QueryBase q = CreateQueryBase((Node)root);
            q.QueryFactoryTypeName = this.GetType().AssemblyQualifiedName;
            q.ExecutionMode = mode;

            switch (mode)
            {
                case ExecutionMode.Graywulf:
                    GetInitializedQuery_Graywulf(q, queryString, outputTable);
                    break;
                case ExecutionMode.SingleServer:
                    GetInitializedQuery_SingleServer(q, queryString, outputTable);
                    break;
                default:
                    throw new NotImplementedException();
            }

            return q;
        }

        public abstract ParserLib.Parser CreateParser();

        public abstract SqlParser.SqlValidator CreateValidator();

        public abstract SqlParser.SqlNameResolver CreateNameResolver();

        protected abstract QueryBase CreateQueryBase(Node root);

        protected abstract void GetInitializedQuery_Graywulf(QueryBase query, string queryString, string outputTable);

        protected abstract void GetInitializedQuery_SingleServer(QueryBase query, string queryString, string outputTable);

        #region Job scheduling functions

        public abstract JobInstance ScheduleAsJob(QueryBase query, string queueName, string comments);

        #endregion
    }
}
