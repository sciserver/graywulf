using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Data;
using System.Data.SqlClient;
using Jhu.Graywulf.Entities.Mapping;

namespace Jhu.Graywulf.Entities
{
    public enum DefaultEnum
    {
        A = 0,
        B = 1
    }

    public enum ByteEnum : byte
    {
        A = 0,
        B = 1
    }

    public enum UInt16Enum : ushort
    {
        A = 0,
        B = 1
    }

    public enum UInt32Enum : uint
    {
        A = 0,
        B = 1
    }

    public enum UInt64Enum : ulong
    {
        A = 0,
        B = 1
    }

    public enum SByteEnum : sbyte
    {
        A = 0,
        B = 1,
        C = -1
    }

    public enum Int16Enum : short
    {
        A = 0,
        B = 1,
        C = -1
    }

    public enum Int32Enum : int
    {
        A = 0,
        B = 1,
        C = -1
    }

    public enum Int64Enum : long
    {
        A = 0,
        B = 1,
        C = -1
    }

    [DbTable]
    class EnumEntity : Entity
    {
        [DbColumn(Binding = DbColumnBinding.Key)]
        public int ID { get; set; }

        [DbColumn]
        public string Name { get; set; }

        [DbColumn]
        public DefaultEnum DefaultEnum { get; set; }

        [DbColumn]
        public ByteEnum ByteEnum { get; set; }

        [DbColumn]
        public UInt16Enum UInt16Enum { get; set; }

        [DbColumn]
        public UInt32Enum UInt32Enum { get; set; }

        [DbColumn]
        public UInt64Enum UInt64Enum { get; set; }

        [DbColumn]
        public SByteEnum SByteEnum { get; set; }

        [DbColumn]
        public Int16Enum Int16Enum { get; set; }

        [DbColumn]
        public Int32Enum Int32Enum { get; set; }

        [DbColumn]
        public Int64Enum Int64Enum { get; set; }

        [DbColumn]
        public DefaultEnum? DefaultEnumNullable { get; set; }

        [DbColumn]
        public ByteEnum? ByteEnumNullable { get; set; }

        [DbColumn]
        public UInt16Enum? UInt16EnumNullable { get; set; }

        [DbColumn]
        public UInt32Enum? UInt32EnumNullable { get; set; }

        [DbColumn]
        public UInt64Enum? UInt64EnumNullable { get; set; }

        [DbColumn]
        public SByteEnum? SByteEnumNullable { get; set; }

        [DbColumn]
        public Int16Enum? Int16EnumNullable { get; set; }

        [DbColumn]
        public Int32Enum? Int32EnumNullable { get; set; }

        [DbColumn]
        public Int64Enum? Int64EnumNullable { get; set; }

        public EnumEntity()
        {
        }

        public EnumEntity(Context context)
            : base(context)
        {
        }
    }
}
