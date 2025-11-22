using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace TcpExample.Composition
{
    /// <summary>
    /// シンプルな DI コンテナ。シングルトン/トランジェント登録とコンストラクタインジェクションに対応。
    /// </summary>
    public sealed class SimpleContainer
    {
        private sealed class Registration
        {
            public Func<SimpleContainer, object> Factory { get; set; }
            public Type ImplementationType { get; set; }
            public bool Singleton { get; set; }
            public object Instance { get; set; }
        }

        private readonly object _sync = new object();
        private readonly Dictionary<Type, Registration> _registrations = new Dictionary<Type, Registration>();
        private readonly HashSet<Type> _resolving = new HashSet<Type>();

        public void RegisterSingleton<TService>(Func<SimpleContainer, TService> factory) where TService : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            RegisterInternal(typeof(TService), new Registration
            {
                Factory = c => factory(c),
                Singleton = true
            });
        }

        public void RegisterTransient<TService>(Func<SimpleContainer, TService> factory) where TService : class
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            RegisterInternal(typeof(TService), new Registration
            {
                Factory = c => factory(c),
                Singleton = false
            });
        }

        public void RegisterSingleton<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            RegisterInternal(typeof(TService), new Registration
            {
                ImplementationType = typeof(TImplementation),
                Singleton = true
            });
        }

        public void RegisterTransient<TService, TImplementation>()
            where TService : class
            where TImplementation : class, TService
        {
            RegisterInternal(typeof(TService), new Registration
            {
                ImplementationType = typeof(TImplementation),
                Singleton = false
            });
        }

        public TService Resolve<TService>() where TService : class
        {
            return (TService)Resolve(typeof(TService));
        }

        public object Resolve(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }

            Registration registration;
            lock (_sync)
            {
                if (!_registrations.TryGetValue(serviceType, out registration))
                {
                    throw new InvalidOperationException("Type not registered: " + serviceType.FullName);
                }

                if (_resolving.Contains(serviceType))
                {
                    throw new InvalidOperationException("Circular dependency detected for " + serviceType.FullName);
                }

                _resolving.Add(serviceType);
            }

            try
            {
                if (registration.Singleton && registration.Instance != null)
                {
                    return registration.Instance;
                }

                var instance = CreateInstance(registration, serviceType);
                if (instance == null)
                {
                    throw new InvalidOperationException("Factory returned null for " + serviceType.FullName);
                }

                if (registration.Singleton)
                {
                    lock (_sync)
                    {
                        if (registration.Instance == null)
                        {
                            registration.Instance = instance;
                        }
                        else
                        {
                            instance = registration.Instance;
                        }
                    }
                }

                return instance;
            }
            finally
            {
                lock (_sync)
                {
                    _resolving.Remove(serviceType);
                }
            }
        }

        private void RegisterInternal(Type serviceType, Registration registration)
        {
            lock (_sync)
            {
                if (_registrations.ContainsKey(serviceType))
                {
                    throw new InvalidOperationException("Type already registered: " + serviceType.FullName);
                }

                _registrations[serviceType] = registration;
            }
        }

        private object CreateInstance(Registration registration, Type serviceType)
        {
            if (registration.Factory != null)
            {
                return registration.Factory(this);
            }

            var implType = registration.ImplementationType ?? serviceType;
            if (implType.IsAbstract || implType.IsInterface)
            {
                throw new InvalidOperationException("Cannot instantiate abstract/interface: " + implType.FullName);
            }

            var ctor = ChooseConstructor(implType);
            var parameters = ctor.GetParameters();
            if (parameters.Length == 0)
            {
                return Activator.CreateInstance(implType);
            }

            var args = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                args[i] = Resolve(parameters[i].ParameterType);
            }

            return ctor.Invoke(args);
        }

        private ConstructorInfo ChooseConstructor(Type type)
        {
            var ctors = type.GetConstructors();
            if (ctors.Length == 0)
            {
                throw new InvalidOperationException("No public constructor found for " + type.FullName);
            }

            return ctors.OrderByDescending(c => c.GetParameters().Length).First();
        }
    }
}
