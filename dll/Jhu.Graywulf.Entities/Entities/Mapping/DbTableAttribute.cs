using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Entities.Mapping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class DbTableAttribute : Attribute
    {
        #region Private member variables

        private string name;

        #endregion
        #region Properties

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        #endregion
        #region Constructors and initializers

        public DbTableAttribute()
        {
            InitializeMembers();
        }

        public DbTableAttribute(string name)
        {
            InitializeMembers();

            this.name = name;
        }

        private void InitializeMembers()
        {
            this.name = null;
        }

        #endregion
    }
}
