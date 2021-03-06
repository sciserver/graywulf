﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Parsing
{
    public partial class ColumnDefinition : IColumnReference, ITableReference, IDataTypeReference
    {
        public ColumnReference ColumnReference
        {
            get { return ColumnName.ColumnReference; }
            set { ColumnName.ColumnReference = value; }
        }

        public DatabaseObjectReference DatabaseObjectReference
        {
            get { return ColumnReference.TableReference; }
        }

        public TableReference TableReference
        {
            get { return ColumnReference.TableReference; }
            set { ColumnReference.TableReference = value; }
        }

        public DataTypeReference DataTypeReference
        {
            get { return DataTypeWithSize.DataTypeReference; }
            set { DataTypeWithSize.DataTypeReference = value; }
        }

        public ColumnName ColumnName
        {
            get { return FindDescendant<ColumnName>(); }
        }

        public DataTypeSpecification DataTypeWithSize
        {
            get { return FindDescendant<DataTypeSpecification>(); }
        }

        public bool IsNullable
        {
            get
            {
                var k = FindDescendantRecursive<ColumnNullSpecification>();
                return k?.IsNullable ?? false;
            }
        }
        
        public override void Interpret()
        {
            base.Interpret();

            ColumnReference = ColumnReference.Interpret(this);
        }
    }
}
