using System.Collections.Generic;

namespace TcpExample.Application.Models
{
    public class SettingsModel
    {
        public string EndpointIp { get; set; }
        public int Port { get; set; }
        public bool AutoResponseEnabled { get; set; }
        public IList<AutoResponseRuleModel> AutoResponses { get; set; } = new List<AutoResponseRuleModel>();
    }
}
