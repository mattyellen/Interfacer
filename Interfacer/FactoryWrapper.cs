using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using ImpromptuInterface;

namespace Interfacer
{
    public class FactoryWrapper : DynamicObject
    {
        private readonly Type _class;

        public FactoryWrapper(Type @class)
        {
            _class = @class;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = Impromptu.InvokeConstructor(_class, args);
            return true;
        }
    }
}
