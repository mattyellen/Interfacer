using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Interfacer.Generators;

namespace Interfacer
{
    public static class InterfacerFactory
    {
        private static AdapterGenerator _adapterGenerator;

        public static void Initialize(Assembly assembly)
        {
            if (_adapterGenerator == null)
            {
                _adapterGenerator = new AdapterGenerator();
            }

            foreach (var @interface in assembly.GetTypes().Where(t => t.IsInterface))
            {
                var attribute = GetInterfacerAttribute(@interface);
                if (attribute != null)
                {
                    _adapterGenerator.AddAdapterClass(@interface, attribute);
                }
            }

            _adapterGenerator.Generate();
        }

        public static TInterface Create<TInterface>() where TInterface : class
        {
            VerifyInterfaceType(typeof(TInterface));

            var attribute = GetInterfacerAttribute(typeof(TInterface));

            if (attribute.Type != WrappedObjectType.Instance)
            {
                return _adapterGenerator.CreateAdapterObject<TInterface>();
            }

            var wrappedObject = Activator.CreateInstance(attribute.Class);
            return _adapterGenerator.CreateAdapterObject<TInterface>(wrappedObject);
        }

        private static void VerifyInterfaceType(Type type)
        {
            if (!type.IsInterface)
            {
                throw new InvalidOperationException("InterfacerFactory.Create<> can only be used with an interface.");
            }

            if (GetInterfacerAttribute(type) == null)
            {
                throw new InvalidOperationException("InterfacerFactory.Create<> can only be used with an interface on which the Interfacer attribute is applied.");
            }
        }


        private static InterfacerAttribute GetInterfacerAttribute(Type type)
        {
            return (InterfacerAttribute) type.GetCustomAttributes(true).FirstOrDefault(a => a is InterfacerAttribute);
        }
    }
}
