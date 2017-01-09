using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.DynamicProxy;
using Interfacer.Exceptions;

namespace Interfacer.Proxies
{
    public class InstanceProxy : IInterceptor
    {
        private readonly object _wrappedObject;

        public InstanceProxy(object wrappedObject)
        {
            _wrappedObject = wrappedObject;
        }

        public void Intercept(IInvocation invocation)
        {
            var isGenericMethod = invocation.GenericArguments != null;

            var targetMethod = (from m in _wrappedObject.GetType().GetMethods()
                where MethodMatchesSignature(m, invocation)
                select m).SingleOrDefault();

            if (targetMethod == null)
            {
                throw new MethodNotFoundException($"Failed to find a method on object of type {_wrappedObject.GetType()} with signature {invocation.Method}");
            }

            if (isGenericMethod)
            {
                targetMethod = targetMethod.MakeGenericMethod(invocation.GenericArguments);
            }

            var result = targetMethod.Invoke(_wrappedObject, invocation.Arguments);

            var attribute = InterfacerFactory.GetInterfacerAttribute(invocation.Method.ReturnType);
            if (attribute != null &&
                attribute.Type == WrappedObjectType.Instance &&
                attribute.Class.IsAssignableFrom(targetMethod.ReturnType))
            {
                result = new ProxyGenerator().CreateInterfaceProxyWithoutTarget(invocation.Method.ReturnType, new InstanceProxy(result));
            }

            invocation.ReturnValue = result;
        }

        private bool MethodMatchesSignature(MethodInfo methodInfo, IInvocation invocation)
        {
            var targetMethodInfo = invocation.Method;

            if (methodInfo.IsPublic && !methodInfo.IsStatic &&
                methodInfo.Name == targetMethodInfo.Name &&
                methodInfo.IsGenericMethod == targetMethodInfo.IsGenericMethod)
            {
                if (invocation.GenericArguments != null &&
                    methodInfo.IsGenericMethod)
                {
                    if (methodInfo.GetGenericArguments().Length == invocation.GenericArguments.Length)
                    {
                        methodInfo = methodInfo.MakeGenericMethod(invocation.GenericArguments);
                    }
                    else
                    {
                        return false;
                    }
                }

                var sourceParamTypes = methodInfo.GetParameters().Select(p => p.ParameterType);
                var targetParamTypes = targetMethodInfo.GetParameters().Select(p => p.ParameterType);

                if (sourceParamTypes.SequenceEqual(targetParamTypes))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
