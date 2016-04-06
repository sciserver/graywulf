using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Jhu.Graywulf.Entities
{
    public interface IDatabaseTableObject
    {
        void LoadFromDataReader(SqlDataReader reader);
    }
}
