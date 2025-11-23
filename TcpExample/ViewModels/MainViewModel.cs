using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Collections.Generic;
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
        private readonly IManageAutoResponseRulesUseCase _ruleUseCase;

        public MainViewModel(ISettingsService settingsService, IManageAutoResponseRulesUseCase ruleUseCase)
        {
            _settingsService = settingsService ?? throw new ArgumentNullException(nameof(settingsService));
            _ruleUseCase = ruleUseCase ?? throw new ArgumentNullException(nameof(ruleUseCase));
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
                    var conn = GetPrimaryConnection();
                    conn.EndpointIp = value;
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
                    var conn = GetPrimaryConnection();
                    conn.Port = value;
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
                    _settingsService.Current.AutoResponse.Enabled = value;
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
            _ruleUseCase.AddRule(ToModel(vm));
        }

        private bool CanRemoveRule(object parameter)
        {
            return SelectedRule != null;
        }

        private void RemoveRule(object parameter)
        {
            if (SelectedRule != null)
            {
                _ruleUseCase.RemoveRule(ToModel(SelectedRule));
                AutoResponses.Remove(SelectedRule);
                SelectedRule = null;
            }
        }

        private void EnsureCurrentSettings()
        {
            if (_settingsService.Current == null)
            {
                _settingsService.SetCurrent(new SettingsModel());
            }

            if (_settingsService.Current.Connection1 == null)
            {
                _settingsService.Current.Connection1 = new ConnectionSettingsModel { EndpointIp = "127.0.0.1", Port = 9000 };
            }
            if (_settingsService.Current.Connection2 == null)
            {
                _settingsService.Current.Connection2 = new ConnectionSettingsModel { EndpointIp = "127.0.0.1", Port = 9000 };
            }
        }

        private void InitializeFromCurrent()
        {
            var settings = _settingsService.Current;
            var conn = GetPrimaryConnection();
            EndpointIp = string.IsNullOrWhiteSpace(conn.EndpointIp) ? "127.0.0.1" : conn.EndpointIp;
            Port = conn.Port <= 0 ? 9000 : conn.Port;
            AutoResponseEnabled = settings.AutoResponse.Enabled;

            AutoResponses.Clear();
            if (settings.AutoResponse.Rules != null && settings.AutoResponse.Rules.Any())
            {
                foreach (var rule in settings.AutoResponse.Rules.OrderBy(r => r.Priority))
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
                _ruleUseCase.AddRule(ToModel(defaultRule));
            }
        }

        private ConnectionSettingsModel GetPrimaryConnection()
        {
            var current = _settingsService.Current;
            if (current == null)
            {
                current = new SettingsModel();
                _settingsService.SetCurrent(current);
            }

            if (current.Connection1 == null)
            {
                current.Connection1 = new ConnectionSettingsModel
                {
                    EndpointIp = "127.0.0.1",
                    Port = 9000
                };
            }

            return current.Connection1;
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
