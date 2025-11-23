using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TcpExample.Application.Models
{
    public class AutoResponseSettingsModel : INotifyPropertyChanged
    {
        private bool _enabled;
        private IList<AutoResponseRuleModel> _rules = new List<AutoResponseRuleModel>();

        [DefaultValue(true)]
        public bool Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled, value); }
        }

        public IList<AutoResponseRuleModel> Rules
        {
            get { return _rules; }
            set { SetProperty(ref _rules, value); }
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
