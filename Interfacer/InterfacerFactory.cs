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
            if (attribute is ApplyToInstanceAttribute)
            {
                var wrappedObject = Activator.CreateInstance(attribute.Class);
                return (TInterface) ValueConverter
                    .For(attribute.Class, wrappedObject)
                    .To(typeof(TInterface))
                    .Convert()
                    .Value;
            }

            if (attribute is ApplyToStaticAttribute)
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


        internal static InterfacerBaseAttribute GetInterfacerAttribute(Type type)
        {
            return (InterfacerBaseAttribute) type.GetCustomAttributes(true).FirstOrDefault(a => a is InterfacerBaseAttribute);
        }
    }
}
