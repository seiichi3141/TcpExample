using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;

namespace TcpExample.Application.Models
{
    [XmlRoot("TcpToolConfig")]
    public class SettingsModel : INotifyPropertyChanged
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

        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value))
            {
                return false;
            }

            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
