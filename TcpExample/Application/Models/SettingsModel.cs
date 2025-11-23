using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TcpExample.Application.Models
{
    public class SettingsModel : INotifyPropertyChanged
    {
        private ConnectionSettingsModel _connection = new ConnectionSettingsModel();
        private AutoResponseSettingsModel _autoResponse = new AutoResponseSettingsModel();

        public ConnectionSettingsModel Connection
        {
            get { return _connection; }
            set { SetProperty(ref _connection, value); }
        }

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
