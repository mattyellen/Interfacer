using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImpromptuInterface;
using ImpromptuInterface.InvokeExt;

namespace Interfacer
{
    public static class InterfacerFactory
    {
        public static TInterface Create<TInterface>() where TInterface : class
        {
            VerifyInterfaceType(typeof(TInterface));

            var attribute = GetInterfacerAttribute(typeof(TInterface));
            var createMethod = new Dictionary<WrappedObjectType, Func<Type, object>>
            {
                {WrappedObjectType.Factory, CreateFactory },
                {WrappedObjectType.Static, CreateStaticWrapper },
                {WrappedObjectType.Instance, CreateInstance }
            };

            return createMethod[attribute.Type](attribute.Class).ActLike<TInterface>();
        }

        public static void Validate<TInterface>()
        {
            Validate(typeof(TInterface));
        }

        public static void Validate(Type interfaceType)
        {
            
        }

        private static object CreateInstance(Type @class)
        {
            return Activator.CreateInstance(@class);
        }

        private static object CreateStaticWrapper(Type @class)
        {
            return new StaticWrapper(@class);
        }

        private static object CreateFactory(Type @class)
        {
            return new FactoryWrapper(@class);
        }

        private static void VerifyInterfaceType(Type type)
        {
            if (!type.IsInterface)
            {
                throw new InvalidOperationException("InterfacerFactory.Create<> can only be used with an interface.");
            }

            if (GetInterfacerAttribute(type) == null)
            {
                throw new InvalidOperationException("InterfacerFactory.Create<> can only be used with an interface on which the InterfacerFactory attribute is applied.");
            }
        }


        private static InterfacerAttribute GetInterfacerAttribute(Type type)
        {
            return (InterfacerAttribute) type.GetCustomAttributes(true).FirstOrDefault(a => a is InterfacerAttribute);
        }
    }
}
