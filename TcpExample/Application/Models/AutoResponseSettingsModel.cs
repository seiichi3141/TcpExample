using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TcpExample.Application.Serialization;

namespace TcpExample.Application.Models
{
    public class AutoResponseSettingsModel : ObservableSettingsBase
    {
        private bool _enabled;
        private List<AutoResponseRuleModel> _rules = new List<AutoResponseRuleModel>();

        [SettingDefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled, value); }
        }

        [XmlArray("AutoResponses")]
        [XmlArrayItem("Rule")]
        [XmlComment("Auto response rule list")]
        public List<AutoResponseRuleModel> Rules
        {
            get { return _rules; }
            set { SetProperty(ref _rules, value); }
        }
    }
}
