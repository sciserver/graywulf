using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jhu.Graywulf.Web.Api
{
    public class AcceptMimeType : ICloneable, IComparable
    {
        #region Private member variables

        private int index;
        private string mimeType;
        private int? level;
        private double? quality;

        #endregion
        #region Properties

        /// <summary>
        /// Gets or sets the index of the mime type as it appears
        /// in the accept header.
        /// </summary>
        public int Index
        {
            get { return index; }
            set { index = value; }
        }

        /// <summary>
        /// Gets or sets the mime type part of the accept header.
        /// </summary>
        public string MimeType
        {
            get { return mimeType; }
            set { mimeType = value; }
        }

        /// <summary>
        /// Gets or sets the level parameter of the accept header mime type.
        /// </summary>
        public int? Level
        {
            get { return level; }
            set { level = value; }
        }

        /// <summary>
        /// Gets or sets the quality parameter of the accept header mime type.
        /// </summary>
        public double? Quality
        {
            get { return quality; }
            set { quality = value; }
        }

        #endregion
        #region Constructors and initializers

        public AcceptMimeType()
        {
            InitializeMembers();
        }

        public AcceptMimeType(AcceptMimeType old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.index = 0;
            this.mimeType = null;
            this.level = null;
            this.quality = null;
        }

        private void CopyMembers(AcceptMimeType old)
        {
            this.index = old.index;
            this.mimeType = old.mimeType;
            this.level = old.level;
            this.quality = old.quality;
        }

        public object Clone()
        {
            return new AcceptMimeType(this);
        }

        #endregion

        public int CompareTo(object obj)
        {
            var other = (AcceptMimeType)obj;

            if (this.quality.HasValue && other.quality.HasValue)
            {
                return -this.quality.Value.CompareTo(other.quality.Value);
            }
            else if (this.quality.HasValue)
            {
                return 1;
            }
            else if (other.quality.HasValue)
            {
                return -1;
            }
            else
            {
                return index.CompareTo(other.index);
            }
        }

        public override string ToString()
        {
            return mimeType;
        }
    }
}
