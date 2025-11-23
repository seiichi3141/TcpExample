using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TcpExample.Application.Models
{
    /// <summary>
    /// 設定モデル用の共通 INotifyPropertyChanged 実装。
    /// </summary>
    public abstract class ObservableSettingsBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value))
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
