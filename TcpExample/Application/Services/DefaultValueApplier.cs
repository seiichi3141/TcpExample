using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using TcpExample.Application.Serialization;
using System.ComponentModel;
using System.Globalization;

namespace TcpExample.Application.Services
{
    /// <summary>
    /// DefaultValue 属性を用いてオブジェクトにデフォルト値を適用する簡易ヘルパー。
    /// </summary>
    public static class DefaultValueApplier
    {
        public static void Apply(object target)
        {
            if (target == null)
            {
                return;
            }

            var type = target.GetType();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                // Skip indexers
                if (prop.GetIndexParameters().Length > 0)
                {
                    continue;
                }

                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue;
                }

                var current = prop.GetValue(target);
                object updated = current;
                // Prefer custom SettingDefaultValue to avoid XmlSerializer skipping values; fall back to DefaultValueAttribute for legacy.
                var customDefault = prop.GetCustomAttribute<SettingDefaultValueAttribute>();
                var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();
                var defaultValue = ConvertDefault(customDefault != null ? customDefault.Value : defaultAttr?.Value, prop.PropertyType);

                if (prop.PropertyType == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace(current as string) && defaultValue != null)
                    {
                        prop.SetValue(target, defaultValue);
                    }
                }
                else if (prop.PropertyType.IsValueType)
                {
                    if (current == null || current.Equals(Activator.CreateInstance(prop.PropertyType)))
                    {
                        if (defaultValue != null)
                        {
                            prop.SetValue(target, defaultValue);
                        }
                    }
                }
                else
                {
                    if (current == null)
                    {
                        if (defaultValue != null)
                        {
                            prop.SetValue(target, defaultValue);
                            updated = prop.GetValue(target);
                        }
                        else
                        {
                            // recursively create and apply if parameterless ctor exists
                            var ctor = prop.PropertyType.GetConstructor(Type.EmptyTypes);
                            if (ctor != null)
                            {
                                var instance = Activator.CreateInstance(prop.PropertyType);
                                prop.SetValue(target, instance);
                                updated = instance;
                            }
                        }
                    }
                    else
                    {
                        // avoid recursing into collections themselves; handled below
                        updated = current;
                        if (!(current is IEnumerable) || current is string)
                        {
                            Apply(current);
                        }
                    }
                }

                // For collections, apply defaults to each element
                updated = prop.GetValue(target);
                if (updated is IEnumerable enumerable && !(updated is string))
                {
                    foreach (var item in enumerable)
                    {
                        Apply(item);
                    }
                }
            }
        }

        private static object ConvertDefault(object value, Type targetType)
        {
            if (value == null)
            {
                return null;
            }

            var nonNullable = Nullable.GetUnderlyingType(targetType) ?? targetType;
            try
            {
                if (nonNullable.IsEnum)
                {
                    if (value is string s)
                    {
                        return Enum.Parse(nonNullable, s);
                    }
                    return Enum.ToObject(nonNullable, value);
                }

                if (nonNullable == typeof(Guid))
                {
                    return value is Guid g ? g : new Guid(value.ToString());
                }

                return Convert.ChangeType(value, nonNullable, CultureInfo.InvariantCulture);
            }
            catch
            {
                return value;
            }
        }
    }
}
