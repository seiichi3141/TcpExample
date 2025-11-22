using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public interface ISettingsService
    {
        SettingsModel Current { get; }

        SettingsModel Load();
        void SetCurrent(SettingsModel model);
        void Save(SettingsModel model);
    }
}
