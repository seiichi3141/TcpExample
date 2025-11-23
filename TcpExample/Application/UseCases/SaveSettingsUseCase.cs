namespace TcpExample.Application.Services
{
    public sealed class SaveSettingsUseCase : ISaveSettingsUseCase
    {
        private readonly ISettingsService _settingsService;

        public SaveSettingsUseCase(ISettingsService settingsService)
        {
            _settingsService = settingsService;
        }

        public void Execute()
        {
            _settingsService.Save(_settingsService.Current);
        }
    }
}
