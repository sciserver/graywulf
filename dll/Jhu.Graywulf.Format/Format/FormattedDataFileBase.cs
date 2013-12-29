using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    public abstract class FormattedDataFileBase : DataFileBase, IDisposable, ICloneable
    {
        private Encoding encoding;
        private CultureInfo culture;
        private NumberStyles numberStyle;
        private DateTimeStyles dateTimeStyle;

        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        public CultureInfo Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        public NumberStyles NumberStyle
        {
            get { return numberStyle; }
            set { numberStyle = value; }
        }

        public DateTimeStyles DateTimeStyle
        {
            get { return dateTimeStyle; }
            set { dateTimeStyle = value; }
        }

        protected FormattedDataFileBase()
        {
            InitializeMembers();
        }

        protected FormattedDataFileBase(FormattedDataFileBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        protected FormattedDataFileBase(Uri uri, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode)
        {
            InitializeMembers();

            this.encoding = encoding;
            this.culture = culture;
        }

        protected FormattedDataFileBase(Stream stream, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(stream, fileMode)
        {
            InitializeMembers();

            this.encoding = encoding;
            this.culture = culture;
        }

        private void InitializeMembers()
        {
            this.encoding = null;
            this.culture = null;
            this.numberStyle = NumberStyles.Float;
            this.dateTimeStyle = DateTimeStyles.None;
        }

        private void CopyMembers(FormattedDataFileBase old)
        {
            this.encoding = (Encoding)old.encoding.Clone();
            this.culture = (CultureInfo)old.culture.Clone();
            this.numberStyle = old.numberStyle;
            this.dateTimeStyle = old.dateTimeStyle;
        }
    }
}