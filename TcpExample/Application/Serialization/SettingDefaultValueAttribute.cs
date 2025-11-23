using System;

namespace TcpExample.Application.Serialization
{
    /// <summary>
    /// XmlSerializer に影響を与えずにデフォルト値を宣言するための属性。
    /// DefaultValueAttribute は XmlSerializer が既定値を省略するため、設定ファイルが空になるのを防ぐ。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public sealed class SettingDefaultValueAttribute : Attribute
    {
        public object Value { get; }

        public SettingDefaultValueAttribute(object value)
        {
            Value = value;
        }
    }
}
