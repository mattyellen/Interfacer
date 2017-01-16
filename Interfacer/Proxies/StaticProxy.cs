using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.DynamicProxy;

namespace Interfacer.Proxies
{
    public class StaticProxy : ProxyBase, IInterceptor
    {
        public StaticProxy(Type wrappedType)
        {
            WrappedType = wrappedType;
        }

        protected override object WrappedObject => null;
        protected override Type WrappedType { get; }
        protected override bool IsMethodTypeStatic => true;

        public void Intercept(IInvocation invocation)
        {
            InterceptBase(invocation);
        }
    }
}
