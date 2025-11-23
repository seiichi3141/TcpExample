using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TcpExample.Application.Models
{
    public class ConnectionSettingsModel : INotifyPropertyChanged
    {
        private string _endpointIp;
        private int _port;

        [Required]
        [DefaultValue("127.0.0.1")]
        public string EndpointIp
        {
            get { return _endpointIp; }
            set { SetProperty(ref _endpointIp, value); }
        }

        [Range(1, 65535)]
        [DefaultValue(9000)]
        public int Port
        {
            get { return _port; }
            set { SetProperty(ref _port, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
            {
                return false;
            }

            storage = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            return true;
        }
    }
}
