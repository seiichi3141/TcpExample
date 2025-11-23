using TcpExample.Infrastructure.Config;

namespace TcpExample.Application.Services
{
    /// <summary>
    /// 設定ファイルの入出力を抽象化するポート。
    /// </summary>
    public interface IConfigStorage
    {
        TcpToolConfig Load(string path);
        TcpToolConfig LoadOrDefault(string path);
        void Save(string path, TcpToolConfig config);
    }
}
