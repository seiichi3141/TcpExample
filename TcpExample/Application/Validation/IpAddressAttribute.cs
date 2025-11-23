using System;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace TcpExample.Application.Validation
{
    /// <summary>
    /// IP アドレス形式を検証するカスタム属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public sealed class IpAddressAttribute : ValidationAttribute
    {
        public IpAddressAttribute()
            : base("IP アドレスが不正です。")
        {
        }

        public override bool IsValid(object value)
        {
            var text = value as string;
            if (string.IsNullOrWhiteSpace(text))
            {
                return false;
            }

            return IPAddress.TryParse(text, out _);
        }
    }
}
