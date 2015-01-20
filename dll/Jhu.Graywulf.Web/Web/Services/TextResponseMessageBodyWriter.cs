using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Channels;
using System.Runtime.Serialization;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;

namespace Jhu.Graywulf.Web.Services
{
    /// <summary>
    /// Implements a writer that can serialize objects as a text file.
    /// </summary>
    class TextResponseMessageBodyWriter : StreamBodyWriter
    {
        private Encoding encoding;
        private object value;

        public TextResponseMessageBodyWriter(object value)
            : this(value, Encoding.ASCII)
        {
        }

        public TextResponseMessageBodyWriter(object value, Encoding encoding)
            : base(false)
        {
            this.encoding = encoding;
            this.value = value;
        }

        protected override void OnWriteBodyContents(Stream stream)
        {
            if (value != null)
            {
                var writer = new StreamWriter(stream, encoding);

                WriteBodyContents(writer);

                writer.Flush();
            }
        }

        private void WriteBodyContents(TextWriter writer)
        {
            var type = value.GetType();
            var props = type.GetProperties();

            if (value is IDataReader)
            {
                WriteDataReader(writer, (IDataReader)value);
            }
            else if (!(value is String) && type.IsArray)
            {
                WriteArray(writer, (Array)value);
                return;
            }
            else if (!(value is String) && value is IEnumerable)
            {
                WriteEnumerable(writer, (IEnumerable)value);
            }
            else if (props.Length == 1 && props[0].PropertyType.IsArray)
            {
                WriteArray(writer, (Array)props[0].GetValue(value, null));
            }
            else if (props.Length == 1 && props[0].PropertyType.GetInterface(typeof(IEnumerable).ToString()) != null)
            {
                WriteEnumerable(writer, (IEnumerable)props[0].GetValue(value, null));
            }
            else
            {
                WriteObject(writer, value);
            }
        }

        private void WriteDataReader(TextWriter writer, IDataReader reader)
        {
            // Write header
            int q = 0;
            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.GetFieldType(i).IsPrimitive)
                {
                    if (q > 0)
                    {
                        writer.Write("\t");
                    }

                    writer.Write(reader.GetName(i));
                    q++;
                }
            }
            writer.WriteLine();

            // Write rows
            var values = new object[reader.FieldCount];
            while (reader.Read())
            {
                reader.GetValues(values);

                q = 0;
                for (int i = 0; i < values.Length; i++)
                {
                    if (reader.GetFieldType(i).IsPrimitive)
                    {
                        if (q > 0)
                        {
                            writer.Write("\t");
                        }

                        writer.Write(values[i].ToString());
                        q++;
                    }
                }
                writer.WriteLine();
            }
        }

        private void WriteArray(TextWriter writer, Array results)
        {
            if (results.Rank != 1)
            {
                throw new InvalidOperationException();
            }

            for (int i = 0; i < results.Length; i++)
            {
                if (i == 0)
                {
                    WriteObjectHeader(writer, results.GetValue(i));
                }

                WriteObject(writer, results.GetValue(i));
            }
        }

        private void WriteEnumerable(TextWriter writer, IEnumerable results)
        {
            int q = 0;
            foreach (var value in results)
            {
                if (q == 0)
                {
                    WriteObjectHeader(writer, value);
                }

                WriteObject(writer, value);
                q++;
            }
        }

        private void WriteObjectHeader(TextWriter writer, object value)
        {
            var type = value.GetType();

            if (type.IsPrimitive)
            {
                //WritePrimitiveValue(writer, value);
            }
            else
            {
                WriteComplexObjectHeader(writer, value);
            }
        }

        private void WriteObject(TextWriter writer, object value)
        {
            var type = value.GetType();

            if (type.IsPrimitive || value is String)
            {
                WritePrimitiveValue(writer, value);
            }
            else
            {
                WriteComplexObject(writer, value);
            }
        }

        private void WritePrimitiveValue(TextWriter writer, object value)
        {
            writer.WriteLine(value.ToString());
        }

        private void WriteComplexObjectHeader(TextWriter writer, object value)
        {
            var type = value.GetType();
            var props = type.GetProperties();

            writer.Write("#");

            int q = 0;
            for (int i = 0; i < props.Length; i++)
            {
                if (!IsIgnoredProperty(props[i]))
                {
                    if (q > 0)
                    {
                        writer.Write("\t");
                    }

                    writer.Write(props[i].Name);
                    q++;
                }
            }

            writer.WriteLine();
        }

        private void WriteComplexObject(TextWriter writer, object value)
        {
            var type = value.GetType();
            var props = type.GetProperties();

            int q = 0;
            for (int i = 0; i < props.Length; i++)
            {
                if (!IsIgnoredProperty(props[i]))
                {
                    if (q > 0)
                    {
                        writer.Write("\t");
                    }

                    writer.Write(props[i].GetValue(value, null).ToString());
                    q++;
                }
            }

            writer.WriteLine();
        }

        private bool IsIgnoredProperty(PropertyInfo prop)
        {
            var attrs = prop.GetCustomAttributes(typeof(IgnoreDataMemberAttribute), true);
            return attrs != null && attrs.Length != 0;
        }
    }
}