using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public interface ISettingsValidator
    {
        ValidationResult Validate(SettingsModel settings);
    }
}
