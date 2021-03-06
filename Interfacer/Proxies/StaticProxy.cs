﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace Interfacer.Proxies
{
    internal class StaticProxy : ProxyBase, IInterceptor
    {
        public StaticProxy(Type wrappedType)
        {
            WrappedType = wrappedType;
        }

        protected override object WrappedObject => null;
        protected override Type WrappedType { get; }
        protected override bool IsMethodTypeStatic => true;

        protected override IEnumerable<MethodSignatureInfo> GetMatchingSignatureInfo(IInvocation invocation)
        {
            var isConstructorCall = IsConstructorCall(invocation);

            if (isConstructorCall)
            {
                var wrappedType = WrappedType;

                if (WrappedType.ContainsGenericParameters)
                {
                    if (WrappedType.GetGenericArguments().Length == invocation.Method.ReturnType.GetGenericArguments().Length)
                    {
                        wrappedType = wrappedType.MakeGenericType(invocation.Method.ReturnType.GetGenericArguments());
                    }
                    else
                    {
                        return new List<MethodSignatureInfo>();
                    }
                }

                return from m in wrappedType.GetConstructors()
                    select GetMatchingSignatureInfoForConstructor(m, invocation, wrappedType);
            }
            else
            {
                return from m in WrappedType.GetMethods()
                       select GetMatchingSignatureInfoForMethod(m, invocation);
            }
        }

        public void Intercept(IInvocation invocation)
        {
            InterceptBase(invocation);
        }

        private MethodSignatureInfo GetMatchingSignatureInfoForConstructor(ConstructorInfo checkTargetConstructor, IInvocation invocation, Type wrappedType)
        {
            var invokedMethod = invocation.Method;

            if (!checkTargetConstructor.IsPublic)
            {
                return new MethodSignatureInfo();
            }

            return new MethodSignatureInfo
            {
                ParameterConverters = GetParameterConverters(checkTargetConstructor, invocation),
                Method = checkTargetConstructor,
                ReturnValueConverter = ValueConverter.For(wrappedType).To(invokedMethod.ReturnType)
            };
        }
    }
}
