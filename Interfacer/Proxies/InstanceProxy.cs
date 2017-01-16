using System;
using Castle.DynamicProxy;

namespace Interfacer.Proxies
{
    public class InstanceProxy : ProxyBase, IInterceptor
    {
        public InstanceProxy(object wrappedObject)
        {
            WrappedObject = wrappedObject;
        }

        protected override object WrappedObject { get; }
        protected override Type WrappedType => WrappedObject.GetType();
        protected override bool IsMethodTypeStatic => false;

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