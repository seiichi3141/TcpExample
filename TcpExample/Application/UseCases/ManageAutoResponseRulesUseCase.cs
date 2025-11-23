using System;
using System.Linq;
using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public sealed class ManageAutoResponseRulesUseCase : IManageAutoResponseRulesUseCase
    {
        private readonly ISettingsService _settingsService;

        public ManageAutoResponseRulesUseCase(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void AddRule(AutoResponseRuleModel rule)
        {
            if (rule == null)
            {
                throw new ArgumentNullException(nameof(rule));
            }

            _settingsService.Current.AutoResponse.Rules.Add(rule);
        }

        public void RemoveRule(AutoResponseRuleModel rule)
        {
            if (rule == null)
            {
                return;
            }

            var current = _settingsService.Current.AutoResponse.Rules
                .FirstOrDefault(r => r.Name == rule.Name && r.Priority == rule.Priority);
            if (current != null)
            {
                _settingsService.Current.AutoResponse.Rules.Remove(current);
            }
        }
    }
}
