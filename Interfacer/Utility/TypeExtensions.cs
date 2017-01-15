using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Interfacer.Utility
{
    internal static class TypeExtensions
    {
        public static bool IsInstanceInterfaceOf(this Type interfacerType, Type objectType)
        {
            var attribute = InterfacerFactory.GetInterfacerAttribute(interfacerType);
            return attribute != null &&
                   attribute.Type == WrappedObjectType.Instance &&
                   attribute.Class == objectType;
        }
    }
}
