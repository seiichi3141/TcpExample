using System;
using System.IO;
using System.Linq;
using TcpExample.Application.Models;
using TcpExample.Application.Services;

namespace TcpExample.Infrastructure
{
    /// <summary>
    /// 設定の読込/保存と検証・デフォルト適用を担当するサービス。
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
            var existed = File.Exists(_path);
            Current = _storage.LoadOrDefault(_path);
            DefaultValueApplier.Apply(Current);
            Sanitize(Current);
            var validation = _validator.Validate(Current);
            if (!validation.IsValid)
            {
                Current = CreateDefaultSettings();
            }
            if (!existed)
            {
                _storage.Save(_path, Current);
            }
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
            var validation = _validator.Validate(Current);
            if (!validation.IsValid)
            {
                Current = CreateDefaultSettings();
            }

            _storage.Save(_path, Current);
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

            if (settings.Connection1 == null)
            {
                settings.Connection1 = new ConnectionSettingsModel { EndpointIp = "127.0.0.1", Port = 9000 };
            }

            if (settings.Connection2 == null)
            {
                settings.Connection2 = new ConnectionSettingsModel { EndpointIp = "127.0.0.1", Port = 9000 };
            }

            if (settings.AutoResponse == null)
            {
                settings.AutoResponse = new AutoResponseSettingsModel();
            }

            if (settings.AutoResponse.Rules != null && settings.AutoResponse.Rules.Any())
            {
                var validRules = settings.AutoResponse.Rules
                    .Where(r => !string.IsNullOrWhiteSpace(r.Pattern))
                    .OrderBy(r => r.Priority)
                    .ToList();
                settings.AutoResponse.Rules = validRules;
            }

            if (settings.AutoResponse.Rules == null || !settings.AutoResponse.Rules.Any())
            {
                settings.AutoResponse.Rules = new System.Collections.Generic.List<AutoResponseRuleModel>
                {
                    new AutoResponseRuleModel
                    {
                        Name = "PingPong",
                        Pattern = "PING",
                        Response = "PONG",
                        Enabled = true,
                        Priority = 1
                    }
                };
            }
        }

        private SettingsModel CreateDefaultSettings()
        {
            var settings = new SettingsModel();
            DefaultValueApplier.Apply(settings);
            Sanitize(settings);
            return settings;
        }
    }
}
