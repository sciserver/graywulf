﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jhu.Graywulf.Parsing;
using Jhu.Graywulf.Sql.Parsing;
using Jhu.Graywulf.Sql.QueryGeneration;
using Jhu.Graywulf.Sql.NameResolution;

namespace Jhu.Graywulf.Sql.Extensions.Parsing
{
    public class ServerMessageMagicToken : MagicTokenBase
    {
        private TableReference destinationTable;

        public TableReference DestinationTable
        {
            get { return destinationTable; }
            set { destinationTable = value; }
        }

        public ServerMessageMagicToken()
        {
            InitializeMembers();
        }

        public ServerMessageMagicToken(ServerMessageMagicToken old)
        {
            CopyMembers(old);
        }

        private void InitializeMembers()
        {
            this.destinationTable = null;
        }

        private void CopyMembers(ServerMessageMagicToken old)
        {
            this.destinationTable = old.destinationTable;
        }

        public override object Clone()
        {
            return new ServerMessageMagicToken(this);
        }

        public override bool Match(Parser parser)
        {
            throw new NotImplementedException();
        }

        public override void Write(QueryRendering.QueryRendererBase renderer, TextWriter writer)
        {
            var tr = renderer.MapTableReference(destinationTable);
            var msg = new IO.Tasks.ServerMessage()
            {
                DestinationDatabase = tr.DatabaseName,
                DestinationSchema = tr.SchemaName,
                DestinationName = tr.DatabaseObjectName,
            };

            writer.Write("PRINT '" + msg.Serialize().Replace("'", "''") + "'");
        }
    }
}
