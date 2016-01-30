using System;
using System.Collections.Generic;
using System.Linq;

namespace EasyZMq.Infrastructure
{
    public class SuperSimpleIoC
    {
        private readonly Dictionary<Type, RegisteredObject> _configItems = new Dictionary<Type, RegisteredObject>();

        public void Register<T, TU>() where TU : T
        {
            if (_configItems.ContainsKey(typeof(T))) return;

            var configItem = new RegisteredObject
            {
                TypeToResolve = typeof(T),
                ConcreteType = typeof(TU)
            };

            _configItems.Add(typeof(T), configItem);
        }

        public void Register<T>(Func<T> createInstance)
        {
            if (_configItems.ContainsKey(typeof(T))) return;

            var instance = createInstance();
            var configItem = new RegisteredObject
            {
                TypeToResolve = typeof(T),
                ConcreteType = instance.GetType(),
                Instance = instance
            };

            _configItems.Add(typeof(T), configItem);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        private object Resolve(Type type)
        {
            if (!_configItems.ContainsKey(type)) return null;

            var configItem = _configItems[type];
            if (configItem.Instance != null) return configItem.Instance;

            var parameters = ResolveConstructorParameters(configItem.ConcreteType).ToArray();
            configItem.Instance = Activator.CreateInstance(configItem.ConcreteType, parameters);

            return configItem.Instance;
        }

        private IEnumerable<object> ResolveConstructorParameters(Type concreteType)
        {
            var constructorInfo = concreteType.GetConstructors().First();
            return constructorInfo.GetParameters().Select(parameterInfo => Resolve(parameterInfo.ParameterType));
        }
    }

    public class RegisteredObject
    {
        public Type TypeToResolve { get; set; }
        public Type ConcreteType { get; set; }
        public dynamic Instance { get; set; }
    }
}
