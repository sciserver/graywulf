using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;


namespace Jhu.Graywulf.Format
{
    public abstract class FormattedDataFileBase : DataFileBase
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
    }
}