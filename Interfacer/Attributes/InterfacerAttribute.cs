using System;

namespace Interfacer.Attributes
{
    public enum WrappedObjectType
    {
        Factory,
        Static,
        Instance
    }

    [AttributeUsage(AttributeTargets.Interface)]
    public class InterfacerAttribute : Attribute
    {
        public readonly WrappedObjectType Type;
        public readonly Type Class;

        public InterfacerAttribute(WrappedObjectType type, Type @class)
        {
            Type = type;
            Class = @class;
        }
    }
}
