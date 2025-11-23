using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public interface ILoadSettingsUseCase
    {
        SettingsModel Execute();
    }
}
