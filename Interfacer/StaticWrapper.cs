using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using ImpromptuInterface;
using ImpromptuInterface.Dynamic;
using Microsoft.CSharp.RuntimeBinder;
using ImpromptuInterface.InvokeExt;
using ImpromptuInterface.Optimization;

namespace Interfacer
{
    public class StaticWrapper : ImpromptuForwarder
    {
        public StaticWrapper(Type target) : base(target.WithStaticContext())
        {
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var typeArgs = GetGenericTypes(binder);
            if (typeArgs != null && typeArgs.Any())
            {
                return TryInvokeGenericMember(binder, typeArgs.ToArray(), args, out result);
            }

            return base.TryInvokeMember(binder, args, out result);
        }

        private static IList<Type> GetGenericTypes(InvokeMemberBinder binder)
        {
            var csharpBinder =
                binder.GetType().GetInterface("Microsoft.CSharp.RuntimeBinder.ICSharpInvokeOrInvokeMemberBinder");
            var typeArgs = csharpBinder.GetProperty("TypeArguments").GetValue(binder, null) as IList<Type>;
            return typeArgs;
        }

        private bool TryInvokeGenericMember(InvokeMemberBinder binder, Type[] typeArgs, object[] args, out object result)
        {
            if (CallTarget == null)
            {
                result = null;
                return false;
            }

            var tArgs = Util.NameArgsIfNecessary(binder.CallInfo, args);
            var name = new InvokeMemberName(binder.Name, typeArgs);

            try
            {
                result = Impromptu.InvokeMember(CallTarget, name, tArgs);

            }
            catch (RuntimeBinderException)
            {
                result = null;
                try
                {
                    Impromptu.InvokeMemberAction(CallTarget, name, tArgs);
                }
                catch (RuntimeBinderException)
                {
                    return false;
                }
            }
            return true;
        }
    }
}