using System;
using System.IO;
using System.Xml.Serialization;
using TcpExample.Application.Services;

namespace TcpExample.Infrastructure
{
    /// <summary>
    /// XML 設定ファイルの保存/読込を行う。
    /// </summary>
    public sealed class ConfigStorage : IConfigStorage
    {
        public TcpToolConfig Load(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is not specified.", nameof(path));
            }

            if (!File.Exists(path))
            {
                return TcpToolConfig.CreateDefault();
            }

            var serializer = new XmlSerializer(typeof(TcpToolConfig));
            using (var stream = File.OpenRead(path))
            {
                return (TcpToolConfig)serializer.Deserialize(stream);
            }
        }

        public TcpToolConfig LoadOrDefault(string path)
        {
            try
            {
                return Load(path);
            }
            catch
            {
                return TcpToolConfig.CreateDefault();
            }
        }

        public void Save(string path, TcpToolConfig config)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentException("Path is not specified.", nameof(path));
            }

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            var directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var serializer = new XmlSerializer(typeof(TcpToolConfig));
            using (var stream = File.Create(path))
            {
                serializer.Serialize(stream, config);
            }
        }
    }
}
