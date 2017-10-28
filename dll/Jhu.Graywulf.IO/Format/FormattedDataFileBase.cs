using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization;
using Jhu.Graywulf.IO;

namespace Jhu.Graywulf.Format
{
    [Serializable]
    [DataContract(Namespace = "")]
    public abstract class FormattedDataFileBase : DataFileBase, IDisposable, ICloneable
    {
        [NonSerialized]
        private Encoding encoding;

        [NonSerialized]
        private CultureInfo culture;

        [NonSerialized]
        private NumberStyles numberStyle;

        [NonSerialized]
        private DateTimeStyles dateTimeStyle;

        [NonSerialized]
        private NullStyles nullStyle;

        [IgnoreDataMember]
        public Encoding Encoding
        {
            get { return encoding; }
            set { encoding = value; }
        }

        [DataMember(Name="Encoding")]
        private string Encoding_ForXml
        {
            get { return encoding != null ? encoding.WebName : null; }
            set { encoding = value != null ? System.Text.Encoding.GetEncoding(value) : null; }
        }

        [IgnoreDataMember]
        public CultureInfo Culture
        {
            get { return culture; }
            set { culture = value; }
        }

        [DataMember(Name="Culture")]
        private string Culture_ForXml
        {
            get { return culture != null ? culture.Name : null; }
            set { culture = value != null ? System.Globalization.CultureInfo.GetCultureInfo(value) : null; }
        }

        // TODO: remove this and add to column data type format instead
        [IgnoreDataMember]
        public NumberStyles NumberStyle
        {
            get { return numberStyle; }
            set { numberStyle = value; }
        }

        // TODO: remove this and add to column data type format instead
        [IgnoreDataMember]
        public DateTimeStyles DateTimeStyle
        {
            get { return dateTimeStyle; }
            set { dateTimeStyle = value; }
        }

        [DataMember]
        public NullStyles NullStyle
        {
            get { return nullStyle; }
            set { nullStyle = value; }
        }

        protected FormattedDataFileBase()
        {
            InitializeMembers(new StreamingContext());
        }

        protected FormattedDataFileBase(FormattedDataFileBase old)
            : base(old)
        {
            CopyMembers(old);
        }

        protected FormattedDataFileBase(Uri uri, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(uri, fileMode)
        {
            InitializeMembers(new StreamingContext());

            this.encoding = encoding;
            this.culture = culture;
        }

        protected FormattedDataFileBase(Stream stream, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(stream, fileMode)
        {
            InitializeMembers(new StreamingContext());

            this.encoding = encoding;
            this.culture = culture;
        }

        [OnDeserializing]
        private void InitializeMembers(StreamingContext context)
        {
            this.encoding = System.Text.Encoding.UTF8;
            this.culture = System.Globalization.CultureInfo.InvariantCulture;
            this.numberStyle = NumberStyles.Float;
            this.dateTimeStyle = DateTimeStyles.None;
            this.nullStyle = NullStyles.NullText;
        }

        private void CopyMembers(FormattedDataFileBase old)
        {
            this.encoding = Util.DeepCloner.CloneObject(old.encoding);
            this.culture = Util.DeepCloner.CloneObject(old.culture);
            this.numberStyle = old.numberStyle;
            this.dateTimeStyle = old.dateTimeStyle;
            this.nullStyle = old.nullStyle;
        }
    }
}