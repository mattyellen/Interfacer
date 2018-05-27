using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Castle.DynamicProxy;
using Interfacer.Attributes;
using Interfacer.Exceptions;
using Interfacer.Proxies;
using Interfacer.Utility;

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
                var proxy = proxyGenerator.CreateResolvableInterfaceProxyWithoutTarget<TInterface>(new StaticProxy(attribute.Class));

                return proxy;
            }

            throw new NotImplementedException();
        }

        public static void Verify<TInterface>() where TInterface : class
        {
            Verify(typeof(TInterface));
        }

        public static void Verify(Type verifyType)
        {
            VerifyInterfaceType(verifyType);
            var attribute = GetInterfacerAttribute(verifyType);

            var exceptions = new List<MethodNotFoundException>();
            foreach (var type in new[] { verifyType }.Concat(verifyType.GetInterfaces()))
            {
                exceptions.AddRange(type.GetMethods()
                    .Select(method => FindMatchingMethod(method, attribute.Class, attribute is ApplyToStaticAttribute))
                    .Where(exception => exception != null));
            }

            if (exceptions.Any())
            {
                throw new InvalidInterfaceException(verifyType, exceptions);
            }
        }

        private static MethodNotFoundException FindMatchingMethod(MethodInfo method, Type targetClass, bool isStatic)
        {
            var isConstructor = method.GetCustomAttributes(typeof(ConstructorAttribute), true).Any();
            var fixedInterfaceMethod = method.ToString();

            foreach (var type in method.GetParameters().Select(p => p.ParameterType).Concat(new[] {method.ReturnType}))
            {
                var elementType = type.IsByRef ? type.GetElementType() : type;

                var attribute = GetInterfacerAttribute(elementType);
                if (attribute is ApplyToInstanceAttribute)
                {
                    fixedInterfaceMethod = fixedInterfaceMethod.Replace(elementType.ToString(), attribute.Class.ToString());
                }
            }

            if (isConstructor)
            {
                var args = new Regex(@"\(.*\)").Match(fixedInterfaceMethod).Value;
                var checkConstructor = "Void .ctor" + args;
                foreach (var checkMethod in targetClass.GetConstructors())
                {
                    if (checkConstructor == checkMethod.ToString())
                    {
                        return null;
                    }
                }

                var possibleMatches = targetClass.GetConstructors();
                return new ConstructorNotFoundException(targetClass, method, possibleMatches);
            }
            else
            {
                foreach (var checkMethod in targetClass.GetMethods().Where(m => m.IsStatic == isStatic))
                {
                    if (fixedInterfaceMethod == checkMethod.ToString())
                    {
                        return null;
                    }
                }

                var possibleMatches = targetClass.GetMethods()
                    .Where(m => m.IsStatic == isStatic &&
                                m.Name == method.Name)
                    .Cast<MethodBase>();
                return new MethodNotFoundException(targetClass, method, possibleMatches);
            }
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
