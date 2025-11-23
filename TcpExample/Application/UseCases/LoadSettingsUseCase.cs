using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public sealed class LoadSettingsUseCase : ILoadSettingsUseCase
    {
        private readonly ISettingsService _settingsService;

        public LoadSettingsUseCase(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public SettingsModel Execute()
        {
            return _settingsService.Load();
        }
    }
}
