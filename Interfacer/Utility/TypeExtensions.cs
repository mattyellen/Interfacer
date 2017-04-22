using System;
using System.Linq;
using Interfacer.Attributes;

namespace Interfacer.Utility
{
    internal static class TypeExtensions
    {
        public static bool IsInstanceInterfaceOf(this Type interfacerType, Type objectType)
        {
            var attribute = InterfacerFactory.GetInterfacerAttribute(interfacerType);
            if (!(attribute is ApplyToInstanceAttribute))
            {
                return false;
            }
            
            var @class = attribute.Class;
            if (@class.ContainsGenericParameters &&
                @class.GetGenericArguments().Count(a => a.IsGenericParameter) ==
                objectType.GetGenericArguments().Length)
            {
                @class = @class.MakeGenericType(objectType.GetGenericArguments().ToArray());
            }


            return @class == objectType;
        }
    }
}
