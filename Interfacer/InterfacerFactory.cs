using System;
using System.Linq;
using Castle.DynamicProxy;
using Interfacer.Proxies;

namespace Interfacer
{
    public static class InterfacerFactory
    {
        public static TInterface Create<TInterface>() where TInterface : class
        {
            VerifyInterfaceType(typeof(TInterface));

            var attribute = GetInterfacerAttribute(typeof(TInterface));

            var wrappedObject = Activator.CreateInstance(attribute.Class);
            var proxyGenerator = new ProxyGenerator();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(new InstanceProxy(wrappedObject));
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


        internal static InterfacerAttribute GetInterfacerAttribute(Type type)
        {
            return (InterfacerAttribute) type.GetCustomAttributes(true).FirstOrDefault(a => a is InterfacerAttribute);
        }
    }
}
