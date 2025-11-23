using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public interface IManageAutoResponseRulesUseCase
    {
        void AddRule(AutoResponseRuleModel rule);
        void RemoveRule(AutoResponseRuleModel rule);
    }
}
