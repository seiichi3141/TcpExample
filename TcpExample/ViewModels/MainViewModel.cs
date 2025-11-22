using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TcpExample.Application.Models;
using TcpExample.Application.Services;

namespace TcpExample.ViewModels
{
    public sealed class MainViewModel : ObservableObject
    {
        private string _endpointIp = "127.0.0.1";
        private int _port = 9000;
        private bool _autoResponseEnabled = true;
        private AutoResponseRuleViewModel _selectedRule;
        private readonly ISettingsService _settingsService;

        public MainViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            EnsureCurrentSettings();

            AutoResponses = new ObservableCollection<AutoResponseRuleViewModel>
            {
                // Filled below from current settings
            };

            AddRuleCommand = new RelayCommand(AddRule);
            RemoveRuleCommand = new RelayCommand(RemoveRule, CanRemoveRule);

            InitializeFromCurrent();
        }

        public string EndpointIp
        {
            get { return _endpointIp; }
            set
            {
                if (SetProperty(ref _endpointIp, value))
                {
                    _settingsService.Current.EndpointIp = value;
                }
            }
        }

        public int Port
        {
            get { return _port; }
            set
            {
                if (SetProperty(ref _port, value))
                {
                    _settingsService.Current.Port = value;
                }
            }
        }

        public bool AutoResponseEnabled
        {
            get { return _autoResponseEnabled; }
            set
            {
                if (SetProperty(ref _autoResponseEnabled, value))
                {
                    _settingsService.Current.AutoResponseEnabled = value;
                }
            }
        }

        public ObservableCollection<AutoResponseRuleViewModel> AutoResponses { get; }

        public AutoResponseRuleViewModel SelectedRule
        {
            get { return _selectedRule; }
            set
            {
                if (SetProperty(ref _selectedRule, value))
                {
                    (RemoveRuleCommand as RelayCommand)?.RaiseCanExecuteChanged();
                }
            }
        }

        public ICommand AddRuleCommand { get; }
        public ICommand RemoveRuleCommand { get; }

        private void AddRule()
        {
            var nextPriority = AutoResponses.Any() ? AutoResponses.Max(r => r.Priority) + 1 : 1;
            var vm = new AutoResponseRuleViewModel
            {
                Name = "New Rule",
                Pattern = string.Empty,
                Response = string.Empty,
                Enabled = true,
                Priority = nextPriority
            };
            AutoResponses.Add(vm);
            _settingsService.Current.AutoResponses.Add(ToModel(vm));
        }

        private bool CanRemoveRule(object parameter)
        {
            return SelectedRule != null;
        }

        private void RemoveRule(object parameter)
        {
            if (SelectedRule != null)
            {
                var model = _settingsService.Current.AutoResponses.FirstOrDefault(r => r.Priority == SelectedRule.Priority && r.Name == SelectedRule.Name);
                if (model != null)
                {
                    _settingsService.Current.AutoResponses.Remove(model);
                }

                AutoResponses.Remove(SelectedRule);
                SelectedRule = null;
            }
        }

        private void EnsureCurrentSettings()
        {
            if (_settingsService.Current == null)
            {
                _settingsService.SetCurrent(new SettingsModel
                {
                    EndpointIp = "127.0.0.1",
                    Port = 9000,
                    AutoResponseEnabled = true
                });
            }
        }

        private void InitializeFromCurrent()
        {
            var settings = _settingsService.Current;
            EndpointIp = string.IsNullOrWhiteSpace(settings.EndpointIp) ? "127.0.0.1" : settings.EndpointIp;
            Port = settings.Port <= 0 ? 9000 : settings.Port;
            AutoResponseEnabled = settings.AutoResponseEnabled;

            AutoResponses.Clear();
            if (settings.AutoResponses != null && settings.AutoResponses.Any())
            {
                foreach (var rule in settings.AutoResponses.OrderBy(r => r.Priority))
                {
                    AutoResponses.Add(new AutoResponseRuleViewModel
                    {
                        Name = rule.Name,
                        Pattern = rule.Pattern,
                        Response = rule.Response,
                        Enabled = rule.Enabled,
                        Priority = rule.Priority
                    });
                }
            }
            else
            {
                var defaultRule = new AutoResponseRuleViewModel
                {
                    Name = "PingPong",
                    Pattern = "PING",
                    Response = "PONG",
                    Enabled = true,
                    Priority = 1
                };
                AutoResponses.Add(defaultRule);
                _settingsService.Current.AutoResponses.Add(ToModel(defaultRule));
            }
        }

        private static AutoResponseRuleModel ToModel(AutoResponseRuleViewModel vm)
        {
            return new AutoResponseRuleModel
            {
                Name = vm.Name ?? string.Empty,
                Pattern = vm.Pattern ?? string.Empty,
                Response = vm.Response ?? string.Empty,
                Enabled = vm.Enabled,
                Priority = vm.Priority
            };
        }
    }
}
