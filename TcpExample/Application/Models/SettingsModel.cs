using System.Collections.Generic;
using System.Xml.Serialization;

namespace TcpExample.Application.Models
{
    [XmlRoot("TcpToolConfig")]
    public class SettingsModel : ObservableSettingsBase
    {
        private ConnectionSettingsModel _connection1 = new ConnectionSettingsModel();
        private ConnectionSettingsModel _connection2 = new ConnectionSettingsModel();
        private AutoResponseSettingsModel _autoResponse = new AutoResponseSettingsModel();

        [XmlElement("Connection1")]
        public ConnectionSettingsModel Connection1
        {
            get { return _connection1; }
            set { SetProperty(ref _connection1, value); }
        }

        [XmlElement("Connection2")]
        public ConnectionSettingsModel Connection2
        {
            get { return _connection2; }
            set { SetProperty(ref _connection2, value); }
        }

        [XmlElement("AutoResponse")]
        public AutoResponseSettingsModel AutoResponse
        {
            get { return _autoResponse; }
            set { SetProperty(ref _autoResponse, value); }
        }

    }
}
