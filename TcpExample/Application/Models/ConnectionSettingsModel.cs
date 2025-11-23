using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using TcpExample.Application.Serialization;
using TcpExample.Application.Validation;

namespace TcpExample.Application.Models
{
    public class ConnectionSettingsModel : ObservableSettingsBase
    {
        private string _endpointIp;
        private int _port;

        [Required]
        [SettingDefaultValue("127.0.0.1")]
        [IpAddress]
        public string EndpointIp
        {
            get { return _endpointIp; }
            set { SetProperty(ref _endpointIp, value); }
        }

        [Range(1, 65535)]
        [SettingDefaultValue(9000)]
        public int Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }
    }
}
