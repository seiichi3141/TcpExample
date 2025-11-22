using System;
using System.Linq;
using System.Net;
using TcpExample.Application.Models;

namespace TcpExample.Application.Services
{
    public sealed class SettingsValidator : ISettingsValidator
    {
        public ValidationResult Validate(SettingsModel settings)
        {
            var result = new ValidationResult();
            if (settings == null)
            {
                result.Errors.Add("設定が存在しません。");
                return result;
            }

            if (string.IsNullOrWhiteSpace(settings.EndpointIp) || !IPAddress.TryParse(settings.EndpointIp, out _))
            {
                result.Errors.Add("IP アドレスが不正です。");
            }

            if (settings.Port <= 0 || settings.Port > 65535)
            {
                result.Errors.Add("ポートは 1-65535 の範囲で指定してください。");
            }

            if (settings.AutoResponses != null)
            {
                var duplicatePriority = settings.AutoResponses
                    .GroupBy(r => r.Priority)
                    .FirstOrDefault(g => g.Count() > 1);
                if (duplicatePriority != null)
                {
                    result.Errors.Add("自動応答ルールの優先度が重複しています。");
                }

                foreach (var rule in settings.AutoResponses)
                {
                    if (rule == null)
                    {
                        result.Errors.Add("自動応答ルールに不正な項目があります。");
                        continue;
                    }

                    if (string.IsNullOrWhiteSpace(rule.Pattern))
                    {
                        result.Errors.Add($"ルール '{rule.Name ?? "(名称未設定)"}' のパターンが未設定です。");
                    }
                }
            }

            return result;
        }
    }
}
