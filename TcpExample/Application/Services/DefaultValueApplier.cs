using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

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
                if (!prop.CanRead || !prop.CanWrite)
                {
                    continue;
                }

                var current = prop.GetValue(target);
                var defaultAttr = prop.GetCustomAttribute<DefaultValueAttribute>();

                if (prop.PropertyType == typeof(string))
                {
                    if (string.IsNullOrWhiteSpace(current as string) && defaultAttr != null)
                    {
                        prop.SetValue(target, defaultAttr.Value);
                    }
                }
                else if (prop.PropertyType.IsValueType)
                {
                    if (current == null || current.Equals(Activator.CreateInstance(prop.PropertyType)))
                    {
                        if (defaultAttr != null)
                        {
                            prop.SetValue(target, defaultAttr.Value);
                        }
                    }
                }
                else
                {
                    if (current == null)
                    {
                        if (defaultAttr != null)
                        {
                            prop.SetValue(target, defaultAttr.Value);
                        }
                        else
                        {
                            // recursively create and apply if parameterless ctor exists
                            var ctor = prop.PropertyType.GetConstructor(Type.EmptyTypes);
                            if (ctor != null)
                            {
                                var instance = Activator.CreateInstance(prop.PropertyType);
                                prop.SetValue(target, instance);
                                Apply(instance);
                            }
                        }
                    }
                    else
                    {
                        Apply(current);
                    }
                }

                // For collections, apply defaults to each element
                var updated = prop.GetValue(target);
                if (updated is IEnumerable enumerable && !(updated is string))
                {
                    foreach (var item in enumerable)
                    {
                        Apply(item);
                    }
                }
            }
        }
    }
}
