using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.NetworkInformation;
using Jhu.Graywulf.RemoteService;

namespace Jhu.Graywulf.Registry
{
    public partial class Machine
    {
        public override IList<DiagnosticMessage> RunDiagnostics()
        {
            List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

            msg.Add(Ping());
            msg.Add(PingBulkOpService());

            LoadDiskGroups(false);
            foreach (DiskGroup dg in this.DiskGroups.Values)
            {
                msg.AddRange(dg.RunDiagnostics());
            }

            LoadServerInstances(false);
            foreach (ServerInstance si in this.ServerInstances.Values)
            {
                msg.AddRange(si.RunDiagnostics());
            }

            return msg;
        }

        private DiagnosticMessage Ping()
        {
            DiagnosticMessage msg = new DiagnosticMessage()
            {
                EntityName = GetFullyQualifiedName(),
                NetworkName = hostName.ResolvedValue,
                ServiceName = "Ping"
            };

            Ping ping = new Ping();
            PingReply reply = ping.Send(hostName.ResolvedValue);

            if (reply.Status == IPStatus.Success)
            {
                msg.Status = DiagnosticMessageStatus.OK;
            }
            else
            {
                msg.Status = DiagnosticMessageStatus.Error;
                msg.ErrorMessage = reply.Status.ToString();
            }

            return msg;
        }

        private DiagnosticMessage PingBulkOpService()
        {
            DiagnosticMessage msg = new DiagnosticMessage()
            {
                EntityName = GetFullyQualifiedName(),
                NetworkName = hostName.ResolvedValue,
                ServiceName = "RemoteService"
            };

            try
            {
                var c = RemoteServiceHelper.GetControlObject(hostName.ResolvedValue);
                string res = c.Hello();

                msg.Status = DiagnosticMessageStatus.OK;
            }
            catch (System.Exception ex)
            {
                msg.Status = DiagnosticMessageStatus.Error;
                msg.ErrorMessage = ex.Message;
            }

            return msg;
        }

    }
}
