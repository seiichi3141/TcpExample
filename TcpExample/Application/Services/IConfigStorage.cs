using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    /// <summary>
    /// 設定ファイルの入出力を抽象化するポート。
    /// </summary>
    public interface IConfigStorage
    {
        SettingsModel Load(string path);
        SettingsModel LoadOrDefault(string path);
        void Save(string path, SettingsModel config);
    }
}
