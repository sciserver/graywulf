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

        [XmlElement("destinationSchema")]
        public string DestinationSchema { get; set; }

        [XmlElement("destinationName")]
        public string DestinationName { get; set; }
    }
}
