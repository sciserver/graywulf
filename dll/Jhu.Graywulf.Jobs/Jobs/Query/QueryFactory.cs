using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using Jhu.Graywulf.Registry;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Schema;
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

        public static QueryFactory Create(Federation federation)
        {
            return Create(federation.QueryFactory, federation.RegistryContext);
        }

        public static QueryFactory Create(string typeName, RegistryContext context)
        {
            var ft = Type.GetType(typeName);
            var res = (QueryFactory)Activator.CreateInstance(ft, new object[] { context });
            return res;
        }
        
        [NonSerialized]
        private static Type[] queryTypes = null;

        #endregion
        #region Private member variables

        #endregion
        #region Properties

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

        protected QueryFactory(RegistryContext context)
            : base(context)
        {
            InitializeMembers(new StreamingContext());
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
        }

        #endregion

        protected abstract Type[] LoadQueryTypes();

        public SqlQuery CreateQuery(string queryString)
        {
            var parser = CreateParser();
            var root = parser.Execute(queryString);

            SqlQuery q = CreateQueryBase((Node)root);
            q.QueryFactoryTypeName = Util.TypeNameFormatter.ToUnversionedAssemblyQualifiedName(this.GetType());
            InitializeQuery(q, queryString);

            return q;
        }

        public abstract Parsing.Parser CreateParser();

        public abstract Sql.Validation.SqlValidator CreateValidator();

        public abstract Sql.NameResolution.SqlNameResolver CreateNameResolver();

        protected abstract SqlQuery CreateQueryBase(Node root);

        protected abstract void InitializeQuery(SqlQuery query, string queryString);

        public void AppendUserDatabase(SqlQuery query, SqlServerDataset userDatabase, ServerInstance serverInstance)
        {
            // TODO: modify this to change default output database

            userDatabase.IsMutable = true;
            query.CustomDatasets.Add(userDatabase);
            query.DefaultSourceDataset = userDatabase;
            query.DefaultOutputDataset = userDatabase;
        }

        #region Job scheduling functions

        public abstract JobInstance ScheduleAsJob(string jobName, SqlQuery query, string queueName, TimeSpan timeout, string comments);

        #endregion
    }
}
