using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.ParserLib;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Schema.SqlServer;

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
        #region Static members

        private static QueryFactory Create(string typeName)
        {
            var ft = Type.GetType(typeName);
            var res = (QueryFactory)Activator.CreateInstance(ft, true);

            return res;
        }

        public static QueryFactory Create(Federation federation)
        {
            var res = Create(federation.QueryFactory);
            res.Context = federation.Context;
            res.Federation = federation;

            return res;
        }

        [NonSerialized]
        private static Type[] queryTypes = null;

        #endregion
        #region Private member variables

        private Federation federation;

        #endregion
        #region Properties

        protected Federation Federation
        {
            get { return federation; }
            private set { federation = value; }
        }

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

        #endregion
        #region Constructors and initializers

        protected QueryFactory()
            : base()
        {
            InitializeMembers(new StreamingContext());
        }

        protected QueryFactory(Context context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        protected QueryFactory(Federation federation)
            : base(federation.Context)
        {
            InitializeMembers(new StreamingContext());

            this.federation = federation;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.federation = null;
        }

        #endregion

        protected abstract Type[] LoadQueryTypes();

        public QueryBase CreateQuery(string queryString)
        {
            var parser = CreateParser();
            var root = parser.Execute(queryString);

            QueryBase q = CreateQueryBase((Node)root);
            q.QueryFactoryTypeName = Util.TypeNameFormatter.ToUnversionedAssemblyQualifiedName(this.GetType());
            InitializeQuery(q, queryString);

            return q;
        }

        public abstract ParserLib.Parser CreateParser();

        public abstract SqlParser.SqlValidator CreateValidator();

        public abstract SqlParser.SqlNameResolver CreateNameResolver();

        protected abstract QueryBase CreateQueryBase(Node root);

        protected abstract void InitializeQuery(QueryBase query, string queryString);

        #region Job scheduling functions

        public abstract JobInstance ScheduleAsJob(string jobName, QueryBase query, string queueName, string comments);

        #endregion
    }
}
