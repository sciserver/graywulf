using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Jhu.Graywulf.Schema
{
    /// <summary>
    /// Reflects a function or stored procedure parameter
    /// </summary>
    [Serializable]
    [DataContract(Namespace = "")]
    public class Parameter : Variable, ICloneable
    {
        [NonSerialized]
        private ParameterDirection direction;

        [NonSerialized]
        private bool hasDefaultValue;

        [NonSerialized]
        private object defaultValue;

        [IgnoreDataMember]
        public string ParameterName
        {
            get { return Name; }
            set { Name = value; }
        }

        /// <summary>
        /// Gets or sets the direction of the parameter.
        /// </summary>
        [DataMember]
        public ParameterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        [DataMember]
        public bool HasDefaultValue
        {
            get { return hasDefaultValue; }
            set { hasDefaultValue = value; }
        }

        [DataMember]
        public object DefaultValue
        {
            get { return defaultValue; }
            set { defaultValue = value; }
        }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Parameter()
        {
            InitializeMembers();
        }

        public Parameter(DatabaseObject parent)
            : base(parent)
        {
            InitializeMembers(); 
        }


        /// <summary>
        /// Copy constructor
        /// </summary>
        /// <param name="old"></param>
        public Parameter(Parameter old)
        {
            CopyMembers(old);
        }

        /// <summary>
        /// Initializes member variable to their default values.
        /// </summary>
        private void InitializeMembers()
        {
            this.direction = ParameterDirection.Unknown;
            this.hasDefaultValue = false;
            this.defaultValue = null;
        }

        /// <summary>
        /// Copies member variables.
        /// </summary>
        /// <param name="old"></param>
        private void CopyMembers(Parameter old)
        {
            this.direction = old.direction;
            this.hasDefaultValue = old.hasDefaultValue;
            this.defaultValue = old.defaultValue;
        }

        #region ICloneable Members

        /// <summary>
        /// Returns a copy of the parameter
        /// </summary>
        /// <returns></returns>
        public object Clone()
        {
            return new Parameter(this);
        }

        #endregion
    }
}
