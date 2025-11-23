using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TcpExample.Application.Models;
using ValidationResultModel = TcpExample.Application.Models.ValidationResult;

namespace TcpExample.Application.Services
{
    public sealed class SettingsValidator : ISettingsValidator
    {
        public ValidationResultModel Validate(SettingsModel settings)
        {
            var result = new ValidationResultModel();
            if (settings == null)
            {
                result.Errors.Add("設定が存在しません。");
                return result;
            }

            ValidateObject(settings.Connection, result);
            ValidateObject(settings.AutoResponse, result);

            var rules = settings.AutoResponse?.Rules;
            if (rules != null)
            {
                var duplicatePriority = rules.GroupBy(r => r.Priority).FirstOrDefault(g => g.Count() > 1);
                if (duplicatePriority != null)
                {
                    result.Errors.Add("自動応答ルールの優先度が重複しています。");
                }

                foreach (var rule in rules)
                {
                    ValidateObject(rule, result);
                }
            }

            return result;
        }

        private static void ValidateObject(object target, ValidationResultModel aggregate)
        {
            if (target == null)
            {
                return;
            }

            var context = new ValidationContext(target, serviceProvider: null, items: null);
            var list = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            if (!Validator.TryValidateObject(target, context, list, validateAllProperties: true))
            {
                foreach (var validationResult in list)
                {
                    if (!string.IsNullOrWhiteSpace(validationResult.ErrorMessage))
                    {
                        aggregate.Errors.Add(validationResult.ErrorMessage);
                    }
                }
            }
        }
    }
}
