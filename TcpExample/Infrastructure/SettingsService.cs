using System;
using System.IO;
using System.Linq;
using TcpExample.Application.Models;
using TcpExample.Application.Services;

namespace TcpExample.Infrastructure
{
    /// <summary>
    /// 設定の読込/保存を担当するサービス。パスはコンストラクタで受け取る。
    /// </summary>
    public sealed class SettingsService : ISettingsService
    {
        private const string ConfigFileName = "appsettings.xml";
        private readonly IConfigStorage _storage;
        private readonly ISettingsValidator _validator;
        private readonly string _path;
        public SettingsModel Current { get; private set; }

        public SettingsService(IConfigStorage storage, ISettingsValidator validator)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _path = Path.Combine(baseDir, ConfigFileName);
        }

        public SettingsModel Load()
        {
            var config = _storage.LoadOrDefault(_path);
            Current = Map(config);
            Sanitize(Current);
            ValidateOrThrow(Current);
            return Current;
        }

        public void SetCurrent(SettingsModel model)
        {
            Current = model ?? throw new ArgumentNullException(nameof(model));
            ValidateOrThrow(Current);
        }

        public void Save(SettingsModel model)
        {
            SetCurrent(model);
            Sanitize(Current);
            var config = Map(model);
            _storage.Save(_path, config);
        }

        private void ValidateOrThrow(SettingsModel settings)
        {
            var result = _validator.Validate(settings);
            if (!result.IsValid)
            {
                throw new ArgumentException(string.Join(Environment.NewLine, result.Errors));
            }
        }

        private static void Sanitize(SettingsModel settings)
        {
            if (settings == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(settings.EndpointIp))
            {
                settings.EndpointIp = "127.0.0.1";
            }

            if (settings.Port <= 0 || settings.Port > 65535)
            {
                settings.Port = 9000;
            }

            if (settings.AutoResponses != null && settings.AutoResponses.Any())
            {
                // 空パターンのルールを除外
                var validRules = settings.AutoResponses
                    .Where(r => !string.IsNullOrWhiteSpace(r.Pattern))
                    .OrderBy(r => r.Priority)
                    .ToList();
                settings.AutoResponses = validRules;
            }

            if (settings.AutoResponses == null || !settings.AutoResponses.Any())
            {
                settings.AutoResponses = new[]
                {
                    new AutoResponseRuleModel
                    {
                        Name = "PingPong",
                        Pattern = "PING",
                        Response = "PONG",
                        Enabled = true,
                        Priority = 1
                    }
                }.ToList();
            }
        }

        private static SettingsModel Map(TcpToolConfig config)
        {
            if (config == null)
            {
                return new SettingsModel
                {
                    EndpointIp = "127.0.0.1",
                    Port = 9000,
                    AutoResponseEnabled = true
                };
            }

            var model = new SettingsModel
            {
                EndpointIp = string.IsNullOrWhiteSpace(config.EndpointIp) ? "127.0.0.1" : config.EndpointIp,
                Port = config.Port <= 0 ? 9000 : config.Port,
                AutoResponseEnabled = config.AutoResponseEnabled
            };

            if (config.AutoResponses != null && config.AutoResponses.Any())
            {
                foreach (var rule in config.AutoResponses.OrderBy(r => r.Priority))
                {
                    model.AutoResponses.Add(new AutoResponseRuleModel
                    {
                        Name = rule.Name,
                        Pattern = rule.Pattern,
                        Response = rule.Response,
                        Enabled = rule.Enabled,
                        Priority = rule.Priority
                    });
                }
            }

            return model;
        }

        private static TcpToolConfig Map(SettingsModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            var config = new TcpToolConfig
            {
                EndpointIp = model.EndpointIp ?? string.Empty,
                Port = model.Port,
                AutoResponseEnabled = model.AutoResponseEnabled
            };

            if (model.AutoResponses != null)
            {
                foreach (var rule in model.AutoResponses.OrderBy(r => r.Priority))
                {
                    config.AutoResponses.Add(new AutoResponseRuleConfig
                    {
                        Name = rule.Name ?? string.Empty,
                        Pattern = rule.Pattern ?? string.Empty,
                        Response = rule.Response ?? string.Empty,
                        Enabled = rule.Enabled,
                        Priority = rule.Priority
                    });
                }
            }

            return config;
        }
    }
}
