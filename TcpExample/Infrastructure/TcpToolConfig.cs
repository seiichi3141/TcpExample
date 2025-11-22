using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TcpExample.Infrastructure
{
    [Serializable]
    [XmlRoot("TcpToolConfig")]
    public class TcpToolConfig
    {
        [XmlAttribute("version")]
        public string Version { get; set; } = "1.0";

        public string EndpointIp { get; set; } = "127.0.0.1";

        public int Port { get; set; } = 9000;

        public bool AutoResponseEnabled { get; set; } = true;

        [XmlArray("AutoResponses")]
        [XmlArrayItem("Rule")]
        public List<AutoResponseRuleConfig> AutoResponses { get; set; } = new List<AutoResponseRuleConfig>();

        public static TcpToolConfig CreateDefault()
        {
            var config = new TcpToolConfig();
            config.AutoResponses.Add(new AutoResponseRuleConfig
            {
                Name = "PingPong",
                Pattern = "PING",
                Response = "PONG",
                Enabled = true,
                Priority = 1
            });
            return config;
        }
    }

    [Serializable]
    public class AutoResponseRuleConfig
    {
        public string Name { get; set; }
        public string Pattern { get; set; }
        public string Response { get; set; }
        public bool Enabled { get; set; }
        public int Priority { get; set; }
    }
}
