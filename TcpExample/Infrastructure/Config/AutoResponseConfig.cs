using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TcpExample.Infrastructure.Config
{
    [Serializable]
    public class AutoResponseConfig
    {
        public bool Enabled { get; set; } = true;

        [XmlArray("AutoResponses")]
        [XmlArrayItem("Rule")]
        public List<AutoResponseRuleConfig> Rules { get; set; } = new List<AutoResponseRuleConfig>();
    }
}
