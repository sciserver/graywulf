using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    [DbTable]
    class AuxColumnEntity : Entity
    {
        [DbColumn(Binding = DbColumnBinding.Key)]
        public int ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        [DbColumn(Binding = DbColumnBinding.Auxiliary)]
        public string SomeValue { get; set; }

        public AuxColumnEntity()
        {
        }

        public AuxColumnEntity(Context context)
            : base(context)
        {
        }

        protected internal override string GetTableQuery()
        {
            return "SELECT * FROM AuxColumnEntity";
        }
    }
}
