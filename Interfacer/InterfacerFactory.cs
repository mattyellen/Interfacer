using System;
using System.Linq;
using Castle.DynamicProxy;
using Interfacer.Attributes;
using Interfacer.Proxies;

namespace Interfacer
{
    public static class InterfacerFactory
    {
        public static TInterface Create<TInterface>() where TInterface : class
        {
            VerifyInterfaceType(typeof(TInterface));

            var attribute = GetInterfacerAttribute(typeof(TInterface));

            if (attribute.Type == WrappedObjectType.Instance)
            {
                var wrappedObject = Activator.CreateInstance(attribute.Class);
                return (TInterface) ValueConverter
                    .For(attribute.Class, wrappedObject)
                    .To(typeof(TInterface))
                    .Convert()
                    .Value;
            }

            if (attribute.Type == WrappedObjectType.Static)
            {
                var proxyGenerator = new ProxyGenerator();
                return proxyGenerator.CreateInterfaceProxyWithoutTarget<TInterface>(new StaticProxy(attribute.Class));
            }

            throw new NotImplementedException();
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
