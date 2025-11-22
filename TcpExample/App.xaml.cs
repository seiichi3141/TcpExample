using System.Windows;
using TcpExample.Application.Services;
using TcpExample.Composition;
using TcpExample.Infrastructure;
using TcpExample.ViewModels;

namespace TcpExample
{
    /// <summary>
    /// アプリケーションのエントリポイント。DI で依存を構成し、設定の読込/保存を行う。
    /// </summary>
    public partial class App : System.Windows.Application
    {
        private SimpleContainer _container;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            _container = BuildContainer();

            var settingsService = _container.Resolve<ISettingsService>();
            settingsService.Load();

            var mainViewModel = _container.Resolve<MainViewModel>();

            var window = new MainWindow { DataContext = mainViewModel };
            window.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            SaveConfigSafely();
            base.OnExit(e);
        }

        private void SaveConfigSafely()
        {
            if (_container == null)
            {
                return;
            }

            try
            {
                var vm = _container.Resolve<MainViewModel>();
                var settingsService = _container.Resolve<ISettingsService>();
                settingsService.Save(settingsService.Current);
            }
            catch
            {
                // 保存失敗は終了を妨げない
            }
        }

        private SimpleContainer BuildContainer()
        {
            var container = new SimpleContainer();

            container.RegisterSingleton<IConfigStorage, ConfigStorage>();
            container.RegisterSingleton<ISettingsValidator, SettingsValidator>();
            container.RegisterSingleton<ISettingsService, SettingsService>();
            container.RegisterSingleton<MainViewModel, MainViewModel>();

            return container;
        }
    }
}
