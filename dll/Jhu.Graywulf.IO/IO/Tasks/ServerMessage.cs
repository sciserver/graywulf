using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace Jhu.Graywulf.IO.Tasks
{
    [XmlRoot("graywulf")]
    public class ServerMessage
    {
        [XmlElement("destinationDatabase")]
        public string DestinationDatabase { get; set; }

        [XmlElement("destinationSchema")]
        public string DestinationSchema { get; set; }

        [XmlElement("destinationName")]
        public string DestinationName { get; set; }

        public static ServerMessage Deserialize(string message)
        {
            if (message.StartsWith("<graywulf>"))
            {
                var s = new XmlSerializer(typeof(ServerMessage));

                using (var tr = new StringReader(message))
                {
                    return (ServerMessage)s.Deserialize(tr);
                }
            }
            else
            {
                return null;
            }
        }

        public string Serialize()
        {
            var s = new XmlSerializer(typeof(ServerMessage));
            var settings = new XmlWriterSettings()
            {
                Indent = false,
                OmitXmlDeclaration = true,
            };
            var ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            using (var tw = new StringWriter())
            {
                using (var w = XmlWriter.Create(tw, settings))
                {
                    s.Serialize(w, this, ns);
                    return tw.ToString();
                }
            }
        }
    }
}
