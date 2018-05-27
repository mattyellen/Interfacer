using System;
using Castle.DynamicProxy;

namespace Interfacer.Utility
{
    public static class ProxyGeneratorExtensions
    {
        public static TInterface CreateResolvableInterfaceProxyWithoutTarget<TInterface>(
            this ProxyGenerator proxyGenerator,
            params IInterceptor[] interceptors) where TInterface : class
        {
            return (TInterface)proxyGenerator.CreateResolvableInterfaceProxyWithoutTarget(typeof(TInterface), null, interceptors);
        }

        public static object CreateResolvableInterfaceProxyWithoutTarget(
            this ProxyGenerator proxyGenerator,
            Type interfaceToProxy, 
            Type[] additionalInterfacesToProxy,
            params IInterceptor[] interceptors)
        {
            // Castle.Windsor can't resolve to a proxy without a target, so use a fake one that implements the required interface.
            var nullProxy = proxyGenerator.CreateInterfaceProxyWithoutTarget(interfaceToProxy);
            return proxyGenerator.CreateInterfaceProxyWithTarget(interfaceToProxy, additionalInterfacesToProxy, nullProxy, interceptors);
        }
    }
}
