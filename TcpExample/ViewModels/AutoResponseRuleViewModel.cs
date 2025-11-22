using System;

namespace TcpExample.ViewModels
{
    public sealed class AutoResponseRuleViewModel : ObservableObject
    {
        private string _name;
        private string _pattern;
        private string _response;
        private bool _enabled;
        private int _priority;

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        public string Pattern
        {
            get { return _pattern; }
            set { SetProperty(ref _pattern, value); }
        }

        public string Response
        {
            get { return _response; }
            set { SetProperty(ref _response, value); }
        }

        public bool Enabled
        {
            get { return _enabled; }
            set { SetProperty(ref _enabled, value); }
        }

        public int Priority
        {
            get { return _priority; }
            set { SetProperty(ref _priority, value); }
        }

        public AutoResponseRuleViewModel Clone()
        {
            return new AutoResponseRuleViewModel
            {
                Name = Name,
                Pattern = Pattern,
                Response = Response,
                Enabled = Enabled,
                Priority = Priority
            };
        }
    }
}
