using System;
using System.Xml.Serialization;

namespace TcpExample.Infrastructure.Config
{
    [Serializable]
    [XmlRoot("TcpToolConfig")]
    public class TcpToolConfig
    {
        [XmlAttribute("version")]
        public string Version { get; set; } = "1.0";

        public ConnectionConfig Connection { get; set; } = new ConnectionConfig();

        public AutoResponseConfig AutoResponse { get; set; } = new AutoResponseConfig();

        public static TcpToolConfig CreateDefault()
        {
            return new TcpToolConfig();
        }
    }
}
