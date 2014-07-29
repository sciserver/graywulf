using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Jhu.Graywulf.Schema;
using Jhu.Graywulf.Format;

namespace Jhu.Graywulf.Format
{
    /// <summary>
    /// Implements functionality responsible for reading and writing a single
    /// block within an XML file.
    /// </summary>
    [Serializable]
    public abstract class XmlDataFileBlock : FormattedDataFileBlockBase
    {
        #region Properties

        /// <summary>
        /// Gets the objects wrapping the whole XML file.
        /// </summary>
        private XmlDataFile File
        {
            get { return (XmlDataFile)file; }
        }

        #endregion
        #region Constructors and initializers

        /// <summary>
        /// Initializes an XML data file block object.
        /// </summary>
        /// <param name="file"></param>
        public XmlDataFileBlock(XmlDataFile file)
            : base(file)
        {
            InitializeMembers();
        }

        public XmlDataFileBlock(XmlDataFileBlock old)
            : base(old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
        }

        private void CopyMembers(XmlDataFileBlock old)
        {
        }

        #endregion
    }
}
