using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;
using Interfacer.Attributes;

namespace Interfacer.Proxies
{
    internal class InstanceProxy : ProxyBase, IInterceptor
    {
        public InstanceProxy(object wrappedObject)
        {
            WrappedObject = wrappedObject;
        }

        protected override object WrappedObject { get; }
        protected override Type WrappedType => WrappedObject.GetType();
        protected override bool IsMethodTypeStatic => false;

        protected override IEnumerable<MethodSignatureInfo> GetMatchingSignatureInfo(IInvocation invocation)
        {
            return from m in WrappedType.GetMethods()
                    select GetMatchingSignatureInfoForMethod(m, invocation);
        }

        public void Intercept(IInvocation invocation)
        {
            if (invocation.Method.Name == "get_" + nameof(ICanGetWrappedObject.__InterfacerWrappedObject__))
            {
                invocation.ReturnValue = WrappedObject;
                return;
            }

            InterceptBase(invocation);
        }
    }
}