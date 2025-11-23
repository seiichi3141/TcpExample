using System;
using System.IO;
using System.Linq;
using TcpExample.Application.Models;
using TcpExample.Application.Services;
using TcpExample.Infrastructure.Config;

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
            DefaultValueApplier.Apply(Current);
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
            DefaultValueApplier.Apply(Current);
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

        private void Sanitize(SettingsModel settings)
        {
            if (settings == null)
            {
                return;
            }

            if (settings.Connection == null)
            {
                settings.Connection = new ConnectionSettingsModel();
            }

            if (string.IsNullOrWhiteSpace(settings.Connection.EndpointIp))
            {
                settings.Connection.EndpointIp = "127.0.0.1";
            }

            if (settings.Connection.Port <= 0 || settings.Connection.Port > 65535)
            {
                settings.Connection.Port = 9000;
            }

            if (settings.AutoResponse == null)
            {
                settings.AutoResponse = new AutoResponseSettingsModel();
            }

            if (settings.AutoResponse.Rules != null && settings.AutoResponse.Rules.Any())
            {
                // 空パターンのルールを除外
                var validRules = settings.AutoResponse.Rules
                    .Where(r => !string.IsNullOrWhiteSpace(r.Pattern))
                    .OrderBy(r => r.Priority)
                    .ToList();
                settings.AutoResponse.Rules = validRules;
            }

            if (settings.AutoResponse.Rules == null || !settings.AutoResponse.Rules.Any())
            {
                settings.AutoResponse.Rules = new List<AutoResponseRuleModel>();
            }
        }

        private static SettingsModel Map(TcpToolConfig config)
        {
            if (config == null)
            {
                return null;
            }

            var connection = config.Connection ?? new ConnectionConfig();
            var autoResponse = config.AutoResponse ?? new AutoResponseConfig();

            var model = new SettingsModel
            {
                Connection = new ConnectionSettingsModel
                {
                    EndpointIp = string.IsNullOrWhiteSpace(connection.EndpointIp) ? "127.0.0.1" : connection.EndpointIp,
                    Port = connection.Port <= 0 ? 9000 : connection.Port
                },
                AutoResponse = new AutoResponseSettingsModel
                {
                    Enabled = autoResponse.Enabled
                }
            };

            if (autoResponse.Rules != null && autoResponse.Rules.Any())
            {
                foreach (var rule in autoResponse.Rules.OrderBy(r => r.Priority))
                {
                    model.AutoResponse.Rules.Add(new AutoResponseRuleModel
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
                Connection = new ConnectionConfig
                {
                    EndpointIp = model.Connection?.EndpointIp ?? string.Empty,
                    Port = model.Connection?.Port ?? 0
                },
                AutoResponse = new AutoResponseConfig
                {
                    Enabled = model.AutoResponse?.Enabled ?? true
                }
            };

            if (model.AutoResponse?.Rules != null)
            {
                foreach (var rule in model.AutoResponse.Rules.OrderBy(r => r.Priority))
                {
                    config.AutoResponse.Rules.Add(new AutoResponseRuleConfig
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
