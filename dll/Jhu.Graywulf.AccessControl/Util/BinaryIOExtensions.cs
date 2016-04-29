using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Util
{
    static class BinaryIOExtensions
    {
        public static void WriteNullString(this BinaryWriter writer, string value)
        {
            writer.Write(value == null);

            if (value != null)
            {
                writer.Write(value);
            }
        }

        public static string ReadNullString(this BinaryReader reader)
        {
            bool isnull = reader.ReadBoolean();

            if (isnull)
            {
                return null;
            }
            else
            {
                return reader.ReadString();
            }
        }
    }
}
