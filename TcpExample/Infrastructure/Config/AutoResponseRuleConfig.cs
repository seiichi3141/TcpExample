using System;

namespace TcpExample.Infrastructure.Config
{
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
