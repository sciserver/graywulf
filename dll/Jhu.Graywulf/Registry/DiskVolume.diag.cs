using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Jhu.Graywulf.Registry
{
    public partial class DiskVolume
    {
        public override IList<DiagnosticMessage> RunDiagnostics()
        {
            List<DiagnosticMessage> msg = new List<DiagnosticMessage>();

            msg.Add(TestNetworkShare());

            return msg;
        }

        private DiagnosticMessage TestNetworkShare()
        {
            DiagnosticMessage msg = new DiagnosticMessage()
            {
                EntityName = this.GetFullyQualifiedName(),
                NetworkName = this.UncPath.ResolvedValue,
                ServiceName = "Network Share"
            };

            try
            {
                Guid g = Guid.NewGuid();

                string path = Path.Combine(
                    this.UncPath.ResolvedValue,
                    String.Format("{0}.txt", g.ToString()));

                using (StreamWriter outfile = File.CreateText(path))
                {
                    outfile.WriteLine("Hello");
                }

                File.Delete(path);

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
