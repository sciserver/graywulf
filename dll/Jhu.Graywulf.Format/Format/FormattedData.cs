using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;


namespace Jhu.Graywulf.Format
{
   public abstract class FormattedData : DataFileBase
    {
                
        private Encoding encoding;
        private bool detectEncoding;
        private CultureInfo culture;
        private NumberStyles numberStyle;
        private DateTimeStyles dateTimeStyle;
        public delegate bool ParserDelegate(string value, out object result);
        public delegate string FormatterDelegate(object value, string format);
        
        [NonSerialized]
        public ParserDelegate[] columnParsers;
        [NonSerialized]
        public FormatterDelegate[] columnFormatters;
        private bool generateIdentity;

        public bool DetectEncoding
        {
            get { return detectEncoding; }
            set { detectEncoding = value; }
        }

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

        public bool GenerateIdentity
        {
            get { return generateIdentity; }
            set { generateIdentity = value; }
        }

        protected FormattedData()
        {
        }

        protected FormattedData(string path, bool detectEncoding, CultureInfo culture)
            : base(path, DataFileMode.Read)
        {
            InitializeMembers();
            this.detectEncoding = detectEncoding;
            this.encoding = null;
            this.culture = culture;
        }

        protected FormattedData(string path, DataFileMode fileMode, Encoding encoding, CultureInfo culture)
            : base(path, fileMode)
        {
            if (path == null)
            {
                throw new ArgumentNullException("path");
            }

            InitializeMembers();

            this.detectEncoding = false;
            this.encoding = encoding;
            this.culture = culture;
        }

        protected FormattedData(string path, CultureInfo culture)
            : base(path, DataFileMode.Read)
        {
            InitializeMembers();
            
            this.encoding = null;
            this.culture = culture;
        }

        protected FormattedData(TextReader input, CultureInfo culture) {

            InitializeMembers();
            this.encoding = null;
            this.culture = culture;        
        }

        protected FormattedData(XmlReader input, CultureInfo culture)
        {
            InitializeMembers();
            this.encoding = null;
            this.culture = culture;
        }
        
       protected FormattedData(TextWriter output, Encoding encoding, CultureInfo culture)
       {
            InitializeMembers();
            this.encoding = encoding;
            this.culture = culture;        
       }

       protected FormattedData(XmlWriter write, DataFileMode fileMode)
       {
           InitializeMembers();
       }

       protected FormattedData(string path, DataFileMode fileMode)
       {
           InitializeMembers();
       }

       protected FormattedData(XmlTextWriter write, Encoding encoding, CultureInfo culture) 
       {
           InitializeMembers();
       }
       
       private void InitializeMembers()
       {        
            this.detectEncoding = true;
            this.encoding = null;
            this.culture = null;
            this.numberStyle = NumberStyles.Float;
            this.dateTimeStyle = DateTimeStyles.None;

            this.generateIdentity = true;
       }

       private void InitializeColumnParsers()
       {
            columnParsers = new ParserDelegate[Columns.Count];

            for (int i = 0; i < columnParsers.Length; i++)
            {
                columnParsers[i] = GetParserDelegate(Columns[i]);
            }
       }

       private void InitializeColumnFormatters()
       {
            columnFormatters = new FormatterDelegate[Columns.Count];

            for (int i = 0; i < columnFormatters.Length; i++)
            {
                columnFormatters[i] = GetFormatterDelegate(Columns[i]);
            }
       }

       protected override void OnColumnsCreated()
       {
            base.OnColumnsCreated();

            if ((FileMode & DataFileMode.Read) != 0)
            {
                InitializeColumnParsers();
            }

            if ((FileMode & DataFileMode.Write) != 0)
            {
                InitializeColumnFormatters();
            }
        }

        protected virtual ParserDelegate GetParserDelegate(DataFileColumn column)
        {
            var t = column.DataType.Type;

            if (t == typeof(SByte))
            {
                return delegate(string s, out object p)
                {
                    SByte v;
                    var r = SByte.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int16))
            {
                return delegate(string s, out object p)
                {
                    Int16 v;
                    var r = Int16.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int32))
            {
                return delegate(string s, out object p)
                {
                    Int32 v;
                    var r = Int32.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Int64))
            {
                return delegate(string s, out object p)
                {
                    Int64 v;
                    var r = Int64.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Byte))
            {
                return delegate(string s, out object p)
                {
                    Byte v;
                    var r = Byte.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt16))
            {
                return delegate(string s, out object p)
                {
                    UInt16 v;
                    var r = UInt16.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt32))
            {
                return delegate(string s, out object p)
                {
                    UInt32 v;
                    var r = UInt32.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(UInt64))
            {
                return delegate(string s, out object p)
                {
                    UInt64 v;
                    var r = UInt64.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(bool))
            {
                // *** TODO: parse numbers!
                return delegate(string s, out object p)
                {
                    bool v;
                    var r = bool.TryParse(s, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(float))
            {
                return delegate(string s, out object p)
                {
                    float v;
                    var r = float.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(double))
            {
                return delegate(string s, out object p)
                {
                    double v;
                    var r = double.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(decimal))
            {
                return delegate(string s, out object p)
                {
                    decimal v;
                    var r = decimal.TryParse(s, numberStyle, culture, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(char))
            {
                return delegate(string s, out object p)
                {
                    if (s.Length == 1)
                    {
                        p = s[0];
                        return true;
                    }
                    else
                    {
                        p = null;
                        return false;
                    }
                };
            }
            else if (t == typeof(string))
            {
                return delegate(string s, out object p)
                {
                    p = s;
                    return true;
                };
            }
            else if (t == typeof(DateTime))
            {
                return delegate(string s, out object p)
                {
                    DateTime v;
                    var r = DateTime.TryParse(s, culture, dateTimeStyle, out v);
                    p = v;
                    return r;
                };
            }
            else if (t == typeof(Guid))
            {
                return delegate(string s, out object p)
                {
                    Guid v;
                    var r = Guid.TryParse(s, out v);
                    p = v;
                    return r;
                };
            }

            throw new NotImplementedException();
        }

        protected virtual FormatterDelegate GetFormatterDelegate(DataFileColumn column)
        {
            var t = column.DataType.Type;

            if (t == typeof(SByte))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (SByte)o);
                };
            }
            else if (t == typeof(Int16))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Int16)o);
                };
            }
            else if (t == typeof(Int32))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Int32)o);
                };
            }
            else if (t == typeof(Int64))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Int64)o);
                };
            }
            else if (t == typeof(Byte))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Byte)o);
                };
            }
            else if (t == typeof(UInt16))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (UInt16)o);
                };
            }
            else if (t == typeof(UInt32))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (UInt32)o);
                };
            }
            else if (t == typeof(UInt64))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (UInt64)o);
                };
            }
            else if (t == typeof(bool))
            {
                // TODO: use numbers?
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (bool)o);
                };
            }
            else if (t == typeof(float))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (float)o);
                };
            }
            else if (t == typeof(double))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (double)o);
                };
            }
            else if (t == typeof(decimal))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (decimal)o);
                };
            }
            else if (t == typeof(char))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (string)o);
                };
            }
            else if (t == typeof(string))
            {
                return delegate(object o, string f)
                {
                    return (string)o;
                };
            }
            else if (t == typeof(DateTime))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (DateTime)o);
                };
            }
            else if (t == typeof(Guid))
            {
                return delegate(object o, string f)
                {
                    return String.Format(culture, f, (Guid)o);
                };
            }

            throw new NotImplementedException();
        }
       
        protected bool GetBestColumnTypeEstimate(string value, out Type type, out int size, out int rank)
        {
            if (String.IsNullOrEmpty(value))
            {
                type = null;
                size = 0;
                rank = 0;
                return false;
            }

            // Try from the simplest to the most complex syntax

            // Only assume Int32 or Int64 for integers. ID's might start from low numbers and the first 1000
            // rows would suggest an ID is only Int16 while it is Int32.

            // double and float are indistinguishable based on their string representations
            // while the number of decimal digits could be used, standard .net functions don't provide
            // this functionality, so we always assume double by default

            rank = 0;        

            Int32 int32v;
            if (Int32.TryParse(value, numberStyle, culture, out int32v))
            {
                type = typeof(Int32);
                size = 0;
                return true;
            }
            rank++;

            Int64 int64v;
            if (Int64.TryParse(value, numberStyle, culture, out int64v))
            {
                type = typeof(Int64);
                size = 0;
                return true;
            }
            rank++;          

            double doublev;
            if (double.TryParse(value, numberStyle, culture, out doublev))
            {
                type = typeof(double);
                size = 0;
                return true;
            }
            rank++;

            Guid guidv;
            if (Guid.TryParse(value, out guidv))
            {
                type = typeof(Guid);
                size = 0;
                return true;
            }
            rank++;

            DateTime datetimev;
            if (DateTime.TryParse(value, culture, dateTimeStyle, out datetimev))
            {
                type = typeof(DateTime);
                size = 0;
                return true;
            }
            rank++;

            if (value.Length == 1)
            {
                type = typeof(char);
                size = 0;
                return true;
            }
            rank++;

            bool boolv;
            if (bool.TryParse(value, out boolv))
            {
                type = typeof(bool);
                size = 0;
                return true;
            }
            rank++;

            // Last case: it's a string
            type = typeof(string);
            size = value.Length;
            return true;
        }

    }
}

